using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Prota;
using Prota.Unity;

namespace Prota.Editor
{
    /// <summary>
    /// 文件同步工具数据类
    /// 管理文件同步条目的配置
    /// </summary>
    [Serializable]
    public class FileSyncToolData
    {
        const string filePath = "Editor/FileSyncTool.json";
        public static FileSyncToolData instance => JsonResource.GetOrCreate<FileSyncToolData>(filePath).data;
        
        [Serializable]
        public class FileSyncEntry
        {
            public string name = "新条目";
            public string sourcePath = "";
            public string targetPath = "";
            public bool autoSync = false;
            public string filter = ".*"; // 正则表达式，用于筛选需要同步的文件
            
            [NonSerialized]
            Regex cachedRegex;
            
            [NonSerialized]
            string cachedFilterString;
            
            // 不匹配任何字符串的正则表达式（用于 filter 不合法时）
            static readonly Regex neverMatchRegex = new Regex("(?!)");
            
            /// <summary>
            /// 检查 filter 字符串是否是合法的正则表达式
            /// </summary>
            public bool IsFilterValid()
            {
                if (string.IsNullOrEmpty(filter))
                    return false;
                
                try
                {
                    _ = new Regex(filter);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            /// <summary>
            /// 获取缓存的 Regex 对象，如果 filter 变化则更新缓存
            /// 当 filter 不合法或为空时，返回一个不匹配任何字符串的 Regex
            /// </summary>
            public Regex GetFilterRegex()
            {
                if (string.IsNullOrEmpty(filter))
                    return neverMatchRegex;
                
                if (cachedRegex == null || cachedFilterString != filter)
                {
                    try
                    {
                        cachedRegex = new Regex(filter);
                        cachedFilterString = filter;
                    }
                    catch
                    {
                        // filter 不合法时，返回一个不匹配任何字符串的 Regex
                        cachedRegex = neverMatchRegex;
                        cachedFilterString = filter; // 仍然保存 filter 字符串，以便检测变化
                    }
                }
                
                return cachedRegex;
            }
            
            /// <summary>
            /// 检查源文件或文件夹是否存在
            /// </summary>
            public bool SourceExists()
            {
                if (string.IsNullOrEmpty(sourcePath))
                    return false;
                return System.IO.File.Exists(sourcePath) || System.IO.Directory.Exists(sourcePath);
            }
            
            /// <summary>
            /// 检查目标路径是否有效
            /// </summary>
            public bool TargetPathValid()
            {
                return !string.IsNullOrEmpty(targetPath);
            }
            
            /// <summary>
            /// 检查是否可以执行同步
            /// </summary>
            public bool CanSync()
            {
                return SourceExists() && TargetPathValid();
            }
            
            /// <summary>
            /// 检查是否可以执行自动同步
            /// </summary>
            public bool CanAutoSync()
            {
                if (!autoSync || !CanSync())
                    return false;
                
                // 源路径和目标路径都必须存在（文件或文件夹都可以）
                bool sourceExists = System.IO.File.Exists(sourcePath) || System.IO.Directory.Exists(sourcePath);
                bool targetExists = System.IO.File.Exists(targetPath) || System.IO.Directory.Exists(targetPath);
                
                return sourceExists && targetExists;
            }
        }
        
        public List<FileSyncEntry> entries = new();
        public bool pauseAutoSync = false;
    }
}

