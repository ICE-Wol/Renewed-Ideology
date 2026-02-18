using UnityEngine;
using Prota.Unity;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static class SizeGetSet
    {
        static readonly List<Func<Component, Vector2>> getters = new();
        static readonly List<Action<Component, Vector2>> setters = new();
        static readonly Dictionary<Type, int> indexMap = new();
        
        static SizeGetSet()
        {
            void Add(Type type, Func<Component, Vector2> getter, Action<Component, Vector2> setter)
            {
                indexMap[type] = getters.Count;
                getters.Add(getter);
                setters.Add(setter);
            }
            
            Add(typeof(RectTransform), c => ((RectTransform)c).sizeDelta, (c, v) => ((RectTransform)c).sizeDelta = v);
            Add(typeof(SpriteRenderer), c => ((SpriteRenderer)c).size, (c, v) => ((SpriteRenderer)c).size = v);
        }
        
        public static bool GetSizeComponent(this GameObject g, out Component sizeTarget, out int typeIndex)
        {
            if(g.TryGetComponent<RectTransform>(out var rt))
            {
                sizeTarget = rt;
                typeIndex = indexMap[typeof(RectTransform)];
                return true;
            }
            
            if(g.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sizeTarget = sr;
                typeIndex = indexMap[typeof(SpriteRenderer)];
                return true;
            }
            
            sizeTarget = null;
            typeIndex = -1;
            return false;
        }
        
        public static bool GetSizeComponent(this GameObject g, out Component sizeTarget, out Func<Component, Vector2> getter, out Action<Component, Vector2> setter)
        {
            if(!g.GetSizeComponent(out sizeTarget, out var typeIndex))
            {
                getter = null;
                setter = null;
                return false;
            }
            
            getter = getters[typeIndex];
            setter = setters[typeIndex];
            return true;
        }
        
        public static Vector2 GetSize(this Component c, int typeIndex)
        {
            return getters[typeIndex](c);
        }
        
        public static void SetSize(this Component c, int typeIndex, Vector2 size)
        {
            setters[typeIndex](c, size);
        }
    }
}

