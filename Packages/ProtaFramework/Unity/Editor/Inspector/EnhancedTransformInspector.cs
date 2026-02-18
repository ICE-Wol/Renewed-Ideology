using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Prota.Unity;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;


namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;
    using Settings = Prota.Editor.EnhancedInspectorSettings;

    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class CustomTransformInspector : Editor
    {
        Editor originalEditor = null;
        
        void OnEnable()
        {
            originalEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
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
            
            DrawOps(serializedObject, targets);
            DrawCommonButtons(serializedObject, targets);
            serializedObject.ApplyModifiedProperties();
        }
        
        public enum ExistState
        {
            All,
            None,
            Some,
        } 
        
        public static void DrawOps(SerializedObject so, UnityEngine.Object[] targets)
        {
            if(Application.isPlaying) return;
            
            ProtaEditorUtils.SeperateLine(2);
            
            if(IsUI(targets))
            {
                // foreach(var t in targets)
                //     (t as Transform).GetOrCreate<RectTransform>();
                // 
                var localPosition = so.FindProperty("m_LocalPosition");
                var localRotation = so.FindProperty("m_LocalRotation");
                var localScale = so.FindProperty("m_LocalScale");
                var anchorMin = so.FindProperty("m_AnchorMin");
                var anchorMax = so.FindProperty("m_AnchorMax");
                var pivot = so.FindProperty("m_Pivot");
                var sizeDelta = so.FindProperty("m_SizeDelta");
                if(GUILayout.Button("Set Identity"))
                {
                    localPosition.vector3Value = Vector3.zero;
                    localRotation.quaternionValue = Quaternion.identity;
                    localScale.vector3Value = Vector3.one;
                    anchorMin.vector2Value = Vector2.one * 0.5f;
                    anchorMax.vector2Value = Vector2.one * 0.5f;
                    pivot.vector2Value = new Vector2(0.5f, 0.5f);
                    sizeDelta.vector2Value = Vector2.one;
                }
                if(GUILayout.Button("Set Follow Parent Size"))
                {
                    localPosition.vector3Value = Vector3.zero;
                    localRotation.quaternionValue = Quaternion.identity;
                    localScale.vector3Value = Vector3.one;
                    anchorMin.vector2Value = Vector2.zero;
                    anchorMax.vector2Value = Vector2.one;
                    pivot.vector2Value = new Vector2(0.5f, 0.5f);
                    sizeDelta.vector2Value = Vector2.zero;
                }
            }
            else if(IsNotUI(targets))
            {
                var localPosition = so.FindProperty("m_LocalPosition");
                var localRotation = so.FindProperty("m_LocalRotation");
                var localScale = so.FindProperty("m_LocalScale");
                if(GUILayout.Button("Set Identity"))
                {   
                    localPosition.vector3Value = Vector3.zero;
                    localRotation.quaternionValue = Quaternion.identity;
                    localScale.vector3Value = Vector3.one;
                }
            }
        }
        
        public static void DrawCommonButtons(SerializedObject so, UnityEngine.Object[] targets)
        {
            if(Application.isPlaying) return;
            EditorGUI.BeginChangeCheck();
            
            ProtaEditorUtils.SeperateLine(2);
            
            additionalDrawBefore?.Invoke(so, targets);
            
            bool isUI = IsUI(targets);
            bool isNotUI = IsNotUI(targets);
            
            EditorGUILayout.BeginHorizontal();
            if(isNotUI)
            {
                DrawComponentButton<SpriteRenderer>("SpriteRenderer", targets);
            }
            else if(isUI)
            {
                DrawComponentButton<Image>("Image", targets);
            }
            if(isNotUI)
            {
                DrawComponentButton<TextMeshPro>("TextMeshPro", targets);
            }
            else if(isUI)
            {
                DrawComponentButton<TextMeshProUGUI>("TextMeshPro UI", targets);
            }
            
            DrawComponentButton<SortingGroup>("SortingGroup", targets);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if(isNotUI)
            {
                DrawComponentButton<Rigidbody2D>("Rigidbody2D", targets);
                DrawComponentButton<BoxCollider2D>("BoxCollider2D", targets);
                DrawComponentButton<CircleCollider2D>("CircleCollider2D", targets);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            DrawComponentButton<ProtaTweener>("ProtaTweener", targets);
            DrawComponentButton<ParticleSystem>("ParticleSystem", targets);
            EditorGUILayout.EndHorizontal();
            
            additionalDrawAfter?.Invoke(so, targets);
            
            if(EditorGUI.EndChangeCheck())
            {
                foreach (Transform t in targets)
                    EditorUtility.SetDirty(t.gameObject);
            }
        }
        
        public static void DrawComponentButton<T>(string buttonLabel, UnityEngine.Object[] targets)
            where T : Component
        {
            EditorGUILayout.BeginHorizontal();
            var existState = GetExistState<T>(targets);
            var cc = GUI.color;
            if(existState == ExistState.All) GUI.color = new Color(0.6f, 0.7f, 1f, 1f);
            else if(existState == ExistState.Some) GUI.color = new Color(0.9f, 1f, 0.6f, 1f);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(T)).image;
            var content = new GUIContent(buttonLabel, icon);
            if(GUILayout.Button(content, GUILayout.MinWidth(0), GUILayout.Height(16), GUILayout.ExpandWidth(true)))
            {
                if(existState != ExistState.All)
                {
                    foreach (var t in Selection.transforms)
                        t.gameObject.GetOrCreate<T>();
                }
                else if(existState == ExistState.All)
                {
                    foreach (var t in Selection.transforms)
                        DestroyImmediate(t.gameObject.GetComponent<T>());
                }
            }
            GUI.color = cc;
            EditorGUILayout.EndHorizontal();
        }

        
        static bool IsUI(UnityEngine.Object[] t)
        {
            return t
                .Select(x => x as Transform)
                .All(x => x.gameObject.TryGetComponentInParent<Canvas>(out _) && x is RectTransform);
        }
        
        static bool IsNotUI(UnityEngine.Object[] t)
        {
            return t
                .Select(x => x as Transform)
                .All(x => !x.gameObject.TryGetComponentInParent<Canvas>(out _));
        }
        
        static bool IsUI(GameObject g)
        {
            return g.TryGetComponentInParent<Canvas>(out _);
        }
        
        public static ExistState GetExistState<T>(UnityEngine.Object[] targets)
            where T: Component
        {
            var comps = targets
                .Select(t => t as Transform)
                .Where(t => t)
                .Select(t => t.TryGetComponent<T>(out _))
                .ToArray();
                
            // Debug.LogError(targets.ToStringJoined("\n"));
                
            var count = comps
                .Where(x => x)
                .Count();
            
            if(count == 0)
            {
                return ExistState.None;
            }
            else if(count == comps.Length)
            {
                return ExistState.All;
            }
            else
            {
                return ExistState.Some;
            }
        }
        
        public static event Action<SerializedObject, UnityEngine.Object[]> additionalDrawBefore;
        public static event Action<SerializedObject, UnityEngine.Object[]> additionalDrawAfter;
        
    }
}
