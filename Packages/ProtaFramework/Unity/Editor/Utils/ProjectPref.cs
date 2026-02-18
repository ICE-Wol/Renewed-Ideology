using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using DL.FastCsvParser;
using System.Text;

namespace Prota.Editor
{
    public static class ProjectPref
    {
        private static readonly string saveFilePath = Path.Combine("UserSettings", "project.pref.csv");
        private static Dictionary<string, string> _prefs = new Dictionary<string, string>();
        private static bool _initialized = false;
        
        static bool dirty = false;

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            Load();
            _initialized = true;
            EditorApplication.update += Save;
        }

        private static void Load()
        {
            _prefs.Clear();
            var fileInfo = new FileInfo(saveFilePath);
            if (!fileInfo.Exists) File.WriteAllText(saveFilePath, "__place__holder__,1");
            var csv = Csv.Parse(File.ReadAllText(saveFilePath));
            foreach(var row in csv)
            {
                if (row.Count < 2) continue;
                _prefs[row[0]] = row[1];
            }
        }

        private static void Save()
        {
            if (!dirty) return;
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
            var sb = new StringBuilder();
            foreach(var i in _prefs)
            {
                sb.Append($"\"{i.Key}\"");
                sb.Append(',');
                sb.Append($"\"{i.Value}\"");
                sb.Append('\n');
            }
            File.WriteAllText(saveFilePath, sb.ToString());
            dirty = false;
        }

        public static string GetString(string key, string defaultValue = "")
        {
            EnsureInitialized();
            return _prefs.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            EnsureInitialized();
            _prefs[key] = value;
            dirty = true;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            var str = GetString(key);
            return int.TryParse(str, out var value) ? value : defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            var str = GetString(key);
            return float.TryParse(str, out var value) ? value : defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            SetString(key, value.ToString());
        }
        
        public static void DeleteKey(string key)
        {
            EnsureInitialized();
            if (_prefs.Remove(key)) dirty = true;
        }

        public static void DeleteAll()
        {
            _prefs.Clear();
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            dirty = true;
            _initialized = false;
        }
    }
    
    public class ProjectPrefEntryBool
    {
        private readonly string _key;
        private bool _value;

        public ProjectPrefEntryBool(string key, bool defaultValue = false)
        {
            _key = key;
            _value = ProjectPref.GetInt(key, defaultValue ? 1 : 0) != 0;
        }

        public bool value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ProjectPref.SetInt(_key, value ? 1 : 0);
                }
            }
        }
    }
    
    public class ProjectPrefEntryInt
    {
        private readonly string _key;
        private int _value;

        public ProjectPrefEntryInt(string key, int defaultValue = 0)
        {
            _key = key;
            _value = ProjectPref.GetInt(key, defaultValue);
        }

        public int value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ProjectPref.SetInt(_key, value);
                }
            }
        }
    }
    
    public class ProjectPrefEntryFloat
    {
        private readonly string _key;
        private float _value;

        public ProjectPrefEntryFloat(string key, float defaultValue = 0f)
        {
            _key = key;
            _value = ProjectPref.GetFloat(key, defaultValue);
        }

        public float value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ProjectPref.SetFloat(_key, value);
                }
            }
        }
    }
    
    public class ProjectPrefEntryString
    {
        private readonly string _key;
        private string _value;

        public ProjectPrefEntryString(string key, string defaultValue = "")
        {
            _key = key;
            _value = ProjectPref.GetString(key, defaultValue);
        }

        public string value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ProjectPref.SetString(_key, value);
                }
            }
        }
    }
    
    public class ProjectPrefEntryVector
    {
        private readonly string _key;
        private Vector4 _value;

        public ProjectPrefEntryVector(string key)
        {
            _key = key;
            var s = ProjectPref.GetString(key, "0,0,0,0");
            var parts = s.Split(',');
            _value.x = float.Parse(parts[0]);
            _value.y = float.Parse(parts[1]);
            _value.z = float.Parse(parts[2]);
            _value.w = float.Parse(parts[3]);
        }
        
        public Vector2 vec2
        {
            get => new Vector2(_value.x, _value.y);
            set
            {
                if (_value.x != value.x || _value.y != value.y)
                {
                    _value.x = value.x;
                    _value.y = value.y;
                    ProjectPref.SetString(_key, $"{_value.x},{_value.y},{_value.z},{_value.w}");
                }
            }
        }
        
        public Vector3 vec3
        {
            get => new Vector3(_value.x, _value.y, _value.z);
            set
            {
                if (_value.x != value.x || _value.y != value.y || _value.z != value.z)
                {
                    _value.x = value.x;
                    _value.y = value.y;
                    _value.z = value.z;
                    ProjectPref.SetString(_key, $"{_value.x},{_value.y},{_value.z},{_value.w}");
                }
            }
        }
        
        public Vector4 vec4
        {
            get => _value;
            set
            {
                if (_value.x != value.x || _value.y != value.y || _value.z != value.z || _value.w != value.w)
                {
                    _value = value;
                    ProjectPref.SetString(_key, $"{_value.x},{_value.y},{_value.z},{_value.w}");
                }
            }
        }
        
        public Color color
        {
            get => new Color(_value.x, _value.y, _value.z, _value.w);
            set
            {
                if (_value.x != value.r || _value.y != value.g || _value.z != value.b || _value.w != value.a)
                {
                    _value.x = value.r;
                    _value.y = value.g;
                    _value.z = value.b;
                    _value.w = value.a;
                    ProjectPref.SetString(_key, $"{_value.x},{_value.y},{_value.z},{_value.w}");
                }
            }
        }
        
    }
}
