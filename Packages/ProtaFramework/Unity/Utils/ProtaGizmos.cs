using UnityEngine;
using Prota.Unity;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prota.Unity
{
    public static partial class ProtaGizmos
    {
        // Helper to restore color
        public struct ColorScope : System.IDisposable
        {
            Color original;
            bool changed;
            public ColorScope(Color? color)
            {
                if (color.HasValue)
                {
                    original = Gizmos.color;
                    Gizmos.color = color.Value;
                    changed = true;
                }
                else
                {
                    original = Color.white; // dummy
                    changed = false;
                }
            }
            public void Dispose()
            {
                if (changed) Gizmos.color = original;
            }
        }

        public static void DrawBox2D(Vector3 pos, Vector2 size, float rotation, Color? color = null)
        {
            using(new ColorScope(color))
            {
                var halfSize = size.Abs() / 2;
                var bottomLeft = -halfSize;
                var topRight = halfSize;
                var bottomRight = new Vector2(halfSize.x, -halfSize.y);
                var topLeft = new Vector2(-halfSize.x, halfSize.y);
                var bottomLeftRotated = pos + bottomLeft.Rotate(rotation).ToVec3();
                var topRightRotated = pos + topRight.Rotate(rotation).ToVec3();
                var bottomRightRotated = pos + bottomRight.Rotate(rotation).ToVec3();
                var topLeftRotated = pos + topLeft.Rotate(rotation).ToVec3();
                Gizmos.DrawLine(bottomLeftRotated, bottomRightRotated);
                Gizmos.DrawLine(bottomRightRotated, topRightRotated);
                Gizmos.DrawLine(topRightRotated, topLeftRotated);
                Gizmos.DrawLine(topLeftRotated, bottomLeftRotated);
            }
        }
        
        public static void DrawCross(Vector3 pos, float size, Color? color = null)
        {
            using(new ColorScope(color))
            {
                var halfSize = size / 2;
                Gizmos.DrawLine(pos + new Vector3(-halfSize, -halfSize, 0), pos + new Vector3(halfSize, halfSize, 0));
                Gizmos.DrawLine(pos + new Vector3(halfSize, -halfSize, 0), pos + new Vector3(-halfSize, halfSize, 0));
            }
        }
        
        
        public static void DrawArrow(Vector3 from, Vector3 to, Color? color = null, float? _size = null)
        {
            using(new ColorScope(color))
            {
                var size = _size ?? 0.1f;
                Gizmos.DrawLine(from, to);
                var arrowSide = to.To(from).normalized.ToVec2() * size;
                Gizmos.DrawLine(to, to + arrowSide.Rotate(30).ToVec3());
                Gizmos.DrawLine(to, to + arrowSide.Rotate(-30).ToVec3());
            }
        }
        
        public static void DrawGradientLine(Vector3 from, Vector3 to, int segmentCount, Color colorFrom, Color colorTo)
        {
            if(segmentCount <= 0)
            {
                DrawDirLine(from, to, colorFrom);
                return;
            }
            
            var originalColor = Gizmos.color;
            
            var segment = (to - from) / segmentCount;
            for(int i = 0; i < segmentCount; i++)
            {
                var color = Color.Lerp(colorFrom, colorTo, (float)i / (segmentCount - 1));
                Gizmos.color = color;
                Gizmos.DrawLine(from + segment * i, from + segment * (i + 1));
            }
            
            Gizmos.color = originalColor;
        }
        
        public static void DrawDirLine(Vector3 from, Vector3 to, Color? color = null)
        {
            using(new ColorScope(color))
            {
                const int segment = 10;
                float a = 0.5f;
                Gizmos.DrawLine(from, (from, to).Lerp(a));
                for(int i = 0; i < segment; i++)
                {
                    var f = a;
                    a = 1 - (1 - a) * 0.5f;
                    Gizmos.DrawLine((from, to).Lerp((f + a) / 2), (from, to).Lerp(a));
                }
            }
        }
        
        public static void DrawCircle(Vector3 center, float radius, Color? color = null, int segment = 32)
        {
            using(new ColorScope(color))
            {
                float segStep = 360f / segment;
                for(int i = 0; i < segment; i++)
                {
                    var a = i * segStep * Mathf.Deg2Rad;
                    var b = (i + 1) * segStep * Mathf.Deg2Rad;
                    var p1 = center + new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius;
                    var p2 = center + new Vector3(Mathf.Cos(b), Mathf.Sin(b)) * radius;
                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
        
        public static void DrawArc(Vector3 center, float radius, float centerAngle, float angleRange, Color? color = null, int segment = 32)
        {
            using(new ColorScope(color))
            {
                var cc = center.ToVec2();
                float segStep = angleRange / segment;
                var fromAngle = (centerAngle - angleRange / 2).NormalizeAngle360();
                var toAngle = (centerAngle + angleRange / 2).NormalizeAngle360();
                for(int i = 0; i < segment; i++)
                {
                    var a = fromAngle + i * segStep;
                    var b = fromAngle + (i + 1) * segStep;
                    Gizmos.DrawLine(cc + a.AngleToVec2() * radius, cc + b.AngleToVec2() * radius);
                }
                Gizmos.DrawLine(cc, cc + fromAngle.AngleToVec2() * radius);
                Gizmos.DrawLine(cc, cc + toAngle.AngleToVec2() * radius);
            }
        }
        
        public static void DrawDashLine(Vector3 from, Vector3 to, Color? color1 = null, Color? color2 = null, float dashLength = 0.1f)
        {
            var c1 = color1 ?? Gizmos.color;
            var c2 = color2 ?? Gizmos.color;
            var originalColor = Gizmos.color;
            
            var dir = to - from;
            var length = dir.magnitude;
            var dashCount = Mathf.FloorToInt(length / dashLength);
            var dashDir = dir.normalized * dashLength;
            for(int i = 0; i < dashCount; i++)
            {
                var p1 = from + dashDir * i;
                var p2 = from + dashDir * (i + 1);
                Gizmos.color = i % 2 == 0 ? c1 : c2;
                Gizmos.DrawLine(p1, p2);
            }
            Gizmos.color = originalColor;
        }
        
        public static void DrawText(Vector3 position, string text, Color? color = null)
        {
#if UNITY_EDITOR
            var c = color ?? Gizmos.color;
            var style = new GUIStyle();
            style.normal.textColor = c;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 12; 
            Handles.Label(position, text, style);
#endif
        }
    }
}
