using UnityEngine;
using UnityEditor;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomEditor(typeof(GradientTexture), false)]
    public class GradientTextureInspector : UnityEditor.Editor
    {
        SerializedProperty gradientProperty;
        SerializedProperty textureSizeProperty;
        
        void OnEnable()
        {
            gradientProperty = serializedObject.FindProperty("gradient");
            textureSizeProperty = serializedObject.FindProperty("textureSize");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 检测 gradient 变化
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gradientProperty);
            bool gradientChanged = EditorGUI.EndChangeCheck();
            
            // 检测 textureSize 变化
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(textureSizeProperty);
            bool sizeChanged = EditorGUI.EndChangeCheck();
            
            serializedObject.ApplyModifiedProperties();
            
            var target = this.target as GradientTexture;
            
            // 如果 gradient 或 textureSize 被修改, 自动生成贴图
            if (gradientChanged || sizeChanged)
            {
                GenerateTexture(target, silent: true);
            }
            
            EditorGUILayout.Space();
            
            // 生成贴图按钮 (手动触发)
            if (GUILayout.Button("Generate Gradient Texture"))
            {
                GenerateTexture(target);
            }
            
            // 显示生成的贴图预览
            if (target.GetTexture() != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Generated Texture:", EditorStyles.boldLabel);
                var texture = target.GetTexture();
                var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));
                rect.height = Mathf.Min(rect.width * texture.height / texture.width, 200);
                EditorGUI.DrawPreviewTexture(rect, texture);
            }
        }
        
        void GenerateTexture(GradientTexture gradientTexture, bool silent = false)
        {
            if (gradientTexture.gradient == null)
            {
                if (!silent) Debug.LogError("Gradient is null. Cannot generate texture.");
                return;
            }
            
            if (gradientTexture.textureSize.x <= 0 || gradientTexture.textureSize.y <= 0)
            {
                if (!silent) Debug.LogError("Texture size must be greater than 0. Cannot generate texture.");
                return;
            }
            
            Texture2D texture = gradientTexture.GetTexture();
            
            if (texture != null)
            {
                // 使用 Reinitialize 重新设置贴图尺寸，保留 asset 对象和 GUID
                texture.Reinitialize(
                    gradientTexture.textureSize.x,
                    gradientTexture.textureSize.y,
                    TextureFormat.RGBA32,
                    false
                );
            }
            else
            {
                // 创建新贴图
                texture = new Texture2D(
                    gradientTexture.textureSize.x,
                    gradientTexture.textureSize.y,
                    TextureFormat.RGBA32,
                    false
                );
                texture.name = "GradientTexture";
                AssetDatabase.AddObjectToAsset(texture, gradientTexture);
            }
            
            // 填充像素
            var pixels = new Color[texture.width * texture.height];
            float widthInv = texture.width > 1 ? 1f / (texture.width - 1) : 0f;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float t = x * widthInv;
                    pixels[y * texture.width + x] = gradientTexture.gradient.Evaluate(t);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            // 使用 SerializedObject 设置 generatedTexture 字段
            var serializedObject = new SerializedObject(gradientTexture);
            var textureProperty = serializedObject.FindProperty("generatedTexture");
            textureProperty.objectReferenceValue = texture;
            serializedObject.ApplyModifiedProperties();
            
            // 标记资源为已修改
            EditorUtility.SetDirty(texture);
            EditorUtility.SetDirty(gradientTexture);
            AssetDatabase.SaveAssets();
            
            if (!silent)
            {
                Debug.Log($"Gradient texture generated: {texture.width}x{texture.height}");
            }
        }
    }
}

