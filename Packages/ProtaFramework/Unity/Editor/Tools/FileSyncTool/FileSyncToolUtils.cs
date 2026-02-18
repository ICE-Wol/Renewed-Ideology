using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Prota.Unity;
using Prota;
using UnityEngine.Profiling;

namespace Prota.Editor
{
    /// <summary>
    /// 文件同步工具工具类
    /// 提供文件处理相关的静态方法
    /// </summary>
    public static class FileSyncToolUtils
    {
        /// <summary>
        /// 路径类型
        /// </summary>
        public enum PathType
        {
            Invalid,    // 无效路径
            File,       // 文件
            Directory   // 文件夹
        }
        
        /// <summary>
        /// 获取路径类型
        /// </summary>
        public static PathType GetPathType(string path)
        {
            if (string.IsNullOrEmpty(path))
                return PathType.Invalid;
            
            if (File.Exists(path))
                return PathType.File;
            
            if (Directory.Exists(path))
                return PathType.Directory;
            
            return PathType.Invalid;
        }
        
        /// <summary>
        /// 检查文件是否匹配 filter 正则表达式
        /// </summary>
        public static bool MatchesFilter(string filePath, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            
            try
            {
                string fileName = Path.GetFileName(filePath);
                return Regex.IsMatch(fileName, filter);
            }
            catch
            {
                // 正则表达式无效时，不匹配任何文件
                return false;
            }
        }
        
        /// <summary>
        /// 检查文件是否匹配 filter 正则表达式（使用 Regex 对象）
        /// </summary>
        public static bool MatchesFilter(string filePath, Regex regex)
        {
            if (regex == null)
                return true;

            using var _ = new ProfilerScope("Regex match");
            try
            {
                string fileName = Path.GetFileName(filePath);
                return regex.IsMatch(fileName);
            }
            catch
            {
                // 正则表达式无效时，不匹配任何文件
                return false;
            }
        }
        
        /// <summary>
        /// 检查是否可以进行同步
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="reason">不能同步的原因</param>
        /// <returns>是否可以同步</returns>
        public static bool CanSync(string sourcePath, string targetPath, out string reason)
        {
            PathType sourceType = GetPathType(sourcePath);
            PathType targetType = GetPathType(targetPath);
            
            if (sourceType == PathType.Invalid)
            {
                reason = "无效输入路径";
                return false;
            }
            
            if (targetType == PathType.Invalid)
            {
                reason = "无效输出路径";
                return false;
            }
            
            if (sourceType != targetType)
            {
                reason = "类型不匹配";
                return false;
            }
            
            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// 同步文件（核心方法，不依赖 FileSyncEntry）
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="filterRegex">文件过滤器正则表达式（可为null）</param>
        /// <param name="entryName">条目名称（用于日志，可为null）</param>
        /// <param name="results">同步结果集合（如果为null则不收集）</param>
        /// <param name="watcher">FileSyncToolWatcher 实例，用于获取文件信息</param>
        public static void SyncFile(string sourcePath, string targetPath, Regex filterRegex, string entryName, SyncResultCollection results, FileSyncToolWatcher watcher = null)
        {
            string fileName = Path.GetFileName(sourcePath);
            
            // 设置路径信息
            results?.SetPaths(sourcePath, targetPath);
            
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                string reason = "源文件不存在或路径为空";
                results?.AddSkipped(fileName, sourcePath, targetPath, reason);
                return;
            }
            
            if (string.IsNullOrEmpty(targetPath))
            {
                string reason = "目标路径不能为空";
                results?.AddSkipped(fileName, sourcePath, targetPath, reason);
                return;
            }
            
            // 检查文件是否匹配 filter
            if (!MatchesFilter(sourcePath, filterRegex))
            {
                results?.AddFilterMismatch(fileName, sourcePath);
                return;
            }
            
            try
            {
                // 检查主文件内容是否相同
                bool canCheckContent = watcher != null && !string.IsNullOrEmpty(entryName);
                bool mainFileEqual = canCheckContent && AreFileContentsEqual(sourcePath, targetPath, watcher, entryName);
                bool metaNeedsSync = canCheckContent && MetaNeedsSync(sourcePath, targetPath, watcher, entryName);
                
                // 如果主文件内容相同且meta文件也相同，才跳过
                if (mainFileEqual && !metaNeedsSync)
                {
                    string reason = "文件内容相同，跳过";
                    results?.AddSkipped(fileName, sourcePath, targetPath, reason);
                    return;
                }

                bool hasChanges;

                var originallyExists = File.Exists(targetPath);
                hasChanges = CopyFile(sourcePath, targetPath, watcher, entryName);
                if (originallyExists)
                {
                    results?.AddSourceNewerCopied(fileName, sourcePath, targetPath, hasChanges);
                }
                else
                {
                    results?.AddSourceToTarget(fileName, sourcePath, targetPath, hasChanges);
                }
                
                if (hasChanges && !string.IsNullOrEmpty(entryName))
                {
                    // Debug.Log($"文件同步成功: {entryName}\n源: {sourcePath}\n目标: {targetPath}");
                }
            }
            catch (Exception e)
            {
                string reason = $"文件同步失败: {e.Message}";
                // Debug.LogError($"{reason}\n{e.StackTrace}");
                results?.AddSkipped(fileName, sourcePath, targetPath, reason);
            }
        }
        
