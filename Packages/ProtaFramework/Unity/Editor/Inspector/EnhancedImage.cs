using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using Prota.Unity;
using System.Runtime.CompilerServices;

namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(Image), true)]
    [CanEditMultipleObjects]
    public class CustomImageInspector : Editor
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
            
            var sourceImageProp = serializedObject.FindProperty("m_Sprite");
            var colorProp = serializedObject.FindProperty("m_Color");
            var materialProp = serializedObject.FindProperty("m_Material");
            var imageTypeProp = serializedObject.FindProperty("m_Type");
            var preserveAspectProp = serializedObject.FindProperty("m_PreserveAspect");
            var fillMethodProp = serializedObject.FindProperty("m_FillMethod");
            var fillOriginProp = serializedObject.FindProperty("m_FillOrigin");
            var fillAmountProp = serializedObject.FindProperty("m_FillAmount");
            var fillClockwiseProp = serializedObject.FindProperty("m_FillClockwise");
            var raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
            var pixelsPerUnitMultiplierProp = serializedObject.FindProperty("m_PixelsPerUnitMultiplier");
            
            Undo.RecordObjects(targets, "Image Inspector");
            
            // Source Image
            EditorGUILayout.PropertyField(sourceImageProp, new GUIContent("Source Image"));
            
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
            
            // Image Type
            EditorGUILayout.PropertyField(imageTypeProp, new GUIContent("Image Type"));
            
            // Preserve Aspect
            EditorGUILayout.PropertyField(preserveAspectProp, new GUIContent("Preserve Aspect"));
            
            // Pixels Per Unit Multiplier
            EditorGUILayout.PropertyField(pixelsPerUnitMultiplierProp, new GUIContent("Pixels Per Unit Multiplier"));
            
            // Fill Method (only show when Image Type is Filled)
            if (imageTypeProp.enumValueIndex == (int)Image.Type.Filled)
            {
                EditorGUILayout.PropertyField(fillMethodProp, new GUIContent("Fill Method"));
                EditorGUILayout.PropertyField(fillOriginProp, new GUIContent("Fill Origin"));
                EditorGUILayout.PropertyField(fillAmountProp, new GUIContent("Fill Amount"));
                EditorGUILayout.PropertyField(fillClockwiseProp, new GUIContent("Fill Clockwise"));
            }
            
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
                originalEditor = CreateEditor(targets, Type.GetType("UnityEditor.UI.ImageEditor, UnityEditor.UI"));
            }
            originalEditor.OnInspectorGUI();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        static EditorPrefEntryBool useEnhancedInspector = new EditorPrefEntryBool("ProtaEditor.UseEnhancedImageInspector", true);
        
        [MenuItem("ProtaFramework/Functionality/Image Use Enhanced Inspector", false, 2800)]
        static void ToggleUseEnhancedInspector()
        {
            useEnhancedInspector.value = !useEnhancedInspector.value;
            Menu.SetChecked("ProtaFramework/Functionality/Image Use Enhanced Inspector", useEnhancedInspector.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/Image Use Enhanced Inspector", true)]
        static bool ToggleUseEnhancedInspectorValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Image Use Enhanced Inspector", useEnhancedInspector.value);
            return true;
        }
    }
} 