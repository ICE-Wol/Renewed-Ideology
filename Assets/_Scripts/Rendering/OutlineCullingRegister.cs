using UnityEngine;

/// <summary>
/// 挂载到需要剔除描边的物体上（如Title），自动注册到剔除系统
/// </summary>
[RequireComponent(typeof(Renderer))]
public class OutlineCullingRegister : MonoBehaviour
{
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.sortingLayerName = "Default";
    }

    void Start()
    {
        if (_renderer != null)
        {
            TagOutlineCullingRegistry.Register(_renderer);
        }
    }

    void OnDestroy()
    {
        if (_renderer != null)
        {
            TagOutlineCullingRegistry.Unregister(_renderer);
        }
    }

    void OnEnable()
    {
        if (_renderer != null)
        {
            TagOutlineCullingRegistry.Register(_renderer);
        }
    }

    void OnDisable()
    {
        if (_renderer != null)
        {
            TagOutlineCullingRegistry.Unregister(_renderer);
        }
    }
}
