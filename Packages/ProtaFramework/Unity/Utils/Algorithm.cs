using System.Collections.Generic;
using UnityEngine;
using Prota;
using System.Linq;

namespace Prota.Unity
{
    public static partial class Algorithm
    {
        public static List<Vector2> RemoveStraightVertex(Vector2[] vertices, float tolerance = 0.5f)
        {
            if (vertices == null || vertices.Length < 3)
                return vertices.ToList();

            var result = new List<Vector2>();
            int count = vertices.Length;
            
            var prev = vertices[count - 1];
            for (int i = 0; i < count; i++)
            {
                var curr = vertices[i];
                var next = vertices[(i + 1) % count];

                // Use cross product to check for collinearity
                Vector2 dir1 = prev.To(curr).normalized;
                Vector2 dir2 = curr.To(next).normalized;
                float angle = Vector2.Angle(dir1, dir2);
                Debug.Log($"{i}: {prev}, {curr}, {next}, {angle}");
                if (angle > tolerance)
                {
                    result.Add(curr);
                    prev = curr;
                    Debug.Log($"add {i}: {curr} : {result.Count - 1}");
                }
            }

            return result;
        }
    }
}
