using System;
using UnityEngine;
using System.Collections.Generic;
using Prota;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        static string[] seperator = new string[]{ ",", "|", ";" };
        
        // format accepted:
        // 1,2
        // (1,2)
        // (1, 2)
        // regardless of the separator (comma, space, or both)
        public static bool TryParseToVector2(this string s, out Vector2 value)
        {
            value = default;
            if(s.StartsWith("Vector2")) s = s[7..];
            while(s.StartsWith("(") && s.EndsWith(")")) s = s[1..^1];
            var parts = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length != 2) return false;
            if(!float.TryParse(parts[0], out var x)) return false;
            if(!float.TryParse(parts[1], out var y)) return false;
            value = new Vector2(x, y);
            return true;
        }
        
        // format accepted:
        // 1,2,3
        // (1,2,3)
        // (1, 2, 3)
        // regardless of the separator (comma, space, or both)
        public static bool TryParseToVector3(this string s, out Vector3 value)
        {
            value = default;
            if(s.StartsWith("Vector3")) s = s[7..];
            while(s.StartsWith("(") && s.EndsWith(")")) s = s[1..^1];
            var parts = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length != 3) return false;
            if(!float.TryParse(parts[0], out var x)) return false;
            if(!float.TryParse(parts[1], out var y)) return false;
            if(!float.TryParse(parts[2], out var z)) return false;
            value = new Vector3(x, y, z);
            return true;
        }
        
        // format accepted:
        // 1,2,3,4
        // (1,2,3,4)
        // (1, 2, 3, 4)
        // regardless of the separator (comma, space, or both)
        public static bool TryParseToVector4(this string s, out Vector4 value)
        {
            value = default;
            if(s.StartsWith("Vector4")) s = s[7..];
            while(s.StartsWith("(") && s.EndsWith(")")) s = s[1..^1];
            var parts = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length != 4) return false;
            if(!float.TryParse(parts[0], out var x)) return false;
            if(!float.TryParse(parts[1], out var y)) return false;
            if(!float.TryParse(parts[2], out var z)) return false;
            if(!float.TryParse(parts[3], out var w)) return false;
            value = new Vector4(x, y, z, w);
            return true;
        }
        
        
        // format accepted:
        // (0.5, 0.5, 1, 1)
        // (0.5, 0.5, 1)
        // 0.5, 0.5, 1, 1
        // 0.5, 0.5, 1
        // 07AABF
        // 700AAFFF
        // #007AAF
        // #007AAF0F
        // regardless of the separator (comma, space, or both)
        public static bool TryParseToColor(this string input, out Color value)
        {
            input = input.Trim();

            // Check for Hex format (with or without '#' symbol)
            if (input.StartsWith("#") || Prota.MethodExtensions.hexColorRegex.IsMatch(input) || Prota.MethodExtensions.hexColorWithAlphaRegex.IsMatch(input))
            {
                value = ParseHex(input.StartsWith("#") ? input.Substring(1) : input);
                return true;
            }
            
            // Tuple or CSV formats
            if(input.StartsWith("(") && input.EndsWith(")")) input = input[1..^1];
            var parts = input.Split(',');
            if (parts.Length >= 3 && parts.Length <= 4)
            {
                float r = float.Parse(parts[0].Trim());
                float g = float.Parse(parts[1].Trim());
                float b = float.Parse(parts[2].Trim());
                float a = parts.Length == 4 ? float.Parse(parts[3].Trim()) : 1f;
                value = new Color(r, g, b, a);
                return true;
            }

            value = default;
            return false;
        }
        
        static Color ParseHex(string hex)
        {
            if (hex.Length == 6 || hex.Length == 8)
            {
                float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                float a = hex.Length == 8
                    ? int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f
                    : 1f;
                return new Color(r, g, b, a);
            }

            throw new System.FormatException("Invalid hex color format");
        }
        
        
        /// <summary>
        /// 解析Sprite路径格式
        /// 路径格式: "Path/To/Texture[SpriteName]" 或 "Path/To/Texture"
        /// 当[SpriteName]不存在时, 默认使用Texture名称(不带扩展名)
        /// </summary>
        /// <param name="path">Sprite路径</param>
        /// <param name="extensionName">扩展名, 可以带有点号也可以不带.</param>
        /// <param name="texturePath">输出的纹理路径</param>
        /// <param name="spriteName">输出的Sprite名称</param>
        /// <returns>是否解析成功</returns>
        public static bool ParseSpritePath(this string path, string extensionName, out string texturePath, out string spriteName)
        {
            texturePath = null;
            spriteName = null;
            
            if(path.NullOrEmpty()) return false;
            
            if(extensionName.NullOrEmpty()) extensionName = "";
            else if(!extensionName.StartsWith(".")) extensionName = "." + extensionName;
            
            // 解析路径格式: "Path/To/Texture[SpriteName]"
            texturePath = path;
            
            var firstIndexOfBracket = path.IndexOf('[');
            var lastIndexOfBracket = path.LastIndexOf('[');
            var firstIndexOfBackBracket = path.IndexOf(']');
            var lastIndexOfBackBracket = path.LastIndexOf(']');
            
            // 没有找到括号.
            if(firstIndexOfBracket < 0 && firstIndexOfBackBracket < 0)
            {
                texturePath = path;
                spriteName = System.IO.Path.GetFileNameWithoutExtension(path);
                return true;
            }
            
            // 同类括号出现多次.
            if(firstIndexOfBackBracket != lastIndexOfBackBracket) return false;
            if(firstIndexOfBracket != lastIndexOfBracket) return false;
            
            // 有左括号没有右括号.
            if(firstIndexOfBracket < 0 && firstIndexOfBackBracket >= 0) return false;
            
            // 有右括号没有左括号.
            if(firstIndexOfBracket >= 0 && firstIndexOfBackBracket < 0) return false;
            
            texturePath = path[.. firstIndexOfBracket];
            if(firstIndexOfBackBracket == lastIndexOfBackBracket - 1)
            {
                spriteName = "";
            }
            else
            {
                spriteName = path[(firstIndexOfBracket + 1) .. lastIndexOfBackBracket];
            }
            
            return true;
        }

		public static bool ParseSpritePath(this string path, out string texturePath, out string spriteName)
		{
			return ParseSpritePath(path, ".png", out texturePath, out spriteName);
		}

		public static string AppendFrame(this string str)
		{
			return $"{Time.frameCount} :: {str}";
		}
		
		public static string AppendTime(this string str)
        {
            return $"{Time.time} :: {str}";
        }
    }
}
