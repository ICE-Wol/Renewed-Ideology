using UnityEngine;
using UnityEditor;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomEditor(typeof(CurveTexture), false)]
    public class CurveTextureInspector : UnityEditor.Editor
    {
        SerializedProperty curveProperty;
        SerializedProperty textureSizeProperty;
        
        void OnEnable()
        {
            curveProperty = serializedObject.FindProperty("curve");
            textureSizeProperty = serializedObject.FindProperty("textureSize");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 检测 curve 变化
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(curveProperty);
            bool curveChanged = EditorGUI.EndChangeCheck();
            
            // 检测 textureSize 变化
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(textureSizeProperty);
            bool sizeChanged = EditorGUI.EndChangeCheck();
            
            serializedObject.ApplyModifiedProperties();
            
            var target = this.target as CurveTexture;
            
            // 如果 curve 或 textureSize 被修改, 自动生成贴图
            if (curveChanged || sizeChanged)
            {
                GenerateTexture(target, silent: true);
            }
            
            EditorGUILayout.Space();
            
            // 生成贴图按钮 (手动触发)
            if (GUILayout.Button("Generate Curve Texture"))
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
        
        void GenerateTexture(CurveTexture curveTexture, bool silent = false)
        {
            if (curveTexture.curve == null)
            {
                if (!silent) Debug.LogError("Curve is null. Cannot generate texture.");
                return;
            }
            
            if (curveTexture.textureSize.x <= 0 || curveTexture.textureSize.y <= 0)
            {
                if (!silent) Debug.LogError("Texture size must be greater than 0. Cannot generate texture.");
                return;
            }
            
            Texture2D texture = curveTexture.GetTexture();
            
            if (texture != null)
            {
                // 使用 Reinitialize 重新设置贴图尺寸，保留 asset 对象和 GUID
                texture.Reinitialize(
                    curveTexture.textureSize.x,
                    curveTexture.textureSize.y,
                    TextureFormat.RGBA32,
                    false
                );
            }
            else
            {
                // 创建新贴图
                texture = new Texture2D(
                    curveTexture.textureSize.x,
                    curveTexture.textureSize.y,
                    TextureFormat.RGBA32,
                    false
                );
                texture.name = "CurveTexture";
                AssetDatabase.AddObjectToAsset(texture, curveTexture);
            }
            
            // 填充像素
            var pixels = new Color[texture.width * texture.height];
            float widthInv = texture.width > 1 ? 1f / (texture.width - 1) : 0f;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float t = x * widthInv;
                    float value = curveTexture.curve.Evaluate(t);
                    // 将 0-1 的值转换为灰度颜色 (0=黑色, 1=白色)
                    pixels[y * texture.width + x] = new Color(value, value, value, 1f);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            // 使用 SerializedObject 设置 generatedTexture 字段
            var serializedObject = new SerializedObject(curveTexture);
            var textureProperty = serializedObject.FindProperty("generatedTexture");
            textureProperty.objectReferenceValue = texture;
            serializedObject.ApplyModifiedProperties();
            
            // 标记资源为已修改
            EditorUtility.SetDirty(texture);
            EditorUtility.SetDirty(curveTexture);
            AssetDatabase.SaveAssets();
            
            if (!silent)
            {
                Debug.Log($"Curve texture generated: {texture.width}x{texture.height}");
            }
        }
    }
}

