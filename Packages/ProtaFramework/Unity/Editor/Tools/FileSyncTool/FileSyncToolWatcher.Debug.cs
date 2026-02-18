using System;
using System.IO;
using UnityEngine;
using Prota;
using Prota.Unity;
using System.Linq;

namespace Prota.Editor
{
    public partial class FileSyncToolWatcher
    {
        /// <summary>
        /// 打印监视器数据
        /// </summary>
        public void PrintWatcherData(string entryName, Func<string, bool> filter = null)
        {
            if (!_watchers.TryGetValue(entryName, out var pair))
            {
                Debug.LogWarning($"[FileSyncToolWatcher] No watchers found for entry '{entryName}'");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== Watcher Data for '{entryName}' ===");

            sb.AppendLine("--- Source Watcher ---");
            if (pair.sourceWatcher != null)
            {
                var root = pair.sourceWatcher.GetDirectory(pair.sourceWatcher.rootPath);
                AppendDirectoryNode(sb, root, 0, pair.sourceWatcher, pair.targetWatcher, root.fullPath, filter);
            }
            else
            {
                sb.AppendLine("Source watcher is null");
            }

            sb.AppendLine();
            sb.AppendLine("--- Target Watcher ---");
            if (pair.targetWatcher != null)
            {
                var root = pair.targetWatcher.GetDirectory(pair.targetWatcher.rootPath);
                AppendDirectoryNode(sb, root, 0, pair.targetWatcher, pair.sourceWatcher, root.fullPath, filter);
            }
            else
            {
                sb.AppendLine("Target watcher is null");
            }

            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 打印监视器数据到文件
        /// </summary>
        public void PrintWatcherDataToFile(string entryName, string filePath, Func<string, bool> filter = null)
        {
            if (!_watchers.TryGetValue(entryName, out var pair))
            {
                Debug.LogWarning($"[FileSyncToolWatcher] No watchers found for entry '{entryName}'");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== Watcher Data for '{entryName}' ===");
            sb.AppendLine($"Time: {DateTime.Now}");

            sb.AppendLine("--- Source Watcher ---");
            if (pair.sourceWatcher != null)
            {
                var root = pair.sourceWatcher.GetDirectory(pair.sourceWatcher.rootPath);
                AppendDirectoryNode(sb, root, 0, pair.sourceWatcher, pair.targetWatcher, root.fullPath, filter);
            }
            else
            {
                sb.AppendLine("Source watcher is null");
            }

            sb.AppendLine();
            sb.AppendLine("--- Target Watcher ---");
            if (pair.targetWatcher != null)
            {
                var root = pair.targetWatcher.GetDirectory(pair.targetWatcher.rootPath);
                AppendDirectoryNode(sb, root, 0, pair.targetWatcher, pair.sourceWatcher, root.fullPath, filter);
            }
            else
            {
                sb.AppendLine("Target watcher is null");
            }

            try
            {
                File.WriteAllText(filePath, sb.ToString());
                Debug.Log($"Watcher data written to {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write watcher data to file: {ex.Message}");
            }
        }

        private void AppendDirectoryNode(System.Text.StringBuilder sb, DirectoryNode node, int indent, DirectoryTreeWatcher currentWatcher, DirectoryTreeWatcher otherWatcher, string rootPath, Func<string, bool> filter)
        {
            string indentStr = new string(' ', indent * 2);
            
            // 目录状态
            string dirStatus = "";
            if (otherWatcher != null && !string.IsNullOrEmpty(rootPath))
            {
                string relativePath = Path.GetRelativePath(rootPath, node.fullPath);
                if (relativePath == ".") relativePath = "";
                
                var otherDirFullPath = string.IsNullOrEmpty(relativePath)
                    ? otherWatcher.rootPath
                    : Path.Combine(otherWatcher.rootPath, relativePath).ToStandardFullPath();
                var otherDir = otherWatcher.GetDirectory(otherDirFullPath);
                if (!otherDir.isValid)
                {
                    dirStatus = " [Missing in Other]";
                }
            }

            sb.AppendLine($"{indentStr}[Dir] {node.name} (LastWrite: {node.lastWriteTime}){dirStatus}");
            
            var fileNames = currentWatcher.GetDirectoryChildFiles(node.fullPath);
            foreach (var fileName in fileNames)
            {
                var fullPath = Path.Combine(node.fullPath, fileName).ToStandardFullPath();
                var file = currentWatcher.GetFile(fullPath);
                if (!file.isValid)
                {
                    sb.AppendLine($"{indentStr} - {fullPath} invalid file state");
                    continue;
                }
                
                // 文件状态
                string fileStatus = "";

                // 检查是否被筛选
                bool isFiltered = false;
                if (filter != null)
                {
                    try
                    {
                        if (!filter(file.name))
                        {
                            isFiltered = true;
                            fileStatus += " [Filtered]";
                        }
                    }
                    catch
                    {
                        // 忽略异常
                    }
                }

                if (otherWatcher != null && !string.IsNullOrEmpty(rootPath))
                {
                    string relativePath = Path.GetRelativePath(rootPath, file.fullPath);
                    var otherFile = otherWatcher.GetFile(Path.Combine(otherWatcher.rootPath, relativePath).ToStandardFullPath());
                    
                    if (!otherFile.isValid)
                    {
                        fileStatus += " [Missing in Other]";
                    }
                    else
                    {
                        // 比较时间
                        // DateTime.Compare(t1, t2) > 0 => t1 > t2 (t1 is later/newer)
                        int timeCompare = DateTime.Compare(file.lastWriteTimeUtc, otherFile.lastWriteTimeUtc);
                        string timeStr;
                        if (Math.Abs((file.lastWriteTimeUtc - otherFile.lastWriteTimeUtc).TotalSeconds) < 1.0)
                        {
                            timeStr = "SameTime";
                        }
                        else
                        {
                            timeStr = timeCompare > 0 ? "Newer" : "Older";
                        }
                        
                        // 比较内容
                        byte[] thisContent = currentWatcher.ReadFileContent(file.fullPath);
                        byte[] otherContent = otherWatcher.ReadFileContent(otherFile.fullPath);
                        bool contentSame = thisContent.SequenceEqual(otherContent);
                        string contentStr = contentSame ? "ContentSame" : "ContentDiff";
                        
                        fileStatus += $" [{timeStr}, {contentStr}] filtered:{isFiltered}";
                    }
                }

                sb.AppendLine($"{indentStr} - {file.name} (LastWrite: {file.lastWriteTime}){fileStatus}");
            }
            
            var dirNames = currentWatcher.GetDirectoryChildDirectories(node.fullPath);
            foreach (var dirName in dirNames)
            {
                var dir = currentWatcher.GetDirectory(Path.Combine(node.fullPath, dirName).ToStandardFullPath());
                if (!dir.isValid) continue;
                AppendDirectoryNode(sb, dir, indent + 1, currentWatcher, otherWatcher, rootPath, filter);
            }
        }
    }
}
