using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Prota
{
    public static partial class MethodExtensions
    {
		
		public static void Complete(this IEnumerator enumerator)
		{
			for(int i = 0; i < 100000000; i++)
			{
				if(!enumerator.MoveNext()) break;
			}
		}
		
        public static T FirstElement<T>(this IEnumerable<T> e)
        {
            var s = e.GetEnumerator();
            if(!s.MoveNext()) throw new ArgumentException("collecetion doesn't have element.");
            return s.Current;
        }
        
        public static Task DoCoroutineAsync<T>(this IEnumerable<T> x, CancellationToken? _token)
            => DoCoroutineAsync(x.GetEnumerator(), _token);
        
        public static async Task DoCoroutineAsync<T>(this IEnumerator<T> x, CancellationToken? _token)
        {
            var token = _token ?? CancellationToken.None;
            Func<bool> run = () => x.MoveNext();
            while(await Task.Run(run, token));
        }
        
        public static string ToListString<T>(this IEnumerable<T> list, Func<T, string> f, string seperator = null)
            => string.Join(seperator ?? ",",  list.Select(x => $"[{f(x)}]"));
        
        public static bool SameContent<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if((a == null) != (b == null)) return false;
            
            using var _ = TempHashSet.Get<T>(out var ha);
            using var __ = TempHashSet.Get<T>(out var hb);
            ha.AddRange(a);
            hb.AddRange(b);
            foreach(var i in ha)
            {
                if(!hb.Contains(i)) return false;
                hb.Remove(i);
            }
            return hb.Count == 0;
        }
        
        public static bool SameContentAndOrder<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if((a == null) != (b == null)) return false;
            
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            while(true)
            {
                var na = ea.MoveNext();
                var nb = eb.MoveNext();
                if(na != nb) return false;
                if(!na) return true;
                if(!EqualityComparer<T>.Default.Equals(ea.Current, eb.Current)) return false;
            }
        }
        
        public static void Diff<T>(this IEnumerable<T> a, IEnumerable<T> b, Action<T, T> f)
        {
            using var _ = TempHashSet.Get<T>(out var ha);
            using var __ = TempHashSet.Get<T>(out var hb);
            ha.AddRange(a);
            hb.AddRange(b);
            foreach(var i in ha)
            {
                if(hb.Contains(i)) continue;
                f(i, default);
            }
            foreach(var i in hb)
            {
                if(ha.Contains(i)) continue;
                f(default, i);
            }
        }
        
        public static bool SameContentAndCount<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if((a == null) != (b == null)) return false;
            
            using var _ = TempDict.Get<T, int>(out var ha);
            using var __ = TempDict.Get<T, int>(out var hb);
            
            foreach(var i in a)
            {
                if(!ha.ContainsKey(i)) ha[i] = 0;
                ha[i]++;
            }
            
            foreach(var i in b)
            {
                if(!hb.ContainsKey(i)) hb[i] = 0;
                hb[i]++;
            }
            
            foreach(var i in ha)
            {
                if(!hb.ContainsKey(i.Key)) return false;
            }
            
            foreach(var i in hb)
            {
                if(!ha.ContainsKey(i.Key)) return false;
            }
            
            foreach(var i in ha)
            {
                if(hb[i.Key] != i.Value) return false;
            }
            
            return true;
        }
        
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> e)
            => e == null ? Enumerable.Empty<T>() : e;
        
        // 元素 i 移动到 (i + a) % len.
        public static IEnumerable<T> RightRotate<T>(this IEnumerable<T> e, int a)
        {
            var n = e.Count();
            a = a.Repeat(n);
            if(a == 0) return e;
            return e.TakeLast(a).Concat(e.Take(n - a));
        }
        
        // 元素 i 移动到 (i - a) % len.
        public static IEnumerable<T> LeftRotate<T>(this IEnumerable<T> e, int a)
        {
            var n = e.Count();
            a = a.Repeat(n);
            if(a == 0) return e;
            return e.Skip(a).Concat(e.Take(a));
        }
        
        public static IEnumerable<T> WhereNotSelf<T>(this IEnumerable<T> e, T self)
            => e.Where(x => !EqualityComparer<T>.Default.Equals(x, self));
        
        public static IEnumerable<T> WhereNotSelf<T>(this IEnumerable<T> e, int selfIndex)
        {
            return e.Where((x, i) => i != selfIndex);
        }
        
        public static IEnumerable<R> CastNotNull<R>(this IEnumerable e)
        {
            foreach(var i in e) if(i is R r) yield return r;
        }
        
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> e) where T : class
            => e.Where(x => x != null);
            
        public static IEnumerable<T> WhereNotNull<T, R>(this IEnumerable<T> e, Func<T, R> f) where R : class
        {
            foreach(var i in e) if(i != null && f(i) != null) yield return i;
        }
        
        public static List<T> ToList<T>(this IEnumerable<T> a, List<T> b)
        {
            b.Clear();
            b.AddRange(a);
            return b;
        }
        
    }
}
