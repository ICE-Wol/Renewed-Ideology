using UnityEngine;

namespace Prota.Unity
{
    // 存储曲线数据和贴图大小, 可以生成对应的渐变贴图. 曲线值 0 为黑色, 1 为白色.
    [CreateAssetMenu(fileName = "Curve Texture", menuName = "Prota Framework/Curve Texture")]
    public class CurveTexture : ScriptableObject
    {
        // 曲线数据 (0=黑色, 1=白色)
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        // 贴图大小
        public Vector2Int textureSize = new Vector2Int(256, 1);
        
        // 生成的贴图引用 (作为 sub-asset 存储)
        [SerializeField] Texture2D generatedTexture;
        
        // 获取生成的贴图
        public Texture2D GetTexture()
        {
            return generatedTexture;
        }
    }
}

