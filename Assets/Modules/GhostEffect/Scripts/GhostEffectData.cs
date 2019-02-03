using UnityEngine;

public class GhostEffectData
{
    public Mesh mesh;
    public Color color;

    private float m_duration;
    private float m_timer;

    public void Setup(Mesh mesh, Color color, float duration)
    {
        this.mesh = mesh;
        this.color = color;
        m_duration = duration;
        m_timer = duration;
    }

    public bool Update(float deltaTime)
    {
        m_timer -= deltaTime;
        color.a = Mathf.Lerp(0, 1, m_timer / m_duration);

        return m_timer <= 0;
    }
}