using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _Scripts.Tools {
    public static class MethodExt {
        public static string GetNamePath(this Component t)
        {
            t = t.transform;
            var sb = new List<string>();
            while(t != null)
            {
                sb.Add(t.gameObject.name);
                t = (t as Transform).parent;
            }
            sb.Reverse();
            return sb.ToStringJoined("/");
        }
        
        public static string ToStringJoined(this List<string> s, string separator)
        {
            return string.Join(separator, s);
        }
        
        public static string ToStringJoined(this StringBuilder s, string separator)
        {
            return s.ToString().TrimEnd(separator.ToCharArray());
        }
    }
}