        private static void CollectOperation(string sourcePath, string targetPath, List<SyncOperation> collectedOperations, SyncOperationType type = SyncOperationType.CopyFile)
        {
            collectedOperations.Add(new SyncOperation { type = type, sourcePath = sourcePath, targetPath = targetPath });
        }

        static bool TryCollectMetaCopyIfNeeded(
            DirectoryNode sourceDirNode,
            DirectoryNode targetDirNode,
            string fileName,
            string targetDir,
            List<SyncOperation> collectedOperations,
            out string sourceMetaPath,
            out string targetMetaPath)
        {
            string metaFileName = fileName + ".meta";

            sourceMetaPath = null;
            targetMetaPath = null;

            if (!sourceDirNode.TryGetChildFile(metaFileName, out var sourceMetaNode) || !sourceMetaNode.isValid)
                return false;

            if (!MetaNeedsSync(sourceDirNode, targetDirNode, fileName))
                return false;

            sourceMetaPath = sourceMetaNode.fullPath;
            targetMetaPath = Path.Combine(targetDir, metaFileName).ToStandardPath();

            CollectOperation(sourceMetaPath, targetMetaPath, collectedOperations);
            return true;
        }

        /// <summary>
        /// 同步文件夹
        /// </summary>
        public static void SyncDirectory(DirectoryNode sourceDirNode, DirectoryNode targetDirNode, string targetPath, Regex filterRegex, bool forceSync, SyncResultCollection results, FileSyncToolWatcher watcher, string entryName)
        {
            var operations = new List<SyncOperation>();
            int syncedCount = 0;
            
            SyncDirectoryRecursive(sourceDirNode, targetDirNode, targetPath, filterRegex, forceSync, ref syncedCount, results, operations);
            
            foreach (var op in operations)
            {
                op.Apply();
            }
            
            if (syncedCount > 0 && !string.IsNullOrEmpty(entryName))
            {
                Debug.Log($"文件夹同步成功: {entryName}\n源: {sourceDirNode.fullPath}\n目标: {targetPath}\n同步了 {syncedCount} 个文件");
            }
        }

