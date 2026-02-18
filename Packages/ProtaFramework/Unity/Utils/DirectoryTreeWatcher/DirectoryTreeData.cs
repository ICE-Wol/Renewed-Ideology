using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace Prota.Unity
{
    /// <summary>
    /// 目录树快照数据
    /// </summary>
    internal class DirectoryTreeData
    {
        public readonly DirectoryNodeData root;
        
        // 绝对路径 -> index 的映射
        public readonly Dictionary<string, int> fileMap;
        public readonly Dictionary<string, int> directoryMap;

        public readonly FileNodeData[] files;
        public readonly DirectoryNodeData[] directories;

        public DirectoryTreeData(
            DirectoryNodeData root,
            Dictionary<string, int> fileMap,
            Dictionary<string, int> directoryMap,
            FileNodeData[] files,
            DirectoryNodeData[] directories
        )
        {
            this.root = root;
            this.fileMap = fileMap;
            this.directoryMap = directoryMap;
            this.files = files;
            this.directories = directories;
        }
        
        public ref readonly FileNodeData GetFile(string fullPath)
        {
            if(!fileMap.TryGetValue(fullPath, out var index)) return ref FileNodeData.none;
            return ref files[index];
        }

        public ref readonly DirectoryNodeData GetDirectory(string fullPath)
        {
            if(!directoryMap.TryGetValue(fullPath, out var index)) return ref DirectoryNodeData.none;
            return ref directories[index];
        }

        public static void CompareAndNotify(
            DirectoryTreeData oldTree,
            DirectoryTreeData newTree,
            Action<DirectoryTreeWatcher.ChangeEvent> onChanged
        )
        {
            var oldRoot = oldTree.root;
            var newRoot = newTree.root;
            CompareAndNotifyDirectory(oldTree, newTree, oldRoot, newRoot, onChanged);
        }

        private static void CompareAndNotifyDirectory(
            DirectoryTreeData oldTree,
            DirectoryTreeData newTree,
            DirectoryNodeData oldNode,
            DirectoryNodeData newNode,
            Action<DirectoryTreeWatcher.ChangeEvent> onChanged
        )
        {
            using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.CompareAndNotifyDirectory"))
            {
                using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.CompareAndNotifyDirectory.Files"))
                {
                    var allFiles = new HashSet<string>();
                    foreach (var fileName in oldNode.filesMap.Keys) allFiles.Add(fileName);
                    foreach (var fileName in newNode.filesMap.Keys) allFiles.Add(fileName);

                    foreach (var fileName in allFiles)
                    {
                        var oldExists = oldNode.filesMap.TryGetValue(fileName, out var oldFileFullPath);
                        var newExists = newNode.filesMap.TryGetValue(fileName, out var newFileFullPath);

                        if (oldExists && !newExists)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Deleted, oldFileFullPath));
                            continue;
                        }

                        if (!oldExists && newExists)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Created, newFileFullPath));
                            continue;
                        }

                        var oldFile = oldTree.GetFile(oldFileFullPath);
                        var newFile = newTree.GetFile(newFileFullPath);

                        if(!newTree.root.fullPath.StartsWith("S:"))
                        {
                            UnityEngine.Debug.Log(oldFile.lastWriteTimeUtc + " " + newFile.lastWriteTimeUtc);
                        }

                        if (oldFile.lastWriteTimeUtc != newFile.lastWriteTimeUtc)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Changed, newFileFullPath));
                        }
                    }
                }

                using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.CompareAndNotifyDirectory.Directories"))
                {
                    var allDirs = new HashSet<string>();
                    foreach (var dirName in oldNode.directoriesMap.Keys) allDirs.Add(dirName);
                    foreach (var dirName in newNode.directoriesMap.Keys) allDirs.Add(dirName);

                    foreach (var dirName in allDirs)
                    {
                        var oldExists = oldNode.directoriesMap.TryGetValue(dirName, out var oldDirFullPath);
                        var newExists = newNode.directoriesMap.TryGetValue(dirName, out var newDirFullPath);

                        if (oldExists && !newExists)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Deleted, oldDirFullPath));
                            continue;
                        }

                        if (!oldExists && newExists)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Created, newDirFullPath));
                            continue;
                        }

                        var oldDir = oldTree.GetDirectory(oldDirFullPath);
                        var newDir = newTree.GetDirectory(newDirFullPath);

                        if (oldDir.lastWriteTimeUtc != newDir.lastWriteTimeUtc)
                        {
                            onChanged?.Invoke(new DirectoryTreeWatcher.ChangeEvent(WatcherChangeTypes.Changed, newDirFullPath));
                        }

                        CompareAndNotifyDirectory(oldTree, newTree, oldDir, newDir, onChanged);
                    }
                }
            }
        }

        public static DirectoryTreeData Scan(string path, DirectoryTreeData oldData)
        {
            using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.Scan " + path))
            {
                path = path.ToStandardFullPath();
                var fileMap = new Dictionary<string, int>();
                var dirMap = new Dictionary<string, int>();
                var files = new List<FileNodeData>();
                var directoryNodes = new List<DirectoryNodeData>();
                var root = ScanDirectory(path, oldData, fileMap, dirMap, files, directoryNodes);
                return new DirectoryTreeData(root, fileMap, dirMap, files.ToArray(), directoryNodes.ToArray());
            }
        }

        static DateTime GetLastTimeUtcSafe(string path)
        {
            try
            {
                return Directory.GetLastWriteTimeUtc(path);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[DirectoryTreeWatcher] Error getting directory info {path}: {e}");
                return DateTime.UtcNow;
            }
        }

        private static DirectoryNodeData ScanDirectory(
            string path,
            DirectoryTreeData oldTree,
            Dictionary<string, int> fileMap,
            Dictionary<string, int> dirMap,
            List<FileNodeData> files,
            List<DirectoryNodeData> directoryNodes
        )
        {
            var actualFiles = null as List<DirectoryChildEntry>;
            var actualDirs = null as List<DirectoryChildEntry>;
            
            using (new ProfilerScope(path))
            {
                using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.ScanDirectory.GetFiles"))
                {
                    try
                    {
                        var filesInDirectory = new List<DirectoryChildEntry>();
                        var filesArray = Directory.GetFiles(path);
                        foreach (var file in filesArray)
                        {
                            var fileFullPath = file.ToStandardPath();
                            var fileName = Path.GetFileName(fileFullPath);
                            filesInDirectory.Add(new DirectoryChildEntry(fileName, fileFullPath));
                            
                            // 根据文件的最后写入时间判断是否真的需要更新. 如果不需要, 则直接复用原节点.
                            if (!TryReuseOldData(fileFullPath, oldTree, out var fileNode))
                            {
                                fileNode = CreateFileNodeData(fileFullPath);
                            }
                            
                            var fileIndex = files.Count;
                            files.Add(fileNode);
                            fileMap[fileFullPath] = fileIndex;
                        }
                        
                        actualFiles = filesInDirectory;
                    }
                    catch (ThreadAbortException)
                    {
                        return DirectoryNodeData.none;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"[DirectoryTreeWatcher] Error scanning files in {path}:\n>>>\n{ex}\n<<<");
                    }
                }

                using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.ScanDirectory.GetDirectories"))
                {
                    try
                    {
                        var dirs = new List<DirectoryChildEntry>();
                        var directoryPaths = Directory.GetDirectories(path);
                        foreach (var dir in directoryPaths)
                        {
                            var dirFullPath = dir.ToStandardPath();
                            var dirName = Path.GetFileName(dirFullPath);
                            dirs.Add(new DirectoryChildEntry(dirName, dirFullPath));
                            ScanDirectory(dirFullPath, oldTree, fileMap, dirMap, files, directoryNodes);
                        }

                        actualDirs = dirs;
                    }
                    catch (ThreadAbortException)
                    {
                        return DirectoryNodeData.none;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"[DirectoryTreeWatcher] Error scanning directories in {path}:\n>>>\n{ex}\n<<<");
                    }
                }
            }
            
            using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.ScanDirectory.CreateNode"))
            {
                var node = new DirectoryNodeData(
                    name: Path.GetFileName(path),
                    fullPath: path,
                    lastWriteTimeUtc: GetLastTimeUtcSafe(path),
                    files: actualFiles,
                    directories: actualDirs
                );
                var dirIndex = directoryNodes.Count;
                directoryNodes.Add(node);
                dirMap[path] = dirIndex;
                return node;
            }
        }
        
        static bool TryReuseOldData(string fileFullPath, DirectoryTreeData oldData, out FileNodeData fileData)
        {
            fileData = FileNodeData.none;
            if(oldData == null) return false;
            var oldDataInstance = oldData;
            if(!oldDataInstance.fileMap.TryGetValue(fileFullPath, out var oldFileIndex)) return false;
            var oldFileData = oldDataInstance.files[oldFileIndex];
            var realLastWriteTime = File.GetLastWriteTimeUtc(fileFullPath);
            var oldLastWriteTime = oldFileData.lastWriteTimeUtc;
            if(realLastWriteTime != oldLastWriteTime) return false;
            fileData = oldFileData;
            return true;
        }
        
        internal static FileNodeData CreateFileNodeData(string path)
        {
            using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.CreateFileNodeData"))
            {
                try
                {
                    path = path.ToStandardPath();
                    var info = new FileInfo(path);
                    if (!info.Exists) return FileNodeData.none;

                    byte[] content = Array.Empty<byte>();
                    using (new ProfilerScope("DirectoryTreeWatcher.DirectoryTreeData.CreateFileNodeData.ReadContent"))
                    {
                        try
                        {
                            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            content = new byte[fs.Length];
                            fs.Read(content, 0, content.Length);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogWarning($"[DirectoryTreeWatcher] Failed to read file content for {path}: {ex.Message}");
                        }
                    }

                    return new FileNodeData(
                        name: info.Name,
                        fullPath: path,
                        lastWriteTimeUtc: info.LastWriteTimeUtc,
                        content: content
                    );
                }
                catch (ThreadAbortException)
                {
                    return FileNodeData.none;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[DirectoryTreeWatcher] Failed to create file node for {path}: {ex.Message}");
                    return FileNodeData.none;
                }
            }
        }
    }
}
