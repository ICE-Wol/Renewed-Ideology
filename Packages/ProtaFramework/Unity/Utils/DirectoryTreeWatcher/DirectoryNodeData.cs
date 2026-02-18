using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    internal struct DirectoryChildEntry
    {
        public readonly string relative;
        public readonly string full;
        
        public DirectoryChildEntry(string relative, string full)
        {
            this.relative = relative;
            this.full = full;
        }
        
        public static explicit operator(string relative, string full)(DirectoryChildEntry entry)
            => (entry.relative, entry.full);
        
        public static explicit operator DirectoryChildEntry((string relative, string full) entry)
            => new(entry.relative, entry.full);
    }
    
    /// <summary>
    /// 内部目录节点数据
    /// </summary>
    internal readonly struct DirectoryNodeData
    {
        public readonly string name;
        public readonly string fullPath;
        public readonly DateTime lastWriteTimeUtc;
        
        readonly Dictionary<string, string> _files;
        readonly Dictionary<string, string> _directories;
        
        readonly string[] _childFileNames;
        readonly string[] _childDirectoryNames;
        
        public IReadOnlyDictionary<string, string> filesMap => _files;
        public IReadOnlyDictionary<string, string> directoriesMap => _directories;
        public IReadOnlyList<string> childFileNames => _childFileNames;
        public IReadOnlyList<string> childDirectoryNames => _childDirectoryNames;
        
        public DirectoryNodeData(
            string name,
            string fullPath,
            DateTime lastWriteTimeUtc,
            IEnumerable<DirectoryChildEntry> files,
            IEnumerable<DirectoryChildEntry> directories
        )
        {
            this.name = name;
            this.fullPath = fullPath;
            this.lastWriteTimeUtc = lastWriteTimeUtc;
            _files = files.ToDictionary(x => x.relative, x => x.full);
            _directories = directories.ToDictionary(x => x.relative, x => x.full);
            _childFileNames = files.Select(x => x.relative).ToArray();
            _childDirectoryNames = directories.Select(x => x.relative).ToArray();
        }

        public static DirectoryNodeData none = new();

        public bool isNone => fullPath == null;
    }
}
