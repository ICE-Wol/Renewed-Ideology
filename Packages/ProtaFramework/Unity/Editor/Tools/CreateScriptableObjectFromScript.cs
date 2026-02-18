using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 从脚本文件创建同名ScriptableObject实例的工具
/// </summary>
public static class CreateScriptableObjectFromScript
{
	private const string MENU_ITEM_PATH = "Assets/Create ScriptableObject Instance";
	
	[MenuItem(MENU_ITEM_PATH, false, 2)]
	public static void CreateScriptableObjectInstance()
	{
		if (Selection.activeObject == null)
			return;
		
		string scriptPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (string.IsNullOrEmpty(scriptPath))
			return;
		
		// 检查是否是.cs文件
		if (!scriptPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
			return;
		
		// 使用LoadAssetAtPath加载MonoScript
		MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
		if (script == null)
			return;
		
		// 通过MonoScript获取对应的类型
		Type scriptType = script.GetClass();
		if (scriptType == null)
			return;
		
		// 判断是否为ScriptableObject类型
		if (!typeof(ScriptableObject).IsAssignableFrom(scriptType) || scriptType.IsAbstract)
		{
			Debug.LogWarning($"脚本 '{Path.GetFileNameWithoutExtension(scriptPath)}' 不是有效的ScriptableObject类型");
			return;
		}
		
		// 获取脚本文件名(不含扩展名)
		string scriptFileName = Path.GetFileNameWithoutExtension(scriptPath);
		
		// 获取脚本所在目录
		string directory = Path.GetDirectoryName(scriptPath);
		
		// 创建ScriptableObject实例
		ScriptableObject instance = ScriptableObject.CreateInstance(scriptType);
		
		// 生成资源文件路径
		string assetPath = Path.Combine(directory, scriptFileName + ".asset").Replace('\\', '/');
		
		// 确保路径唯一
		assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
		
		// 创建资源
		AssetDatabase.CreateAsset(instance, assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		// 选中新创建的对象
		Selection.activeObject = instance;
		EditorGUIUtility.PingObject(instance);
		
		Debug.Log($"已创建ScriptableObject实例: {assetPath}");
	}
	
	[MenuItem(MENU_ITEM_PATH, true)]
	public static bool ValidateCreateScriptableObjectInstance()
	{
		if (Selection.activeObject == null)
			return false;
		
		string scriptPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (string.IsNullOrEmpty(scriptPath))
			return false;
		
		// 检查是否是.cs文件
		if (!scriptPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
			return false;
		
		// 使用LoadAssetAtPath加载MonoScript
		MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
		if (script == null)
			return false;
		
		// 通过MonoScript获取对应的类型
		Type scriptType = script.GetClass();
		if (scriptType == null)
			return false;
		
		// 判断是否为ScriptableObject类型
		return typeof(ScriptableObject).IsAssignableFrom(scriptType) && !scriptType.IsAbstract;
	}
}

