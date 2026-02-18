using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    /// <summary>
    /// 同步操作类型
    /// </summary>
    public enum SyncOperationType
    {
        CopyFile,
        CreateDirectory
    }

    /// <summary>
    /// 同步操作任务
    /// </summary>
    public struct SyncOperation
    {
        public SyncOperationType type;
        public string sourcePath;
        public string targetPath;

        public bool Apply()
        {
            try
            {
                if (type == SyncOperationType.CreateDirectory)
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                        return true;
                    }
                    return false;
                }
                else // CopyFile
                {
                    ProtaFileUtils.CopyWithExactCase(sourcePath, targetPath);
                    Debug.Log($"同步成功: {sourcePath} -> {targetPath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"操作失败: {type} {targetPath}. Error: {ex}");
                return false;
            }
        }
    }
}
