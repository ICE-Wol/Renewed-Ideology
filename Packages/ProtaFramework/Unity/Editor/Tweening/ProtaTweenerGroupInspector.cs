using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweenerGroup), false)]
    [CanEditMultipleObjects]
    public class ProtaTweenerGroupInspector : UnityEditor.Editor
    {
        SerializedProperty tweeners;

        void OnEnable()
        {
            tweeners = serializedObject.FindProperty("tweeners");
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObjects(targets, "ProtaTweenerGroupInspector");

            serializedObject.UpdateIfRequiredOrScript();

            DrawTweeners();

            EditorGUILayout.Space();

            using(new EditorGUI.DisabledScope(targets.Length != 1))
            {
                if(GUILayout.Button("收集全部子节点 tweener"))
                {
                    CollectAllTweeners();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawTweeners()
        {
            EditorGUILayout.LabelField($"tweeners : {tweeners.arraySize}");
            
            var groups = new Dictionary<string, List<SerializedProperty>>();
            
            for(int i = 0; i < tweeners.arraySize; i++)
            {
                var element = tweeners.GetArrayElementAtIndex(i);
                var tweener = element.objectReferenceValue as ProtaTweener;
                var key = tweener != null ? tweener.animName : "";
                if(!groups.TryGetValue(key, out var list))
                {
                    list = new List<SerializedProperty>();
                    groups.Add(key, list);
                }
                list.Add(element);
            }
            
            var keys = new List<string>(groups.Keys);
            keys.Sort(System.StringComparer.Ordinal);
            
            foreach(var key in keys)
            {
                var list = groups[key];
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(string.IsNullOrEmpty(key) ? "(Empty animName)" : key, EditorStyles.boldLabel);
                
                ProtaTweener firstTweener = null;
                for(int i = 0; i < list.Count; i++)
                {
                    var t = list[i].objectReferenceValue as ProtaTweener;
                    if(t == null) continue;
                    firstTweener = t;
                    break;
                }
                
                if(firstTweener != null)
                {
                    var progress = firstTweener.progress;
                    EditorGUI.BeginChangeCheck();
                    var newProgress = EditorGUILayout.Slider(progress, 0f, 1f);
                    if(EditorGUI.EndChangeCheck())
                    {
                        var tweenerObjects = new List<UnityEngine.Object>();
                        for(int i = 0; i < list.Count; i++)
                        {
                            var t = list[i].objectReferenceValue as ProtaTweener;
                            if(t == null) continue;
                            tweenerObjects.Add(t);
                        }
                        
                        if(tweenerObjects.Count > 0)
                        {
                            Undo.RecordObjects(tweenerObjects.ToArray(), "Change ProtaTweener group progress");
                            foreach(var obj in tweenerObjects)
                            {
                                var t = obj as ProtaTweener;
                                t.SetTo(newProgress);
                                EditorUtility.SetDirty(t);
                            }
                        }
                    }
                }
                
                for(int i = 0; i < list.Count; i++)
                {
                    var element = list[i];
                    
                    using(new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(element, GUIContent.none);
                        
                        var tweener = element.objectReferenceValue as ProtaTweener;
                        var animName = tweener != null ? tweener.animName : "";
                        
                        EditorGUI.BeginChangeCheck();
                        var newName = EditorGUILayout.TextField(animName);
                        if(EditorGUI.EndChangeCheck() && tweener != null)
                        {
                            Undo.RecordObject(tweener, "Change ProtaTweener animName");
                            tweener.animName = newName;
                            EditorUtility.SetDirty(tweener);
                        }
                    }
                }
            }
        }

        void CollectAllTweeners()
        {
            if(targets.Length != 1) return;

            var group = target as ProtaTweenerGroup;
            if(group == null) return;

            var result = new List<ProtaTweener>();
            CollectTweeners(group.transform, result);

            tweeners.arraySize = result.Count;
            for(int i = 0; i < result.Count; i++)
            {
                tweeners.GetArrayElementAtIndex(i).objectReferenceValue = result[i];
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(group);
        }

        static void CollectTweeners(Transform parent, List<ProtaTweener> result)
        {
            foreach(Transform child in parent)
            {
                var compCount = child.gameObject.GetComponentCount();
                for(int i = 0; i < compCount; i++)
                {
                    var comp = child.gameObject.GetComponentAtIndex(i);
                    if(comp is ProtaTweener tweener) result.Add(tweener);
                }

                CollectTweeners(child, result);
            }
        }
    }
}
