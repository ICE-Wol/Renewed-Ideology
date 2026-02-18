using System;
using System.IO;
using System.Text;

namespace Prota
{
    public static class ProtaFileUtils
    {
        public static void CopyWithExactCase(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                // 临时文件名，保证不会与目标冲突
                string tempPath = targetPath + ".tmp_" + Guid.NewGuid().ToString("N");

                // 先重命名目标文件到临时名字
                File.Move(targetPath, tempPath);

                try
                {
                    // 拷贝源文件到目标名字
                    File.Copy(sourcePath, targetPath);
                }
                finally
                {
                    // 删除临时文件
                    File.Delete(tempPath);
                }
            }
            else
            {
                File.Copy(sourcePath, targetPath);
            }
        }

        // 安全写文件: 先写入临时文件, 成功后再替换原文件, 避免写入过程中崩溃导致原文件损坏.
        public static void SafeWriteAllText(string filePath, string contents)
            => SafeWriteAllText(filePath, contents, Encoding.UTF8);
        
        // 安全写文件: 先写入临时文件, 成功后再替换原文件, 避免写入过程中崩溃导致原文件损坏.
        public static void SafeWriteAllText(string filePath, string contents, Encoding encoding)
        {
            var file = new FileInfo(filePath);
            if(file.Directory != null) file.Directory.EnsureExists();
            var tempFilePath = filePath + ".tmp";
            try
            {
                File.WriteAllText(tempFilePath, contents, encoding);
                if(File.Exists(filePath))
                {
                    File.Replace(tempFilePath, filePath, null);
                }
                else
                {
                    File.Move(tempFilePath, filePath);
                }
            }
            catch
            {
                if(File.Exists(tempFilePath)) File.Delete(tempFilePath);
                throw;
            }
        }
        
        public static void ForceWriteAllText(
            string path,
            string content,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = encoding.GetBytes(content);
            ForceWriteAllBytes(path, bytes);
        }

        public static void ForceWriteAllBytes(
            string path,
            byte[] data)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            using var fs = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.WriteThrough);

            fs.Write(data, 0, data.Length);
            fs.Flush(true); // 强制落盘
        }
    }
}