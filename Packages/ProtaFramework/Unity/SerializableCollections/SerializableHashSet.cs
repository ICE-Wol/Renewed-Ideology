using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Prota
{
    public static class SerializableHashSet
    {
        [Serializable] public class Int : SerializableHashSet<int> { }
        [Serializable] public class Float : SerializableHashSet<float> { }
        [Serializable] public class Double : SerializableHashSet<double> { }
        [Serializable] public class Bool : SerializableHashSet<bool> { }
        [Serializable] public class String : SerializableHashSet<string> { }
        [Serializable] public class Vector2 : SerializableHashSet<Vector2> { }
        [Serializable] public class Vector3 : SerializableHashSet<Vector3> { }
        [Serializable] public class Vector4 : SerializableHashSet<Vector4> { }
        [Serializable] public class Color : SerializableHashSet<Color> { }
        [Serializable] public class Color32 : SerializableHashSet<Color32> { }
        [Serializable] public class Rect : SerializableHashSet<Rect> { }
        [Serializable] public class Vector3Int : SerializableHashSet<Vector3Int> { }
        [Serializable] public class Vector2Int : SerializableHashSet<Vector2Int> { }
    }
    
    [Serializable]
    public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField] T[] items = null;
        
        public void OnAfterDeserialize()
        {
            Clear();
            this.EnsureCapacity(Mathf.CeilToInt(items.Length / 0.72f));
            for(int i = 0; i < items.Length; i++)
            {
                this.Add(items[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            items = new T[Count];
            var i = 0;
            foreach(var item in this)
            {
                items[i++] = item;
            }
        }
    }
}
