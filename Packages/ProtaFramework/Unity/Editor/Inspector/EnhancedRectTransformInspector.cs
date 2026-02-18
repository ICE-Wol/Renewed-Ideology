using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Prota.Unity;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;
    using Settings = Prota.Editor.EnhancedInspectorSettings;


    [CustomEditor(typeof(RectTransform), true)]
    [CanEditMultipleObjects]
    public class CustomRectTransformInspector : Editor
    {
        Editor originalEditor = null;
        
        void OnEnable()
        {
            originalEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.RectTransformEditor, UnityEditor"));
        }
    
        void OnDisable()
        {
            DestroyImmediate(originalEditor);
            originalEditor = null;
        }
        
    
        public override void OnInspectorGUI()
        {
            if(Settings.useOriginalInspector.value)
            {
                originalEditor.OnInspectorGUI();
            }
            
            CustomTransformInspector.DrawOps(serializedObject, targets);
            CustomTransformInspector.DrawCommonButtons(serializedObject, targets);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
