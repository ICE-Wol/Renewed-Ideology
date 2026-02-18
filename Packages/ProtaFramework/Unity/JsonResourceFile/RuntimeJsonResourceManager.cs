using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    public class RuntimeJsonResourceManager : SingletonComponent<RuntimeJsonResourceManager>
    {
        // 文件比较器: 按 nextUpdateTime 排序, 时间相同则按 filePath 排序
        class FileComparer : IComparer<RuntimeJsonResourceFile>
        {
            public int Compare(RuntimeJsonResourceFile x, RuntimeJsonResourceFile y)
            {
                if(x == null && y == null) return 0;
                if(x == null) return 1;
                if(y == null) return -1;
                
                int timeCompare = x.nextUpdateTime.CompareTo(y.nextUpdateTime);
                if(timeCompare != 0) return timeCompare;
                
                // 如果时间相同, 使用 filePath 作为次要排序键
                return string.Compare(x.filePath, y.filePath, System.StringComparison.Ordinal);
            }
        }
        
        // 按照下一次更新时间排序的文件集合
        readonly SortedSet<RuntimeJsonResourceFile> sortedByNextUpdate = new(new FileComparer());
        
        // 正在保存中的文件队列
        readonly Queue<RuntimeJsonResourceFile> savingQueue = new();
        
        void Update()
        {
            // 检查保存队列中第一个文件是否完成保存
            while(savingQueue.Count > 0)
            {
                var savingFile = savingQueue.Peek();
                if(savingFile.isSaving)
                {
                    // 如果还在保存中, 停止检查
                    break;
                }
                
                // 保存完成, 从队列移除
                savingQueue.Dequeue();
                
                // 更新下一次更新时间并重新加入 sortedByNextUpdate
                savingFile.UpdateNextUpdateTime();
                sortedByNextUpdate.Add(savingFile);
            }
            
            // 如果集合为空, 直接返回
            if(sortedByNextUpdate.Count == 0)
            {
                return;
            }
            
            // 获取最早需要更新的文件
            var file = sortedByNextUpdate.Min;
            if(file == null)
            {
                return;
            }
            
            // 检查是否到了更新时间
            if(Time.realtimeSinceStartup >= file.nextUpdateTime)
            {
                // 从 sortedByNextUpdate 移除, 加入 savingQueue
                sortedByNextUpdate.Remove(file);
                savingQueue.Enqueue(file);
                
                // 保存文件
                file.SaveToFile();
            }
        }
        
        /// <summary>
        /// 注册文件到管理器
        /// </summary>
        public void RegisterFile(RuntimeJsonResourceFile file)
        {
            sortedByNextUpdate.Add(file);
        }
    }
}

