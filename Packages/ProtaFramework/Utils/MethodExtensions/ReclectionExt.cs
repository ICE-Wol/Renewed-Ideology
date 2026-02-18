using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static string ToInfoString(this MethodInfo method)
        {
            return $"{method.DeclaringType.Name}.{method.Name}";
        }
        
        public static string ToInfoStringFull(this MethodInfo method)
        {
            return $"{method.DeclaringType.FullName}.{method.Name}({method.GetParameters().ToStrings().Join(",")})";
        }
    }
}