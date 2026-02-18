using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;

public enum TitleDecoMode
{
    LinearMove,  // 模式1：横向移动
    CircleRotate // 模式2：绕圆旋转
}

public class TitleDecoCubeManager : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField]
    private GameObject decoCubePrefab;

    [Header("Generation Settings")]
    [SerializeField]
    private int cubeCount = 10;

    [Header("Mode Settings")]
    [SerializeField]
    private TitleDecoMode currentMode = TitleDecoMode.LinearMove;

    [Header("Approach Settings")]
    [SerializeField]
    private float positionApproachRate = 10f;

    [SerializeField]
    private float scaleApproachRate = 10f;

    [Header("=== Mode 1: Linear Move Settings ===")]
    [SerializeField]
    private Vector2 xRange = new Vector2(-5.0f, 5.0f);

    [SerializeField]
    private float rotationSpeed = 45.0f;

    [SerializeField]
    private AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1.0f);

    [SerializeField]
    private Vector3 maxScale = new Vector3(0.5f, 5.0f, 0.5f);

    [SerializeField]
    private AnimationCurve yOffsetCurve = AnimationCurve.Linear(0, 0, 1, 0.0f); // y方向平移曲线

    [SerializeField]
    private float maxYOffset = 1.0f; // y方向最大偏移量

    [SerializeField]
    private float yPosition = 0.0f;

    [SerializeField]
    private float zPosition = 0.0f;

    [SerializeField]
    private float moveSpeed = 2.0f;

    [SerializeField]
    private bool moveForward = true;

    [Header("=== Mode 2: Circle Rotate Settings ===")]
    [SerializeField]
    private Vector3 circleCenter = Vector3.zero;

    [SerializeField]
    private Vector3 circleScale = new Vector3(1f, 1f, 1f); // 模式2的统一scale

    [Header("Mode 2: Random Settings")]
    [SerializeField]
    private Vector2 randomRadiusRange = new Vector2(2f, 5f); // 半径随机范围

    [SerializeField]
    private Vector2 randomOrientationRangeX = new Vector2(-30f, 30f); // 初始旋转X范围

    [SerializeField]
    private Vector2 randomOrientationRangeY = new Vector2(0f, 360f);  // 初始旋转Y范围

    [SerializeField]
    private Vector2 randomOrientationRangeZ = new Vector2(-30f, 30f); // 初始旋转Z范围

    [SerializeField]
    private Vector2 randomSpeedRange = new Vector2(-15f, 15f); // 转速范围（每个轴）

    // 运行时生成的值
    private float circleRadius = 3.0f; // 圆的半径
    private Vector3 circleOrientation = Vector3.zero; // 圆平面的静态倾斜角度 (XYZ)
    private Vector3 circleRotationSpeed = Vector3.zero; // 绕XYZ轴的旋转速度

    // 内部数据
    private List<TitleDecoCubeCtrl> cubes = new List<TitleDecoCubeCtrl>();
    private List<Vector3> currentPositions = new List<Vector3>(); // 当前实际位置
    private List<Vector3> currentScales = new List<Vector3>();    // 当前实际scale
    private List<float> cubeAngles = new List<float>();           // 模式2中每个cube在圆上的角度

    private float moveTimer = 0.0f;
    private Vector3 circleRotation = Vector3.zero; // 圆的当前旋转角度

    void Start()
    {
        GenerateCubes();
    }

    void Update()
    {
        // 检测鼠标点击切换模式
        if (Input.GetMouseButtonDown(0))
        {
            ToggleMode();
        }

        // 更新圆的旋转（模式2使用）
        circleRotation += circleRotationSpeed * Time.deltaTime;

        // 更新所有cube
        UpdateCubes();
    }

    private void ToggleMode()
    {
        if (currentMode == TitleDecoMode.LinearMove)
        {
            currentMode = TitleDecoMode.CircleRotate;
            // 进入模式2时随机生成初始旋转和转速
            RandomizeCircleSettings();
        }
        else
        {
            currentMode = TitleDecoMode.LinearMove;
        }
    }

    private void RandomizeCircleSettings()
    {
        // 随机生成半径
        circleRadius = Random.Range(randomRadiusRange.x, randomRadiusRange.y);

        // 随机生成圆平面的初始旋转角度
        circleOrientation = new Vector3(
            Random.Range(randomOrientationRangeX.x, randomOrientationRangeX.y),
            Random.Range(randomOrientationRangeY.x, randomOrientationRangeY.y),
            Random.Range(randomOrientationRangeZ.x, randomOrientationRangeZ.y)
        );

        // 随机生成转速（每个轴独立随机，但保持较小的值）
        circleRotationSpeed = new Vector3(
            Random.Range(randomSpeedRange.x, randomSpeedRange.y),
            Random.Range(randomSpeedRange.x, randomSpeedRange.y),
            Random.Range(randomSpeedRange.x, randomSpeedRange.y)
        );

        // 重置当前旋转角度
        circleRotation = Vector3.zero;
    }

    private void GenerateCubes()
    {
        if (decoCubePrefab == null)
        {
            Debug.LogError("DecoCube Prefab is not assigned!");
            return;
        }

        // 清除之前的cubes
        foreach (var cube in cubes)
        {
            if (cube != null)
            {
                Destroy(cube.gameObject);
            }
        }
        cubes.Clear();
        currentPositions.Clear();
        currentScales.Clear();
        cubeAngles.Clear();

        // 计算spacing
        float spacing = (cubeCount > 1) ? (xRange.y - xRange.x) / (cubeCount - 1) : 0;

        // 生成新的cubes
        for (int i = 0; i < cubeCount; i++)
        {
            // 计算初始位置（均分）
            float baseXPos = xRange.x + i * spacing;
            Vector3 localPosition = new Vector3(baseXPos, yPosition, zPosition);

            // 生成prefab
            GameObject cubeObj = Instantiate(decoCubePrefab, transform);
            cubeObj.transform.localPosition = localPosition;

            // 获取控制脚本
            TitleDecoCubeCtrl cubeCtrl = cubeObj.GetComponent<TitleDecoCubeCtrl>();
            if (cubeCtrl == null)
            {
                cubeCtrl = cubeObj.AddComponent<TitleDecoCubeCtrl>();
            }

            // 设置旋转速度
            cubeCtrl.rotationSpeed = rotationSpeed;

            // 设置旋转初始偏移量
            float rotationOffset = (i * 360.0f / cubeCount) % 360.0f;
            cubeCtrl.SetRotationOffset(rotationOffset);

            cubes.Add(cubeCtrl);

            // 初始化当前位置和scale
            currentPositions.Add(localPosition);
            currentScales.Add(cubeObj.transform.localScale);

            // 初始化圆上的角度（均匀分布）
            float angle = (360.0f / cubeCount) * i;
            cubeAngles.Add(angle);
        }
    }

    private void UpdateCubes()
    {
        if (cubes.Count == 0) return;

        moveTimer += Time.deltaTime;

        for (int i = 0; i < cubes.Count; i++)
        {
            TitleDecoCubeCtrl cube = cubes[i];
            if (cube == null) continue;

            // 计算目标位置和scale
            Vector3 targetPosition;
            Vector3 targetScale;
            Quaternion targetRotation = Quaternion.identity;

            if (currentMode == TitleDecoMode.LinearMove)
            {
                // 模式1：横向移动
                CalculateMode1Target(i, out targetPosition, out targetScale);
            }
            else
            {
                // 模式2：绕圆旋转
                CalculateMode2Target(i, out targetPosition, out targetScale, out targetRotation);
            }

            // 使用 ApproachRef 平滑趋近目标值
            Vector3 pos = currentPositions[i];
            Vector3 scale = currentScales[i];

            pos.ApproachRef(targetPosition, positionApproachRate);
            scale.ApproachRef(targetScale, scaleApproachRate);

            currentPositions[i] = pos;
            currentScales[i] = scale;

            // 应用到cube
            cube.transform.localPosition = pos;
            cube.transform.localScale = scale;

            // 模式2时应用旋转（面朝圆心）
            if (currentMode == TitleDecoMode.CircleRotate)
            {
                cube.transform.localRotation = Quaternion.Slerp(
                    cube.transform.localRotation,
                    targetRotation,
                    Time.deltaTime * positionApproachRate * 0.5f
                );
            }
        }
    }

    private void CalculateMode1Target(int index, out Vector3 targetPosition, out Vector3 targetScale)
    {
        // 计算每个cube的基础x位置（均分）
        float spacing = (cubes.Count > 1) ? (xRange.y - xRange.x) / (cubes.Count - 1) : 0;
        float baseXPos = xRange.x + index * spacing;

        // 使用全局时间计算每个方块的当前位置（在范围内循环）
        float cycleLength = xRange.y - xRange.x;
        float moveDirection = moveForward ? 1.0f : -1.0f;
        float totalMovement = moveSpeed * moveDirection * moveTimer;

        // 每个方块的位置 = 基础位置 + 循环移动
        float currentPos = baseXPos + (totalMovement % cycleLength);

        // 确保位置在范围内
        if (currentPos > xRange.y)
            currentPos -= cycleLength;
        else if (currentPos < xRange.x)
            currentPos += cycleLength;

        // 根据当前X轴位置在整个范围内的比例计算scale
        float normalizedX = Mathf.InverseLerp(xRange.x, xRange.y, currentPos);
        float curveValue = scaleCurve.Evaluate(normalizedX);
        targetScale = curveValue * maxScale;

        // 根据normalizedX计算y方向偏移量
        float yOffset = yOffsetCurve.Evaluate(normalizedX) * maxYOffset;
        targetPosition = new Vector3(currentPos, yPosition + yOffset, zPosition);
    }

    private void CalculateMode2Target(int index, out Vector3 targetPosition, out Vector3 targetScale, out Quaternion targetRotation)
    {
        // 获取该cube在圆上的基础角度
        float baseAngle = cubeAngles[index];

        // 计算在圆上的位置
        // 首先在XZ平面上计算基础圆位置（加上动态旋转的Y分量）
        float angleRad = (baseAngle + circleRotation.y) * Mathf.Deg2Rad;
        Vector3 localCirclePos = new Vector3(
            Mathf.Cos(angleRad) * circleRadius,
            0,
            Mathf.Sin(angleRad) * circleRadius
        );

        // 应用动态旋转（X和Z轴分量）
        Quaternion dynamicRotation = Quaternion.Euler(circleRotation.x, 0, circleRotation.z);
        localCirclePos = dynamicRotation * localCirclePos;

        // 应用静态圆平面倾斜角度
        Quaternion orientationQuat = Quaternion.Euler(circleOrientation);
        localCirclePos = orientationQuat * localCirclePos;

        // 加上圆心位置
        targetPosition = circleCenter + localCirclePos;

        // 统一scale
        targetScale = circleScale;

        // 计算面朝圆心的旋转
        Vector3 directionToCenter = (circleCenter - targetPosition).normalized;
        if (directionToCenter != Vector3.zero)
        {
            // 计算up向量，使其与圆平面法线对齐
            Vector3 circleUp = orientationQuat * Vector3.up;
            targetRotation = Quaternion.LookRotation(directionToCenter, circleUp);
        }
        else
        {
            targetRotation = Quaternion.identity;
        }
    }

    // 公开方法：手动设置模式
    public void SetMode(TitleDecoMode mode)
    {
        currentMode = mode;
    }

    // 公开方法：获取当前模式
    public TitleDecoMode GetCurrentMode()
    {
        return currentMode;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            GenerateCubes();
        }
    }
}
