using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Prota.Unity;

namespace Prota.Editor
{
	public class LocalizationWindow : EditorWindow
	{
		Vector2 controllersScrollPosition;
		bool showControllers = true;

		[MenuItem("ProtaFramework/Localization/Localization Window")]
		public static void ShowWindow()
		{
			var window = GetWindow<LocalizationWindow>("Localization");
			window.titleContent = new GUIContent("Localization");
			window.Show();
		}

		void OnEnable()
		{
			EditorApplication.update += Repaint;
		}

		void OnDisable()
		{
			EditorApplication.update -= Repaint;
		}

		void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Localization Settings", EditorStyles.boldLabel);
			if (GUILayout.Button("Reload", GUIPreset.width[70]))
			{
				LocalizationDatabase.Refresh();
				Repaint();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			EditorGUILayout.LabelField("Current Language:", LocalizationDatabase.currentLanguage);
			EditorGUILayout.Space();

			var availableLanguages = LocalizationDatabase.availableCsvFilePaths;
			int languageCount = availableLanguages.Count;
			if (languageCount > 0)
			{
				var languageOptions = new string[languageCount];
				for (int i = 0; i < languageCount; i++)
				{
					languageOptions[i] = availableLanguages[i];
				}

				int currentIndex = Array.IndexOf(languageOptions, LocalizationDatabase.currentLanguage);
				if (currentIndex < 0) currentIndex = 0;

				int newIndex = EditorGUILayout.Popup("Select Language:", currentIndex, languageOptions);
				if (newIndex != currentIndex && newIndex >= 0 && newIndex < languageOptions.Length)
				{
					string newLanguage = languageOptions[newIndex];
					LocalizationDatabase.SetLanguage(newLanguage);
					Debug.Log($"Language set to: {newLanguage}");
				}
			}
			else
			{
				EditorGUILayout.HelpBox("No language files found in StreamingAssets/Localization", MessageType.Warning);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();

			var controllers = LocalizationController.all;
			var controllerList = new List<LocalizationController>(controllers);
			controllerList.Sort((a, b) => string.Compare(a ? a.name : "", b ? b.name : "", StringComparison.Ordinal));

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			showControllers = EditorGUILayout.Foldout(showControllers, $"LocalizationControllers ({controllerList.Count})", true);
			if (showControllers)
			{
				controllersScrollPosition = EditorGUILayout.BeginScrollView(controllersScrollPosition, GUIPreset.minHeight[120]);
				for (int i = 0; i < controllerList.Count; i++)
				{
					var controller = controllerList[i];
					if (controller == null) continue;
					EditorGUILayout.ObjectField(controller, typeof(LocalizationController), true);
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}
	}
}
