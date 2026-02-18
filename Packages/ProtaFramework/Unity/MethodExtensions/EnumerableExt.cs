using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System.Buffers;
using System;
using System.Linq;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
		
        public static void SetActiveAll(this IEnumerable<UnityEngine.GameObject> obj, bool active = true)
        {
            if(obj == null) return;
            foreach(var o in obj) if(o != null) o.SetActive(active);
        }
        
        public static void ActiveDestroyAll(this IEnumerable<UnityEngine.GameObject> list)
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) g.ActiveDestroy();
        }
        
    
        public static void DestroyAll<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) UnityEngine.Object.Destroy(g);
        }
        
        public static void DestroyAllImmediate<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) UnityEngine.Object.DestroyImmediate(g);
        }
        
        public static IEnumerable<T> CommonPrefix<T, G>(this IEnumerable<G> list)
            where G: IEnumerable<T>
        {
            // 列表数为 0
            var n = list.Count();
            if(n == 0) yield break;
            
            var g = list.Select(x => x.GetEnumerator()).ToArray();
            for(int i = 0; i < 1e12; i++)
            {    
                foreach(var x in g) if(!x.MoveNext()) yield break;
                var v = g[0].Current;
                if(g.Skip(1).All(x => EqualityComparer<T>.Default.Equals(x.Current, v)))
                    yield return v;
                else
                    yield break;
            }
        }
    }
    
}
