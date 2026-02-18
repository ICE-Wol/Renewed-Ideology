
using UnityEngine;
using Prota.Unity;
using System.IO;
using System;

namespace Prota.Unity
{
    public static partial class ProtaDebug
    {
        static object filelock = new object();
        public static void Logf(this string message)
        {
            lock(filelock)
            {
                File.AppendAllText("log.txt", $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static void DrawBox2D(Vector3 pos, Vector2 size, float rotation, Color? color = null, float? duration = null)
        {
            color ??= Color.green;
            duration ??= 0;
            var halfSize = size.Abs() / 2;
            var bottomLeft = -halfSize;
            var topRight = halfSize;
            var bottomRight = new Vector2(halfSize.x, -halfSize.y);
            var topLeft = new Vector2(-halfSize.x, halfSize.y);
            var bottomLeftRotated = pos + bottomLeft.Rotate(rotation).ToVec3();
            var topRightRotated = pos + topRight.Rotate(rotation).ToVec3();
            var bottomRightRotated = pos + bottomRight.Rotate(rotation).ToVec3();
            var topLeftRotated = pos + topLeft.Rotate(rotation).ToVec3();
            Debug.DrawLine(bottomLeftRotated, bottomRightRotated, color.Value, duration.Value);
            Debug.DrawLine(bottomRightRotated, topRightRotated, color.Value, duration.Value);
            Debug.DrawLine(topRightRotated, topLeftRotated, color.Value, duration.Value);
            Debug.DrawLine(topLeftRotated, bottomLeftRotated, color.Value, duration.Value);
        }
        
        public static void DrawCross(Vector3 pos, float size, Color? color = null, float? duration = null)
        {
            color ??= Color.green;
            duration ??= 0;
            var halfSize = size / 2;
            Debug.DrawLine(pos + new Vector3(-halfSize, -halfSize, 0), pos + new Vector3(halfSize, halfSize, 0), color.Value, duration.Value);
            Debug.DrawLine(pos + new Vector3(halfSize, -halfSize, 0), pos + new Vector3(-halfSize, halfSize, 0), color.Value, duration.Value);
        }
        
        
        public static void DrawArrow(Vector3 from, Vector3 to, Color? _color = null, float? _duration = null, float? _size = null)
        {
            var color = _color ?? Color.green;
            var duration = _duration ?? 0;
            var size = _size ?? 0.1f;
            Debug.DrawLine(from, to, color, duration);
            var arrowSide = to.To(from).normalized.ToVec2() * size;
            Debug.DrawLine(to, to + arrowSide.Rotate(30).ToVec3(), color, duration);
            Debug.DrawLine(to, to + arrowSide.Rotate(-30).ToVec3(), color, duration);
        }
        
        public static void DrawGradientLine(Vector3 from, Vector3 to, int segmentCount, Color colorFrom, Color colorTo, float? _duration = null)
        {
            var duration = _duration ?? 0;
            if(segmentCount <= 0)
            {
                DrawDirLine(from, to, colorFrom, duration);
                return;
            }
            var segment = (to - from) / segmentCount;
            for(int i = 0; i < segmentCount; i++)
            {
                var color = Color.Lerp(colorFrom, colorTo, i / (segmentCount - 1));
                Debug.DrawLine(from + segment * i, from + segment * (i + 1), color, duration);
            }
        }
        
        public static void DrawDirLine(Vector3 from, Vector3 to, Color? color = null, float? duration = null)
        {
            color ??= Color.green;
            duration ??= 0;
            
            const int segment = 10;
            float a = 0.5f;
            Debug.DrawLine(from, (from, to).Lerp(a));
            for(int i = 0; i < segment; i++)
            {
                var f = a;
                a = 1 - (1 - a) * 0.5f;
                Debug.DrawLine((from, to).Lerp((f + a) / 2), (from, to).Lerp(a), color.Value, duration.Value);
            }
        }
        
        public static void DrawCircle(Vector3 center, float radius, Color? color = null, float? duration = null, int segment = 32)
        {
            color ??= Color.green;
            duration ??= 0;
            float segStep = 360f / segment;
            for(int i = 0; i < segment; i++)
            {
                var a = i * segStep * Mathf.Deg2Rad;
                var b = (i + 1) * segStep * Mathf.Deg2Rad;
                var p1 = center + new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius;
                var p2 = center + new Vector3(Mathf.Cos(b), Mathf.Sin(b)) * radius;
                Debug.DrawLine(p1, p2, color.Value, duration.Value);
            }
        }
        
        public static void DrawArc(Vector3 center, float radius, float centerAngle, float angleRange, Color? color = null, float? duration = null, int segment = 32)
        {
            var cc = center.ToVec2();
            color ??= Color.green;
            duration ??= 0;
            float segStep = angleRange / segment;
            var fromAngle = (centerAngle - angleRange / 2).NormalizeAngle360();
            var toAngle = (centerAngle + angleRange / 2).NormalizeAngle360();
            for(int i = 0; i < segment; i++)
            {
                var a = fromAngle + i * segStep;
                var b = fromAngle + (i + 1) * segStep;
                Debug.DrawLine(cc + a.AngleToVec2() * radius, cc + b.AngleToVec2() * radius, color.Value, duration.Value);
            }
            Debug.DrawLine(cc, cc + fromAngle.AngleToVec2() * radius, color.Value, duration.Value);
            Debug.DrawLine(cc, cc + toAngle.AngleToVec2() * radius, color.Value, duration.Value);
        }
        
        public static void DrawDashLine(Vector3 from, Vector3 to, Color? color1 = null, Color? color2 = null, float? duration = null, float dashLength = 0.1f)
        {
            color1 ??= Color.green;
            color2 ??= Color.green;
            duration ??= 0;
            var dir = to - from;
            var length = dir.magnitude;
            var dashCount = Mathf.FloorToInt(length / dashLength);
            var dashDir = dir.normalized * dashLength;
            for(int i = 0; i < dashCount; i++)
            {
                var p1 = from + dashDir * i;
                var p2 = from + dashDir * (i + 1);
                Debug.DrawLine(p1, p2, i % 2 == 0 ? color1.Value : color2.Value, duration.Value);
            }
        }
        
        public static void DrawText(Vector3 position, string text, float scale, Color? color = null, float? duration = null)
        {
            if(!Application.isPlaying) return;
            var cc = color ?? Color.green;
            var dd = (duration ?? 0f).Max(0.01f);
            var g = new GameObject("DebugText");
            GameObject.DontDestroyOnLoad(g);
            g.transform.position = position;
            g.transform.localScale = Vector3.one * scale;
            var textMesh = g.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.color = cc;
            textMesh.characterSize = 1f;
            textMesh.fontSize = 16;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.fontStyle = FontStyle.Bold;
            textMesh.color = cc;
            var ds = g.AddComponent<DestroyAfter>();
            ds.delay = dd;
            ds.destroyAfterEvent = DestroyAfter.DestroyAfterEvent.Start;
        }
    }
}
