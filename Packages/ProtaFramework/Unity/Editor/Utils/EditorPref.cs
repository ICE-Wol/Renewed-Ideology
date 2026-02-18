using UnityEngine;
using UnityEditor;
using Prota.Editor;
using Prota.Unity;
using Prota;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prota.Editor
{
    [System.Serializable]
    public class EditorPrefData : ISerializationCallbackReceiver
    {
        [System.Serializable]
        public class SerializedData
        {
            public List<string> longKeys = new();
            public List<long> longValues = new();
            
            public List<string> doubleKeys = new();
            public List<double> doubleValues = new();
            
            public List<string> vector4Keys = new();
            public List<Vector4> vector4Values = new();
            
            public List<string> rectKeys = new();
            public List<Rect> rectValues = new();
            
            public List<string> vector3IntKeys = new();
            public List<Vector3Int> vector3IntValues = new();
            
            public List<string> stringKeys = new();
            public List<string> stringValues = new();
            
            public List<string> boolKeys = new();
            public List<bool> boolValues = new();
        }
        
        // 运行时数据
        public Dictionary<string, long> longData = new();
        public Dictionary<string, double> doubleData = new();
        public Dictionary<string, Vector4> vector4Data = new();
        public Dictionary<string, Rect> rectData = new();
        public Dictionary<string, Vector3Int> vector3IntData = new();
        public Dictionary<string, string> stringData = new();
        public Dictionary<string, bool> boolData = new();
        
        // 序列化数据
        [SerializeField] private SerializedData serializedData = new();
        
        public int GetValue(string key, int defaultValue = 0)
        {
            // int 使用 long 存储
            return longData.TryGetValue(key, out var longValue) ? (int)longValue : defaultValue;
        }
        
        public float GetValue(string key, float defaultValue = 0f)
        {
            // float 使用 double 存储
            return doubleData.TryGetValue(key, out var doubleValue) ? (float)doubleValue : defaultValue;
        }
        
        public long GetValue(string key, long defaultValue = 0)
        {
            return longData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public double GetValue(string key, double defaultValue = 0.0)
        {
            return doubleData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public Vector2 GetValue(string key, Vector2 defaultValue = default)
        {
            return vector4Data.TryGetValue(key, out var vec4) ? vec4.ToVec2() : defaultValue;
        }
        
        public Vector3 GetValue(string key, Vector3 defaultValue = default)
        {
            return vector4Data.TryGetValue(key, out var vec4) ? vec4.ToVec3() : defaultValue;
        }
        
        public Vector4 GetValue(string key, Vector4 defaultValue = default)
        {
            return vector4Data.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public Color GetValue(string key, Color defaultValue = default)
        {
            return vector4Data.TryGetValue(key, out var vec4) ? (Color)vec4 : defaultValue;
        }
        
        public Rect GetValue(string key, Rect defaultValue = default)
        {
            return rectData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public Vector2Int GetValue(string key, Vector2Int defaultValue = default)
        {
            return vector3IntData.TryGetValue(key, out var vec3Int) ? vec3Int.ToVec2Int() : defaultValue;
        }
        
        public Vector3Int GetValue(string key, Vector3Int defaultValue = default)
        {
            return vector3IntData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public string GetValue(string key, string defaultValue = "")
        {
            return stringData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public bool GetValue(string key, bool defaultValue = false)
        {
            return boolData.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public void SetValue(string key, int value)
        {
            // int 使用 long 存储
            longData[key] = (long)value;
        }
        
        public void SetValue(string key, float value)
        {
            // float 使用 double 存储
            doubleData[key] = (double)value;
        }
        
        public void SetValue(string key, long value)
        {
            longData[key] = value;
        }
        
        public void SetValue(string key, double value)
        {
            doubleData[key] = value;
        }
        
        public void SetValue(string key, Vector2 value)
        {
            vector4Data[key] = value.ToVec4();
        }
        
        public void SetValue(string key, Vector3 value)
        {
            vector4Data[key] = value.ToVec4();
        }
        
        public void SetValue(string key, Vector4 value)
        {
            vector4Data[key] = value;
        }
        
        public void SetValue(string key, Color value)
        {
            vector4Data[key] = (Vector4)value;
        }
        
        public void SetValue(string key, Rect value)
        {
            rectData[key] = value;
        }
        
        public void SetValue(string key, Vector2Int value)
        {
            vector3IntData[key] = value.ToVec3Int();
        }
        
        public void SetValue(string key, Vector3Int value)
        {
            vector3IntData[key] = value;
        }
        
        public void SetValue(string key, string value)
        {
            stringData[key] = value;
        }
        
        public void SetValue(string key, bool value)
        {
            boolData[key] = value;
        }
        
        
        public bool HasKey(string key)
        {
            return longData.ContainsKey(key) || doubleData.ContainsKey(key) ||
                   vector4Data.ContainsKey(key) || rectData.ContainsKey(key) || 
                   vector3IntData.ContainsKey(key) || stringData.ContainsKey(key) || 
                   boolData.ContainsKey(key);
        }
        
        public void DeleteKey(string key)
        {
            longData.Remove(key);
            doubleData.Remove(key);
            vector4Data.Remove(key);
            rectData.Remove(key);
            vector3IntData.Remove(key);
            stringData.Remove(key);
            boolData.Remove(key);
        }
        
        public void OnBeforeSerialize()
        {
            // 将 Dictionary 数据转换为序列化格式
            serializedData.longKeys.Clear();
            serializedData.longValues.Clear();
            foreach (var kvp in longData)
            {
                serializedData.longKeys.Add(kvp.Key);
                serializedData.longValues.Add(kvp.Value);
            }
            
            serializedData.doubleKeys.Clear();
            serializedData.doubleValues.Clear();
            foreach (var kvp in doubleData)
            {
                serializedData.doubleKeys.Add(kvp.Key);
                serializedData.doubleValues.Add(kvp.Value);
            }
            
            serializedData.vector4Keys.Clear();
            serializedData.vector4Values.Clear();
            foreach (var kvp in vector4Data)
            {
                serializedData.vector4Keys.Add(kvp.Key);
                serializedData.vector4Values.Add(kvp.Value);
            }
            
            
            serializedData.rectKeys.Clear();
            serializedData.rectValues.Clear();
            foreach (var kvp in rectData)
            {
                serializedData.rectKeys.Add(kvp.Key);
                serializedData.rectValues.Add(kvp.Value);
            }
            
            serializedData.vector3IntKeys.Clear();
            serializedData.vector3IntValues.Clear();
            foreach (var kvp in vector3IntData)
            {
                serializedData.vector3IntKeys.Add(kvp.Key);
                serializedData.vector3IntValues.Add(kvp.Value);
            }
            
            serializedData.stringKeys.Clear();
            serializedData.stringValues.Clear();
            foreach (var kvp in stringData)
            {
                serializedData.stringKeys.Add(kvp.Key);
                serializedData.stringValues.Add(kvp.Value);
            }
            
            serializedData.boolKeys.Clear();
            serializedData.boolValues.Clear();
            foreach (var kvp in boolData)
            {
                serializedData.boolKeys.Add(kvp.Key);
                serializedData.boolValues.Add(kvp.Value);
            }
        }
        
        public void OnAfterDeserialize()
        {
            // 将序列化数据转换回 Dictionary
            longData.Clear();
            for (int i = 0; i < serializedData.longKeys.Count && i < serializedData.longValues.Count; i++)
            {
                longData[serializedData.longKeys[i]] = serializedData.longValues[i];
            }
            
            doubleData.Clear();
            for (int i = 0; i < serializedData.doubleKeys.Count && i < serializedData.doubleValues.Count; i++)
            {
                doubleData[serializedData.doubleKeys[i]] = serializedData.doubleValues[i];
            }
            
            vector4Data.Clear();
            for (int i = 0; i < serializedData.vector4Keys.Count && i < serializedData.vector4Values.Count; i++)
            {
                vector4Data[serializedData.vector4Keys[i]] = serializedData.vector4Values[i];
            }
            
            
            rectData.Clear();
            for (int i = 0; i < serializedData.rectKeys.Count && i < serializedData.rectValues.Count; i++)
            {
                rectData[serializedData.rectKeys[i]] = serializedData.rectValues[i];
            }
            
            vector3IntData.Clear();
            for (int i = 0; i < serializedData.vector3IntKeys.Count && i < serializedData.vector3IntValues.Count; i++)
            {
                vector3IntData[serializedData.vector3IntKeys[i]] = serializedData.vector3IntValues[i];
            }
            
            stringData.Clear();
            for (int i = 0; i < serializedData.stringKeys.Count && i < serializedData.stringValues.Count; i++)
            {
                stringData[serializedData.stringKeys[i]] = serializedData.stringValues[i];
            }
            
            boolData.Clear();
            for (int i = 0; i < serializedData.boolKeys.Count && i < serializedData.boolValues.Count; i++)
            {
                boolData[serializedData.boolKeys[i]] = serializedData.boolValues[i];
            }
        }
    }
    
    public class EditorPrefJsonResourceFile : JsonResourceFile<EditorPrefData>
    {
        public EditorPrefJsonResourceFile(string filePath) : base(filePath)
        {
        }
    }
    
    public abstract class UpdateOnPrefChangeEntry
    {
        public readonly string key;
        
        public UpdateOnPrefChangeEntry(string key)
        {
            this.key = key;
        }
        
        protected static EditorPrefData GetData()
        {
            var file = JsonResource.GetOrCreate<EditorPrefData>("Editor/Prota/EditorPref.json");
            return file.data;
        }
    }
    
    public class EditorPrefEntryInt : UpdateOnPrefChangeEntry
    {
        public readonly int defaultValue = default;
        
        public int value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryInt(string key, int defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }

    public class EditorPrefEntryFloat : UpdateOnPrefChangeEntry
    {
        public readonly float defaultValue = default;
        
        public float value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryFloat(string key, float defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryString : UpdateOnPrefChangeEntry
    {
        public readonly string defaultValue = default;
        
        public string value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        /// <summary>
        /// 获取或设置 FileInfo,自动处理工程根目录相对路径转换
        /// </summary>
        public FileInfo fileInfo
        {
            get
            {
                var relativePath = value;
                if (string.IsNullOrEmpty(relativePath)) return null;
                
                // 获取工程根目录（Assets 的父目录）
                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                if (string.IsNullOrEmpty(projectRoot))
                {
                    throw new InvalidOperationException("无法获取工程根目录路径");
                }
                
                // 如果已经是绝对路径,直接使用
                if (Path.IsPathRooted(relativePath))
                {
                    return new FileInfo(relativePath);
                }
                
                // 相对路径,相对于工程根目录
                var fullPath = Path.GetFullPath(Path.Combine(projectRoot, relativePath));
                return new FileInfo(fullPath);
            }
            set
            {
                if (value == null)
                {
                    this.value = "";
                    return;
                }
                
                var fullPath = value.FullName;
                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                if (string.IsNullOrEmpty(projectRoot))
                {
                    throw new InvalidOperationException("无法获取工程根目录路径");
                }
                
                // 检查文件是否在工程根目录下
                if (fullPath.StartsWith(projectRoot))
                {
                    var relativePath = Path.GetRelativePath(projectRoot, fullPath);
                    this.value = relativePath.Replace('\\', '/');
                }
                else
                {
                    // 如果不在工程根目录下,存储完整路径
                    this.value = fullPath;
                }
            }
        }
        
        /// <summary>
        /// 获取相对于工程根目录的路径
        /// </summary>
        public string projectRelativePath
        {
            get
            {
                var path = value;
                if (string.IsNullOrEmpty(path)) return "";
                
                // 如果已经是绝对路径,尝试转换为相对路径
                if (Path.IsPathRooted(path))
                {
                    var fullPath = Path.GetFullPath(path);
                    var projectRoot = Path.GetDirectoryName(Application.dataPath);
                    if (string.IsNullOrEmpty(projectRoot))
                    {
                        throw new InvalidOperationException("无法获取工程根目录路径");
                    }
                    
                    if (fullPath.StartsWith(projectRoot))
                    {
                        var relativePath = Path.GetRelativePath(projectRoot, fullPath);
                        return relativePath.Replace('\\', '/');
                    }
                    else
                    {
                        throw new ArgumentException($"文件路径 '{path}' 不在工程根目录 '{projectRoot}' 下");
                    }
                }
                
                return path;
            }
        }
        
        public EditorPrefEntryString(string key, string defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }

        public override string ToString() => value;
    }
    
    public class EditorPrefEntryString<T> : UpdateOnPrefChangeEntry
    {
        public Func<string, T> getter;
        public Func<T, string> setter;
        
        readonly string defaultValue = default;
        
        public string rawValue
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public T value
        {
            get => getter(rawValue);
            set => rawValue = setter(value);
        }
        
        public EditorPrefEntryString(string key, Func<string, T> getter, Func<T, string> setter, string defaultValue = default) : base(key)
        {
            this.getter = getter;
            this.setter = setter;
        }
    }
    
    public class EditorPrefEntryVec2 : UpdateOnPrefChangeEntry
    {
        public readonly Vector2 defaultValue = default;
        
        public Vector2 value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryVec2(string key, Vector2 defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryVec3 : UpdateOnPrefChangeEntry
    {
        public readonly Vector3 defaultValue = default;
        
        public Vector3 value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryVec3(string key, Vector3 defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryVec4 : UpdateOnPrefChangeEntry
    {
        public readonly Vector4 defaultValue = default;
        
        public Vector4 value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryVec4(string key, Vector4 defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryColor : UpdateOnPrefChangeEntry
    {
        public readonly Color defaultValue = default;
        
        public Color value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryColor(string key, Color defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryVec2Int : UpdateOnPrefChangeEntry
    {
        public readonly Vector2Int defaultValue = default;
        
        public Vector2Int value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryVec2Int(string key, Vector2Int defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryVec3Int : UpdateOnPrefChangeEntry
    {
        public readonly Vector3Int defaultValue = default;
        
        public Vector3Int value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryVec3Int(string key, Vector3Int defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    
    public class EditorPrefEntryBool : UpdateOnPrefChangeEntry
    {
        public readonly bool defaultValue = default;
        
        public bool value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryBool(string key, bool defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryRect : UpdateOnPrefChangeEntry
    {
        public readonly Rect defaultValue = default;
        
        public Rect value
        {
            get => GetData().GetValue(key, defaultValue);
            set => GetData().SetValue(key, value);
        }
        
        public EditorPrefEntryRect(string key, Rect defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class EditorPrefEntryObject<T> : UpdateOnPrefChangeEntry where T : UnityEngine.Object
    {
        public readonly T defaultValue = default;
        
        public T value
        {
            get
            {
                var path = GetData().GetValue(key, "");
                if (string.IsNullOrEmpty(path))
                {
                    return defaultValue;
                }
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            set
            {
                if (value == null)
                {
                    GetData().SetValue(key, "");
                }
                else
                {
                    var path = AssetDatabase.GetAssetPath(value);
                    GetData().SetValue(key, path);
                }
            }
        }
        
        public EditorPrefEntryObject(string key, T defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
}
