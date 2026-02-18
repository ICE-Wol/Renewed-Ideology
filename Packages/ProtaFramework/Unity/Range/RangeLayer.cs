using System;
using System.Collections.Generic;

namespace Prota.Unity
{

	[Serializable]
	public partial struct RangeLayer : IEquatable<RangeLayer>
	{
		public int id;
		
		public RangeLayer(int id)
		{
			this.id = id;
		}
		
		/// <summary>
		/// 新项目需要新在 [InitializeOnLoadMethod] 或 [RuntimeInitializeOnLoadMethod] 中
		/// 填写这个 dictionary, 注册可用的 Range Layer.
		/// </summary>
		/// <returns></returns>
		public static readonly Dictionary<string, RangeLayer> nameToLayer = new();
		
		public static RangeLayer None => new(0);

		public override bool Equals(object obj) => obj is RangeLayer other && Equals(other);
		public bool Equals(RangeLayer other) => id == other.id;
		public override int GetHashCode() => id;
		public static bool operator ==(RangeLayer left, RangeLayer right) => left.id == right.id;
		public static bool operator !=(RangeLayer left, RangeLayer right) => left.id != right.id;
		public static bool operator <(RangeLayer left, RangeLayer right) => left.id < right.id;
		public static bool operator >(RangeLayer left, RangeLayer right) => left.id > right.id;
		public static bool operator <=(RangeLayer left, RangeLayer right) => left.id <= right.id;
		public static bool operator >=(RangeLayer left, RangeLayer right) => left.id >= right.id;
		
		public override string ToString()
		{
			foreach (var kv in nameToLayer)
			{
				if (kv.Value.id == id) return kv.Key;
			}
			return $"Unknown({id})";
		}
	}
}
