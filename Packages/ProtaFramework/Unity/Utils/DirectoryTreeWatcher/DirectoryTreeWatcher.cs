using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Prota;
using System.Runtime.CompilerServices;
using System.Linq;

[assembly: InternalsVisibleTo("ProtaFramework.Unity.Tests")]

namespace Prota.Unity
{
    /// <summary>
    /// 目录树监视器
    /// </summary>
    public partial class DirectoryTreeWatcher : IDisposable
    {
        /// <summary>
        /// 文件系统变更事件参数
        /// </summary>
        public readonly struct ChangeEvent
        {
            /// <summary>
            /// 变更类型
            /// </summary>
            public readonly WatcherChangeTypes changeType;
            
            /// <summary>
            /// 文件或目录的完整路径
            /// </summary>
            public readonly string fullPath;
            
            /// <summary>
            /// 旧路径（仅用于 Renamed 事件）
            /// </summary>
            public readonly string oldFullPath;
            
            public readonly Exception exception;

            public ChangeEvent(WatcherChangeTypes changeType, string fullPath, string oldFullPath = null)
            {
                this.changeType = changeType;
                this.fullPath = fullPath;
                this.oldFullPath = oldFullPath;
                this.exception = null;
            }
            
            public ChangeEvent(Exception exception)
            {
                this.changeType = default;
                this.fullPath = null;
                this.oldFullPath = null;
                this.exception = exception;
            }
        }


        readonly string _rootPath;
        DirectoryTreeData _treeData;
        
        public string rootPath => _rootPath;

        private DateTime _lastScanTime;
        
        private bool _disposed = false;
        
        public event Action<ChangeEvent> onChanged;

        public DirectoryTreeWatcher(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            _rootPath = Path.GetFullPath(path).ToStandardPath();
            
            // 初始扫描
            _treeData = DirectoryTreeData.Scan(_rootPath, null);
            _lastScanTime = DateTime.UtcNow;

            // 启动扫描线程
            _shouldStop = false;
            _scanThread = new Thread(ScanLoop)
            {
                IsBackground = true,
                Name = "DirectoryTreeWatcher"
            };
            _scanThread.Start();
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _shouldStop = true;
            _scanTrigger.Set(); // 唤醒线程以便退出
            
            if (_scanThread != null && _scanThread.IsAlive)
            {
                // 等待线程结束，但不无限等待
                _scanThread.Join(1000);
            }

            _scanTrigger.Dispose();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 设置扫描间隔（毫秒）
        /// </summary>
        public void SetScanInterval(int intervalMs)
        {
            if (intervalMs < 0)
            {
                throw new ArgumentException("Scan interval must be non-negative", nameof(intervalMs));
            }

            while(intervalMs != Interlocked.CompareExchange(ref _scanIntervalMs, intervalMs, _scanIntervalMs))
            {
                // 等待直到设置成功
            }
        }

        /// <summary>
        /// 获取当前扫描间隔（毫秒）
        /// </summary>
        public int GetScanInterval()
        {
            return _scanIntervalMs;
        }

        /// <summary>
        /// 获取最后一次扫描完成的时间 (UTC)
        /// </summary>
        public DateTime GetLastScanTime()
        {
            return _lastScanTime;
        }

        /// <summary>
        /// 注册扫描完成回调
        /// 如果正在扫描，将回调添加到列表；如果未在扫描，立即执行回调
        /// </summary>
        public void RegisterOnScanFinished(Action callback)
        {
            if (callback == null) return;

            bool invokeImmediately = false;
            lock (_scanFinishedCallbacks)
            {
                if (Interlocked.CompareExchange(ref _scanningCount, 0, 0) > 0)
                {
                    _scanFinishedCallbacks.Add(callback);
                }
                else
                {
                    invokeImmediately = true;
                }
            }

            if (invokeImmediately)
            {
                callback.Invoke();
            }
        }

        /// <summary>
        /// 手动触发扫描
        /// </summary>
        public void TriggerScan()
        {
            _scanTrigger.Set();
        }

        // ============================================================================
        // ============================================================================

        /// <summary>
        /// 获取指定路径的文件节点.
        /// </summary>
        /// <param name="fullPath">绝对路径</param>
        public FileNode GetFile(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
                throw new ArgumentException("fullPath must be absolute path", nameof(fullPath));
            return new FileNode(_treeData, fullPath);
        }

        /// <summary>
        /// 获取指定路径的目录节点.
        /// </summary>
        /// <param name="fullPath">绝对路径或null</param>
        public DirectoryNode GetDirectory(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return new DirectoryNode(_treeData, _rootPath);
            if(!Path.IsPathRooted(fullPath))
                throw new ArgumentException("fullPath must be absolute path", nameof(fullPath));
            return new DirectoryNode(_treeData, fullPath);
        }
    
        // ============================================================================
        // ============================================================================
        
        DirectoryNodeData FindDirectoryNodeData(string fullPath)
        {
            return _treeData.GetDirectory(fullPath);
        }

        FileNodeData FindFileNodeData(string fullPath)
        {
            return _treeData.GetFile(fullPath);
        }

        // ============================================================================
        // ============================================================================

        public string GetFileName(string fullPath)
        {
            var fileData = FindFileNodeData(fullPath);
            return fileData.name; // 找不到就返回 null
        }

        public DateTime GetFileLastWriteTimeUtc(string fullPath)
        {
            var fileData = FindFileNodeData(fullPath);
            return fileData.lastWriteTimeUtc;
        }

        public bool GetFileIsValid(string fullPath)
        {
            var fileData = FindFileNodeData(fullPath);
            return !fileData.isNone;
        }

        public string GetDirectoryName(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            return dirData.name; // 找不到就返回 null
        }

        public DateTime GetDirectoryLastWriteTimeUtc(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            return dirData.lastWriteTimeUtc;
        }

        public bool GetDirectoryIsValid(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            return !dirData.isNone;
        }
        
        
        public IReadOnlyList<string> GetDirectoryChildFiles(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            if(dirData.isNone) return Array.Empty<string>();
            return dirData.childFileNames;
        }
        
        public IReadOnlyList<string> GetDirectoryChildDirectories(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            if(dirData.isNone) return Array.Empty<string>();
            return dirData.childDirectoryNames; 
        }
        
        static readonly IReadOnlyDictionary<string, string> emptyMap = new Dictionary<string, string>();
        
        public IReadOnlyDictionary<string, string> GetDirectoryChildFilesMap(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            if(dirData.isNone) return emptyMap;
            return dirData.filesMap;
        }
        
        public IReadOnlyDictionary<string, string> GetDirectoryChildDirectoriesMap(string fullPath)
        {
            var dirData = FindDirectoryNodeData(fullPath);
            if(dirData.isNone) return emptyMap;
            return dirData.directoriesMap; 
        }

        /// <summary>
        /// Public API: 读取文件内容
        /// </summary>
        public byte[] ReadFileContent(string fullPath)
        {
            var fileData = FindFileNodeData(fullPath);
            return fileData.content ?? Array.Empty<byte>();
        }
        
        public string ReadFileContentUtf8(string fullPath)
        {
            var fileData = FindFileNodeData(fullPath);
            return fileData.utf8Content ?? string.Empty;
        }
    }
}
