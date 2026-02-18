using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Prota;
using Prota.Unity;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Prota.Editor
{
    public partial class FileSyncToolWindow
    {
        /// <summary>
        /// 绘制 Debug 模式下的 Watcher 信息
        /// </summary>
        private void DrawDebugWatcherInfo(FileSyncToolData.FileSyncEntry entry, FileSyncToolUtils.PathType sourceType, FileSyncToolUtils.PathType targetType)
        {
            // 显示 Watcher 状态
            if (debugMode && sourceType == FileSyncToolUtils.PathType.Directory && targetType == FileSyncToolUtils.PathType.Directory)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                var watcherInfo = watcher.GetWatcherInfo(entry.name);
                if (watcherInfo.hasWatcher)
                {
                    EditorGUILayout.LabelField(
                        $"Src: {watcherInfo.sourceLastUpdateTime.ToLocalTime():HH:mm:ss} {watcherInfo.sourceLastScanTimeConsume.TotalMilliseconds:F1}ms {watcherInfo.sourceScanIntervalMs}ms",
                        GUIPreset.width[210]);
                    EditorGUILayout.LabelField(
                        $"Dst: {watcherInfo.targetLastUpdateTime.ToLocalTime():HH:mm:ss} {watcherInfo.targetLastScanTimeConsume.TotalMilliseconds:F1}ms {watcherInfo.targetScanIntervalMs}ms",
                        GUIPreset.width[210]);

                    if (FileSyncToolAutoSync.TryGetLastTimingInfo(entry.name, out var timingInfo))
                    {
                        EditorGUILayout.LabelField(
                            $"Auto: {timingInfo.lastSyncTimeUtc.ToLocalTime():HH:mm:ss} {timingInfo.lastTotalMs:F1}ms A:{timingInfo.lastApplyOperationsMs:F1}ms ({timingInfo.lastExecutedOperationsCount}/{timingInfo.lastCollectedOperationsCount})",
                            GUIPreset.width[260]);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Auto: -", GUIPreset.width[260]);
                    }
                    
                    if (GUILayout.Button("Print", GUIPreset.width[50]))
                    {
                        var regex = entry.GetFilterRegex();
                        watcher.PrintWatcherData(entry.name, name => regex.IsMatch(name));
                    }
                    
                    if (GUILayout.Button("PrintToFile", GUIPreset.width[80]))
                    {
                        string filePath = Path.Combine(Application.dataPath, "..", "watcherstate.log");
                        var regex = entry.GetFilterRegex();
                        watcher.PrintWatcherDataToFile(entry.name, filePath, name => regex.IsMatch(name));
                    }
                }
                else
                {
                    using (new ColorScope(new Color(1f, 0.5f, 0.5f)))
                    {
                        EditorGUILayout.LabelField("Watcher: 未创建", GUIPreset.width[140]);
                    }
                }
            
                // Debug 模式下的打印列表按钮
                if (debugMode && FileSyncToolUtils.CanSync(entry.sourcePath, entry.targetPath, out _) && sourceType == FileSyncToolUtils.PathType.Directory)
                {
                    if (GUILayout.Button("打印列表(Debug)", GUIPreset.width[120]))
                    {
                        var results = new SyncResultCollection();
                        Regex filterRegex = entry.GetFilterRegex();
                        var operations = new List<SyncOperation>();
                        
                        int syncedCount = 0;
                        var sourceWatcher = watcher.GetSourceWatcher(entry.name);
                        var targetWatcher = watcher.GetTargetWatcher(entry.name);
                        var sourceNode = sourceWatcher.GetDirectory(sourceWatcher.rootPath);
                        var targetNode = targetWatcher.GetDirectory(targetWatcher.rootPath);
                        
                        FileSyncToolUtils.SyncDirectoryRecursive(sourceNode, targetNode, entry.targetPath, filterRegex, forceSync: true, ref syncedCount, results, operations);
                        
                        System.Text.StringBuilder sb = new();
                        sb.AppendLine($"[Debug] 收集到 {operations.Count} 个同步操作 (Entry: {entry.name})");
                        foreach(var op in operations)
                        {
                            sb.AppendLine($"Sync Src: {op.sourcePath}\nDst: {op.targetPath}\n");
                        }
                        string outputFilePath = Path.Combine(Application.dataPath, "..", "FileSyncDebugList.txt");
                        File.WriteAllText(outputFilePath, sb.ToString());
                        Debug.Log($"[Debug] 列表信息已输出到文件: {outputFilePath}");
                        GUIUtils.ClearFocus();
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
        }
    }
}
