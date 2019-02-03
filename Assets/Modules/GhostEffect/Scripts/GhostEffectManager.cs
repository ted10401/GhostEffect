using JSLCore;
using UnityEngine;
using System.Collections.Generic;

public class GhostEffectManager : MonoSingleton<GhostEffectManager>
{
    private Queue<Mesh> m_meshPool;
    private Queue<GhostEffectData> m_ghostInfoPool;
    private Dictionary<GameObject, GhostEffect> m_ghosts = new Dictionary<GameObject, GhostEffect>();
    private float m_deltaTime;

    private void Awake()
    {
        m_meshPool = new Queue<Mesh>();
        m_ghostInfoPool = new Queue<GhostEffectData>();
    }

    private void Update()
    {
        m_deltaTime = Time.deltaTime;
        foreach (KeyValuePair<GameObject, GhostEffect> kvp in m_ghosts)
        {
            kvp.Value.Update(m_deltaTime);
        }
    }

    public void AddEffect(GhostEffectComponent ghostEffectComponent)
    {
        if(m_ghosts.ContainsKey(ghostEffectComponent.gameObject))
        {
            return;
        }

        m_ghosts.Add(ghostEffectComponent.gameObject, new GhostEffect(ghostEffectComponent));
    }

    public void RemoveEffect(GhostEffectComponent ghostEffectComponent)
    {
        if(!m_ghosts.ContainsKey(ghostEffectComponent.gameObject))
        {
            return;
        }

        m_ghosts[ghostEffectComponent.gameObject].Destroy();
        m_ghosts.Remove(ghostEffectComponent.gameObject);
    }

    public Mesh GetMesh()
    {
        if (m_meshPool.Count > 0)
        {
            return m_meshPool.Dequeue();
        }
        else
        {
            return new Mesh();
        }
    }

    private void RecoveryMesh(Mesh mesh)
    {
        m_meshPool.Enqueue(mesh);
    }

    public GhostEffectData GetGhostInfo()
    {
        if(m_ghostInfoPool.Count > 0)
        {
            return m_ghostInfoPool.Dequeue();
        }
        else
        {
            return new GhostEffectData();
        }
    }

    public void RecoveryGhostInfo(GhostEffectData ghostInfo)
    {
        m_ghostInfoPool.Enqueue(ghostInfo);

        RecoveryMesh(ghostInfo.mesh);
        ghostInfo.mesh = null;
    }
}
