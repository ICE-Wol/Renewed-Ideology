using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweenerStateControl), false)]
    [CanEditMultipleObjects]
    public class ProtaTweenerStateControlInspector : UnityEditor.Editor
	{
		IntStringCache intStringCache = new IntStringCache(x => x.ToString());
		
        SerializedProperty currentState;
        SerializedProperty states;

        struct TweenerConfig
        {
            public bool play;
            public bool playReversed;
        }

        void OnEnable()
        {
            currentState = serializedObject.FindProperty("currentState");
            states = serializedObject.FindProperty("states");
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObjects(targets, "ProtaTweenerStateControlInspector");

            serializedObject.UpdateIfRequiredOrScript();

            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(currentState, new GUIContent("当前状态 currentState"));
            }

            EditorGUILayout.Space();

            for(int i = 0; i < states.arraySize; i++)
            {
                var entry = states.GetArrayElementAtIndex(i);
                var state = entry.FindPropertyRelative("state");
                var tweeners = entry.FindPropertyRelative("tweeners");

                using(new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    using(new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(intStringCache[i], GUIPreset.width[22]);
                        EditorGUILayout.PropertyField(state, GUIContent.none);
                        if(GUILayout.Button("删除", GUIPreset.width[48]))
                        {
                            states.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    DrawTweeners(tweeners);
                }
            }

            EditorGUILayout.Space();

            using(new EditorGUI.DisabledScope(targets.Length != 1))
            {
                if(GUILayout.Button("收集子节点 tweener"))
                {
                    CollectTweenersForAllStates();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }

            if(GUILayout.Button("新增状态条目 add state entry"))
            {
                var idx = states.arraySize;
                states.InsertArrayElementAtIndex(idx);

                var entry = states.GetArrayElementAtIndex(idx);
                entry.FindPropertyRelative("state").stringValue = string.Empty;
                entry.FindPropertyRelative("tweeners").ClearArray();

                serializedObject.ApplyModifiedProperties();
                CollectTweenersForAllStates();
                serializedObject.UpdateIfRequiredOrScript();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawTweeners(SerializedProperty tweeners)
        {
            using(new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"tweeners : {tweeners.arraySize}");

                if(GUILayout.Button("+", GUIPreset.width[22]))
                {
                    AddTweener(tweeners);
                }
            }

            for(int i = 0; i < tweeners.arraySize; i++)
            {
                var element = tweeners.GetArrayElementAtIndex(i);
                var tweener = element.FindPropertyRelative("tweener");
                var play = element.FindPropertyRelative("shouldPlay");
                var playReversed = element.FindPropertyRelative("shouldPlayReverse");

                using(new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(tweener, GUIContent.none);
                    var tweenerObj = tweener.objectReferenceValue as ProtaTweener;
                    var tweenerName = tweenerObj?.animName ?? "<none>";
                    EditorGUILayout.LabelField(tweenerName, GUIPreset.width[90]);
                    EditorGUI.BeginChangeCheck();
                    var newPlay = GUILayout.Toggle(play.boolValue, "Play", GUIPreset.width[50]);
                    var newPlayReversed = GUILayout.Toggle(playReversed.boolValue, "Rev", GUIPreset.width[50]);
                    if(EditorGUI.EndChangeCheck())
                    {
                        play.boolValue = newPlay;
                        playReversed.boolValue = newPlayReversed;
                    }

                    if(GUILayout.Button("x", GUIPreset.width[22]))
                    {
                        tweeners.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }
        }

        void AddTweener(SerializedProperty tweeners)
        {
            var idx = tweeners.arraySize;
            tweeners.InsertArrayElementAtIndex(idx);

            var element = tweeners.GetArrayElementAtIndex(idx);
            element.FindPropertyRelative("tweener").objectReferenceValue = null;
            element.FindPropertyRelative("play").boolValue = true;
            element.FindPropertyRelative("playReversed").boolValue = false;
        }

        void CollectTweenersForAllStates()
        {
            if(targets.Length != 1) return;

            var controller = target as ProtaTweenerStateControl;
            var result = new List<ProtaTweener>();
            CollectTweeners(controller.transform, controller, result);

            var statesArray = controller.states;
            for(int stateIndex = 0; stateIndex < statesArray.Length; stateIndex++)
            {
                var entry = statesArray[stateIndex];
                var tweeners = entry.tweeners;

                var cache = new Dictionary<ProtaTweener, TweenerConfig>();
                if(tweeners != null)
                {
                    for(int i = 0; i < tweeners.Length; i++)
                    {
                        var t = tweeners[i].tweener;
                        if(t == null) continue;

                        var config = new TweenerConfig
                        {
                            play = tweeners[i].shouldPlay,
                            playReversed = tweeners[i].shouldPlayReverse,
                        };
                        cache[t] = config;
                    }
                }

                var newTweeners = new ProtaTweenerStateControl.TweenerState[result.Count];
                for(int i = 0; i < result.Count; i++)
                {
                    var tweener = result[i];
                    var stateConfig = new ProtaTweenerStateControl.TweenerState
                    {
                        tweener = tweener,
                        shouldPlay = true,
                        shouldPlayReverse = false,
                    };

                    if(tweener != null && cache.TryGetValue(tweener, out var config))
                    {
                        stateConfig.shouldPlay = config.play;
                        stateConfig.shouldPlayReverse = config.playReversed;
                    }

                    newTweeners[i] = stateConfig;
                }

                entry.tweeners = newTweeners;
                statesArray[stateIndex] = entry;
            }

            controller.states = statesArray;
            EditorUtility.SetDirty(controller);
        }

        static void CollectTweeners(Transform parent, ProtaTweenerStateControl self, List<ProtaTweener> result)
        {
            foreach(Transform child in parent)
            {
                var otherCtrl = child.GetComponent<ProtaTweenerStateControl>();
                if(otherCtrl != null && !ReferenceEquals(otherCtrl, self)) continue;

                var compCount = child.gameObject.GetComponentCount();
                for(int i = 0; i < compCount; i++)
                {
                    var comp = child.gameObject.GetComponentAtIndex(i);
                    if(comp is ProtaTweener tweener) result.Add(tweener);
                }

                CollectTweeners(child, self, result);
            }
        }
    }
}