        /// <summary>
        /// 递归同步文件夹
        /// </summary>
        public static void SyncDirectoryRecursive(
            DirectoryNode sourceDirNode,
            DirectoryNode targetDirNode,
            string targetDir,
            Regex filterRegex,
            bool forceSync,
            ref int syncedCount,
            SyncResultCollection results,
            List<SyncOperation> collectedOperations
            )
        {
            using var _ = new ProfilerScope(sourceDirNode.fullPath);

            if (collectedOperations == null)
            {
                Debug.LogError("[FileSyncToolUtils] SyncDirectoryRecursive requires collectedOperations list");
                return;
            }
            
            if(!sourceDirNode.isRefValid)
                throw new Exception("[FileSyncToolUtils] SyncDirectoryRecursive sourceDirNode is null.");
            
            if(!targetDirNode.isRefValid)
                throw new Exception("[FileSyncToolUtils] SyncDirectoryRecursive targetDirNode is null.");
            
            if(!sourceDirNode.isValid)
                throw new Exception("[FileSyncToolUtils] SyncDirectoryRecursive sourceDirNode is invalid.");
            
            string sourceDir = sourceDirNode.fullPath;
            
            // 确保目标目录存在（如果不存在则创建）
            if (!targetDirNode.isValid)
            {
                CollectOperation(sourceDir, targetDir, collectedOperations, SyncOperationType.CreateDirectory);
                // 如果目录不存在, 则只创建这个目录.
                // 剩下的文件复制操作等下一轮再进行.
                return;
            }

            // 遍历源目录下的所有文件
            var sourceFileNames = sourceDirNode.files;
            foreach (var srcFileName in sourceFileNames)
            {
                // 跳过 .meta 文件，它们会在主文件同步时一起处理
                if (srcFileName.EndsWith(".meta"))
                    continue;
                
                var fullSrcFileName = sourceDirNode.fileMap[srcFileName];
                var sourceFileNode = sourceDirNode.GetChildFile(srcFileName);

                // 检查文件是否匹配 filter
                if (!MatchesFilter(fullSrcFileName, filterRegex))
                {
                    results?.AddFilterMismatch(srcFileName, fullSrcFileName);
                    continue;
                }

                // target 不存在, 直接复制.
                if (!targetDirNode.TryGetChildFile(srcFileName, out var targetFileNode))
                {
                    // 源文件存在，目标文件不存在，直接复制
                    var fullTargetFileName = Path.Combine(targetDir, srcFileName).ToStandardPath();
                    CollectOperation(fullSrcFileName, fullTargetFileName, collectedOperations);
                    syncedCount++;
                    results?.AddSourceToTarget(srcFileName, fullSrcFileName, fullTargetFileName, true);

                    if (TryCollectMetaCopyIfNeeded(sourceDirNode, targetDirNode, srcFileName, targetDir, collectedOperations, out var sourceMetaPathMissingTarget, out var targetMetaPathMissingTarget))
                    {
                        syncedCount++;
                        results?.AddSourceToTarget(srcFileName + ".meta", sourceMetaPathMissingTarget, targetMetaPathMissingTarget, true);
                    }
                    continue;
                }
                
                // target 存在, 继续执行
                if (forceSync)
                {
                    // 强制同步：比较主文件内容和meta文件
                    bool mainFileEqual = AreFileContentsEqual(sourceFileNode, targetFileNode);
                    bool metaNeedsSync = MetaNeedsSync(sourceDirNode, targetDirNode, srcFileName);

                    if (mainFileEqual && metaNeedsSync)
                    {
                        if (TryCollectMetaCopyIfNeeded(sourceDirNode, targetDirNode, srcFileName, targetDir, collectedOperations, out var sourceMetaPathForceMetaOnly, out var targetMetaPathForceMetaOnly))
                        {
                            syncedCount++;
                            results?.AddForceSyncCopied(srcFileName + ".meta", sourceMetaPathForceMetaOnly, targetMetaPathForceMetaOnly, true);
                        }
                        continue;
                    }

                    if (mainFileEqual)
                    {
                        results?.AddForceSyncSkipped(srcFileName, fullSrcFileName, targetFileNode.fullPath);
                        continue;
                    }
                    
                    // 强制从源复制到目标
                    CollectOperation(fullSrcFileName, targetFileNode.fullPath, collectedOperations);
                    syncedCount++;
                    results?.AddForceSyncCopied(srcFileName, fullSrcFileName, targetFileNode.fullPath, true);

                    if (metaNeedsSync && TryCollectMetaCopyIfNeeded(sourceDirNode, targetDirNode, srcFileName, targetDir, collectedOperations, out var sourceMetaPathForceTogether, out var targetMetaPathForceTogether))
                    {
                        syncedCount++;
                        results?.AddForceSyncCopied(srcFileName + ".meta", sourceMetaPathForceTogether, targetMetaPathForceTogether, true);
                    }
                    continue;
                }
                
                // 比较修改时间（使用 FileNode 的 lastWriteTime）
                DateTime sourceTime = sourceFileNode.lastWriteTime;
                DateTime targetTime = targetFileNode.lastWriteTime;
                
                if (sourceTime > targetTime)
                {
                    // 源文件更新，复制源到目标
                    CollectOperation(fullSrcFileName, targetFileNode.fullPath, collectedOperations);
                    syncedCount++;
                    results?.AddSourceNewerCopied(srcFileName, fullSrcFileName, targetFileNode.fullPath, true);

                    if (TryCollectMetaCopyIfNeeded(sourceDirNode, targetDirNode, srcFileName, targetDir, collectedOperations, out var sourceMetaPathSourceNewer, out var targetMetaPathSourceNewer))
                    {
                        syncedCount++;
                        results?.AddSourceNewerCopied(srcFileName + ".meta", sourceMetaPathSourceNewer, targetMetaPathSourceNewer, true);
                    }
                    continue;
                }

                if (TryCollectMetaCopyIfNeeded(sourceDirNode, targetDirNode, srcFileName, targetDir, collectedOperations, out var sourceMetaPathSyncOnly, out var targetMetaPathSyncOnly))
                {
                    syncedCount++;
                    results?.AddForceSyncCopied(srcFileName + ".meta", sourceMetaPathSyncOnly, targetMetaPathSyncOnly, true);
                    continue;
                }
                
                // 跳过同步
                string skipReason;
                if (sourceTime == targetTime)
                {
                    skipReason = "文件时间相同，跳过";
                }
                else
                {
                    skipReason = "目标文件更新，跳过";
                }
                
                results?.AddSkipped(srcFileName, fullSrcFileName, targetFileNode.fullPath, skipReason);
            }

            // 递归处理子目录（使用 watcher 获取子目录列表）
            // 目标目录被记录时才会递归操作子文件, 也就是说新建目录和复制它的文件会在两个轮次中完成.
            if (targetDirNode.isValid)
            {
                var sourceDirNames = sourceDirNode.directories;
                foreach (var dirName in sourceDirNames)
                {
                    if(!targetDirNode.directoryMap.TryGetValue(dirName, out var targetSubDir))
                    {
                        var sourceSubDirFullPath = sourceDirNode.directoryMap[dirName];
                        var targetSubDirFullPath = Path.Combine(targetDir, dirName).ToStandardPath();
                        CollectOperation(sourceSubDirFullPath, targetSubDirFullPath, collectedOperations, SyncOperationType.CreateDirectory);
                        continue;
                    }
                    
                    var dirFullPath = sourceDirNode.directoryMap[dirName];
                    var sourceSubDirNode = sourceDirNode.GetChildDirectory(dirName);
                    var targetSubDirNode = targetDirNode.GetChildDirectory(dirName);
                    SyncDirectoryRecursive(
                        sourceSubDirNode,
                        targetSubDirNode,
                        targetSubDir,
                        filterRegex,
                        forceSync,
                        ref syncedCount,
                        results,
                        collectedOperations
                    );
                }
            }
        }
        
