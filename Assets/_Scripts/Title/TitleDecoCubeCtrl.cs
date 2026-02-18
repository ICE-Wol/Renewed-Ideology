using UnityEngine;

public class TitleDecoCubeCtrl : MonoBehaviour
{
    [SerializeField]
    public float rotationSpeed = 1.0f;

    [SerializeField]
    private float initialRotationOffset = 0.0f;

    private float currentRotation = 0.0f;
    private Renderer _renderer;

    void Awake()
    {
        // 获取渲染器组件
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // 设置初始旋转偏移
        currentRotation = initialRotationOffset;
        transform.localRotation = Quaternion.Euler(currentRotation, 0, 0);

        // 注册到描边渲染系统
        if (_renderer != null)
        {
            TagOutlineRegistry.Register(_renderer);
        }
    }

    void OnDestroy()
    {
        // 从描边渲染系统注销
        if (_renderer != null)
        {
            TagOutlineRegistry.Unregister(_renderer);
        }
    }

    void Update()
    {
        // 持续旋转
        currentRotation += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(currentRotation, 0, 0);
    }

    // 设置旋转偏移量
    public void SetRotationOffset(float offset)
    {
        initialRotationOffset = offset;
        currentRotation = initialRotationOffset;
        transform.localRotation = Quaternion.Euler(currentRotation, 0, 0);
    }
}