using System;

namespace Prota.Unity
{
    /// <summary>
    /// 内部文件节点数据
    /// </summary>
    internal readonly struct FileNodeData
    {
        public readonly string name;
        public readonly string fullPath;
        public readonly DateTime lastWriteTimeUtc;
        public readonly byte[] content;
        
        public string utf8Content
            => content != null
            ? System.Text.Encoding.UTF8.GetString(content)
            : null;
        
        public FileNodeData(
            string name,
            string fullPath,
            DateTime lastWriteTimeUtc,
            byte[] content
        )
        {
            this.name = name;
            this.fullPath = fullPath;
            this.lastWriteTimeUtc = lastWriteTimeUtc;
            this.content = content;
        }

        public bool isNone => fullPath == null;

        public static FileNodeData none = new();
    }
}
