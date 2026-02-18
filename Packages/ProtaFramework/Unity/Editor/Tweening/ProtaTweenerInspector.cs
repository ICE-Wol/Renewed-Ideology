using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using System.Linq;
using System.Reflection;
namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweener), false)]
    [ExecuteAlways]
    [CanEditMultipleObjects]
    public class ProtaTweenerInspector : UnityEditor.Editor
    {
		protected override void OnHeaderGUI()
		{
			var rect = EditorGUILayout.GetControlRect(false, 0f);
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.y -= rect.height + 6;
			rect.x = 300;
			rect.width = 120;
			EditorGUI.LabelField(rect, (target as ProtaTweener).animName, EditorStyles.textField);
		}
		
        public override void OnInspectorGUI()
        {
			OnHeaderGUI();
			
            Undo.RecordObjects(targets, "ProtaTweenerInspector");
            
			if(ProtaTweenerBatchUpdater.exists)
			{
				if(ProtaTweenerBatchUpdater.instance.allTweeners.Contains(target as ProtaTweener))
				{
					EditorGUILayout.LabelField("batch controlling...");
				}
				else
				{
					EditorGUILayout.LabelField("idle...");
				}
			}
			
            var name = serializedObject.FindProperty("animName");
            EditorGUILayout.PropertyField(name, new GUIContent("名称 name"));
            
            var progress = serializedObject.FindProperty("progress");
            var pv = progress.floatValue;
            EditorGUILayout.PropertyField(progress, new GUIContent("进度 progress"));
            var npv = progress.floatValue;
            var progressChanged = pv != npv;
            
            var play = serializedObject.FindProperty("play");
            EditorGUILayout.PropertyField(play, new GUIContent("是否控制属性 play"));
            
            var running = serializedObject.FindProperty("running");
            EditorGUILayout.PropertyField(running, new GUIContent("是否自动播放 running"));
            
            var playReversed = serializedObject.FindProperty("playReversed");
            EditorGUILayout.PropertyField(playReversed, new GUIContent("是否反向播放 playReversed"));
            
            var loop = serializedObject.FindProperty("loop");
            EditorGUILayout.PropertyField(loop, new GUIContent("是否循环 loop"));
            
			if(!loop.boolValue)
			{
				var keepControlWhenFinish = serializedObject.FindProperty("keepControlWhenFinish");
				EditorGUILayout.PropertyField(keepControlWhenFinish, new GUIContent("播放完毕后继续控制 keepControlWhenFinish"));
			}
			
            if(loop.boolValue)
            {
                var reverseOnLoop = serializedObject.FindProperty("reverseOnLoop");
                EditorGUILayout.PropertyField(reverseOnLoop, new GUIContent("是否循环反向播放 reverseOnLoop"));
            }
            
            var timerType = serializedObject.FindProperty("timerType");
            EditorGUILayout.PropertyField(timerType, new GUIContent("计时器类型 timerType"));
            
            if(targets.Length == 1)
            {
                var tw = target as ProtaTweener;
                if(tw.gameObject.GetColorComponent(out var comp, out int ind))
                {
                    using(new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField("颜色组件 color component", comp, typeof(Component), true);
                    }
                }
                if(tw.gameObject.GetSizeComponent(out var sizeComp, out int sizeInd))
                {
                    using(new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField("尺寸组件 size component", sizeComp, typeof(Component), true);
                    }
                }
            }
            
            if(targets.Length == 1 && (target as ProtaTweener).transform.childCount > 0)
            {
                var child = serializedObject.FindProperty("children");
                EditorGUILayout.PropertyField(child, new GUIContent("子动画 children"));
            }
            if(targets.Length != 1 || (target as ProtaTweener).transform.childCount > 0)
            {
                if(GUILayout.Button("控制所有子节点 control all children"))
				{
					Undo.RecordObjects(targets, "ProtaTweenerInspector");
					BindChildNodes();
					serializedObject.Update();
				}
			}
            
            var preset = serializedObject.FindProperty("preset");
            EditorGUILayout.PropertyField(preset, new GUIContent("动画预设 preset"));
            if(preset.objectReferenceValue == null)
            {
                var d = serializedObject.FindProperty("d");
                var obj = d.managedReferenceValue as TweenDefinition;
                if(obj == null) d.managedReferenceValue = new TweenDefinition();
                EditorGUILayout.PropertyField(d, new GUIContent("动画定义 animation def"));
            }
            
			EditorGUILayout.BeginHorizontal();
			
            if(GUILayout.Button("设置当前状态为起点 set to start"))
            {
                var tweener = target as ProtaTweener;
				var field = tweener.GetType().GetField("cachedTransform", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				field.SetValue(tweener, tweener.transform);
                tweener.RecordFrom();
                serializedObject.Update();
            }
            
            if(GUILayout.Button("设置当前状态为终点 set to end"))
            {
				var tweener = target as ProtaTweener;
				var field = tweener.GetType().GetField("cachedTransform", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				field.SetValue(tweener, tweener.transform);
				tweener.RecordTo();
                serializedObject.Update();
            }
			
			EditorGUILayout.EndHorizontal();
            
            if(progressChanged)
            {
                var tweener = target as ProtaTweener;
                tweener.cachedTransform = tweener.transform;
                tweener.SetTo(npv);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

		void BindChildNodes()
		{
			foreach (var t in targets)
			{
				var temp = new List<ProtaTweener>();
				var tweener = t as ProtaTweener;
				CollectValidChildren(tweener.transform, tweener.animName, temp);
				tweener.children = temp.ToArray();
			}
			
			void CollectValidChildren(Transform parent, string parentAnimName, List<ProtaTweener> result)
			{
				foreach (Transform child in parent)
				{
					var compCount = child.gameObject.GetComponentCount();
					bool found = false;
					for(int i = 0; i < compCount; i++)
					{
						var comp = child.gameObject.GetComponentAtIndex(i);
						if(comp is ProtaTweener tweener && tweener.animName == parentAnimName)
						{
							found = true;
							result.Add(tweener);
						}
					}
					
					if (!found)
					{
						CollectValidChildren(child, parentAnimName, result);
						continue;
					}
				}
			}
		}

	
	}
    
    
    

}
