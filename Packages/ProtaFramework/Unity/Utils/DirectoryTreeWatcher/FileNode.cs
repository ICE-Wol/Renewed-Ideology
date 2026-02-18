using System;

namespace Prota.Unity
{
    /// <summary>
    /// 文件节点访问器
    /// </summary>
    public readonly struct FileNode
    {
        private readonly DirectoryTreeData _treeData;
        private readonly string _fullPath;

        internal FileNode(DirectoryTreeData treeData, string fullPath)
        {
            _treeData = treeData;
            _fullPath = fullPath;
        }

        public string name => _treeData.GetFile(_fullPath).name;

        public string fullPath => _fullPath;
        
        /// <summary>
        /// 最后修改时间 (UTC)
        /// </summary>
        public DateTime lastWriteTimeUtc => data.lastWriteTimeUtc;

        /// <summary>
        /// 最后修改时间 (本地时间)
        /// </summary>
        public DateTime lastWriteTime => lastWriteTimeUtc.ToLocalTime();
        
        public byte[] content => data.content ?? Array.Empty<byte>();
        
        public string contentUtf8 => data.utf8Content ?? string.Empty;
        
        /// <summary>
        /// 节点是否有效 (如果文件被删除，此标记为 false)
        /// </summary>
        public bool isValid => _treeData != null && !data.isNone;
        
        public bool isRefValid => _treeData != null;

        internal ref readonly FileNodeData data => ref _treeData.GetFile(_fullPath);
    }
}
