using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Prota;
using UnityEngine;

/*
用法:
var file = RuntimeJsonResource.Get<MyCustomData>("path/to/file.json");
var data = file.data; // 任意取用
file.SaveBackupFile("backup/file_backup.json");  // 手动保存备份, 路径相对于 json 文件所在文件夹
// 文件只会在创建时读取一次, 之后不再读取, 会自动保存

一个path对应一个 json resource file. 通过 RuntimeJsonResource.Get<T>(path) 来获取.
对于有固定路径的RuntimeJsonResourceFile, 可以自定义 Get 函数, 这些 Get 函数通常放在继承类里面.
*/

namespace Prota.Unity
{

    public abstract class RuntimeJsonResourceFile
    {
        public static readonly Dictionary<string, RuntimeJsonResourceFile> all = new();
        
        public readonly string filePath;
        
		[NonSerialized]
		public string cachedJson; // 存储由该程序保存的最后的文本. 如果这个文本和文件不一样, 表示文件被外部修改了.
        
		public Task saveTask; // 当前正在运行的保存任务, 如果非空则表示正在保存中
        
        /// <summary>
        /// 当前是否正在保存中
        /// </summary>
        public bool isSaving => saveTask != null && !saveTask.IsCompleted;
        
        /// <summary>
        /// 下一次更新的时间 (使用 Time.realtimeSinceStartup)
        /// </summary>
        [NonSerialized]
        public float nextUpdateTime;
        
        public abstract object GetSerializableObject();
        
		/// <summary>
		/// Asserted the returned object is not null.
		/// </summary>
		/// <returns></returns>
        object GetSerializableObjectAsserted()
        {
            var obj = GetSerializableObject();
            if(obj == null) throw new Exception($"GetSerializableObject 返回了 null! 请检查 {filePath} 文件是否存在.");
            return obj;
        }
        
        public RuntimeJsonResourceFile(string filePath)
        {
            this.filePath = filePath;
            saveTask = null;
            nextUpdateTime = Time.realtimeSinceStartup + 5f; // 默认5秒后更新
        }
        
        // 只在创建实例时读取一次文件
        public void LoadOrCreate()
        {
            if(File.Exists(filePath))
            {
                LoadFromFile();
                return;
            }
            else
            {
                SaveToFile();
                return;
            }
        }
        
        public bool TryLoadFromFile()
        {
            if(!File.Exists(filePath))
            {
                return false;
            }
            
            var json = File.ReadAllText(this.filePath);
            var obj = GetSerializableObjectAsserted();
            if(obj is null)
            {
                Debug.LogError($"RuntimeJsonResourceFile: {filePath} 序列化对象不存在, 无法加载.");
                return false;
            }
            JsonUtility.FromJsonOverwrite(json, obj);
            cachedJson = json;
            return true;
        }
        
        public void LoadFromFile()
        {
            if(!TryLoadFromFile())
            {
                throw new Exception($"文件不存在: {filePath}");
            }
        }
        
        // 使用 Task 异步保存文件, 使用 SafeWriteAllText
        public void SaveToFile()
        {
            // 如果已经有保存任务在运行, 不重复创建
            if(isSaving) return;
            
			filePath.AsFileInfo().Directory().EnsureExists();
			
			// 注意这里获取的是 shared object.
			// 严格地说是需要 lock 的.
			var obj = GetSerializableObjectAsserted();
			var json = JsonUtility.ToJson(obj, true);
				
            // 使用 Task 异步保存
            saveTask = Task.Run(() =>
            {
				try
                {
					
					// 如果内容没有变化, 不需要保存
					if(cachedJson != null && cachedJson == json) return;
				
                    ProtaFileUtils.SafeWriteAllText(filePath, json);
                    cachedJson = json;
                }
                catch(Exception ex)
                {
                    Debug.LogError($"[RuntimeJsonResourceFile] 保存文件失败: {filePath}, 错误: {ex}");
                }
				finally
				{
					saveTask = null;
				}
            });
        }
        
