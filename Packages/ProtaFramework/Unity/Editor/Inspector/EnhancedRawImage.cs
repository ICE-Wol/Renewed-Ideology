using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using Prota.Unity;
using System.Runtime.CompilerServices;

namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(RawImage), true)]
    [CanEditMultipleObjects]
    public class CustomRawImageInspector : Editor
    {
        Editor originalEditor = null;
        
        void OnEnable()
        {
            
        }
    
        void OnDisable()
        {
            DestroyImmediate(originalEditor);
            originalEditor = null;
        }

        bool showOriginalInspector = false;
    
        public override void OnInspectorGUI()
        {
            var n = targets.Length;
            
            if(!useEnhancedInspector.value || n > 1)
            {
                ShowDefaultInspector();
                return;
            }
            
            var textureProp = serializedObject.FindProperty("m_Texture");
            var colorProp = serializedObject.FindProperty("m_Color");
            var materialProp = serializedObject.FindProperty("m_Material");
            var uvRectProp = serializedObject.FindProperty("m_UVRect");
            var raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
            
            Undo.RecordObjects(targets, "RawImage Inspector");
            
            // Texture
            EditorGUILayout.PropertyField(textureProp, new GUIContent("Texture"));
            
            // 颜色字段
            var color = colorProp.colorValue;
            EditorGUILayout.PropertyField(colorProp, new GUIContent("Color"));
            
            // HDR颜色字段
            color = EditorGUILayout.ColorField(new GUIContent("Color (HDR)"), colorProp.colorValue, true, true, true);
            
            // RGBA数值调整
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("RGBA");
            color.r = EditorGUILayout.FloatField(color.r);
            color.g = EditorGUILayout.FloatField(color.g);
            color.b = EditorGUILayout.FloatField(color.b);
            color.a = EditorGUILayout.FloatField(color.a);
            colorProp.colorValue = color;
            EditorGUILayout.EndHorizontal();
            
            // Material
            EditorGUILayout.PropertyField(materialProp, new GUIContent("Material"));
            
            // UV Rect
            EditorGUILayout.PropertyField(uvRectProp, new GUIContent("UV Rect"));
            
            // Raycast Target
            EditorGUILayout.PropertyField(raycastTargetProp, new GUIContent("Raycast Target"));
            
            EditorGUILayout.Space();
            
            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if(showOriginalInspector = EditorGUILayout.Foldout(showOriginalInspector, "Show Original Inspector"))
            {
                ShowDefaultInspector();
            }
        }
        
        void ShowDefaultInspector()
        {
            serializedObject.Update();
            if(originalEditor == null || originalEditor.target != this.target)
            {
                if(originalEditor != null) DestroyImmediate(originalEditor);
                originalEditor = CreateEditor(targets, Type.GetType("UnityEditor.UI.RawImageEditor, UnityEditor.UI"));
            }
            originalEditor.OnInspectorGUI();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        static EditorPrefEntryBool useEnhancedInspector = new EditorPrefEntryBool("ProtaEditor.UseEnhancedRawImageInspector", true);
        
        [MenuItem("ProtaFramework/Functionality/RawImage Use Enhanced Inspector", false, 2900)]
        static void ToggleUseEnhancedInspector()
        {
            useEnhancedInspector.value = !useEnhancedInspector.value;
            Menu.SetChecked("ProtaFramework/Functionality/RawImage Use Enhanced Inspector", useEnhancedInspector.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/RawImage Use Enhanced Inspector", true)]
        static bool ToggleUseEnhancedInspectorValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/RawImage Use Enhanced Inspector", useEnhancedInspector.value);
            return true;
        }
    }
} 