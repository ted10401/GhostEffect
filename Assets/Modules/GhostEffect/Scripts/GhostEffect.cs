using UnityEngine;
using System.Collections.Generic;

public class GhostEffect
{
    private readonly int RIM_COLOR_ID = Shader.PropertyToID("_RimColor");

    private float m_intervalTime = 0.25f;
    private float m_duration = 0.5f;
    private Color m_color = Color.red;
    private Material m_material;
    
    private SkinnedMeshRenderer[] m_skinnedMeshRenderers;
    private MeshFilter[] m_meshFilters;
    private int m_skinnedMeshRendererLength;
    private int m_meshFilterLength;
    private int m_combineInstanceLength;
    private CombineInstance[] m_combineInstances;
    private List<GhostEffectData> m_ghostInfos;
    private MaterialPropertyBlock m_materialPropertyBlock;
    private float m_timer;

    public GhostEffect(GhostEffectComponent ghostEffectComponent)
    {
        m_intervalTime = ghostEffectComponent.intervalTime;
        m_duration = ghostEffectComponent.duration;
        m_color = ghostEffectComponent.color;
        m_material = ghostEffectComponent.material;

        Initialize(ghostEffectComponent.gameObject);
    }

    public void Destroy()
    {
        foreach(GhostEffectData ghostInfo in m_ghostInfos)
        {
            GhostEffectManager.Instance.RecoveryGhostInfo(ghostInfo);
        }
    }

    private void Initialize(GameObject gameObject)
    {
        m_skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        m_skinnedMeshRendererLength = m_skinnedMeshRenderers.Length;
        m_meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        m_meshFilterLength = m_meshFilters.Length;

        m_combineInstanceLength = m_skinnedMeshRendererLength + m_meshFilterLength;
        m_combineInstances = new CombineInstance[m_combineInstanceLength];

        for (int i = 0; i < m_combineInstanceLength; i++)
        {
            m_combineInstances[i].mesh = GhostEffectManager.Instance.GetMesh();
        }

        m_ghostInfos = new List<GhostEffectData>();
        m_materialPropertyBlock = new MaterialPropertyBlock();

        m_timer = m_intervalTime;
    }

    public void Update(float deltaTime)
    {
        m_timer -= deltaTime;
        if (m_timer <= 0)
        {
            m_timer = m_intervalTime;
            CreateEffect();
        }

        for (int i = 0, count = m_ghostInfos.Count; i < count; i++)
        {
            if (m_ghostInfos[i].Update(deltaTime))
            {
                GhostEffectManager.Instance.RecoveryGhostInfo(m_ghostInfos[i]);
                m_ghostInfos.RemoveAt(i);

                i--;
                count--;
            }
            else
            {
                m_materialPropertyBlock.SetColor(RIM_COLOR_ID, m_ghostInfos[i].color);
                Graphics.DrawMesh(m_ghostInfos[i].mesh, Matrix4x4.identity, m_material, 0, null, 0, m_materialPropertyBlock);
            }
        }
    }

    private Mesh m_cacheMesh;
    private GhostEffectData m_cacheGhostInfo;
    private void CreateEffect()
    {
        for (int i = 0; i < m_combineInstanceLength; i++)
        {
            if(i < m_skinnedMeshRendererLength)
            {
                m_skinnedMeshRenderers[i].BakeMesh(m_combineInstances[i].mesh);
                m_combineInstances[i].transform = m_skinnedMeshRenderers[i].transform.localToWorldMatrix;
            }
            else
            {
                m_combineInstances[i].mesh = m_meshFilters[i - m_skinnedMeshRendererLength].sharedMesh;
                m_combineInstances[i].transform = m_meshFilters[i - m_skinnedMeshRendererLength].transform.localToWorldMatrix;
            }
        }

        m_cacheMesh = GhostEffectManager.Instance.GetMesh();
        m_cacheMesh.CombineMeshes(m_combineInstances);

        m_cacheGhostInfo = GhostEffectManager.Instance.GetGhostInfo();
        m_cacheGhostInfo.Setup(m_cacheMesh, m_color, m_duration);

        m_ghostInfos.Add(m_cacheGhostInfo);
    }
}
