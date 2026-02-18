using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class ProtaTweenerTimerTypeWindow : EditorWindow
    {
        private const string DefaultPresetKey = "ProtaFramework::ProtaTweenerTimerTypeWindow.DefaultPreset";
        
        private GameObject targetGameObject;
        private EditorPrefEntryObject<ProtaTweenDefinitionPreset> defaultPresetPref;
        
        public static ProtaTweenDefinitionPreset GetDefaultPreset()
        {
            var pref = new EditorPrefEntryObject<ProtaTweenDefinitionPreset>(DefaultPresetKey);
            return pref.value;
        }
        
        [MenuItem("ProtaFramework/Tools/ProtaTweener Timer Type Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProtaTweenerTimerTypeWindow>();
            window.titleContent = new GUIContent("ProtaTweener Timer Type");
            window.Show();
        }
        
        void OnEnable()
        {
            defaultPresetPref = new EditorPrefEntryObject<ProtaTweenDefinitionPreset>(DefaultPresetKey);
            Selection.selectionChanged += OnSelectionChanged;
            UpdateTargetFromSelection();
        }
        
        void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
        
        void OnSelectionChanged()
        {
            UpdateTargetFromSelection();
            Repaint();
        }
        
        void UpdateTargetFromSelection()
        {
            var selected = Selection.activeGameObject;
            if (selected != null)
            {
                targetGameObject = selected;
            }
        }
        
        void OnGUI()
        {
            EditorGUILayout.Space();
            
            defaultPresetPref.value = (ProtaTweenDefinitionPreset)EditorGUILayout.ObjectField(
                "默认 Preset",
                defaultPresetPref.value,
                typeof(ProtaTweenDefinitionPreset),
                false
            );
            
            EditorGUILayout.Space();
            
            targetGameObject = (GameObject)EditorGUILayout.ObjectField(
                "GameObject", 
                targetGameObject, 
                typeof(GameObject), 
                true
            );
            
            EditorGUILayout.Space();
            
            if (targetGameObject == null)
            {
                EditorGUILayout.HelpBox("请选择一个 GameObject", MessageType.Info);
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("设置为Realtime"))
            {
                SetTimerTypeForAllChildren(targetGameObject, TweenTimerType.Realtime);
            }
            
            if (GUILayout.Button("设置为Gametime"))
            {
                SetTimerTypeForAllChildren(targetGameObject, TweenTimerType.Game);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        void SetTimerTypeForAllChildren(GameObject root, TweenTimerType timerType)
        {
            if (root == null) return;
            
            List<ProtaTweener> tweenerList = new List<ProtaTweener>();
            CollectAllTweeners(root.transform, tweenerList);
            
            if (tweenerList.Count == 0)
            {
                Debug.Log($"GameObject {root.name} 及其子物体中没有找到 ProtaTweener 组件");
                return;
            }
            
            Undo.RecordObjects(tweenerList.ToArray(), $"Set ProtaTweener TimerType to {timerType}");
            
            int count = 0;
            foreach (var tweener in tweenerList)
            {
                if (tweener != null)
                {
                    tweener.timerType = timerType;
                    EditorUtility.SetDirty(tweener);
                    count++;
                }
            }
            
            Debug.Log($"已将 {count} 个 ProtaTweener 组件的 timerType 设置为 {timerType}");
        }
        
        void CollectAllTweeners(Transform parent, List<ProtaTweener> result)
        {
            if (parent == null) return;
            
            var compCount = parent.gameObject.GetComponentCount();
            for (int i = 0; i < compCount; i++)
            {
                var comp = parent.gameObject.GetComponentAtIndex(i);
                if (comp is ProtaTweener tweener)
                {
                    result.Add(tweener);
                }
            }
            
            foreach (Transform child in parent)
            {
                CollectAllTweeners(child, result);
            }
        }
    }
}

