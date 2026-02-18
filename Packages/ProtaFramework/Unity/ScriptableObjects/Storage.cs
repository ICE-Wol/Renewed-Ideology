using UnityEngine;

// 这个用来存一些奇怪的数据, 不会在游戏程序里被用到.
[CreateAssetMenu(fileName = "Storage", menuName = "Prota Framework/Storage")]
public class Storage : ScriptableObject
{
    public int interger;
    public float number;
    public string text;
    public bool boolean;
    public Vector2 vector2;
    public Vector3 vector3;
    public Vector4 vector4;
    public Color color;
    public AnimationCurve animationCurve;
    [GradientUsage(true, ColorSpace.Linear)] public Gradient gradientLinear;
    [GradientUsage(true, ColorSpace.Gamma)] public Gradient gradientGamma;
    
}
