using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using Prota;
using Prota.Unity;
using UnityEngine;
using UnityEngine.Profiling;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Prota.Editor
{
    /// <summary>
    /// 文件同步工具自动同步管理器
    /// 独立于窗口运行，在编辑器启动时自动初始化
    /// </summary>
    public static class FileSyncToolAutoSync
    {
        public readonly struct AutoSyncTimingInfo
        {
            public readonly DateTime lastSyncTimeUtc;
            public readonly double lastCollectOperationsMs;
            public readonly double lastApplyOperationsMs;
            public readonly double lastTotalMs;
            public readonly int lastCollectedOperationsCount;
            public readonly int lastExecutedOperationsCount;

            public AutoSyncTimingInfo(
                DateTime lastSyncTimeUtc,
                double lastCollectOperationsMs,
                double lastApplyOperationsMs,
                double lastTotalMs,
                int lastCollectedOperationsCount,
                int lastExecutedOperationsCount)
            {
                this.lastSyncTimeUtc = lastSyncTimeUtc;
                this.lastCollectOperationsMs = lastCollectOperationsMs;
                this.lastApplyOperationsMs = lastApplyOperationsMs;
                this.lastTotalMs = lastTotalMs;
                this.lastCollectedOperationsCount = lastCollectedOperationsCount;
                this.lastExecutedOperationsCount = lastExecutedOperationsCount;
            }
        }

        static readonly Dictionary<string, AutoSyncTimingInfo> _lastTimingByEntryName = new();

        public static bool TryGetLastTimingInfo(string entryName, out AutoSyncTimingInfo timingInfo)
        {
            if (string.IsNullOrEmpty(entryName))
            {
                timingInfo = default;
                return false;
            }

            return _lastTimingByEntryName.TryGetValue(entryName, out timingInfo);
        }

        struct DirectorySyncTask
        {
            public string sourcePath;
            public string targetPath;
            public Regex filterRegex;
            public string entryName;
        }
        
        static EditorTimer syncTimer;
        const float syncInterval = 0.5f;
        
        static FileSyncToolAutoSync()
        {
            EditorTimerManager.OnManagerInitialized(() =>
            {
                syncTimer = EditorTimerManager.NewRepeat(syncInterval, OnSyncTimer);
            });
        }
        
        static FileSyncToolWatcher _watcher;
        
        public static FileSyncToolWatcher GetWatcher()
        {
            return _watcher;
        }
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            _watcher = new FileSyncToolWatcher();
        }

        struct SyncingScope : IDisposable
        {
            public SyncingScope(bool tagSynching)
            {
                syncing = tagSynching;
            }
            
            public void Dispose()
            {
                syncing = false;
            }
        }

        static bool syncing = false;
        
        static void OnSyncTimer()
        {
            if(syncing) return;
            
            var data = FileSyncToolData.instance;
            if (data == null || data.entries == null)
                return;
            
            if (data.pauseAutoSync)
                return;

            using var synchingTag = new SyncingScope(true);
            
            // 收集需要同步的文件夹任务
            var directorySyncTasks = new List<DirectorySyncTask>();
            
            for (int i = 0; i < data.entries.Count; i++)
            {
                var entry = data.entries[i];
                if (!entry.CanAutoSync())
                    continue;
                
                var sourceType = FileSyncToolUtils.GetPathType(entry.sourcePath);
                var targetType = FileSyncToolUtils.GetPathType(entry.targetPath);
                
                // 类型不匹配或路径无效，跳过（GetPathType 已检查存在性，无需再次检查）
                if (sourceType == FileSyncToolUtils.PathType.Invalid
                || targetType == FileSyncToolUtils.PathType.Invalid
                || sourceType != targetType
                || sourceType != FileSyncToolUtils.PathType.Directory)
                    continue;
                
                // 更新 watcher
                _watcher.SetWatcher(entry.name, entry.sourcePath, entry.targetPath);
                
                DirectorySyncTask task = new DirectorySyncTask
                {
                    sourcePath = entry.sourcePath,
                    targetPath = entry.targetPath,
                    filterRegex = entry.GetFilterRegex(),
                    entryName = entry.name
                };
                
                directorySyncTasks.Add(task);
            }
            
            // 同步阶段：批量执行目录同步操作
            for (int i = 0; i < directorySyncTasks.Count; i++)
            {
                var task = directorySyncTasks[i];
                
                try
                {
                    Profiler.BeginSample("FileSyncToolAutoSync_SyncDirectory: " + task.entryName);

                    var totalStopwatch = Stopwatch.StartNew();
                    double collectOperationsMs = 0;
                    double applyOperationsMs = 0;
                    
                    // GetPathType 已检查存在性，无需再次检查 Directory.Exists
                    // 使用与手动同步相同的递归逻辑
                    int syncedCount = 0;
                    var operations = new List<SyncOperation>();
                    
                    if (FileSyncToolUtils.TryGetWatchers(_watcher, task.entryName, out var sourceWatcher, out var targetWatcher))
                    {
                        var sourceDirNode = sourceWatcher.GetDirectory(task.sourcePath.ToStandardFullPath());
                        var targetDirNode = targetWatcher.GetDirectory(task.targetPath.ToStandardFullPath());

                        var collectStopwatch = Stopwatch.StartNew();
                        FileSyncToolUtils.SyncDirectoryRecursive(
                            sourceDirNode,
                            targetDirNode,
                            task.targetPath,
                            task.filterRegex,
                            false,
                            ref syncedCount,
                            null,
                            operations
                        );
                        collectStopwatch.Stop();
                        collectOperationsMs = collectStopwatch.Elapsed.TotalMilliseconds;
                    }
                    
                    // 执行同步操作
                    var applyStopwatch = Stopwatch.StartNew();
                    int executedCount = 0;
                    Profiler.BeginSample("Execute Operation");
                    for (int j = 0; j < operations.Count; j++)
                    {
                        var op = operations[j];
                        Profiler.BeginSample(operations[j].sourcePath);
                        try
                        {
                            if (op.Apply())
                            {
                                executedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"文件同步执行失败: {op.sourcePath} -> {op.targetPath}\n{ex.Message}");
                        }
                        finally
                        {
                            Profiler.EndSample();
                        }
                    }
                    Profiler.EndSample();

                    applyStopwatch.Stop();
                    applyOperationsMs = applyStopwatch.Elapsed.TotalMilliseconds;
                    
                    if (executedCount > 0 && !string.IsNullOrEmpty(task.entryName))
                    {
                        Debug.Log($"自动同步文件夹: {task.entryName}\n同步了 {executedCount} 个文件");
                    }

                    totalStopwatch.Stop();
                    if (!string.IsNullOrEmpty(task.entryName))
                    {
                        _lastTimingByEntryName[task.entryName] = new AutoSyncTimingInfo(
                            DateTime.UtcNow,
                            collectOperationsMs,
                            applyOperationsMs,
                            totalStopwatch.Elapsed.TotalMilliseconds,
                            operations.Count,
                            executedCount
                        );
                    }
                    
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    string errorMsg = !string.IsNullOrEmpty(task.entryName) 
                        ? $"自动同步失败 ({task.entryName}): {e.Message}\n{e.StackTrace}"
                        : $"自动同步失败: {e.Message}\n{e.StackTrace}";
                    Debug.LogError(errorMsg);
                    Debug.LogException(e);
                }
            }
        }
    }
}

