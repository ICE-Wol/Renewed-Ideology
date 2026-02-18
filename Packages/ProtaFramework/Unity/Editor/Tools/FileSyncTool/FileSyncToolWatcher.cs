using System;
using System.Collections.Generic;
using System.IO;
using Prota;
using Prota.Unity;
using UnityEngine;

namespace Prota.Editor
{
    /// <summary>
    /// 文件同步工具监视器管理器
    /// 根据文件夹类型的 SyncTask 维护 DirectoryTreeWatcher
    /// </summary>
    public partial class FileSyncToolWatcher : IDisposable
    {
        public readonly struct WatcherInfo
        {
            public readonly bool hasWatcher;
            public readonly DateTime sourceLastUpdateTime;
            public readonly DateTime targetLastUpdateTime;
            public readonly TimeSpan sourceLastScanTimeConsume;
            public readonly TimeSpan targetLastScanTimeConsume;
            public readonly int sourceScanIntervalMs;
            public readonly int targetScanIntervalMs;

            public WatcherInfo(
                bool hasWatcher,
                DateTime sourceLastUpdateTime,
                DateTime targetLastUpdateTime,
                TimeSpan sourceLastScanTimeConsume,
                TimeSpan targetLastScanTimeConsume,
                int sourceScanIntervalMs,
                int targetScanIntervalMs)
            {
                this.hasWatcher = hasWatcher;
                this.sourceLastUpdateTime = sourceLastUpdateTime;
                this.targetLastUpdateTime = targetLastUpdateTime;
                this.sourceLastScanTimeConsume = sourceLastScanTimeConsume;
                this.targetLastScanTimeConsume = targetLastScanTimeConsume;
                this.sourceScanIntervalMs = sourceScanIntervalMs;
                this.targetScanIntervalMs = targetScanIntervalMs;
            }
        }

        /// <summary>
        /// 监视器对，包含源目录和目标目录的 watcher
        /// </summary>
        private class WatcherPair
        {
            public DirectoryTreeWatcher sourceWatcher;
            public DirectoryTreeWatcher targetWatcher;
            public string sourcePath;
            public string targetPath;
            
            public void Dispose()
            {
                sourceWatcher?.Dispose();
                targetWatcher?.Dispose();
                sourceWatcher = null;
                targetWatcher = null;
            }
        }
        
        private Dictionary<string, WatcherPair> _watchers = new();
        
        /// <summary>
        /// 设置指定 SyncTask 的监视器
        /// 如果路径发生变化，将创建新的监视器并替换旧的
        /// 如果路径不合法，将删除已有的监视器
        /// </summary>
        /// <param name="entryName">条目名称，用作唯一标识</param>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        public void SetWatcher(string entryName, string sourcePath, string targetPath)
        {
            if (string.IsNullOrEmpty(entryName))
            {
                Debug.LogWarning("[FileSyncToolWatcher] Entry name is null or empty, skipping watcher update");
                return;
            }
            
            // 检查路径合法性
            var sourceType = FileSyncToolUtils.GetPathType(sourcePath);
            var targetType = FileSyncToolUtils.GetPathType(targetPath);
            
            // 只有当两个路径都是目录类型时才创建 watcher
            bool shouldCreateWatcher = sourceType == FileSyncToolUtils.PathType.Directory 
                && targetType == FileSyncToolUtils.PathType.Directory;
            
            if (!shouldCreateWatcher)
            {
                // 路径不合法，删除已有的 watcher
                RemoveWatcher(entryName);
                return;
            }
            
            string normalizedSourcePath = sourcePath.ToStandardFullPath();
            string normalizedTargetPath = targetPath.ToStandardFullPath();
            
            // 检查是否已存在
            if (_watchers.TryGetValue(entryName, out var existingPair))
            {
                // 检查路径是否变化
                bool sourceChanged = !string.Equals(
                    existingPair.sourcePath, 
                    normalizedSourcePath, 
                    StringComparison.OrdinalIgnoreCase);
                bool targetChanged = !string.Equals(
                    existingPair.targetPath, 
                    normalizedTargetPath, 
                    StringComparison.OrdinalIgnoreCase);
                
                if (!sourceChanged && !targetChanged)
                {
                    // 路径未变化，无需更新
                    return;
                }
                
                // 路径变化，先清理旧的
                existingPair.Dispose();
            }
            
            // 创建新的 watcher
            try
            {
                var sourceWatcher = new DirectoryTreeWatcher(normalizedSourcePath);
                sourceWatcher.SetScanInterval(600);
                var targetWatcher = new DirectoryTreeWatcher(normalizedTargetPath);
                targetWatcher.SetScanInterval(600);
                
                _watchers[entryName] = new WatcherPair
                {
                    sourceWatcher = sourceWatcher,
                    targetWatcher = targetWatcher,
                    sourcePath = normalizedSourcePath,
                    targetPath = normalizedTargetPath
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FileSyncToolWatcher] Failed to create watchers for entry '{entryName}': {ex}");
            }
        }
        
        /// <summary>
        /// 移除指定条目的监视器
        /// </summary>
        public void RemoveWatcher(string entryName)
        {
            if (string.IsNullOrEmpty(entryName))
                return;
            
            if (_watchers.TryGetValue(entryName, out var pair))
            {
                pair.Dispose();
                _watchers.Remove(entryName);
            }
        }
        
        /// <summary>
        /// 获取源目录的监视器
        /// </summary>
        public DirectoryTreeWatcher GetSourceWatcher(string entryName)
        {
            if (_watchers.TryGetValue(entryName, out var pair))
            {
                return pair.sourceWatcher;
            }
            return null;
        }
        
        /// <summary>
        /// 获取目标目录的监视器
        /// </summary>
        public DirectoryTreeWatcher GetTargetWatcher(string entryName)
        {
            if (_watchers.TryGetValue(entryName, out var pair))
            {
                return pair.targetWatcher;
            }
            return null;
        }
        
        /// <summary>
        /// 获取监视器信息
        /// </summary>
        /// <param name="entryName"></param>
        /// <returns>是否有监视器，以及源和目标监视器的最后更新时间</returns>
        public WatcherInfo GetWatcherInfo(string entryName)
        {
            if (_watchers.TryGetValue(entryName, out var pair))
            {
                var sourceWatcher = pair.sourceWatcher;
                var targetWatcher = pair.targetWatcher;

                return new WatcherInfo(
                    true,
                    sourceWatcher?.GetLastScanTime() ?? DateTime.MinValue,
                    targetWatcher?.GetLastScanTime() ?? DateTime.MinValue,
                    sourceWatcher?.lastScanTimeConsume ?? TimeSpan.Zero,
                    targetWatcher?.lastScanTimeConsume ?? TimeSpan.Zero,
                    sourceWatcher?.GetScanInterval() ?? 0,
                    targetWatcher?.GetScanInterval() ?? 0);
            }
            return default;
        }
        
        /// <summary>
        /// 检查指定条目是否有有效的监视器
        /// </summary>
        public bool HasWatcher(string entryName)
        {
            return !string.IsNullOrEmpty(entryName) && _watchers.ContainsKey(entryName);
        }
        
        public void Dispose()
        {
            foreach (var pair in _watchers.Values)
            {
                pair.Dispose();
            }
            _watchers.Clear();
        }
    }
}

