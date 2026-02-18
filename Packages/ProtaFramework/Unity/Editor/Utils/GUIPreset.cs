using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Editor
{
	public class GUIPreset
	{
		public sealed class GUIContentPresetAccessor
		{
			public delegate GUIContent Factory(string text);

			readonly Factory factory;
			readonly Dictionary<string, GUIContent> cache = new();

			public GUIContentPresetAccessor(Factory factory)
			{
				this.factory = factory;
			}

			public GUIContent this[string text]
			{
				get
				{
					if (cache.TryGetValue(text, out var content)) return content;
					content = factory(text);
					cache[text] = content;
					return content;
				}
			}
		}

		public sealed class GUIContentPresetAccessor2
		{
			public delegate GUIContent Factory(string text, string tooltip);

			readonly Factory factory;
			readonly Dictionary<ContentKey, GUIContent> cache = new();

			public GUIContentPresetAccessor2(Factory factory)
			{
				this.factory = factory;
			}

			public GUIContent this[string text, string tooltip]
			{
				get
				{
					var key = new ContentKey(text, tooltip);
					if (cache.TryGetValue(key, out var content)) return content;
					content = factory(text, tooltip);
					cache[key] = content;
					return content;
				}
			}

			struct ContentKey : IEquatable<ContentKey>
			{
				public readonly string text;
				public readonly string tooltip;

				public ContentKey(string text, string tooltip)
				{
					this.text = text;
					this.tooltip = tooltip;
				}

				public bool Equals(ContentKey other)
				{
					return text == other.text && tooltip == other.tooltip;
				}

				public override bool Equals(object obj)
				{
					return obj is ContentKey other && Equals(other);
				}

				public override int GetHashCode()
				{
					unchecked
					{
						var hash = text != null ? text.GetHashCode() : 0;
						hash = (hash * 397) ^ (tooltip != null ? tooltip.GetHashCode() : 0);
						return hash;
					}
				}
			}
		}

		public sealed class GUIPresetAccessor
		{
			public delegate GUILayoutOption[] Factory(int value);
			
			readonly Factory factory;
			
			readonly Dictionary<int, GUILayoutOption[]> cache = new();

			public GUIPresetAccessor(Factory factory)
			{
				this.factory = factory;
			}

			public GUILayoutOption[] this[int value]
			{
				get
				{
					if (cache.TryGetValue(value, out var options)) return options;
					options = factory(value);
					cache[value] = options;
					return options;
				}
			}
		}
		
		public sealed class GUIPresetAccessor2
		{
			public delegate GUILayoutOption[] Factory(int value1, int value2);
			
			readonly Factory factory;
			
			readonly Dictionary<Vector2Int, GUILayoutOption[]> cache = new();
			
			public GUIPresetAccessor2(Factory factory)
			{
				this.factory = factory;
			}

			public GUILayoutOption[] this[int value1, int value2]
			{
				get
				{
					if (cache.TryGetValue(new Vector2Int(value1, value2), out var options)) return options;
					options = factory(value1, value2);
					cache[new Vector2Int(value1, value2)] = options;
					return options;
				}
			}
		}
		
		public static readonly GUIPresetAccessor width = new GUIPresetAccessor(value => new GUILayoutOption[] { GUILayout.Width(value) });
		public static readonly GUIPresetAccessor height = new GUIPresetAccessor(value => new GUILayoutOption[] { GUILayout.Height(value) });
		public static readonly GUIPresetAccessor minWidth = new GUIPresetAccessor(value => new GUILayoutOption[] { GUILayout.MinWidth(value) });
		public static readonly GUIPresetAccessor minHeight = new GUIPresetAccessor(value => new GUILayoutOption[] { GUILayout.MinHeight(value) });
		
		public static readonly GUIPresetAccessor2 widthHeight = new GUIPresetAccessor2((value1, value2) => new GUILayoutOption[] { GUILayout.Width(value1), GUILayout.Height(value2) });
		public static readonly GUIPresetAccessor2 minWidthHeight = new GUIPresetAccessor2((value1, value2) => new GUILayoutOption[] { GUILayout.MinWidth(value1), GUILayout.MinHeight(value2) });
		
		public static readonly GUILayoutOption expendWidth = GUILayout.ExpandWidth(true);
		public static readonly GUILayoutOption expendHeight = GUILayout.ExpandHeight(true);


		public static readonly GUIContentPresetAccessor content = new GUIContentPresetAccessor(text => new GUIContent(text));
		public static readonly GUIContentPresetAccessor2 contentTooltip = new GUIContentPresetAccessor2((text, tooltip) => new GUIContent(text, tooltip));
	}
}
