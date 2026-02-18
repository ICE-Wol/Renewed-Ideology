using System;
using System.Collections.Generic;
using Prota.Unity;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
	public class SingletonScriptableObjectScannerWindow : EditorWindow
	{
		struct Entry
		{
			public ScriptableObject asset;
			public string path;
			public Type type;
		}
		
		Vector2 scroll;
		readonly List<Entry> entries = new();

		static string BuildInheritanceChainDisplay(Type type)
		{
			string result = GetTypeDisplayName(type);
			for (Type t = type.BaseType; t != null; t = t.BaseType)
			{
				if (t == typeof(ScriptableObject) || t == typeof(UnityEngine.Object) || t == typeof(object))
					break;
				
				result += " -> " + GetTypeDisplayName(t);
			}
			
			return result;
		}

		static string GetTypeDisplayName(Type type)
		{
			if (!type.IsGenericType)
				return type.Name;
			
			string name = type.Name;
			int tickIndex = name.IndexOf('`');
			if (tickIndex >= 0)
				name = name.Substring(0, tickIndex);
			
			Type[] args = type.GetGenericArguments();
			if (args.Length == 0)
				return name;
			
			string argNames = GetTypeDisplayName(args[0]);
			for (int i = 1; i < args.Length; i++)
				argNames += ", " + GetTypeDisplayName(args[i]);
			
			return $"{name}<{argNames}>";
		}
		
		[MenuItem("ProtaFramework/Window/Singleton ScriptableObjects")]
		public static void ShowWindow()
		{
			var window = GetWindow<SingletonScriptableObjectScannerWindow>("Singleton ScriptableObjects");
			window.titleContent = new GUIContent("Singleton ScriptableObjects");
			window.Show();
		}
		
		void OnEnable()
		{
			Scan();
		}
		
		void Scan()
		{
			entries.Clear();
			
			string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
				if (asset == null)
					continue;
				
				Type type = asset.GetType();
				if (!IsSingletonScriptableObject(type))
					continue;
				
				entries.Add(new Entry
				{
					asset = asset,
					path = path,
					type = type,
				});
			}
			
			entries.Sort(static (a, b) =>
			{
				int typeCompare = string.CompareOrdinal(a.type.FullName, b.type.FullName);
				if (typeCompare != 0)
					return typeCompare;
				
				return string.CompareOrdinal(a.path, b.path);
			});
			
			Repaint();
		}
		
		static bool IsSingletonScriptableObject(Type type)
		{
			for (Type t = type; t != null; t = t.BaseType)
			{
				if (!t.IsGenericType)
					continue;
				
				if (t.GetGenericTypeDefinition() == typeof(SingletonScriptableObject<>))
					return true;
			}
			
			return false;
		}
		
		void OnGUI()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (GUILayout.Button("Scan", GUIPreset.width[80]))
				Scan();
			GUILayout.Space(8);
			EditorGUILayout.LabelField($"Count: {entries.Count}");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
			scroll = EditorGUILayout.BeginScrollView(scroll);
			
			for (int i = 0; i < entries.Count;)
			{
				Type type = entries[i].type;
				
				int groupEnd = i + 1;
				for (; groupEnd < entries.Count; groupEnd++)
				{
					if (entries[groupEnd].type != type)
						break;
				}

				int groupCount = groupEnd - i;

				ProtaEditorUtils.SeperateLine(1);
				
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				EditorGUILayout.LabelField($"{BuildInheritanceChainDisplay(type)} ({groupCount})", EditorStyles.boldLabel);
				if (GUILayout.Button("Ping", EditorStyles.toolbarButton, GUIPreset.width[60]))
					EditorGUIUtility.PingObject(entries[i].asset);
				if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUIPreset.width[60]))
				{
					var objects = new UnityEngine.Object[groupCount];
					for (int j = 0; j < groupCount; j++)
						objects[j] = entries[i + j].asset;
					
					Selection.objects = objects;
				}
				EditorGUILayout.EndHorizontal();
				
				for (int j = i; j < groupEnd; j++)
				{
					Entry entry = entries[j];
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.ObjectField(entry.asset, typeof(ScriptableObject), false, GUIPreset.width[300]);
					EditorGUILayout.LabelField(entry.path);
					EditorGUILayout.EndHorizontal();
				}
				
				i = groupEnd;
			}
			
			EditorGUILayout.EndScrollView();
		}
	}
}

