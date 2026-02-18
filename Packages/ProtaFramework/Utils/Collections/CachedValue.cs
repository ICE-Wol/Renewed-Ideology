using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Prota
{
    /*
    CachedKeyValue<long, Vector2> cachedValue;
    if(cachedValue.TryGet(1, out var v)) return v;
    var newValue = ...;
    return cachedValue.Update(1, newValue);
    */
    public struct CachedKeyValue<K, V>
    {
        public K key;
        public V value;
        static EqualityComparer<K> comparer = EqualityComparer<K>.Default;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(K k, out V v)
        {
            if(comparer.Equals(key, k))
            {
                v = value;
                return true;
            }
            
            v = default(V);
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public V Set(K k, V v)
        {
            key = k;
            value = v;
            return v;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(K k, V v, CachedKeyValue<K, V> next)
        {
            Push(next);
            Set(k, v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(CachedKeyValue<K, V> next)
        {
            next.Set(key, value);
        }
    }
    
    public struct CachedValue<T>
    {
        T _value;
        public T value
        {
            get
            {
                if(isDirty && getter != null)
                {
                    _value = getter();
                    isDirty = false;
                    if(isFixed)
                    {
                        getter = null;
                    }
                }
                return _value;
            }
        }
        
        public bool isDirty;
        public bool isFixed { get; private set; }
        
        public Func<T> getter;
        
        public CachedValue(Func<T> getter, bool isFixed = false)
        {
            this.getter = getter;
            _value = default(T);
            isDirty = true;
            this.isFixed = isFixed;
        }
    }
}
