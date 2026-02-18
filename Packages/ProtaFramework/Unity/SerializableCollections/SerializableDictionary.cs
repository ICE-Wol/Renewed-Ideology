using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Prota
{
    public static class SerializableHashMap
    {
        [Serializable] public class IntBool : SerializableHashMap<int, bool> { }
        [Serializable] public class IntInt : SerializableHashMap<int, int> { }
        [Serializable] public class IntFloat : SerializableHashMap<int, float> { }
        [Serializable] public class IntDouble : SerializableHashMap<int, double> { }
        [Serializable] public class IntColor32 : SerializableHashMap<int, Color32> { }
        [Serializable] public class IntVector2 : SerializableHashMap<int, Vector2> { }
        [Serializable] public class IntVector3 : SerializableHashMap<int, Vector3> { }
        [Serializable] public class IntVector4 : SerializableHashMap<int, Vector4> { }
        [Serializable] public class IntColor : SerializableHashMap<int, Color> { }
        [Serializable] public class IntRect : SerializableHashMap<int, Rect> { }
        [Serializable] public class IntVector3Int : SerializableHashMap<int, Vector3Int> { }
        [Serializable] public class IntVector2Int : SerializableHashMap<int, Vector2Int> { }
        [Serializable] public class StringString : SerializableHashMap<string, string> { }
        [Serializable] public class StringInt : SerializableHashMap<string, int> { }
        [Serializable] public class StringFloat : SerializableHashMap<string, float> { }
        [Serializable] public class StringDouble : SerializableHashMap<string, double> { }
        [Serializable] public class StringBool : SerializableHashMap<string, bool> { }
        [Serializable] public class StringVector2 : SerializableHashMap<string, Vector2> { }
        [Serializable] public class StringVector3 : SerializableHashMap<string, Vector3> { }
        [Serializable] public class StringVector4 : SerializableHashMap<string, Vector4> { }
        [Serializable] public class StringColor : SerializableHashMap<string, Color> { }
        [Serializable] public class StringRect : SerializableHashMap<string, Rect> { }
        [Serializable] public class StringVector3Int : SerializableHashMap<string, Vector3Int> { }
        [Serializable] public class StringVector2Int : SerializableHashMap<string, Color32> { }
    }
    
    [Serializable]
    public class SerializableHashMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        struct KeyValuePair
        {
            public TKey key;
            public TValue value;
        }
        
        [SerializeField] KeyValuePair[] keyValuePairs = null;
        
        public void OnAfterDeserialize()
        {
			Clear();
            this.EnsureCapacity(Mathf.CeilToInt(keyValuePairs.Length / 0.72f));
            for(int i = 0; i < keyValuePairs.Length; i++)
            {
                this.Add(keyValuePairs[i].key, keyValuePairs[i].value);
            }
        }

        public void OnBeforeSerialize()
        {
            keyValuePairs = new KeyValuePair[Count];
            var i = 0;
            foreach(var (k, v) in this)
            {
                keyValuePairs[i++] = new KeyValuePair { key = k, value = v };
            }
        }
    }
}
