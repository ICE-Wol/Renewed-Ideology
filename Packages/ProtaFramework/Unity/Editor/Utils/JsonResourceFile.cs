using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Prota;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Profiling;

using Stopwatch = System.Diagnostics.Stopwatch;

/*
用法:
var file = JsonResourceFile.Get<MyCustomData>("path/to/file.json");
var data = file.data; // 任意取用
file.backupFilePath = "file.backup";  // 备份文件夹放在文件旁边, 路径从外部设置
// 文件只会在创建时读取, 会自动保存

一个path对应一个 json resource file. 通过 JsonResource.Get<T>(path) 来获取.
对于有固定路径的JsonResourceFile, 可以自定义 Get 函数, 这些 Get 函数通常放在继承类里面.
*/

namespace Prota.Unity
{

    public abstract class JsonResourceFile
    {
        public static readonly Dictionary<string, JsonResourceFile> all = new();
        
        public readonly string filePath;
        public string backupFolderPath;
        public int maxBackupFiles;
        public string cachedJson; // 存储由该程序保存的最后的文本. 如果这个文本和文件不一样, 表示文件被外部修改了.
        
        public abstract object GetSerializableObject();
        
        object GetSerializableObjectAsserted()
        {
            var obj = GetSerializableObject();
            if(obj == null) throw new Exception($"GetSerializableObject 返回了 null! 请检查 {filePath} 文件是否存在.");
            return obj;
        }
        
        public JsonResourceFile(string filePath)
        {
            this.filePath = filePath;
            backupFolderPath = null;
            maxBackupFiles = 1000;
        }
        
        public void LoadOrCreate()
        {
            LoadOrCreate(out _);
        }
        
        public void LoadOrCreate(out bool isCreated)
        {
            if(File.Exists(filePath))
            {
                LoadFromFile();
                isCreated = false;
                return;
            }
            else
            {
                SaveToFile();
                isCreated = true;
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
                Debug.LogError($"JsonResourceFile: {filePath} 序列化对象不存在, 无法加载.");
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
        
        public void SaveToFile()
        {
            filePath.AsFileInfo().Directory().EnsureExists();
            var obj = GetSerializableObjectAsserted();
            if(obj is null)
            {
                Debug.LogError($"JsonResourceFile: {filePath} 序列化对象不存在, 无法保存.");
            }
            var json = JsonUtility.ToJson(obj, true);
            
            var diff = true;
            if(File.Exists(filePath))
            {
                var originalFileContent = File.ReadAllText(filePath);
                if(originalFileContent == json)
                {
                    diff = false;
                }
            }
            
            if(diff)
            {
                CreateBackupFile(filePath, backupFolderPath);
                File.WriteAllText(filePath, json);
                cachedJson = json;
            }
        }
        
        public bool FileExists()
        {
            return File.Exists(filePath);
        }
        
        public bool FileIsModified()
        {
            var json = File.ReadAllText(filePath);
            return json != cachedJson;
        }
        
        public void UpdateModificationOfFile()
        {
            if(!FileIsModified()) return;
            LoadFromFile();
        }
        
        // ============================================================================
        // ============================================================================
        
        /// <summary>
        /// 为指定文件创建时间戳备份,备份目录位于同级目录下的 backupFolderName.
        /// </summary>
        static void CreateBackupFile(string originalFilePath, string backupFolderName)
        {
            if(backupFolderName.NullOrEmpty()) return;
            
            try
            {
                if (string.IsNullOrEmpty(originalFilePath)) return;
                if (!File.Exists(originalFilePath)) return;
                var fileContent = File.ReadAllText(originalFilePath);
                
                string directory = Path.GetDirectoryName(originalFilePath);
                string backupDirectory = Path.Combine(directory, backupFolderName);
                new DirectoryInfo(backupDirectory).EnsureExists();
                
                string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
                string extension = Path.GetExtension(originalFilePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"{fileName}_{timestamp}{extension}";
                string backupFilePath = Path.Combine(backupDirectory, backupFileName);
                
                ProtaFileUtils.CopyWithExactCase(originalFilePath, backupFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[自动保存] 创建备份失败: {ex.Message}");
            }
        }
        
        
    }

    // ============================================================================
    // ============================================================================

    public class JsonResourceFile<T> : JsonResourceFile
        where T : new()
    {
        public T data = new();
        
        public JsonResourceFile(string filePath) : base(filePath)
        {
            
        }
        
        public override object GetSerializableObject() => data;
    }

    // ============================================================================
    // ============================================================================

    public static class JsonResource
    {
        public static JsonResourceFile<T> Get<T>(string path)
            where T : new()
        {
            return JsonResource.GetSpec<JsonResourceFile<T>, T>(path);
        }
        
        public static G GetSpec<G, T>(string path)
            where G : JsonResourceFile<T>
            where T : new()
        {
            if(path.NullOrEmpty())
            {
                return null;
            }

            if(!JsonResourceFile.all.TryGetValue(path, out var resFile))
            {
                resFile = Activator.CreateInstance(typeof(G), path) as G;
                resFile.LoadFromFile();
                JsonResourceFile.all[path] = resFile;
            }
            
            if(resFile is G t)
            {
                return t;
            }
            
            throw new Exception($"JsonResourceFile<T> 类型不匹配! 应该返回 {typeof(G).Name}, 但是返回了 {resFile.GetType().Name}");
        }
        
        public static JsonResourceFile<T> GetOrCreate<T>(string path)
            where T : new()
        {
            return JsonResource.GetOrCreateSpec<JsonResourceFile<T>, T>(path);
        }
        
        public static G GetOrCreateSpec<G, T>(string path)
            where G : JsonResourceFile<T>
            where T : new()
        {
            if(path.NullOrEmpty())
            {
                return null;
            }

            if(!JsonResourceFile.all.TryGetValue(path, out var resFile))
            {
                resFile = Activator.CreateInstance(typeof(G), path) as G;
                resFile.LoadOrCreate();
                JsonResourceFile.all[path] = resFile;
            }
            
            if(resFile is G t)
            {
                return t;
            }
            
            throw new Exception($"JsonResourceFile<T> 类型不匹配! 应该返回 {typeof(G).Name}, 但是返回了 {resFile.GetType().Name}");
        }
        
    }

    // ============================================================================
    // ============================================================================

    public static class JsonResourceFileAutoUpdate
    {
        static readonly Queue<JsonResourceFile> processQueue = new();
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorTimerManager.OnManagerInitialized(() => {
                EditorTimerManager.NewRepeat(0.5f, EnqueueAllFiles);
            });
            EditorApplication.update += Update;
        }
        
        static void EnqueueAllFiles()
        {
            if(processQueue.Count > 0) return;
            
            foreach(var handle in JsonResourceFile.all.Values)
            {
                processQueue.Enqueue(handle);
            }
        }
        
        static void Update()
        {
            if(processQueue.Count == 0) return;
            
            Profiler.BeginSample("JsonResourceFileAutoUpdate");
            
            var stopwatch = Stopwatch.StartNew();
            const float maxMilliseconds = 2f;
            
            while(processQueue.Count > 0 && stopwatch.Elapsed.TotalSeconds < maxMilliseconds / 1000f)
            {
                var handle = processQueue.Dequeue();
                
                if(!handle.FileExists())
                {
                    JsonResourceFile.all.Remove(handle.filePath);
                    continue;
                }
                
                if(handle.FileIsModified())
                {
                    handle.UpdateModificationOfFile();
                }
                else
                {
                    handle.SaveToFile();
                }
            }
            
            Profiler.EndSample();
        }
    }
}
