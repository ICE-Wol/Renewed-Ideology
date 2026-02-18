using UnityEngine;
using UnityEditor;

namespace Prota.Editor
{

	[CustomEditor(typeof(LocalizationController))]
	[CanEditMultipleObjects]
	public class LocalizationControllerInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			Undo.RecordObjects(targets, "LocalizationControllerInspector");
			
			serializedObject.Update();
			
			var localization = target as LocalizationController;
			
			if(localization == null)
			{
				EditorGUILayout.HelpBox("LocalizationController not found", MessageType.Error);
				return;
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			
			// Localization 部分
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Localization", EditorStyles.boldLabel);
			if (GUILayout.Button("Refresh Database", GUILayout.Width(120)))
			{
				LocalizationDatabase.Refresh();
			}
			EditorGUILayout.EndHorizontal();
			
			// 显示 sourceText 字段
			var sourceTextProperty = serializedObject.FindProperty("_sourceText");
			var elementsProperty = serializedObject.FindProperty("elements");
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(sourceTextProperty, new GUIContent("Source Text"));
			EditorGUILayout.PropertyField(elementsProperty, new GUIContent("Elements"), true);
			bool changed = EditorGUI.EndChangeCheck();
			
			// 先应用修改，避免被 base.OnInspectorGUI() 覆盖
			serializedObject.ApplyModifiedProperties();
			
			// 如果被修改，更新文本显示
			if(changed)
			{
				foreach(var obj in targets)
				{
					if(obj is LocalizationController lc)
					{
						lc.UpdateText();
						EditorUtility.SetDirty(lc);
					}
				}
			}
			
			// 运行时信息
			if(Application.isPlaying)
			{
				EditorGUILayout.Space();
				EditorGUI.BeginDisabledGroup(true);

				// 显示当前语言
				string currentLanguage = LocalizationDatabase.currentLanguage;
				EditorGUILayout.TextField("Current Language:", currentLanguage);
				
				EditorGUILayout.Space();
				
				// 显示翻译后的文本
				LocalizationDatabase.Translate(localization.sourceText, out var translatedText);
				EditorGUILayout.TextField("Translated Text:", translatedText);
				
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Localization info is only available in Play Mode", MessageType.Info);
			}
			
			serializedObject.ApplyModifiedProperties();
			if(EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(localization);
			}
		}
	}
}
