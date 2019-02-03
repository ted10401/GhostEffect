using UnityEngine;

public class GhostEffectComponent : MonoBehaviour
{
    public float intervalTime = 0.25f;
    public float duration = 0.5f;
    public Color color = Color.red;
    public Material material;

    private void Awake()
    {
        GhostEffectManager.Instance.AddEffect(this);
    }
}
