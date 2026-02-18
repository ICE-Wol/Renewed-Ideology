using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using Prota;
using Prota.Unity;

namespace Prota.Editor
{
    /// <summary>
    /// 文件同步工具窗口
    /// 通用的文件匹配和复制功能
    /// </summary>
    public partial class FileSyncToolWindow : EditorWindow
    {
        Vector2 scrollPosition;
        FileSyncToolData data;
        string filterText = "";
        bool debugMode = false;
        FileSyncToolWatcher watcher;
        
        [MenuItem("ProtaFramework/FileSyncTool", priority = 1004)]
        public static void ShowWindow()
        {
            var window = GetWindow<FileSyncToolWindow>("File Sync Tool");
            window.Show();
        }
        
        void OnEnable()
        {
            data = FileSyncToolData.instance;
            watcher = FileSyncToolAutoSync.GetWatcher();
            EditorApplication.update += OnUpdate;
        }
        
        void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            watcher = null;
        }

        void OnUpdate()
        {
            Repaint();
        }
        
        void OnGUI()
        {
            if (data == null)
            {
                EditorGUILayout.HelpBox("数据加载失败", MessageType.Error);
                return;
            }
            
            // 第一行：标题和暂停按钮
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("文件同步工具", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            data.pauseAutoSync = ProtaEditorUtils.DrawToggleButton(data.pauseAutoSync, "暂停自动同步", GUIPreset.width[120]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            // 筛选输入框
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("筛选:", GUIPreset.width[50]);
            filterText = EditorGUILayout.TextField(filterText);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // 绘制匹配筛选条件的条目
            for (int i = 0; i < data.entries.Count; i++)
            {
                var entry = data.entries[i];
                // 如果筛选文本为空，或者条目名称包含筛选文本，则显示
                if (string.IsNullOrEmpty(filterText) || (entry.name != null && entry.name.Contains(filterText)))
                {
                    DrawEntry(i);
                    EditorGUILayout.Space(5);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(10);
            ProtaEditorUtils.SeperateLine(1);
            EditorGUILayout.Space(5);
            
            // 添加按钮
            EditorGUILayout.BeginHorizontal();
            
            debugMode = ProtaEditorUtils.DrawToggleButton(debugMode, "DebugMode", GUIPreset.width[100]);
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("添加条目", GUIPreset.width[150]))
            {
                AddEntry();
                GUIUtils.ClearFocus();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 绘制单个条目
        /// </summary>
        void DrawEntry(int index)
        {
            if (index < 0 || index >= data.entries.Count)
                return;
            
            var entry = data.entries[index];
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // 第一行：自动同步 ToggleButton（左侧）、删除按钮、名称、同步文件按钮（右侧）
            EditorGUILayout.BeginHorizontal();
            
            // 删除按钮
            if (GUILayout.Button("删除", GUIPreset.width[80]))
            {
                RemoveEntry(index);
                GUIUtils.ClearFocus();
                return;
            }
            
            // EditorGUILayout.LabelField(CacheUtils.intStringCache[index], GUIPreset.width[80]);
            entry.name = EditorGUILayout.TextField(entry.name, GUIPreset.expendWidth);
            
			
			// Filter (Regex) 输入行
            EditorGUILayout.LabelField("Filter(Regex)", GUIPreset.width[80]);
            
            // 检查正则表达式是否有效
            bool isValidRegex = entry.IsFilterValid();
            
            // 如果正则表达式无效，背景显示红色
            using (new BackgroundColorScope(isValidRegex ? GUI.backgroundColor : new Color(1f, 0.5f, 0.5f)))
            {
                entry.filter = EditorGUILayout.TextField(entry.filter);
            }
			
            // 自动同步 ToggleButton（右侧）
			if(data.pauseAutoSync)
			{
            	entry.autoSync = ProtaEditorUtils.DrawToggleButton(entry.autoSync, "已暂停同步", GUIPreset.width[100]);
			}
			else
			{
				entry.autoSync = ProtaEditorUtils.DrawToggleButton(entry.autoSync, "自动同步", GUIPreset.width[100]);
			}
            
            // 同步按钮（右侧）- 根据路径类型动态显示
            FileSyncToolUtils.PathType sourceType = FileSyncToolUtils.GetPathType(entry.sourcePath);
            FileSyncToolUtils.PathType targetType = FileSyncToolUtils.GetPathType(entry.targetPath);
            
            string buttonText;
            bool canSync = FileSyncToolUtils.CanSync(entry.sourcePath, entry.targetPath, out string reason);
            
            if (!canSync)
            {
                buttonText = reason;
            }
            else if (sourceType == FileSyncToolUtils.PathType.File)
            {
                buttonText = "同步文件";
            }
            else // Directory
            {
                buttonText = "同步文件夹";
            }
            
            // Debug 模式下的打印列表按钮
            
            GUI.enabled = canSync;
            if (GUILayout.Button(buttonText, GUIPreset.width[150]))
            {
                if (sourceType == FileSyncToolUtils.PathType.File)
                {
                    // 同步文件并收集详细报告
                    var results = new SyncResultCollection();
                    Regex filterRegex = entry.GetFilterRegex();
                    FileSyncToolUtils.SyncFile(entry.sourcePath, entry.targetPath, filterRegex, entry.name, results, watcher);
                    
                    // 输出详细报告
                    if (results.Count > 0)
                    {
                        var report = new System.Text.StringBuilder();
                        report.AppendLine($"=== 文件同步报告: {entry.name} ===");
                        report.AppendLine($"源路径: {results.sourcePath}");
                        report.AppendLine($"目标路径: {results.targetPath}");
                        report.AppendLine();
                        
                        foreach (var result in results.GetResults())
                        {
                            result.AppendLineToStringBuilder(report);
                        }
                        
                        report.AppendLine();
                        report.AppendLine($"总计: 同步 {results.syncedCount} 个文件, 跳过 {results.skippedCount} 个文件");
                        
                        Debug.Log(report.ToString());
                    }
                }
                else
                {
                    // 强制同步并收集详细报告
                    var results = new SyncResultCollection();
                    Regex filterRegex = entry.GetFilterRegex();
                    var operations = new List<SyncOperation>();
                    
                    int syncedCount = 0;
                    
                    if (FileSyncToolUtils.TryGetWatchers(watcher, entry.name, out var sourceWatcher, out var targetWatcher))
                    {
                        var sourceDirNode = sourceWatcher.GetDirectory(entry.sourcePath.ToStandardFullPath());
                        var targetDirNode = targetWatcher.GetDirectory(entry.targetPath.ToStandardFullPath());
                        
                        FileSyncToolUtils.SyncDirectoryRecursive(sourceDirNode, targetDirNode, entry.targetPath, filterRegex, forceSync: true, ref syncedCount, results, operations);
                    }
                    else
                    {
                        Debug.LogError($"无法获取 Watcher，请检查路径是否有效: {entry.name}");
                    }
                    
                    results.syncedCount = syncedCount;
                    
                    // 执行同步操作
                    foreach (var op in operations)
                    {
                        op.Apply();
                    }
                    
                    // 输出详细报告
                    if (results.Count > 0)
                    {
                        var report = new System.Text.StringBuilder();
                        results.AppendHeaderToStringBuilder(report, entry.name);
                        
                        foreach (var result in results.GetResults())
                        {
                            result.AppendLineToStringBuilder(report);
                        }
                        
                        report.AppendLine();
                        report.AppendLine($"总计: 同步 {results.syncedCount} 个文件, 跳过 {results.skippedCount} 个文件");
                        
                        Debug.Log(report.ToString());
                    }
                }
                GUIUtils.ClearFocus();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            DrawDebugWatcherInfo(entry, sourceType, targetType);
            
            // 源路径和目标路径行（整合到一行）
            EditorGUILayout.BeginHorizontal();
            
            // 选择文件/文件夹按钮
            if (GUILayout.Button("选择", GUIPreset.width[100]))
            {
                CreatePathSelectionMenu("源", path => entry.sourcePath = path);
                GUIUtils.ClearFocus();
            }
            
            // 检查源文件是否存在，如果存在则设置文本颜色为绿色
            bool sourceExists = entry.SourceExists();
            using (new ColorScope(new Color(0.5f, 0.9f, 0.5f), shouldReplace: sourceExists))
            {
                entry.sourcePath = EditorGUILayout.TextField(entry.sourcePath);
            }
            
            
            // 中间插入 "=>" 标签
            EditorGUILayout.LabelField("=>", GUIPreset.width[20]);
            
            // 检查目标文件或文件夹是否存在，如果存在则设置文本颜色为绿色
            bool targetExists = !string.IsNullOrEmpty(entry.targetPath) && (File.Exists(entry.targetPath) || Directory.Exists(entry.targetPath));
            using (new ColorScope(new Color(0.5f, 0.9f, 0.5f), shouldReplace: targetExists))
            {
                entry.targetPath = EditorGUILayout.TextField(entry.targetPath);
            }
            
            // 选择文件/文件夹按钮
            if (GUILayout.Button("选择", GUIPreset.width[100]))
            {
                CreatePathSelectionMenu("目标", path => entry.targetPath = path);
                GUIUtils.ClearFocus();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 创建文件/文件夹选择菜单
        /// </summary>
        void CreatePathSelectionMenu(string titlePrefix, Action<string> onPathSelected)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("选择文件"), false, () =>
            {
                string selectedPath = EditorUtility.OpenFilePanel($"选择{titlePrefix}文件", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    onPathSelected(selectedPath.ToStandardPath());
                }
            });
            menu.AddItem(new GUIContent("选择文件夹"), false, () =>
            {
                string selectedPath = EditorUtility.OpenFolderPanel($"选择{titlePrefix}文件夹", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    onPathSelected(selectedPath.ToStandardPath());
                }
            });
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// 添加新条目
        /// </summary>
        void AddEntry()
        {
            var newEntry = new FileSyncToolData.FileSyncEntry();
            newEntry.name = $"条目 {data.entries.Count + 1}";
            data.entries.Add(newEntry);
        }
        
        /// <summary>
        /// 删除条目
        /// </summary>
        void RemoveEntry(int index)
        {
            data.entries.RemoveAt(index);
        }
    }
}

