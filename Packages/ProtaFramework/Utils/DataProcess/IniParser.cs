
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Prota
{
    public class IniParser
    {
        public struct Entry
        {
            public readonly string header;
            public readonly string key;
            
            public readonly string value;
            
            public int intValue => int.Parse(value);
            
            public float floatValue => float.Parse(value, CultureInfo.InvariantCulture);
            
            public bool boolValue => value == "true";
            
            public double doubleValue => double.Parse(value, CultureInfo.InvariantCulture);
            
            public long longValue => long.Parse(value);
            
            public Entry(string header, string key, string value)
            {
                this.header = header;
                this.key = key;
                this.value = value;
            }
        }
        
        public readonly HashMapDict<string, string, Entry> entries = new HashMapDict<string, string, Entry>();
        
        public IniParser(string originalContent)
        {
            string RemoveComment(string x)
            {
                if(x.Contains(";")) return x.Substring(0, x.IndexOf(";"));
                return x;
            }
            
            string curHeader = "$global";
            int line = 0;
            foreach(var st in originalContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                line++;
                var s = RemoveComment(st);
                
                if(string.IsNullOrWhiteSpace(s)) continue;
                
                s = s.Trim();
                if(s.StartsWith("[") && s.EndsWith("]"))
                {
                    curHeader = s.Substring(1, s.Length - 2);
                    continue;
                }
                
                var split = s.Split('=', 2);  // 取第一个等号作为分界点.
                if(split.Length != 2) throw new Exception($"Invalid ini format at line [{line}] \n{ originalContent }");
                var key = split[0].Trim();
                var value = split[1].Trim().Trim('"');  // 如果 value 是用引号包裹的, 那么不算引号.
                entries.AddElement(curHeader, key, new Entry(curHeader, key, value));
            }
        }
    }
    
}

