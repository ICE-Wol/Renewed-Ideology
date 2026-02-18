using System.Collections.Generic;
using System;
using System.Collections;

namespace Prota
{
    public class DenseSet<T>
        : ICollection<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
        where T: class
    {
        T[] _entries;
        readonly Dictionary<T, int> _index;
        int _count;
        
        public bool valid => _entries != null && _index != null;
        public int Count => _count;
        public bool IsReadOnly => false;

        public DenseSet(int capacity)
        {
            _entries = new T[capacity];
            _index = new Dictionary<T, int>(capacity);
            _count = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _entries[i] = null;
            }
            _index.Clear();
            _count = 0;
        }

        void EnsureCapacity()
        {
            if (_count < _entries.Length)
                return;
            
            var newCapacity = _entries.Length == 0 ? 4 : _entries.Length * 2;
            _entries = _entries.Resize(newCapacity);
        }
        
        public void Add(T obj)
        {
            if (_index.ContainsKey(obj))
                throw new Exception("Node already added to Array collection :" + obj);
            
            EnsureCapacity();
            
            _entries[_count] = obj;
            _index[obj] = _count;
            _count++;
        }
        
        public bool TryAdd(T obj)
        {
            if (_index.ContainsKey(obj))
                return false;
            
            EnsureCapacity();
            
            _entries[_count] = obj;
            _index[obj] = _count;
            _count++;
            return true;
        }
        
        public bool TryRemove(T obj)
        {
            if (!_index.TryGetValue(obj, out var i))
            {
                return false;
            }
            
            // we want to remove the last element.
            // simply remove it from the array.
            if(i == _count - 1)
            {
                _entries[i] = null;
                _index.Remove(obj);
                _count--;
                return true;
            }
            
            // set [i] to last element, update its position.
            var lastIndex = _count - 1;
            _entries[i] = _entries[lastIndex];
            _index[_entries[i]] = i;
            
            // remove obj.
            _entries[lastIndex] = null;
            _index.Remove(obj);
            _count--;
            
            return true;
        }

        public bool Remove(T obj)
        {
            if(!TryRemove(obj))
                throw new Exception("Node not found in Array collection :" + obj);
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
            Array.Copy(_entries, 0, array, arrayIndex, _count);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly DenseSet<T> _collection;
            private int _index;
            private T _current;

            internal Enumerator(DenseSet<T> collection)
            {
                _collection = collection;
                _index = 0;
                _current = default;
            }

            public bool MoveNext()
            {
                if (_index < _collection._count)
                {
                    _current = _collection._entries[_index];
                    _index++;
                    return true;
                }
                _index = _collection._count;
                _current = default;
                return false;
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose() { }

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
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
    
    public class DenseSet<K, V> : IReadOnlyDictionary<K, V>
    {
        V[] _entries;
        K[] _keys;
        readonly Dictionary<K, int> _index;
		readonly Func<K, V> getValue;
        int _count;
        
        public bool valid => _entries != null && _keys != null && _index != null && getValue != null;
        public int Count => _count;
        public bool IsReadOnly => false;

        public DenseSet(int capacity, Func<K, V> getValue)
        {
			_entries = new V[capacity];
            _keys = new K[capacity];
            _index = new Dictionary<K, int>(capacity);
			this.getValue = getValue;
            _count = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _entries[i] = default;
                _keys[i] = default;
            }
            _index.Clear();
            _count = 0;
        }

        void EnsureCapacity()
        {
            if (_count < _entries.Length)
                return;
            
            var newCapacity = _entries.Length == 0 ? 4 : _entries.Length * 2;
            _entries = _entries.Resize(newCapacity);
            _keys = _keys.Resize(newCapacity);
        }
        
        public int Add(K key)
            => Add(key, getValue(key));

        public int Add(K key, V value)
        {
            if (_index.ContainsKey(key))
                throw new Exception("Node already added :" + key + " :" + value);
            
            EnsureCapacity();
            
            _entries[_count] = value;
            _keys[_count] = key;
            _index[key] = _count;
            _count++;
			return _count - 1;
        }

        public bool TryAdd(K key)
            => TryAdd(key, getValue(key));

        public bool TryAdd(K key, V value)
        {
            if (_index.ContainsKey(key))
                return false;
            
            EnsureCapacity();
			
            _entries[_count] = value;
            _keys[_count] = key;
            _index[key] = _count;
            _count++;
            return true;
        }

        public bool Remove(K key)
            => RemoveByKey(key);
        
        public bool TryRemoveByKey(K key)
        {
            if (!_index.TryGetValue(key, out var i))
            {
                return false;
            }
            
            // we want to remove the last element.
            // simply remove it from the array.
            if(i == _count - 1)
            {
                _entries[i] = default(V);
                _keys[i] = default(K);
                _index.Remove(key);
                _count--;
                return true;
            }
            
            // set [i] to last element, update its position.
            var lastIndex = _count - 1;
            var lastKey = _keys[lastIndex];
            _entries[i] = _entries[lastIndex];
            _keys[i] = lastKey;
            _index[lastKey] = i;
            
            // remove obj.
            _entries[lastIndex] = default(V);
            _keys[lastIndex] = default(K);
            _index.Remove(key);
            _count--;
            
            return true;
        }
        
        public bool RemoveByKey(K key)
        {
            if(!TryRemoveByKey(key))
                throw new Exception("Node not found :" + key);
            return true;
        }

        public bool Contains(K key)
        {
            return _index.ContainsKey(key);
        }

        public bool ContainsKey(K key)
        {
            return _index.ContainsKey(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            if (_index.TryGetValue(key, out var i))
            {
                value = _entries[i];
                return true;
            }
            value = default;
            return false;
        }
        
        public bool TryGetIndex(K key, out int index)
        {            return _index.TryGetValue(key, out index);
        }
        
        public bool TryGetIndexByKey(K key, out int index)
        {
            return _index.TryGetValue(key, out index);
        }

        public void CopyTo(V[] array, int arrayIndex)
        {
            Array.Copy(_entries, 0, array, arrayIndex, _count);
        }

        public ref V this[K key] => ref _entries[_index[key]];
        V IReadOnlyDictionary<K, V>.this[K key] => _entries[_index[key]];

        public ref V this[int index] => ref _entries[index];
        public ref K KeyAt(int index) => ref _keys[index];

        public IEnumerable<K> Keys
        {            get
            {
                for (int i = 0; i < _count; i++)
                    yield return _keys[i];
            }
        }

        public IEnumerable<V> Values
        {            get
            {
                for (int i = 0; i < _count; i++)
                    yield return _entries[i];
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return new KeyValuePair<K, V>(_keys[i], _entries[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
}

