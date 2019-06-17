using UnityEngine;
/// <summary>
/// Dynamically set the render queue
/// see https://wiki.unity3d.com/index.php/DepthMask
/// </summary>
[AddComponentMenu("Rendering/SetRenderQueue")]
public class SetRenderQueue : MonoBehaviour
{

    [SerializeField]
    protected int[] m_queues = new int[] { 3000 };

    protected void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length && i < m_queues.Length; ++i)
            {
                materials[i].renderQueue = m_queues[i];
            }
        }
    }
}