using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    /// <summary>
    /// 目录节点访问器
    /// </summary>
    public readonly struct DirectoryNode
    {
        private readonly DirectoryTreeData _treeData;
        private readonly string _fullPath;
        
        internal ref readonly DirectoryNodeData data => ref _treeData.GetDirectory(_fullPath);
        
        /// <summary>
        /// 文件列表 (文件名)
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> files
            => data.childFileNames ?? Array.Empty<string>(); 
        
        /// <summary>
        /// 子目录列表 (目录名)
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> directories
            => data.childDirectoryNames ?? Array.Empty<string>();
        
        /// <summary>
        /// 文件映射表 (文件名 -> 文件绝对路径)
        /// </summary>
        /// <returns></returns>
        static readonly IReadOnlyDictionary<string, string> emptyMap = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> fileMap => data.filesMap ?? emptyMap;
        
        /// <summary>
        /// 子目录映射表 (目录名 -> 目录绝对路径)
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, string> directoryMap => data.directoriesMap ?? emptyMap;

        internal DirectoryNode(DirectoryTreeData treeData, string fullPath)
        {
            _treeData = treeData;
            _fullPath = fullPath;
        }
        
        /// <summary>
        /// 节点是否有效 (如果目录被删除，此标记为 false)
        /// </summary>
        public bool isValid => _treeData != null && !data.isNone;


        public string name => data.name;

        public string fullPath => _fullPath;
        
        /// <summary>
        /// 最后修改时间 (UTC)
        /// </summary>
        public DateTime lastWriteTimeUtc
        {
            get
            {
                var data = this.data;
                if(data.isNone) return default;
                return data.lastWriteTimeUtc;
            }
        }
        
        /// <summary>
        /// 最后修改时间 (本地时间)
        /// </summary>
        public DateTime lastWriteTime => lastWriteTimeUtc.ToLocalTime();
        
        public IEnumerable<string> fileRelativePaths => files;

        public IEnumerable<string> directoryRelativePaths => directories;
        
        public bool isRefValid => _treeData != null;
        
        public DirectoryNode GetChildDirectory(string relativeName)
        {
            if(!TryGetChildDirectory(relativeName, out var node))
                throw new KeyNotFoundException($"Directory {relativeName} not found in directory {_fullPath}");
            return node;
        }
        
        public FileNode GetChildFile(string relativeName)
        {
            if(!TryGetChildFile(relativeName, out var node))
                throw new KeyNotFoundException($"File {relativeName} not found in directory {_fullPath}");
            return node;
        }
        
        public bool TryGetChildDirectory(string relativeName, out DirectoryNode node)
        {
            if(!this.directoryMap.TryGetValue(relativeName, out var fullPath))
            {
                node = default;
                return false;
            }
            node = new DirectoryNode(_treeData, fullPath);
            return true;
        }
        
        public bool TryGetChildFile(string relativeName, out FileNode node)
        {
            if(!this.fileMap.TryGetValue(relativeName, out var fullPath))
            {
                node = default;
                return false;
            }
            node = new FileNode(_treeData, fullPath);
            return true;
        }
        
        
    }
}
