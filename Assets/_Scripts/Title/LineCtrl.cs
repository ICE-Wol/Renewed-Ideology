using UnityEngine;

/// <summary>
/// 线段控制器 - 生成LineRenderer并以正弦波形式移动
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LineCtrl : MonoBehaviour
{
    [Header("线段设置")]
    [SerializeField]
    [Tooltip("线段材质")]
    private Material lineMaterial;

    [SerializeField]
    [Tooltip("线段宽度")]
    private float lineWidth = 0.1f;

    [SerializeField]
    [Tooltip("线段长度")]
    private float lineLength = 5f;

    [SerializeField]
    [Tooltip("线段颜色")]
    private Color lineColor = Color.white;

    [Header("正弦波设置")]
    [SerializeField]
    [Tooltip("波长")]
    private float waveLength = 1f;

    [SerializeField]
    [Tooltip("波幅")]
    private float waveAmplitude = 0.5f;

    [SerializeField]
    [Tooltip("波频")]
    private float waveFrequency = 1f;

    [SerializeField]
    [Tooltip("波相位偏移")]
    private float wavePhase = 0f;

    [Header("运动设置")]
    [SerializeField]
    [Tooltip("移动方向")]
    private Vector3 moveDirection = Vector3.right;

    [SerializeField]
    [Tooltip("移动速度")]
    private float moveSpeed = 1f;

    [SerializeField]
    [Tooltip("是否循环移动")]
    private bool loopMovement = true;

    [SerializeField]
    [Tooltip("循环范围")]
    private float loopRange = 10f;

    [Header("顶点设置")]
    [SerializeField]
    [Tooltip("顶点数量")]
    private int vertexCount = 50;

    [SerializeField]
    [Tooltip("是否实时更新")]
    private bool updateInRealtime = true;

    // 内部组件
    private LineRenderer lineRenderer;
    private Vector3[] vertices;
    private Vector3 startPosition;
    private float moveOffset = 0f;

    void Awake()
    {
        InitializeLineRenderer();
        startPosition = transform.position;
    }

    void Start()
    {
        GenerateVertices();
        UpdateLineRenderer();
    }

    void Update()
    {
        if (updateInRealtime)
        {
            // 更新移动偏移
            moveOffset += moveSpeed * Time.deltaTime;

            if (loopMovement)
            {
                moveOffset = Mathf.Repeat(moveOffset, loopRange);
            }

            // 更新顶点位置
            UpdateVertices();

            // 更新LineRenderer
            UpdateLineRenderer();
        }
    }

    /// <summary>
    /// 初始化LineRenderer
    /// </summary>
    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 设置材质
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            // 创建默认材质
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // 设置基本属性
        lineRenderer.material.color = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = vertexCount;
        lineRenderer.useWorldSpace = false; // 使用本地坐标
    }

    /// <summary>
    /// 生成顶点数组
    /// </summary>
    private void GenerateVertices()
    {
        vertices = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            float t = (float)i / (vertexCount - 1); // 归一化位置 (0-1)
            float x = t * lineLength;

            // 正弦波计算
            float y = Mathf.Sin((x / waveLength + wavePhase) * 2f * Mathf.PI) * waveAmplitude;

            vertices[i] = new Vector3(x, y, 0f);
        }
    }

    /// <summary>
    /// 更新顶点位置（包含移动效果）
    /// </summary>
    private void UpdateVertices()
    {
        for (int i = 0; i < vertexCount; i++)
        {
            float t = (float)i / (vertexCount - 1); // 归一化位置 (0-1)
            float x = t * lineLength;

            // 添加移动偏移
            float movedX = x + moveOffset;

            // 正弦波计算（包含时间变化）
            float waveInput = (movedX / waveLength + wavePhase + Time.time * waveFrequency) * 2f * Mathf.PI;
            float y = Mathf.Sin(waveInput) * waveAmplitude;

            vertices[i] = new Vector3(x, y, 0f);
        }
    }

    /// <summary>
    /// 更新LineRenderer
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (lineRenderer != null && vertices != null)
        {
            lineRenderer.SetPositions(vertices);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }

    /// <summary>
    /// 手动刷新线段
    /// </summary>
    public void RefreshLine()
    {
        GenerateVertices();
        UpdateLineRenderer();
    }

    /// <summary>
    /// 设置线段材质
    /// </summary>
    public void SetMaterial(Material material)
    {
        lineMaterial = material;
        if (lineRenderer != null)
        {
            lineRenderer.material = material;
        }
    }

    /// <summary>
    /// 设置线段颜色
    /// </summary>
    public void SetColor(Color color)
    {
        lineColor = color;
        UpdateLineRenderer();
    }

    /// <summary>
    /// 设置波幅
    /// </summary>
    public void SetWaveAmplitude(float amplitude)
    {
        waveAmplitude = amplitude;
        if (!updateInRealtime)
        {
            RefreshLine();
        }
    }

    /// <summary>
    /// 设置波频
    /// </summary>
    public void SetWaveFrequency(float frequency)
    {
        waveFrequency = frequency;
    }

    /// <summary>
    /// 设置波相位
    /// </summary>
    public void SetWavePhase(float phase)
    {
        wavePhase = phase;
        if (!updateInRealtime)
        {
            RefreshLine();
        }
    }

    /// <summary>
    /// 设置移动速度
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// 设置波长
    /// </summary>
    public void SetWaveLength(float length)
    {
        waveLength = length;
        if (!updateInRealtime)
        {
            RefreshLine();
        }
    }

    /// <summary>
    /// 重置移动位置
    /// </summary>
    public void ResetMovement()
    {
        moveOffset = 0f;
        transform.position = startPosition;
    }

    /// <summary>
    /// 获取当前顶点数组
    /// </summary>
    public Vector3[] GetVertices()
    {
        return vertices;
    }

    void OnValidate()
    {
        if (Application.isPlaying && lineRenderer != null)
        {
            RefreshLine();
        }
    }
}