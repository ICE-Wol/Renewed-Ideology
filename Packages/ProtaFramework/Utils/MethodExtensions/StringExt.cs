using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Prota
{
    public static partial class MethodExtensions
    {
        // Cached regex patterns
        public static readonly Regex hexColorRegex = new Regex(@"^[0-9A-Fa-f]{6}$", RegexOptions.Compiled);
        public static readonly Regex hexColorWithAlphaRegex = new Regex(@"^[0-9A-Fa-f]{8}$", RegexOptions.Compiled);
        
        public static readonly Regex numberRegex = new Regex(@"[-+]?\b\d+\b", RegexOptions.Compiled);
        public static readonly Regex floatNumberRegex = new Regex(@"[-+]?(?:\d*\.\d+|\d+\.?\d*)(?:[eE][-+]?\d+)?", RegexOptions.Compiled);
        public static readonly Regex functionCallRegex = new Regex(@"^(\w+)\((.*)\)$", RegexOptions.Compiled);

        public static bool FindFirstNumberIndex(this string input, out int fromIndex, out int toIndex)
        {
            fromIndex = -1;
            toIndex = -1;
            if (string.IsNullOrEmpty(input)) return false;
            Match match = numberRegex.Match(input);
            if (match.Success)
            {
                fromIndex = match.Index;
                toIndex = match.Index + match.Length - 1;
                return true;
            }
            return false;
        }
        
        public static bool FindLastNumberIndex(this string input, out int fromIndex, out int toIndex)
        {
            fromIndex = -1;
            toIndex = -1;
            if(string.IsNullOrEmpty(input)) return false;
            var match = numberRegex.Matches(input);
            if(match.Count == 0) return false;
            var lastMatch = match[^1];
            fromIndex = lastMatch.Index;
            toIndex = lastMatch.Index + lastMatch.Length - 1;
            return true;
        }
        
        public static bool IsTrimNeeded(this string str)
        {
            return FindTrimSize(str, out _, out _);
        }
        
        public static bool FindTrimSize(this string str, out int trimCountAtStart, out int trimCountAtEnd)
        {
            trimCountAtStart = 0;
            trimCountAtEnd = 0;
            if(str.NullOrEmpty()) return false;
            while(trimCountAtStart < str.Length && char.IsWhiteSpace(str[trimCountAtStart]))
                trimCountAtStart++;
            while(trimCountAtEnd < str.Length && char.IsWhiteSpace(str[str.Length - trimCountAtEnd - 1]))
                trimCountAtEnd++;
            // the whole string is whitespace
            if(trimCountAtStart == str.Length)
            {
                trimCountAtEnd = 0;
                return true;
            }
            return trimCountAtStart != 0 || trimCountAtEnd != 0;
        }
		
		
		public static string[] ParseArray(this string str, char separator = ',', bool trim = true)
		{
			if(trim)
			{
				return str.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
			}
			else
			{
				return str.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToArray();
			}
		}

        static Regex intNumberRegex = new(@"-?\d+", RegexOptions.Compiled);
		static string[] splitSeperator = new string[] { ";", "," };
        public static bool TryParseToIntArray(this string str, out int[] result)
        {
            var s = str.Split(splitSeperator, StringSplitOptions.RemoveEmptyEntries);
            result = s.Select(x => int.Parse(x)).ToArray();
            return true;
        }

		public static bool TryParseToFloatArray(this string str, out float[] result)
        {
            var s = str.Split(splitSeperator, StringSplitOptions.RemoveEmptyEntries);
            result = s.Select(x => float.Parse(x)).ToArray();
            return true;
        }
        
        public static bool TryParseToInt(this string s, out int value)
        {
            return int.TryParse(s, out value);
        }
        
        public static bool TryParseToFloat(this string s, out float value)
        {
            return float.TryParse(s, out value);
        }
        
        public static bool TryParseToInt64(this string s, out long value)
        {
            return long.TryParse(s, out value);
        }
        
        public static bool TryParseToDouble(this string s, out double value)
        {
            return double.TryParse(s, out value);
        }
        
        public static bool TryParseToBool(this string s, out bool value)
        {
            if(s == "")
            {
                value = false;
                return true;
            }
            if(bool.TryParse(s.ToLower(), out value)) return true;
            if(s.TryParseToInt(out int intValue))
            {
                value = intValue != 0;
                return true;
            }
            return false;
        }
        
        public static string ToUpperFirstLetter(this string s)
        {
            if(s.Length == 0) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        
        public static string GetLowerStr(this string s)
        {
            if(s.IsLower()) return s;
            return s.ToLower();
        }
        
        public static bool IsLower(this string s)
        {
            foreach(var c in s) if(!char.IsLower(c)) return false;
            return true;
        }

		public static bool NullOrEmpty([NotNullWhen(false)] this string s) => string.IsNullOrEmpty(s);
        
		public static string WhenNullOrEmpty(this string s, string a)
		{
			if (s.NullOrEmpty()) return a;
			return s;
		}
		
        public static string Log(this string x)
        {
            Console.WriteLine(x);
            return x;
        }
        
        public static string LogError(this string x)
        {
            Console.Error.WriteLine(x);
            return x;
        }
        
        public static StringBuilder ToStringBuilder(this string x)
        {
            var s = new StringBuilder();
            s.Append(x);
            return s;
        }
        
        public static string Repeat(this string x, int n)
        {
            if(n < 0) throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative");
            if(n == 0) return string.Empty;
            var sb = new StringBuilder(x.Length * n);
            for(int i = 0; i < n; i++)
                sb.Append(x);
            return sb.ToString();
        }
        
        [ThreadStatic] static StringBuilder _stringBuilder;
        public static string Join(this IEnumerable<string> list, string separator)
        {
            if(_stringBuilder == null) _stringBuilder = new StringBuilder();
            _stringBuilder.Clear();
            var first = true;
            foreach(var s in list)
            {
                if(first) first = false;
                else _stringBuilder.Append(separator);
                _stringBuilder.Append(s);
            }
            return _stringBuilder.ToString();
        }
        
        public static string ToByteSeqString(this string s)
        {
            return s.Select(x => (int)x).ToStringJoined(",");
        }
        
        public static string WithEncoding(this string s, Encoding src, Encoding dst)
        {
            Console.WriteLine(src.GetBytes(s).Select(x => ((char)x).ToString()).ToStringJoined(":"));
            return dst.GetString(src.GetBytes(s));
        }
        
        public static string GBKToUTF8(this string s)
        {
            return s.WithEncoding(Encoding.GetEncoding("gb2312"), Encoding.UTF8);
        }
        
        public static string CommonPrefix(this IEnumerable<string> list)
        {
            if(list == null) return null;
            var sb = new StringBuilder();
            bool first = true;
            foreach(var s in list)
            {
                if(s == null) throw new Exception("invalid operation: null string in list");
                if(first)
                {
                    first = false;
                    sb.Append(s);
                }
                else
                {
                    while(sb.Length > 0 && sb.Length > s.Length) sb.Length--;
                    while(sb.Length > 0 && sb[^1] != s[sb.Length - 1]) sb.Length--;
                }
            }
            return sb.ToString();
        }
        
        
        public delegate bool CharPredicate(char c);
        public static bool FindIndexWhere(this string s, out int index, CharPredicate predicate)
        {
            for(int i = 0; i < s.Length; i++)
            {
                if(predicate(s[i]))
                {
                    index = i;
                    return true;
                }
            }
            
            index = -1;
            return false;
        }
        
        public static bool FindIndexReversedWhere(this string s, out int index, CharPredicate predicate)
        {
            for(int i = s.Length - 1; i >= 0; i--)
            {
                if(predicate(s[i]))
                {
                    index = i;
                    return true;
                }
            }
            
            index = -1;
            return false;
        }
        
        // extension 不带有点号.
        public static bool HasExtension(this string s, string extension)
        {
            return s.EndsWith(extension) && s[s.Length - extension.Length - 1] == '.';
        }
        
        // extension 不带有点号.
        public static string WithExtension(this string s, string extension)
        {
            if(s.HasExtension(extension)) return s;
            var lastIndexOfDot = s.LastIndexOf('.');
            if(lastIndexOfDot < 0) return s + "." + extension;
            return s[..(lastIndexOfDot + 1)] + extension;
        }
		
		/// <summary>
		/// 合法字符串: FunctionName(arg1, arg2, ...)
		/// 括号是必须的
		/// arg 可以包含括号, 并且其中的逗号会被忽略
		/// 例如 Func(arg1(arg2, arg3), arg4) 只有两个参数, 第一个参数是 arg1(arg2, arg3), 第二个参数是 arg4
		/// </summary>
		/// <param name="s"></param>
		/// <param name="functionName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static bool TryParseFunctionCallLike(this string s, out string functionName, out string[] args)
		{
			functionName = null;
			args = null;
			if(string.IsNullOrEmpty(s)) return false;
			
			// Find the opening parenthesis
			int openParenIndex = s.IndexOf('(');
			if(openParenIndex < 0) return false;
			
			// Extract function name (word characters before '(')
			int nameStart = 0;
			while(nameStart < openParenIndex && char.IsWhiteSpace(s[nameStart]))
				nameStart++;
			if(nameStart >= openParenIndex) return false;
			
			int nameEnd = openParenIndex;
			while(nameEnd > nameStart && char.IsWhiteSpace(s[nameEnd - 1]))
				nameEnd--;
			if(nameEnd <= nameStart) return false;
			
			for(int i = nameStart; i < nameEnd; i++)
			{
				if(!char.IsLetterOrDigit(s[i]) && s[i] != '_')
					return false;
			}
			functionName = s.Substring(nameStart, nameEnd - nameStart);
			
			// Find the matching closing parenthesis
			int closeParenIndex = -1;
			int depth = 0;
			for(int i = openParenIndex; i < s.Length; i++)
			{
				if(s[i] == '(')
					depth++;
				else if(s[i] == ')')
				{
					depth--;
					if(depth == 0)
					{
						closeParenIndex = i;
						break;
					}
				}
			}
			if(closeParenIndex < 0) return false;
			
			// Check if there's anything after the closing parenthesis
			for(int i = closeParenIndex + 1; i < s.Length; i++)
			{
				if(!char.IsWhiteSpace(s[i]))
					return false;
			}
			
			// Parse arguments
			int argsStart = openParenIndex + 1;
			int argsEnd = closeParenIndex;
			
			// Trim whitespace from arguments section
			while(argsStart < argsEnd && char.IsWhiteSpace(s[argsStart]))
				argsStart++;
			while(argsEnd > argsStart && char.IsWhiteSpace(s[argsEnd - 1]))
				argsEnd--;
			
			if(argsStart >= argsEnd)
			{
				args = Array.Empty<string>();
				return true;
			}
			
			// Parse arguments by tracking parentheses depth
			var argList = new List<string>();
			int argStart = argsStart;
			depth = 0;
			
			for(int i = argsStart; i < argsEnd; i++)
			{
				if(s[i] == '(')
				{
					depth++;
				}
				else if(s[i] == ')')
				{
					depth--;
				}
				else if(s[i] == ',' && depth == 0)
				{
					// Found a top-level comma, extract the argument
					int argEnd = i;
					while(argEnd > argStart && char.IsWhiteSpace(s[argEnd - 1]))
						argEnd--;
					// Always add the argument, even if it's empty
					argList.Add(s.Substring(argStart, argEnd - argStart));
					argStart = i + 1;
					while(argStart < argsEnd && char.IsWhiteSpace(s[argStart]))
						argStart++;
				}
			}
			
			// Add the last argument
			// Check if there are any arguments processed (argStart was moved by commas)
			// or if we're still at the start (no commas found, single argument case)
			if(argStart <= argsEnd)
			{
				int argEnd = argsEnd;
				while(argEnd > argStart && char.IsWhiteSpace(s[argEnd - 1]))
					argEnd--;
				// Always add the last argument, even if it's empty
				argList.Add(s.Substring(argStart, argEnd - argStart));
			}
			
		args = argList.ToArray();
		return true;
	}
	
	/// <summary>
	/// 解析 "name:value" 格式的字符串，返回名称和整数值
	/// </summary>
	public static bool TryParseNameValue(this string s, out string name, out int value, char separator = ':')
	{
		name = null;
		value = 0;
		if(string.IsNullOrEmpty(s)) return false;
		
		int separatorIndex = s.LastIndexOf(separator);
		if(separatorIndex < 0) return false;
		
		// 扫描 name 部分的起始和结束位置（去除前后空白）
		int nameStart = 0;
		while(nameStart < separatorIndex && char.IsWhiteSpace(s[nameStart]))
			nameStart++;
		
		int nameEnd = separatorIndex;
		while(nameEnd > nameStart && char.IsWhiteSpace(s[nameEnd - 1]))
			nameEnd--;
		
		if(nameStart >= nameEnd) return false;
		
		// 扫描 value 部分的起始和结束位置（去除前后空白）
		int valueStart = separatorIndex + 1;
		while(valueStart < s.Length && char.IsWhiteSpace(s[valueStart]))
			valueStart++;
		
		int valueEnd = s.Length;
		while(valueEnd > valueStart && char.IsWhiteSpace(s[valueEnd - 1]))
			valueEnd--;
		
		if(valueStart >= valueEnd) return false;
		
		// 直接使用位置解析，避免创建中间字符串
		var valueStr = s.AsSpan().Slice(valueStart, valueEnd - valueStart);
		if(!int.TryParse(valueStr, out value)) return false;
		
		name = s.Substring(nameStart, nameEnd - nameStart);
		return true;
	}

		/// <summary>
		/// 解析 "name:value" 格式的字符串，返回名称和浮点数值
		/// </summary>
		public static bool TryParseNameValue(this string s, out string name, out float value, char separator = ':')
		{
			name = null;
			value = 0f;
			if (string.IsNullOrEmpty(s)) return false;

			int separatorIndex = s.LastIndexOf(separator);
			if (separatorIndex < 0) return false;

			// 扫描 name 部分的起始和结束位置（去除前后空白）
			int nameStart = 0;
			while (nameStart < separatorIndex && char.IsWhiteSpace(s[nameStart]))
				nameStart++;

			int nameEnd = separatorIndex;
			while (nameEnd > nameStart && char.IsWhiteSpace(s[nameEnd - 1]))
				nameEnd--;

			if (nameStart >= nameEnd) return false;

			// 扫描 value 部分的起始和结束位置（去除前后空白）
			int valueStart = separatorIndex + 1;
			while (valueStart < s.Length && char.IsWhiteSpace(s[valueStart]))
				valueStart++;

			int valueEnd = s.Length;
			while (valueEnd > valueStart && char.IsWhiteSpace(s[valueEnd - 1]))
				valueEnd--;

			if (valueStart >= valueEnd) return false;

			// 直接使用位置解析，避免创建中间字符串
			var valueStr = s.AsSpan().Slice(valueStart, valueEnd - valueStart);
			if (!float.TryParse(valueStr, out value)) return false;

			name = s.Substring(nameStart, nameEnd - nameStart);
			return true;
		}

		public static bool TryParseNameValues(this string s, out (string name, float value)[] result, char itemSeparator = ',', char valueSeparator = ':')
		{
			result = null;
			if (string.IsNullOrEmpty(s)) return false;

			var list = new List<(string, float)>();
			s.ParseNameValues(list, itemSeparator, valueSeparator);
			
			if (list.Count == 0) return false;
			result = list.ToArray();
			return true;
		}

		public static void ParseNameValues(this string s, List<(string name, float value)> result, char itemSeparator = ',', char valueSeparator = ':')
		{
			if (string.IsNullOrEmpty(s)) return;
			if (result == null) throw new ArgumentNullException(nameof(result));

			var items = s.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in items)
			{
				if (item.TryParseNameValue(out var name, out float value, valueSeparator))
				{
					result.Add((name, value));
				}
			}
		}

		public static bool TryParseNameValues(this string s, out (string name, int value)[] result, char itemSeparator = ',', char valueSeparator = ':')
		{
			result = null;
			if (string.IsNullOrEmpty(s)) return false;

			var list = new List<(string, int)>();
			s.ParseNameValues(list, itemSeparator, valueSeparator);

			if (list.Count == 0) return false;
			result = list.ToArray();
			return true;
		}

		public static void ParseNameValues(this string s, List<(string name, int value)> result, char itemSeparator = ',', char valueSeparator = ':')
		{
			if (string.IsNullOrEmpty(s)) return;
			if (result == null) throw new ArgumentNullException(nameof(result));

			var items = s.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in items)
			{
				if (item.TryParseNameValue(out var name, out int value, valueSeparator))
				{
					result.Add((name, value));
				}
			}
		}
	
	    [ThreadStatic]
		static StringBuilder _sb;
		static StringBuilder cachedStringBuilder => _sb ??= new StringBuilder(1024);
		/// <summary>
		/// 替换规则: $1, $2 替换为 contents[a], contents[b]. 连续两个 $$ 替换为 $.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="contents"></param>
		/// <returns></returns>
		public static string PatternReplace(this string s, IReadOnlyList<string> contents)
		{
			if (contents == null) return s;
			if (contents.Count == 0) return s;
			
			var sb = cachedStringBuilder;
			sb.Clear();

			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];

				// $$ -> $
				if (c == '$' && i + 1 < s.Length && s[i + 1] == '$')
				{
					sb.Append('$');
					i++;
					continue;
				}

				// $123 -> contents[122]
				if (c == '$' && i + 1 < s.Length && char.IsDigit(s[i + 1]))
				{
					int j = i + 1;
					int index = 0;

					while (j < s.Length && char.IsDigit(s[j]))
					{
						index = index * 10 + (s[j] - '0');
						j++;
					}

					index--; // $1 => contents[0]

					if ((uint)index < (uint)contents.Count)
						sb.Append(contents[index]);

					i = j - 1;
					continue;
				}

				sb.Append(c);
			}

			return sb.ToString();
		}
	}
}
