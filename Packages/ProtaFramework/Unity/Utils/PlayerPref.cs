using UnityEngine;
using System;

namespace Prota.Unity
{
    public abstract class UpdateOnPrefChangeEntry
    {
        public readonly string key;
        
        public UpdateOnPrefChangeEntry(string key)
        {
            this.key = key;
        }
    }
    
    public class PlayerPrefEntryInt : UpdateOnPrefChangeEntry
    {
        public readonly int defaultValue = default;
        
        public int value
        {
            get => PlayerPrefs.GetInt(key, defaultValue);
            set => PlayerPrefs.SetInt(key, value);
        }
        
        public PlayerPrefEntryInt(string key, int defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class PlayerPrefEntryFloat : UpdateOnPrefChangeEntry
    {
        public readonly float defaultValue = default;
        
        public float value
        {
            get => PlayerPrefs.GetFloat(key, defaultValue);
            set => PlayerPrefs.SetFloat(key, value);
        }
        
        public PlayerPrefEntryFloat(string key, float defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
    
    public class PlayerPrefEntryString : UpdateOnPrefChangeEntry
    {
        public readonly string defaultValue = default;
        
        public string value
        {
            get => PlayerPrefs.GetString(key, defaultValue);
            set => PlayerPrefs.SetString(key, value);
        }
        
        public PlayerPrefEntryString(string key, string defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }

        public override string ToString() => value;
    }
    
    public class PlayerPrefEntryString<T> : UpdateOnPrefChangeEntry
    {
        public Func<string, T> getter;
        public Func<T, string> setter;
        
        readonly string defaultValue = default;
        
        public string rawValue
        {
            get => PlayerPrefs.GetString(key, defaultValue);
            set => PlayerPrefs.SetString(key, value);
        }
        
        public T value
        {
            get => getter(rawValue);
            set => rawValue = setter(value);
        }
        
        public PlayerPrefEntryString(string key, Func<string, T> getter, Func<T, string> setter, string defaultValue = default) : base(key)
        {
            this.getter = getter;
            this.setter = setter;
            this.defaultValue = defaultValue;
        }
    }
    
    public class PlayerPrefEntryVec : UpdateOnPrefChangeEntry
    {
        public readonly string keyX;
        public readonly string keyY;
        public readonly string keyZ;
        public readonly string keyW;
        
        public Vector4 defaultValue = default;
        
        public Vector4 vec4
        {
            get => new Vector4(
                PlayerPrefs.GetFloat(keyX, defaultValue.x),
                PlayerPrefs.GetFloat(keyY, defaultValue.y),
                PlayerPrefs.GetFloat(keyZ, defaultValue.z),
                PlayerPrefs.GetFloat(keyW, defaultValue.w)
            );
            set
            {
                PlayerPrefs.SetFloat(keyX, value.x);
                PlayerPrefs.SetFloat(keyY, value.y);
                PlayerPrefs.SetFloat(keyZ, value.z);
                PlayerPrefs.SetFloat(keyW, value.w);
            }
        }
        
        public Vector3 vec3
        {
            get => new Vector3(
                PlayerPrefs.GetFloat(keyX, defaultValue.x),
                PlayerPrefs.GetFloat(keyY, defaultValue.y),
                PlayerPrefs.GetFloat(keyZ, defaultValue.z)
            );
            set
            {
                PlayerPrefs.SetFloat(keyX, value.x);
                PlayerPrefs.SetFloat(keyY, value.y);
                PlayerPrefs.SetFloat(keyZ, value.z);
            }
        }
        
        public Vector2 vec2
        {
            get => new Vector2(
                PlayerPrefs.GetFloat(keyX, defaultValue.x),
                PlayerPrefs.GetFloat(keyY, defaultValue.y)
            );
            set
            {
                PlayerPrefs.SetFloat(keyX, value.x);
                PlayerPrefs.SetFloat(keyY, value.y);
            }
        }
        
        public Color color
        {
            get => new Color(
                PlayerPrefs.GetFloat(keyX, defaultValue.x),
                PlayerPrefs.GetFloat(keyY, defaultValue.y),
                PlayerPrefs.GetFloat(keyZ, defaultValue.z),
                PlayerPrefs.GetFloat(keyW, defaultValue.w)
            );
            set
            {
                PlayerPrefs.SetFloat(keyX, value.r);
                PlayerPrefs.SetFloat(keyY, value.g);
                PlayerPrefs.SetFloat(keyZ, value.b);
                PlayerPrefs.SetFloat(keyW, value.a);
            }
        }
        
        public PlayerPrefEntryVec(string key, Vector4 defaultValue = default) : base(key)
        {
            this.keyX = key + "_x";
            this.keyY = key + "_y";
            this.keyZ = key + "_z";
            this.keyW = key + "_w";
            this.defaultValue = defaultValue;
        }
    }
    
    public class PlayerPrefEntryBool : UpdateOnPrefChangeEntry
    {
        public readonly bool defaultValue = default;
        
        public bool value
        {
            get => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
            set => PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
        
        public PlayerPrefEntryBool(string key, bool defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }
    }
}
