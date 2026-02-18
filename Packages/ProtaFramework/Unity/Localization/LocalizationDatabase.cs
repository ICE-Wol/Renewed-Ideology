using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DL.FastCsvParser;
using Prota.Unity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LocalizationDatabase
{
	public const string defaultLanguage = "en";
	const string LocalizationFolder = "localization";
	
	public static string currentLanguage = defaultLanguage;
	
	public static event Action<string> languageChanged = null!;

	static readonly Dictionary<string, string> translationMap = new();

	static string[] csvFilePaths = Array.Empty<string>();
	
	public static IReadOnlyList<string> availableCsvFilePaths => csvFilePaths;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#endif
	static void Initialize()
	{
		Refresh();
	}
	
	public static void SetLanguage(string languageName)
	{
		if (currentLanguage == languageName) return;
		currentLanguage = languageName;
		LoadLanguage(languageName);
		languageChanged?.Invoke(languageName);
	}

	// 重新扫描翻译文件.
	public static void Refresh()
	{
		string folderPath = Path.Combine(Application.streamingAssetsPath, LocalizationFolder);
		if (!Directory.Exists(folderPath))
		{
			csvFilePaths = Array.Empty<string>();
			return;
		}

		csvFilePaths = Directory.GetFiles(folderPath, "*.csv")
			.Select(Path.GetFileNameWithoutExtension)
			.Where(name => !string.IsNullOrEmpty(name))
			.OrderBy(name => name)
			.ToArray();

		// 刷新完毕后更新翻译.
		LoadLanguage(currentLanguage);
		UpdateAllControllers();
	}

	static void UpdateAllControllers()
	{
		foreach(var controller in LocalizationController.all)
		{
			if(controller == null) continue;
			controller.UpdateText();
		}
	}

	public static string Translate(string refText)
	{
		Translate(refText, out var translatedText);
		return translatedText;
	}

	public static bool Translate(string refText, out string translatedText)
	{
		if(string.IsNullOrEmpty(refText))
		{
			translatedText = $"#invalid[{refText}]";
			return false;
		}

		if(translationMap.TryGetValue(refText, out var translated))
		{
			translatedText = translated;
			return true;
		}

		translatedText = $"#invalid[{refText}]";
		return false;
	}

	public static bool LoadLanguage(string languageName)
	{
		if(string.IsNullOrEmpty(languageName))
		{
			Debug.LogError("LocalizationDatabase.LoadLanguage: languageName is null or empty");
			return false;
		}

		string filePath = Path.Combine(Application.streamingAssetsPath, LocalizationFolder, $"{languageName}.csv");

		if(!File.Exists(filePath))
		{
			Debug.LogError($"LocalizationDatabase.LoadLanguage: file not found: {filePath}");
			return false;
		}

		try
		{
			string content = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
			Debug.Log(content);
			var csv = Csv.Parse(content);
			ParseCsv(csv);
			return true;
		}
		catch(CsvParseException ex)
		{
			Debug.LogError($"LocalizationDatabase.LoadLanguage: CSV parse error for {languageName}: {ex.Message}");
			return false;
		}
		catch(Exception ex)
		{
			Debug.LogError($"LocalizationDatabase.LoadLanguage: failed to load {languageName}: {ex.Message}");
			return false;
		}
	}

	static void ParseCsv(Csv csv)
	{
		translationMap.Clear();

		if(csv == null || csv.Count < 2)
		{
			Debug.LogWarning("LocalizationDatabase.ParseCsv: CSV file has less than 2 rows");
			return;
		}

		// 跳过第一行（表头）
		for(int i = 1; i < csv.Count; i++)
		{
			var row = csv[i];
			if(row == null || row.Count < 2)
			{
				continue;
			}

			string refText = row[0].Trim();
			string translatedText = row[1].Trim();

			if(!string.IsNullOrEmpty(refText))
			{
				translationMap[refText] = translatedText;
			}
		}
	}
}