        /// <summary>
        /// 更新下一次更新时间, 设置为当前时间 + 5秒
        /// </summary>
        public void UpdateNextUpdateTime()
        {
            nextUpdateTime = Time.realtimeSinceStartup + 5f;
        }
        
        // ============================================================================
        // ============================================================================
        
        /// <summary>
        /// 保存备份文件, 路径相对于 json 文件所在文件夹.
        /// </summary>
        /// <param name="relativePath">相对于 json 文件所在文件夹的文件路径</param>
        public void SaveBackupFile(string relativePath)
        {
            if(relativePath.NullOrEmpty())
            {
                Debug.LogError($"[RuntimeJsonResourceFile] SaveBackupFile: 路径不能为空");
                return;
            }
            
            try
            {
                if(!File.Exists(filePath))
                {
                    Debug.LogError($"[RuntimeJsonResourceFile] SaveBackupFile: 源文件不存在: {filePath}");
                    return;
                }
                
                string directory = Path.GetDirectoryName(filePath);
                string backupFilePath = Path.Combine(directory, relativePath);
                
                // 确保备份文件的目录存在
                backupFilePath.AsFileInfo().Directory().EnsureExists();
                
                // 复制文件
                ProtaFileUtils.CopyWithExactCase(filePath, backupFilePath);
            }
            catch(Exception ex)
            {
                Debug.LogError($"[RuntimeJsonResourceFile] SaveBackupFile 失败: {ex.Message}");
            }
        }
        
        
    }

    // ============================================================================
    // ============================================================================

    public class RuntimeJsonResourceFile<T> : RuntimeJsonResourceFile
        where T : new()
    {
        public T data = new();
        
        public RuntimeJsonResourceFile(string filePath) : base(filePath)
        {
            
        }
        
        public override object GetSerializableObject() => data;
    }

    // ============================================================================
    // ============================================================================

    public static class RuntimeJsonResource
    {
        public static RuntimeJsonResourceFile<T> Get<T>(string path)
            where T : new()
        {
            return RuntimeJsonResource.GetSpec<RuntimeJsonResourceFile<T>, T>(path);
        }
        
        public static G GetSpec<G, T>(string path)
            where G : RuntimeJsonResourceFile<T>
            where T : new()
        {
            if(path.NullOrEmpty())
            {
                return null;
            }

            if(!RuntimeJsonResourceFile.all.TryGetValue(path, out var resFile))
            {
                resFile = Activator.CreateInstance(typeof(G), path) as G;
                resFile.LoadOrCreate();
                RuntimeJsonResourceFile.all[path] = resFile;
                
                // 注册到管理器
                RuntimeJsonResourceManager.EnsureExists();
                RuntimeJsonResourceManager.instance.RegisterFile(resFile);
            }
            
            if(resFile is G t)
            {
                return t;
            }
            
            throw new Exception($"RuntimeJsonResourceFile<T> 类型不匹配! 应该返回 {typeof(G).Name}, 但是返回了 {resFile.GetType().Name}");
        }
        
        public static RuntimeJsonResourceFile<T> GetOrCreate<T>(string path)
            where T : new()
        {
            return RuntimeJsonResource.GetOrCreateSpec<RuntimeJsonResourceFile<T>, T>(path);
        }
        
        public static G GetOrCreateSpec<G, T>(string path)
            where G : RuntimeJsonResourceFile<T>
            where T : new()
        {
            if(path.NullOrEmpty())
            {
                return null;
            }

            if(!RuntimeJsonResourceFile.all.TryGetValue(path, out var resFile))
            {
                resFile = Activator.CreateInstance(typeof(G), path) as G;
                resFile.LoadOrCreate();
                RuntimeJsonResourceFile.all[path] = resFile;
                
                // 注册到管理器
                RuntimeJsonResourceManager.EnsureExists();
                RuntimeJsonResourceManager.instance.RegisterFile(resFile);
            }
            
            if(resFile is G t)
            {
                return t;
            }
            
            throw new Exception($"RuntimeJsonResourceFile<T> 类型不匹配! 应该返回 {typeof(G).Name}, 但是返回了 {resFile.GetType().Name}");
        }
        
    }
}

