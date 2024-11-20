using _Scripts.Tools;
using DG.Tweening;
using UnityEngine;

public class LakeBGCtrl : MonoBehaviour
{
    private static readonly int DistanceRange = Shader.PropertyToID("_DistanceRange");
    public Material fogMaterial;

    public float tarValue;
    public float value;

    public int timer;

    void Start()
    {
        // 初始化 value 为 0
        //value = 0.1f;

        // 使用 DOTween 创建从 0 到 1 的平滑曲线动画
        //DOTween.To(() => value, x => value = x, 1f, 2f) // 参数依次是：getter、setter、目标值、持续时间
        //    .SetEase(Ease.InOutSine); // 设置缓动函数，这里使用 InOutSine 实现平滑效果
        
        //.OnUpdate(() => Debug.Log("Current value: " + value)) // 可选：每帧更新时打印当前值
        //.OnComplete(() => Debug.Log("Animation complete!")); // 可选：动画完成时执行

        // 常用的缓动函数：
        // DOTween 提供多种缓动函数来控制动画的平滑度：
        //
        // Ease.Linear：线性动画，无加速和减速。
        // Ease.InOutQuad：开始和结束时加速和减速。
        // Ease.InOutCubic：更平滑的缓动效果。
        // Ease.InOutSine：非常适合创建从 0 到 1 的平滑曲线。
        value = tarValue = 1;
    }
    
    void Update() {
        if (timer < 210) tarValue.ApproachRef(10f, 128f);
        else tarValue = 9f + Mathf.Sin(timer * Mathf.Deg2Rad);
        timer++;
        
        value.ApproachRef(tarValue, 16f);
        fogMaterial.SetVector(DistanceRange, new Vector2(1f,value));
    }
}
