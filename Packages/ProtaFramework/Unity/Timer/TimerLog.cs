using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace Prota.Unity
{
    public class TimerLog
    {
        readonly string filePath;
        
        public bool enable = false;
        public bool enableConsole = false;
        
        public TimerLog(string filePath, bool enableConsole = false)
        {
            this.filePath = filePath;
            this.enableConsole = enableConsole;
            
            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 清空文件，从新开始写
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        void AppendToFile(string log)
        {
            try
            {
                File.AppendAllText(filePath, log + Environment.NewLine);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError($"TimerLog.AppendToFile - 写入文件失败: {e}");
            }
        }
        
        public void RecordAdd(Timer timer)
        {
            if(!enable) return;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var stackTrace = new StackTrace(true);
            var log = $"[{timestamp}] [Add] {timer}{Environment.NewLine}{stackTrace}";
            AppendToFile(log);
            if(enableConsole)
            {
                UnityEngine.Debug.Log(log);
            }
        }
        
        public void RecordRemove(Timer timer)
        {
            if(!enable) return;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var stackTrace = new StackTrace(true);
            var log = $"[{timestamp}] [Remove] {timer}{Environment.NewLine}{stackTrace}";
            AppendToFile(log);
            if(enableConsole)
            {
                UnityEngine.Debug.Log(log);
            }
        }
        
        public void Clear()
        {
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

