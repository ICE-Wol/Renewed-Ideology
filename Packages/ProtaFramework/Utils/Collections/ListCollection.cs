using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Prota
{
    public readonly struct ListCollection<T>
        : ICollection<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
        where T: class
    {
        readonly List<T> _entries;
        readonly Dictionary<T, int> _index;
        
        public bool valid => _entries != null && _index != null;
        public int Count => _entries.Count;
        public bool IsReadOnly => false;

        public ListCollection(int capacity)
        {
            _entries = new List<T>(capacity);
            _index = new Dictionary<T, int>(capacity);
        }

        public void Clear()
        {
            _entries.Clear();
            _index.Clear();
        }

        public void Add(T obj)
        {
            if (_index.ContainsKey(obj))
                throw new Exception("Node already added to List collection :" + obj);
            
            _entries.Add(obj);
            _index[obj] = _entries.Count - 1;
        }
        
        public bool TryAdd(T obj)
        {
            if (_index.ContainsKey(obj))
                return false;
            
            _entries.Add(obj);
            _index[obj] = _entries.Count - 1;
            return true;
        }
        
        public bool TryRemove(T obj)
        {
            if (!_index.TryGetValue(obj, out var i))
            {
                return false;
            }
            
            // we want to remove the last element.
            // simply remove it from the list.
            if(i == _entries.Count - 1)
            {
                _entries.RemoveAt(i);
                _index.Remove(obj);
                return true;
            }
            
            // set [i] to last element, update its position.
            var lastIndex = _entries.Count - 1;
            _entries[i] = _entries[lastIndex];
            _index[_entries[i]] = i;
            
            // remove obj.
            _entries.RemoveAt(lastIndex);
            _index.Remove(obj);
            
            return true;
        }

        public bool Remove(T obj)
        {
            if(!TryRemove(obj))
                throw new Exception("Node not found in List collection :" + obj);
            return true;
        }

        public bool Contains(T obj)
        {
            return _index.ContainsKey(obj);
        }
        
        public bool TryGetIndex(T obj, out int index)
        {
            return _index.TryGetValue(obj, out index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get => _entries[index];
            set => _entries[index] = value;
        }
        
        public T Replace(int index, T newValue)
        {
            var originalValue = _entries[index];
            _entries[index] = newValue;
            if(!_index.Remove(originalValue))
                throw new Exception("internal error: failed to remove original value from index");
            _index[newValue] = index;
            return originalValue;
        }
    }
    
    public struct ListCollection<T, K>
        : ICollection<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        readonly List<T> _entries;
        readonly Dictionary<K, int> _index;
        public readonly Func<T, K> getKey;
        
        public bool valid => _entries != null && _index != null && getKey != null;
        public int Count => _entries.Count;
        public bool IsReadOnly => false;

        public ListCollection(int capacity, Func<T, K> getKey)
        {
            _entries = new List<T>(capacity);
            _index = new Dictionary<K, int>(capacity);
            this.getKey = getKey;
        }

        public void Clear()
        {
            _entries.Clear();
            _index.Clear();
        }

        public void Add(T obj)
        {
            var key = getKey(obj);
            if (_index.ContainsKey(key))
                throw new Exception("Node already added :" + obj + " :" + key);
            
            _entries.Add(obj);
            _index[key] = _entries.Count - 1;
        }

        public bool TryAdd(T obj)
        {
            var key = getKey(obj);
            if (_index.ContainsKey(key))
                return false;
            
            _entries.Add(obj);
            _index[key] = _entries.Count - 1;
            return true;
        }

        public bool Remove(T obj)
            => RemoveByKey(getKey(obj));
        
        public bool TryRemoveBykey(K key)
        {
            if (!_index.TryGetValue(key, out var i))
            {
                return false;
            }
            
            // we want to remove the last element.
            // simply remove it from the list.
            if(i == _entries.Count - 1)
            {
                _entries.RemoveAt(i);
                _index.Remove(key);
                return true;
            }
            
            // set [i] to last element, update its position.
            var lastIndex = _entries.Count - 1;
            _entries[i] = _entries[lastIndex];
            _index[getKey(_entries[i])] = i;
            
            // remove obj.
            _entries.RemoveAt(lastIndex);
            _index.Remove(key);
            
            return true;
        }
        
        public bool RemoveByKey(K key)
        {
            if(!TryRemoveBykey(key))
                throw new Exception("Node not found :" + key);
            return true;
        }

        public bool Contains(T obj)
        {
            return _index.ContainsKey(getKey(obj));
        }
        
        public bool ContainsKey(K key)
        {
            return _index.ContainsKey(key);
        }
        
        public bool TryGetIndex(T obj, out int index)
        {
            return _index.TryGetValue(getKey(obj), out index);
        }
        
        public bool TryGetIndexByKey(K key, out int index)
        {
            return _index.TryGetValue(key, out index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get => _entries[index];
            set => _entries[index] = value;
        }
        
        public T Replace(int index, T value)
        {
            var originalValue = _entries[index];
            var originalKey = getKey(originalValue);
            var newKey = getKey(value);
            _entries[index] = value;
            _index.Remove(originalKey);
            _index[newKey] = index;
            return originalValue;
        }
    }
    
}
