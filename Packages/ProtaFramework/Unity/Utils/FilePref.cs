using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    [Serializable]
    public class FileDataContainer
    {
        public List<FileDataEntry> entries = new List<FileDataEntry>();
    }
    
    [Serializable]
    public struct FileDataEntry
    {
        public string key;
        public string value;
        public FileDataEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    public abstract class UpdateOnFilePrefChangeEntry
    {
        public readonly string filePath;
        public readonly string key;
        
        public UpdateOnFilePrefChangeEntry(string filePath, string key)
        {
            this.filePath = filePath;
            this.key = key;
        }
        
        protected List<FileDataEntry> ReadFileData()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<FileDataEntry>();
                }
                
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(json))
                {
                    return new List<FileDataEntry>();
                }
                
                var container = JsonUtility.FromJson<FileDataContainer>(json);
                return container?.entries ?? new List<FileDataEntry>();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to read file {filePath}: {e.Message}");
                return new List<FileDataEntry>();
            }
        }
        
        protected void WriteFileData(List<FileDataEntry> data)
        {
            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var container = new FileDataContainer { entries = data };
                string json = JsonUtility.ToJson(container, true);
                // Debug.Log(filePath + "\n" + json);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write file {filePath}: {e.Message}");
            }
        }
        
        protected int GetIntValue(int defaultValue)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                if (int.TryParse(entry.value, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        
        protected float GetFloatValue(float defaultValue)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                if (float.TryParse(entry.value, out float result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        
        protected bool GetBoolValue(bool defaultValue)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                if (bool.TryParse(entry.value, out bool result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        
        protected string GetStringValue(string defaultValue)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key && !string.IsNullOrEmpty(entry.value))
            {
                return entry.value;
            }
            return defaultValue;
        }
        
        protected void SetValue(string value)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                // Update existing entry
                var index = data.FindIndex(e => e.key == key);
                data[index] = new FileDataEntry(key, value);
            }
            else
            {
                // Add new entry
                data.Add(new FileDataEntry(key, value));
            }
            WriteFileData(data);
        }
    }
    
    public class FilePrefEntryInt : UpdateOnFilePrefChangeEntry
    {
        public readonly int defaultValue = default;
        
        public int value
        {
            get => GetIntValue(defaultValue);
            set => SetValue(value.ToString());
        }
        
        public FilePrefEntryInt(string filePath, string key, int defaultValue = default) : base(filePath, key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class FilePrefEntryFloat : UpdateOnFilePrefChangeEntry
    {
        public readonly float defaultValue = default;
        
        public float value
        {
            get => GetFloatValue(defaultValue);
            set => SetValue(value.ToString());
        }
        
        public FilePrefEntryFloat(string filePath, string key, float defaultValue = default) : base(filePath, key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class FilePrefEntryString : UpdateOnFilePrefChangeEntry
    {
        public readonly string defaultValue = default;
        
        public string value
        {
            get => GetStringValue(defaultValue);
            set => SetValue(value);
        }
        
        public FilePrefEntryString(string filePath, string key, string defaultValue = default) : base(filePath, key)
        {
            this.defaultValue = defaultValue;
        }

        public override string ToString() => value;
    }
    
    public class FilePrefEntryString<T> : UpdateOnFilePrefChangeEntry
    {
        public Func<string, T> getter;
        public Func<T, string> setter;
        
        readonly string defaultValue = default;
        
        public string rawValue
        {
            get => GetStringValue(defaultValue);
            set => SetValue(value);
        }
        
        public T value
        {
            get => getter(rawValue);
            set => rawValue = setter(value);
        }
        
        public FilePrefEntryString(string filePath, string key, Func<string, T> getter, Func<T, string> setter, string defaultValue = default) : base(filePath, key)
        {
            this.getter = getter;
            this.setter = setter;
            this.defaultValue = defaultValue;
        }
    }
    
    public class FilePrefEntryVec : UpdateOnFilePrefChangeEntry
    {
        public readonly string keyX;
        public readonly string keyY;
        public readonly string keyZ;
        public readonly string keyW;
        
        public Vector4 defaultValue = default;
        
        public Vector4 vec4
        {
            get => new Vector4(
                GetFloatValue(keyX, defaultValue.x),
                GetFloatValue(keyY, defaultValue.y),
                GetFloatValue(keyZ, defaultValue.z),
                GetFloatValue(keyW, defaultValue.w)
            );
            set
            {
                SetValue(keyX, value.x);
                SetValue(keyY, value.y);
                SetValue(keyZ, value.z);
                SetValue(keyW, value.w);
            }
        }
        
        public Vector3 vec3
        {
            get => new Vector3(
                GetFloatValue(keyX, defaultValue.x),
                GetFloatValue(keyY, defaultValue.y),
                GetFloatValue(keyZ, defaultValue.z)
            );
            set
            {
                SetValue(keyX, value.x);
                SetValue(keyY, value.y);
                SetValue(keyZ, value.z);
            }
        }
        
        public Vector2 vec2
        {
            get => new Vector2(
                GetFloatValue(keyX, defaultValue.x),
                GetFloatValue(keyY, defaultValue.y)
            );
            set
            {
                SetValue(keyX, value.x);
                SetValue(keyY, value.y);
            }
        }
        
        public Color color
        {
            get => new Color(
                GetFloatValue(keyX, defaultValue.x),
                GetFloatValue(keyY, defaultValue.y),
                GetFloatValue(keyZ, defaultValue.z),
                GetFloatValue(keyW, defaultValue.w)
            );
            set
            {
                SetValue(keyX, value.r);
                SetValue(keyY, value.g);
                SetValue(keyZ, value.b);
                SetValue(keyW, value.a);
            }
        }
        
        public FilePrefEntryVec(string filePath, string key, Vector4 defaultValue = default) : base(filePath, key)
        {
            this.keyX = key + "_x";
            this.keyY = key + "_y";
            this.keyZ = key + "_z";
            this.keyW = key + "_w";
            this.defaultValue = defaultValue;
        }
        
        private float GetFloatValue(string key, float defaultValue)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                if (float.TryParse(entry.value, out float result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        
        private void SetValue(string key, float value)
        {
            var data = ReadFileData();
            var entry = data.FirstOrDefault(e => e.key == key);
            if (entry.key == key)
            {
                // Update existing entry
                var index = data.FindIndex(e => e.key == key);
                data[index] = new FileDataEntry(key, value.ToString());
            }
            else
            {
                // Add new entry
                data.Add(new FileDataEntry(key, value.ToString()));
            }
            WriteFileData(data);
        }
    }
    
    public class FilePrefEntryBool : UpdateOnFilePrefChangeEntry
    {
        public readonly bool defaultValue = default;
        
        public bool value
        {
            get => GetBoolValue(defaultValue);
            set => SetValue(value.ToString());
        }
        
        public FilePrefEntryBool(string filePath, string key, bool defaultValue = default) : base(filePath, key)
        {
            this.defaultValue = defaultValue;
        }
    }
}