        /// <summary>
        /// 获取源和目标 watcher（公共方法，避免重复代码）
        /// </summary>
        public static bool TryGetWatchers(FileSyncToolWatcher watcher, string entryName, out DirectoryTreeWatcher sourceWatcher, out DirectoryTreeWatcher targetWatcher)
        {
            sourceWatcher = null;
            targetWatcher = null;
            
            if (watcher == null || string.IsNullOrEmpty(entryName))
                return false;
            
            sourceWatcher = watcher.GetSourceWatcher(entryName);
            targetWatcher = watcher.GetTargetWatcher(entryName);
            
            return sourceWatcher != null && targetWatcher != null;
        }
        
        /// <summary>
        /// 检查两个文件的内容是否相同
        /// </summary>
        public static bool AreFileContentsEqual(FileNode sourceFileNode, FileNode targetFileNode)
        {
            // 如果任一文件不存在，返回 false
            if (!sourceFileNode.isValid || !targetFileNode.isValid)
                return false;

            using var _ = new ProfilerScope("AreFileContentsEqual");
            try
            {
                // 使用 FileNode 的 content 属性读取文件内容进行比较
                byte[] sourceContent = sourceFileNode.content;
                byte[] targetContent = targetFileNode.content;
                if (ReferenceEquals(sourceContent, targetContent))
                    return true;

                if (sourceContent == null || targetContent == null)
                    return false;

                if (sourceContent.Length != targetContent.Length)
                    return false;

                for (int i = 0; i < sourceContent.Length; i++)
                {
                    if (sourceContent[i] != targetContent[i])
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FileSyncToolUtils] Failed to read file content: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查两个文件的内容是否相同
        /// 只能通过 watcher 读取数据，不支持直接文件系统访问
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="watcher">FileSyncToolWatcher 实例，用于获取文件信息</param>
        /// <param name="entryName">条目名称，用于从 watcher 中获取对应的监视器</param>
        public static bool AreFileContentsEqual(string sourcePath, string targetPath, FileSyncToolWatcher watcher, string entryName)
        {
            if (!TryGetWatchers(watcher, entryName, out var sourceWatcher, out var targetWatcher))
            {
                Debug.LogWarning("[FileSyncToolUtils] AreFileContentsEqual requires watcher and entryName");
                return false;
            }
            
            // 通过 watcher 获取文件节点
            var sourceFileNode = sourceWatcher.GetFile(sourcePath);
            var targetFileNode = targetWatcher.GetFile(targetPath);
            
            return AreFileContentsEqual(sourceFileNode, targetFileNode);
        }
        
        /// <summary>
        /// 检查 meta 文件是否需要同步
        /// </summary>
        public static bool MetaNeedsSync(DirectoryNode sourceDirNode, DirectoryNode targetDirNode, string fileName)
        {
            string metaFileName = fileName + ".meta";

            bool sourceMetaExists = sourceDirNode.TryGetChildFile(metaFileName, out var sourceMetaNode);
            
            // 如果源没有 meta 文件，不需要同步
            if (!sourceMetaExists)
                return false;

            bool targetMetaExists = targetDirNode.TryGetChildFile(metaFileName, out var targetMetaNode);
            
            // 源有 meta 文件但是目标没有, 需要同步.
            if (sourceMetaExists != targetMetaExists)
                return true;
            
            // 两边都有meta文件，检查内容是否相同
            return !AreFileContentsEqual(sourceMetaNode, targetMetaNode);
        }

        /// <summary>
        /// 检查 meta 文件是否存在（通过 watcher 或文件系统）
        /// </summary>
        static bool MetaFileExists(string metaPath, DirectoryTreeWatcher watcher)
        {
            if (watcher != null)
            {
                var metaNode = watcher.GetFile(metaPath);
                return metaNode.isValid;
            }
            return File.Exists(metaPath);
        }
        
        /// <summary>
        /// 检查 meta 文件是否需要同步
        /// </summary>
        public static bool MetaNeedsSync(string sourcePath, string targetPath, FileSyncToolWatcher watcher, string entryName)
        {
            string sourceMetaPath = sourcePath + ".meta";
            string targetMetaPath = targetPath + ".meta";
            
            DirectoryTreeWatcher sourceWatcher = null;
            DirectoryTreeWatcher targetWatcher = null;
            
            if (watcher != null && !string.IsNullOrEmpty(entryName))
            {
                sourceWatcher = watcher.GetSourceWatcher(entryName);
                targetWatcher = watcher.GetTargetWatcher(entryName);
            }
            
            bool sourceMetaExists = MetaFileExists(sourceMetaPath, sourceWatcher);
            bool targetMetaExists = MetaFileExists(targetMetaPath, targetWatcher);
            
            // 如果两边都没有 meta 文件，不需要同步
            if (!sourceMetaExists && !targetMetaExists)
                return false;
            
            // meta文件存在性不一致，需要同步
            if (sourceMetaExists != targetMetaExists)
                return true;
            
            // 两边都有meta文件，检查内容是否相同
            return !AreFileContentsEqual(sourceMetaPath, targetMetaPath, watcher, entryName);
        }
        
        /// <summary>
        /// 检查文件是否需要复制（比较文件内容）
        /// </summary>
        public static bool NeedCopyFile(string sourcePath, string targetPath, FileSyncToolWatcher watcher, string entryName)
        {
            if(watcher == null) throw new ArgumentNullException(nameof(watcher));
            
            // 检查目标文件是否存在
            var targetWatcher = watcher.GetTargetWatcher(entryName);
            var targetFileNode = targetWatcher.GetFile(targetPath);
            if (!targetFileNode.isValid)
                return true;
            
            // 比较文件内容
            return !AreFileContentsEqual(sourcePath, targetPath, watcher, entryName);
        }
        
        /// <summary>
        /// 复制单个文件（如果需要）
        /// </summary>
        /// <returns>是否执行了复制操作</returns>
        public static bool CopyFileIfNeeded(string sourcePath, string targetPath, FileSyncToolWatcher watcher, string entryName)
        {
            if(watcher == null) throw new ArgumentNullException(nameof(watcher));
            
            if (!NeedCopyFile(sourcePath, targetPath, watcher, entryName))
                return false;
            
            // 确保目标目录存在
            string targetDir = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(targetDir))
            {
                targetDir.AsDirectoryInfo().EnsureExists();
            }
            
            ProtaFileUtils.CopyWithExactCase(sourcePath, targetPath);
            return true;
        }
        
        /// <summary>
        /// 复制文件并刷新 Unity 资源数据库（如果目标文件在 Assets 目录下）
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="watcher">FileSyncToolWatcher 实例，用于获取文件信息</param>
        /// <param name="entryName">条目名称，用于从 watcher 中获取对应的监视器</param>
        /// <returns>是否有文件被复制（包括 meta）</returns>
        public static bool CopyFile(string sourcePath, string targetPath, FileSyncToolWatcher watcher, string entryName)
        {
            if(watcher == null) throw new ArgumentNullException(nameof(watcher));
            
            bool hasChanges = CopyFileIfNeeded(sourcePath, targetPath, watcher, entryName);
            
            // 检查是否有对应的 .meta 文件，如果有则一起同步
            string sourceMetaPath = sourcePath + ".meta";
            var sourceWatcher = watcher.GetSourceWatcher(entryName);
            bool sourceMetaExists = MetaFileExists(sourceMetaPath, sourceWatcher);
            
            if (!sourceMetaExists)
                return hasChanges;
            
            string targetMetaPath = targetPath + ".meta";
            if (CopyFileIfNeeded(sourceMetaPath, targetMetaPath, watcher, entryName))
            {
                hasChanges = true;
                Debug.Log($"Meta 文件同步成功: {targetMetaPath}");
            }
            
            return hasChanges;
        }
    }
}



