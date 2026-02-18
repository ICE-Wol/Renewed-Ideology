using System.Collections.Generic;
using System.Text;

namespace Prota.Editor
{
    /// <summary>
    /// 同步结果
    /// </summary>
    public struct SyncResult
    {
        public readonly string filePath;           // 文件路径（相对路径或绝对路径）
        public readonly bool synced;               // 是否执行了同步
        public readonly string fromPath;           // 源路径
        public readonly string toPath;             // 目标路径
        public readonly string reason;             // 同步原因或跳过原因
        
        public SyncResult(string filePath, bool synced, string fromPath, string toPath, string reason)
        {
            this.filePath = filePath;
            this.synced = synced;
            this.fromPath = fromPath;
            this.toPath = toPath;
            this.reason = reason;
        }
        
        /// <summary>
        /// 将自身数据填入 StringBuilder
        /// </summary>
        public void AppendLineToStringBuilder(StringBuilder sb)
        {
            if (synced)
            {
                sb.AppendLine($"[同步] {filePath}");
                sb.AppendLine($"    从: {fromPath}");
                sb.AppendLine($"    到: {toPath}");
                sb.AppendLine($"    原因: {reason}");
            }
            else
            {
                sb.AppendLine($"[跳过] {filePath}");
                sb.AppendLine($"    原因: {reason}");
            }
        }
    }

    /// <summary>
    /// 同步结果集合
    /// </summary>
    public class SyncResultCollection
    {
        readonly List<SyncResult> results = new();
        
        /// <summary>
        /// 已同步的文件数量
        /// </summary>
        public int syncedCount { get; set; } = 0;
        
        /// <summary>
        /// 跳过的文件数量
        /// </summary>
        public int skippedCount { get; set; } = 0;
        
        /// <summary>
        /// 源路径
        /// </summary>
        public string sourcePath { get; private set; } = "";
        
        /// <summary>
        /// 目标路径
        /// </summary>
        public string targetPath { get; private set; } = "";
        
        /// <summary>
        /// 设置路径信息
        /// </summary>
        public void SetPaths(string sourcePath, string targetPath)
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
        }
        
        /// <summary>
        /// 将报告头信息填入 StringBuilder
        /// </summary>
        public void AppendHeaderToStringBuilder(StringBuilder sb, string entryName)
        {
            sb.AppendLine($"=== 文件夹同步报告: {entryName} ===");
            sb.AppendLine($"源路径: {sourcePath}");
            sb.AppendLine($"目标路径: {targetPath}");
            sb.AppendLine($"扫描文件数: {Count}");
            sb.AppendLine();
        }
        
        /// <summary>
        /// 添加同步结果（基础方法）
        /// </summary>
        void AddResult(string filePath, bool synced, string fromPath, string toPath, string reason)
        {
            results.Add(new SyncResult(filePath, synced, fromPath, toPath, reason));
            
            if (synced)
            {
                syncedCount++;
            }
            else
            {
                skippedCount++;
            }
        }
        
        /// <summary>
        /// 添加：不匹配 filter
        /// </summary>
        public void AddFilterMismatch(string relativePath, string sourceFile)
        {
            AddResult(relativePath, false, sourceFile, "", "不匹配 filter");
        }
        
        /// <summary>
        /// 添加：源文件和目标文件都不存在
        /// </summary>
        public void AddBothNotExist(string relativePath, string sourceFile, string targetFile)
        {
            AddResult(relativePath, false, sourceFile, targetFile, "源文件和目标文件都不存在");
        }
        
        /// <summary>
        /// 添加：从源复制到目标
        /// </summary>
        public void AddSourceToTarget(string relativePath, string sourceFile, string targetFile, bool copied)
        {
            AddResult(relativePath, copied, sourceFile, targetFile,
                copied ? "源文件存在，目标文件不存在，已复制" : "源文件存在，目标文件不存在，但复制失败");
        }
        
        /// <summary>
        /// 添加：强制同步已复制
        /// </summary>
        public void AddForceSyncCopied(string relativePath, string sourceFile, string targetFile, bool copied)
        {
            AddResult(relativePath, copied, sourceFile, targetFile,
                copied ? "强制同步：文件内容不同，已复制" : "强制同步：文件内容不同，但复制失败");
        }
        
        /// <summary>
        /// 添加：强制同步跳过（内容相同）
        /// </summary>
        public void AddForceSyncSkipped(string relativePath, string sourceFile, string targetFile)
        {
            AddResult(relativePath, false, sourceFile, targetFile, "强制同步：文件内容相同，跳过");
        }
        
        /// <summary>
        /// 添加：源文件更新已复制
        /// </summary>
        public void AddSourceNewerCopied(string relativePath, string sourceFile, string targetFile, bool copied)
        {
            AddResult(relativePath, copied, sourceFile, targetFile,
                copied ? "源文件更新，已复制" : "源文件更新，但复制失败");
        }
        
        /// <summary>
        /// 添加：其他跳过原因
        /// </summary>
        public void AddSkipped(string relativePath, string sourceFile, string targetFile, string reason)
        {
            AddResult(relativePath, false, sourceFile, targetFile, reason);
        }
        
        /// <summary>
        /// 获取结果数量
        /// </summary>
        public int Count => results.Count;
        
        /// <summary>
        /// 获取所有结果
        /// </summary>
        public IReadOnlyList<SyncResult> GetResults() => results;
    }
}

