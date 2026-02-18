using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace Prota
{
    
    public class HashMapArray<A, B> : Dictionary<A, B[]>
    {
        public IEnumerable<B> elements => this.Values.SelectMany(x => x);
        
        public bool Contains(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            return s.Contains(val);
        }
        
        public void Resize(A key, int size)
        {
            if(!this.TryGetValue(key, out var s))
            {
                s = new B[size];
            }
            else
            {
                Array.Resize(ref s, size);
            }
            this[key] = s;
        }
        
        public void EnsureCap(A key, int i)
        {
            if(!this.TryGetValue(key, out var s))
            {
                s = new B[i.NextPowerOfTwo()];
            }
            else if(s.Length <= i)
            {
                Array.Resize(ref s, i.NextPowerOfTwo());
            }
            this[key] = s;
        }
        
        public HashMapArray<A, B> SetElement(A key, int i, B val)
        {
            this.EnsureCap(key, i);
            this[key][i] = val;
            return this;
        }
        
    }
    
    
    // 相当于 Dictionary<A, List<B>>, 提供了额外的函数.
    public class HashMapList<A, B> : Dictionary<A, List<B>>
    {
        public IEnumerable<B> elements => this.Values.SelectMany(x => x);
        
        public bool Contains(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            return s.Contains(val);
        }
        
        public HashMapList<A, B> SetElement(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            s[i] = val;
            return this;
        }
        
        public HashMapList<A, B> InsertElement(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Insert(i, val);
            return this;
        }
        
        public HashMapList<A, B> AddElement(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(val);
            return this;
        }
        
        public void Remove(Func<KeyValuePair<A, List<B>>, bool> f)
        {
            using var _ = this.Where(f).Select(x => x.Key).ToTempList(out var toBeRemoved);
            foreach(var k in toBeRemoved) this.Remove(k);
        }
        
        public bool RemoveElementAt(A key, int i)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            s.RemoveAt(i);
            if(s.Count == 0) this.Remove(key);
            return true;
        }
        
        public bool RemoveElement(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(val);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
        
        public void RemoveElement(A key, Predicate<B> f)
        {
            if(!this.TryGetValue(key, out var s)) return;
            var res = s.RemoveAll(f);
            if(s.Count == 0) this.Remove(key);
        }
        
        public void RemoveElement(Predicate<B> f)
        {
            foreach(var s in this.Values) s.RemoveAll(f);
            this.Remove(x => x.Value.Count == 0);
        }
        
        public bool InsertElementNoDuplicate(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            if(s.Contains(val)) return false;
            s.Insert(i, val);
            return true;
        }
        
        public bool AddElementNoDuplicate(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            if(s.Contains(val)) return false;
            s.Add(val);
            return true;
        }
        
        public B FirstElement(A key)
        {
            if(!this.TryGetValue(key, out var s)) return default;
            return s[0];
        }
        
        public B LastElement(A key)
        {
            if(!this.TryGetValue(key, out var s)) return default;
            return s[s.Count - 1];
        }
        
        public bool TryGetElement(A key, int i, out B res)
        {
            res = default;
            if(!this.TryGetValue(key, out var s)) return false;
            if(s.TryGetValue(i, out res)) return true;
            return false;
        }
    }
    
    // 相当于 Dictionary<A, HashSet<B>>, 提供了额外的函数.
    public class HashMapSet<A, B> : Dictionary<A, HashSet<B>>
    {
        
        public HashMapSet<A, B> AddElement(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(val);
            return this;
        }
        
        public bool RemoveElement(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(val);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
        
        public bool TryGetElement(A key, out B val)
        {
            val = default;
            if(!this.TryGetValue(key, out var s)) return false;
            val = s.FirstElement();
            return true;
        }
        
        public int RemoveByValue(B val)
        {
            int res = 0;
            var eq = EqualityComparer<B>.Default;
            foreach(var r in this.Keys.ToArray())
            {
                res += this[r].RemoveWhere(x => eq.Equals(x, val));
            }
            return res;
        }
        
        public bool ContainsElement(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            return s.Contains(val);
        }
    }
    
    // 相当于 Dictionary<A, Dictionary<B, C>>, 提供了额外的函数.
    public class HashMapDict<A, B, C> : Dictionary<A, Dictionary<B, C>>
    {
        
        public HashMapDict<A, B, C> SetElement(A key, B key2, C val)
        {
            this.GetOrCreate(key, out var s);
            s[key2] = val;
            return this;
        }
        
        public HashMapDict<A, B, C> AddElement(A key, B key2, C val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(key2, val);
            return this;
        }
        
        public bool RemoveElement(A key, B key2)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(key2);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
        
        public C GetElementOrDefault(A key, B key2)
        {
            if(!this.TryGetValue(key, out var s)) return default;
            if(s.TryGetValue(key2, out var res)) return res;
            return default;
        }
        
        public bool TryGetElement(A key, B key2, out C res)
        {
            res = default;
            if(!this.TryGetValue(key, out var s)) return false;
            if(s.TryGetValue(key2, out res)) return true;
            return false;
        }
        
        public bool ContainsElement(A key, B key2)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            return s.ContainsKey(key2);
        }
    }
    
    
    
    // 双向索引 Dictionary.
    public class PairDictionary<A, B>
        : IEnumerable
        , IDictionary<A, B>
        , IReadOnlyDictionary<A, B>
        , IDictionary
    {
        readonly IDictionary<A, B> dict;
        readonly IDictionary<B, A> rev;
        
        public IDictionary<B, A> inverted => new PairDictionary<B, A>(rev, dict);
        
        public PairDictionary()
        {
            dict = new Dictionary<A, B>();
            rev = new Dictionary<B, A>();
        }
        
        public PairDictionary(IDictionary<A, B> dict, IDictionary<B, A> rev)
        {
            this.dict = new Dictionary<A, B>(dict);
            this.rev = new Dictionary<B, A>(rev);
        }
        
        
        public A GetKeyByValue(B value)
        {
            return rev[value];
        }
        
        public bool TryGetKeyByValue(B value, out A key)
        {
            return rev.TryGetValue(value, out key);
        }
        
        public bool ContainsKey(A key)
        {
            return dict.ContainsKey(key);
        }
        
        public bool ContainsValue(B value)
        {
            return rev.ContainsKey(value);
        }
        
        
        
        public B this[A key]
        {
            get => dict[key];
            set
            {
                dict[key] = value;
                rev[value] = key;
            }
        }
        
        object IDictionary.this[object key]
        {
            get => (dict as IDictionary)[key];
            set
            {
                (dict as IDictionary)[key] = value;
                (rev as IDictionary)[value] = key;
            }
        }

        public ICollection<A> Keys => dict.Keys;

        public ICollection<B> Values => dict.Values;

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        IEnumerable<A> IReadOnlyDictionary<A, B>.Keys => dict.Keys;

        ICollection IDictionary.Keys => (ICollection)dict.Keys;

        IEnumerable<B> IReadOnlyDictionary<A, B>.Values => dict.Values;

        ICollection IDictionary.Values => (ICollection)dict.Values;

        public void Add(A key, B value)
        {
            dict.Add(key, value);
            rev.Add(value, key);
        }

        public void Add(KeyValuePair<A, B> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(object key, object value)
        {
            (this as IDictionary).Add(key, value);
        }

        public void Clear()
        {
            dict.Clear();
            rev.Clear();
        }

        public bool Contains(KeyValuePair<A, B> item)
        {
            return (dict as IDictionary).Contains(item);
        }

        public bool Contains(object key)
        {
            return (dict as IDictionary).Contains(key);
        }

        public void CopyTo(KeyValuePair<A, B>[] array, int arrayIndex)
        {
            (dict as ICollection<KeyValuePair<A, B>>).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            (dict as ICollection).CopyTo(array, index);
        }

        public IEnumerator<KeyValuePair<A, B>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        
        public bool Remove(A key)
        {
            if(!dict.TryGetValue(key, out var value)) return false;
            dict.Remove(key);
            rev.Remove(value);
            return true;
        }

        public bool Remove(KeyValuePair<A, B> item)
        {
            return this.Remove(item.Key);
        }

        public void Remove(object key)
        {
            if(!(key is A keyA)) return;
            this.Remove(keyA);
        }

        public bool TryGetValue(A key, out B value)
        {
            return dict.TryGetValue(key, out value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (dict as System.Collections.IEnumerable).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => (dict as IDictionary).GetEnumerator();
    }

}
