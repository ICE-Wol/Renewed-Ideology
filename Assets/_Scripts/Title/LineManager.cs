using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 线段管理器 - 管理所有LineCtrl并进行参数随机化
/// </summary>
public class LineManager : MonoBehaviour
{
    [Header("随机化设置")]
    [SerializeField]
    [Tooltip("波幅随机范围")]
    private Vector2 waveAmplitudeRange = new Vector2(0.3f, 0.8f);

    [SerializeField]
    [Tooltip("波长随机范围")]
    private Vector2 waveLengthRange = new Vector2(0.8f, 1.5f);

    [SerializeField]
    [Tooltip("波频随机范围")]
    private Vector2 waveFrequencyRange = new Vector2(0.8f, 1.2f);

    [SerializeField]
    [Tooltip("移动速度随机范围")]
    private Vector2 moveSpeedRange = new Vector2(0.5f, 1.5f);

    [SerializeField]
    [Tooltip("相位偏移随机范围（弧度）")]
    private Vector2 phaseOffsetRange = new Vector2(0f, 2f * Mathf.PI);

    [Header("管理设置")]
    [SerializeField]
    [Tooltip("是否在启动时自动随机化")]
    private bool randomizeOnStart = true;

    [SerializeField]
    [Tooltip("随机化种子")]
    private int randomSeed = 0;

    // 管理的LineCtrl列表
    private List<LineCtrl> lineControls = new List<LineCtrl>();

    void Start()
    {
        // 查找场景中的所有LineCtrl
        FindAllLineControls();

        // 如果设置了自动随机化
        if (randomizeOnStart)
        {
            RandomizeAllLines();
        }
    }

    /// <summary>
    /// 查找场景中的所有LineCtrl
    /// </summary>
    private void FindAllLineControls()
    {
        lineControls.Clear();
        LineCtrl[] foundControls = FindObjectsByType<LineCtrl>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (LineCtrl ctrl in foundControls)
        {
            lineControls.Add(ctrl);
        }

        Debug.Log($"找到 {lineControls.Count} 个LineCtrl");
    }

    /// <summary>
    /// 随机化所有线段的参数
    /// </summary>
    public void RandomizeAllLines()
    {
        // 设置随机种子
        if (randomSeed != 0)
        {
            Random.InitState(randomSeed);
        }

        foreach (LineCtrl ctrl in lineControls)
        {
            RandomizeLineParameters(ctrl);
        }
    }

    /// <summary>
    /// 随机化单个线段的参数
    /// </summary>
    private void RandomizeLineParameters(LineCtrl ctrl)
    {
        if (ctrl == null) return;

        // 随机化波幅
        float randomAmplitude = Random.Range(waveAmplitudeRange.x, waveAmplitudeRange.y);
        ctrl.SetWaveAmplitude(randomAmplitude);

        // 随机化波长
        float randomLength = Random.Range(waveLengthRange.x, waveLengthRange.y);
        ctrl.SetWaveLength(randomLength);

        // 随机化波频
        float randomFrequency = Random.Range(waveFrequencyRange.x, waveFrequencyRange.y);
        ctrl.SetWaveFrequency(randomFrequency);

        // 随机化移动速度
        float randomSpeed = Random.Range(moveSpeedRange.x, moveSpeedRange.y);
        ctrl.SetMoveSpeed(randomSpeed);

        // 随机化相位偏移
        float randomPhase = Random.Range(phaseOffsetRange.x, phaseOffsetRange.y);
        ctrl.SetWavePhase(randomPhase);
    }

    /// <summary>
    /// 重新查找线段
    /// </summary>
    public void RefreshLineControls()
    {
        FindAllLineControls();
    }

    /// <summary>
    /// 获取所有管理的LineCtrl
    /// </summary>
    public List<LineCtrl> GetAllLineControls()
    {
        return new List<LineCtrl>(lineControls);
    }

    /// <summary>
    /// 获取线段数量
    /// </summary>
    public int GetLineCount()
    {
        return lineControls.Count;
    }

    /// <summary>
    /// 设置所有线段的移动速度
    /// </summary>
    public void SetAllMoveSpeed(float speed)
    {
        foreach (LineCtrl ctrl in lineControls)
        {
            if (ctrl != null)
            {
                ctrl.SetMoveSpeed(speed);
            }
        }
    }

    /// <summary>
    /// 设置所有线段的波频
    /// </summary>
    public void SetAllWaveFrequency(float frequency)
    {
        foreach (LineCtrl ctrl in lineControls)
        {
            if (ctrl != null)
            {
                ctrl.SetWaveFrequency(frequency);
            }
        }
    }

    /// <summary>
    /// 设置所有线段的波幅
    /// </summary>
    public void SetAllWaveAmplitude(float amplitude)
    {
        foreach (LineCtrl ctrl in lineControls)
        {
            if (ctrl != null)
            {
                ctrl.SetWaveAmplitude(amplitude);
            }
        }
    }

    /// <summary>
    /// 重置所有线段的移动位置
    /// </summary>
    public void ResetAllMovement()
    {
        foreach (LineCtrl ctrl in lineControls)
        {
            if (ctrl != null)
            {
                ctrl.ResetMovement();
            }
        }
    }

    /// <summary>
    /// 设置随机种子
    /// </summary>
    public void SetRandomSeed(int seed)
    {
        randomSeed = seed;
    }

    /// <summary>
    /// 获取当前随机种子
    /// </summary>
    public int GetRandomSeed()
    {
        return randomSeed;
    }
}