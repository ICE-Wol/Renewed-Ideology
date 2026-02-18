using UnityEngine;

namespace Prota.Unity
{
	public static class ProtaMath
	{
		public static void EvenSplit(int total, int count, out int standardAmount, out int standardCount, out int plusOneCount)
		{
			standardAmount = total / count;
			plusOneCount = total - standardAmount * count;
			standardCount = count - plusOneCount;
		}
		
		public static Vector2 PointFromTarget(Vector2 from, Vector2 to, float distance)
		{
			if (from == to) return Vector2.zero;
			var dir = to.To(from).normalized;
			return to + dir * distance;
		}

		public static Vector2 PointFromSource(Vector2 from, Vector2 to, float distance)
		{
			if (from == to) return Vector2.zero;
			var dir = from.To(to).normalized;
			return from + dir * distance;
		}

		public static float Average(float a, float b) => (a + b) * 0.5f;
	}
}
