using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

namespace Prota
{
    public class StructStringCache<T> where T: struct
    {
        public readonly Func<T, string> convert;
        
        readonly Dictionary<T, string> cache = new Dictionary<T, string>();
        
        public StructStringCache(Func<T, string> convert)
        {
            this.convert = convert;
        }
        
        public StructStringCache()
        {
            this.convert = x => x.ToString();
        }
        
        public string this[T x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                cache[x] = y;
                return y;
            }
        }
    }
    
    
    
    // 一个缓存过的 string => string dictionary.
    public class StringCache
    {
        public readonly Func<string, string> convert;
        
        readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        
        
        public StringCache(Func<string, string> convert)
        {
            this.convert = convert;
        }
        
        public StringCache()
        {
            this.convert = x => x;
        }
        
        public string this[string x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                if(y == null) return null;
                cache[x] = y;
                return y;
            }
        }
    }
    
    
    public class IntStringCache
	{
        public readonly Func<int, string> convert;
        
        readonly Dictionary<int, string> cache = new();
        
        public IntStringCache(Func<int, string> convert)
        {
            this.convert = convert;
        }
        
        public IntStringCache()
        {
            this.convert = x => x.ToString();
        }
        
        public string this[int x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                cache[x] = y;
                return y;
            }
        }
    }
    
    public class FloatStringCache
    {
        public readonly Func<float, string> convert;
        
        readonly Dictionary<float, string> cache = new();
        
        public FloatStringCache(Func<float, string> convert)
        {
            this.convert = convert;
        }
        
        public FloatStringCache()
        {
            this.convert = x => x.ToString();
        }
        
        public string this[float x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                cache[x] = y;
                return y;
            }
        }
    }
	
	public class ObjectStringCache<T>
		where T: class
	{
		public readonly Func<T, string> convert;
		
		readonly Dictionary<T, string> cache = new();
		
		public ObjectStringCache(Func<T, string> convert)
		{
			this.convert = convert;
		}
		
		public ObjectStringCache()
		{
			this.convert = x => x.ToString();
		}
		
		public string this[T x]
		{
			get
			{
				if(cache.TryGetValue(x, out var y)) return y;
				y = convert(x);
				cache[x] = y;
				return y;
			}
		}
	}
}
