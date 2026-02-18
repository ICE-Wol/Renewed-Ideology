using UnityEngine;

namespace Prota.Unity
{
    // 存储渐变数据和贴图大小, 可以生成对应的渐变贴图.
	[CreateAssetMenu(fileName = "Gradient Texture", menuName = "Prota Framework/Gradient Texture")]
    public class GradientTexture : ScriptableObject
    {
        // 渐变数据
        public Gradient gradient = new Gradient();
        
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

