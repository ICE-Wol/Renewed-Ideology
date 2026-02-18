using System.Runtime.InteropServices;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using MEC;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace _Scripts.Tools {
    public enum LerpType {
        Linear,
        SInFOutQuad,
        SInFOutCubic,
        FInSOutCubic,
        SInOutFMid,
        FInOutSMid,
        
        EaseInOutQuad,
    }
    public static class Calc {
        //if too big, the following traces will be a rectangle.
        private const float Epsilon = 0.00001f;
        
        public static float GetFunctionedValueOfT(float t, LerpType type) {
            if (type == LerpType.SInFOutQuad) {
                return t * t;
            }
            else if (type == LerpType.SInFOutCubic) {
                return t * t * t;
            }
            else if (type == LerpType.FInSOutCubic) {
                return Mathf.Pow(t, 1 / 3f);
            }
            if (type == LerpType.SInOutFMid) {
                return Mathf.Sin((t - 0.5f) * Mathf.PI) + 1;
            }
            else if (type == LerpType.FInOutSMid) {
                return Mathf.Asin(2 * t - 1f) / Mathf.PI + 0.5f;
            }
            else if (type == LerpType.Linear) {
                return t; // Linear interpolation
            }
            else if (type == LerpType.EaseInOutQuad) {
                return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2f;  
            }
            
            else {
                Debug.LogError("Unknown LerpType: " + type + "Fallback to linear interpolation");
                return t; // Fallback to linear interpolation
            }
        }
        public static float Lerp(float start, float end, float t, LerpType type) {
            return Mathf.Lerp(start, end, GetFunctionedValueOfT(t, type));
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float t, LerpType type) {
            return new Vector3(
                Lerp(start.x, end.x, t, type),
                Lerp(start.y, end.y, t, type),
                Lerp(start.z, end.z, t, type)
            );
        }

        public static float GetDirBetweenPosAndNeg180(this float dir) {
            if (dir > 180) {
                while(dir > 180) {
                    dir -= 360;
                }
            }else if (dir < -180) {
                while(dir < -180) {
                    dir += 360;
                }
            }
            return dir;
        }
        
        public static float GetPlayerDirection(Vector3 selfPos){
            return Vector2.SignedAngle(Vector2.right,PlayerCtrl.instance.transform.position - selfPos);
        }
        
        public static float GetDirection(Vector3 start, Vector3 end) {
            return Vector2.SignedAngle(Vector2.right, end - start);
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float t) {
            return new Vector3(
                Mathf.Lerp(start.x, end.x, t),
                Mathf.Lerp(start.y, end.y, t),
                Mathf.Lerp(start.z, end.z, t)
            );
        }
        
        public static Color LerpColorInRGB(Color start, Color end, float t) {
            return new Color(
                Mathf.Lerp(start.r, end.r, t),
                Mathf.Lerp(start.g, end.g, t),
                Mathf.Lerp(start.b, end.b, t),
                Mathf.Lerp(start.a, end.a, t)
            );
        }

        public static Color LerpColorInHSV(Color start, Color end, float t) {
            Color.RGBToHSV(start, out var startH, out var startS, out var startV);
            Color.RGBToHSV(end, out var endH, out var endS, out var endV);
            var h = Mathf.Lerp(startH, endH, t);
            var s = Mathf.Lerp(startS, endS, t);
            var v = Mathf.Lerp(startV, endV, t);
            return Color.HSVToRGB(h, s, v);
        }

        /// <summary>
        /// 计算二次贝塞尔曲线上某个 t 值对应的点
        /// </summary>
        /// <param name="p0">起点</param>
        /// <param name="p1">控制点</param>
        /// <param name="p2">终点</param>
        /// <param name="t">参数 t，取值范围[0,tscale]</param>
        /// <param name="tScale">缩放倍率，决定参数t取值范围</param>
        /// <returns>曲线上对应的点</returns>
        public static Vector3 GetQuadBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t,float tScale)
        {
            t /= tScale;
            float u = 1 - t;
            Vector3 point = u * u * p0 + 2 * u * t * p1 + t * t * p2;
            return point;
        }
        
        /// <summary>
        /// 计算三次贝塞尔曲线上某个 t 值对应的点
        /// </summary>
        /// <param name="p0">起点</param>
        /// <param name="p1">第一个控制点</param>
        /// <param name="p2">第二个控制点</param>
        /// <param name="p3">终点</param>
        /// <param name="t">参数 t，取值范围 [0, tScale]</param>
        /// <param name="tScale">缩放倍率，决定参数 t 取值范围</param>
        /// <returns>曲线上对应的点</returns>
        public static Vector3 GetCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float tScale)
        {
            t /= tScale;
            float u = 1 - t;
            Vector3 point = 
                u * u * u * p0 + 
                3 * u * u * t * p1 + 
                3 * u * t * t * p2 + 
                t * t * t * p3;
            return point;
        }

        /// <summary>
        /// 圆上按索引 i 取点。angle = startAngle + i * step（度），返回 (radius * cos(angle), radius * sin(angle), 0)。
        /// </summary>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">起始角度（度，0 为右，逆时针为正）</param>
        /// <param name="step">每步角度（度）</param>
        /// <param name="i">索引</param>
        /// <param name="centered">若为 true 且调用带 count 的重载，则围绕 startAngle 对称分布</param>
        public static Vector3 PositionOnCircle(float radius, float startAngle, float step, int i, bool centered)
        {
            if (!centered)
            {
                float angleDeg = startAngle + i * step;
                return new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angleDeg), radius * Mathf.Sin(Mathf.Deg2Rad * angleDeg), 0f);
            }
            // centered 在无 count 时按“不居中”处理
            float a = startAngle + i * step;
            return new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * a), radius * Mathf.Sin(Mathf.Deg2Rad * a), 0f);
        }

        /// <summary>圆上取点，不居中。angle = startAngle + i * step</summary>
        public static Vector3 PositionOnCircle(float radius, float startAngle, float step, int i)
        {
            return PositionOnCircle(radius, startAngle, step, i, false);
        }

        /// <summary>以 origin 为圆心的圆上取点，angle = startAngle + i * step</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, float startAngle, float step, int i, bool centered = false)
        {
            var v = PositionOnCircle(radius, startAngle, step, i, centered);
            return new Vector3(origin.x + v.x, origin.y + v.y, 0f);
        }

        /// <summary>
        /// 圆上取点，共 count 个点。step = 360f / count。centered 时围绕 startAngle 对称分布。
        /// </summary>
        public static Vector3 PositionOnCircle(float radius, float startAngle, float step, int i, int count, bool centered)
        {
            float angleDeg = centered
                ? startAngle + (i - (count - 1) * 0.5f) * step
                : startAngle + i * step;
            return new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angleDeg), radius * Mathf.Sin(Mathf.Deg2Rad * angleDeg), 0f);
        }

        /// <summary>以 origin 为圆心的圆上取点，共 count 个点，step = 360f/count</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, float startAngle, float step, int i, int count, bool centered)
        {
            var v = PositionOnCircle(radius, startAngle, step, i, count, centered);
            return new Vector3(origin.x + v.x, origin.y + v.y, 0f);
        }

        /// <summary>圆上均匀 count 个点，step = 360f/count，不居中</summary>
        public static Vector3 PositionOnCircle(float radius, float startAngle, int i, int count)
        {
            return PositionOnCircle(radius, startAngle, 360f / count, i, count, false);
        }

        /// <summary>圆上均匀 count 个点，step = 360f/count，可选居中</summary>
        public static Vector3 PositionOnCircle(float radius, float startAngle, int i, int count, bool centered)
        {
            return PositionOnCircle(radius, startAngle, 360f / count, i, count, centered);
        }

        /// <summary>以 origin 为圆心的圆上均匀 count 个点，step = 360f/count</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, float startAngle, int i, int count, bool centered = false)
        {
            var v = PositionOnCircle(radius, startAngle, i, count, centered);
            return new Vector3(origin.x + v.x, origin.y + v.y, 0f);
        }

        /// <summary>
        /// 圆上在 [startAngle, endAngle] 之间均匀取 count 个点，返回第 i 个点。
        /// i=0 对应 startAngle，i=count-1 对应 endAngle。
        /// </summary>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">起始角度（度）</param>
        /// <param name="endAngle">结束角度（度）</param>
        /// <param name="i">索引 [0, count-1]</param>
        /// <param name="count">总点数（至少为 1）</param>
        public static Vector3 PositionOnCircle(float radius, float startAngle, float endAngle, int i, int count)
        {
            float step = count <= 1 ? 0f : (endAngle - startAngle) / (count - 1);
            float angleDeg = startAngle + i * step;
            return new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angleDeg), radius * Mathf.Sin(Mathf.Deg2Rad * angleDeg), 0f);
        }

        /// <summary>以 origin 为圆心的圆上 [startAngle, endAngle] 均匀 count 个点，第 i 个点</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, float startAngle, float endAngle, int i, int count)
        {
            var v = PositionOnCircle(radius, startAngle, endAngle, i, count);
            return new Vector3(origin.x + v.x, origin.y + v.y, 0f);
        }

        /// <summary>圆上均匀 count 个点，起始角 0°，不居中</summary>
        public static Vector3 PositionOnCircle(float radius, int i, int count)
        {
            return PositionOnCircle(radius, 0f, 360f / count, i, count, false);
        }

        /// <summary>圆上均匀 count 个点，起始角 0°，可选居中</summary>
        public static Vector3 PositionOnCircle(float radius, int i, int count, bool centered)
        {
            return PositionOnCircle(radius, 0f, 360f / count, i, count, centered);
        }

        /// <summary>以 origin 为圆心的圆上均匀 count 个点，起始角 0°</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, int i, int count, bool centered = false)
        {
            var v = PositionOnCircle(radius, i, count, centered);
            return new Vector3(origin.x + v.x, origin.y + v.y, 0f);
        }

        /// <summary>圆上按角度取点，angle 为度，0 为右，逆时针为正</summary>
        public static Vector3 PositionOnCircle(float radius, float angle)
        {
            return new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        }

        /// <summary>以 origin 为圆心的圆上按角度取点</summary>
        public static Vector3 PositionOnCircle(Vector2 origin, float radius, float angle)
        {
            return new Vector3(origin.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad), origin.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        }

        /// <summary>返回 Vector2 的圆上取点（同上逻辑，仅返回类型不同）</summary>
        public static Vector2 PositionOnCircleV2(float radius, float startAngle, float step, int i, bool centered = false)
        {
            Vector3 v = PositionOnCircle(radius, startAngle, step, i, centered);
            return new Vector2(v.x, v.y);
        }

        public static Vector2 PositionOnCircleV2(float radius, float startAngle, float step, int i)
        {
            return PositionOnCircleV2(radius, startAngle, step, i, false);
        }

        /// <summary>以 origin 为圆心的圆上取点，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(Vector2 origin, float radius, float startAngle, float step, int i, bool centered = false)
        {
            return origin + PositionOnCircleV2(radius, startAngle, step, i, centered);
        }

        public static Vector2 PositionOnCircleV2(float radius, float startAngle, int i, int count, bool centered = false)
        {
            Vector3 v = PositionOnCircle(radius, startAngle, i, count, centered);
            return new Vector2(v.x, v.y);
        }

        /// <summary>以 origin 为圆心的圆上取点，共 count 个点，step = 360f/count，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(Vector2 origin, float radius, float startAngle, int i, int count, bool centered = false)
        {
            return origin + PositionOnCircleV2(radius, startAngle, i, count, centered);
        }

        /// <summary>圆上 [startAngle, endAngle] 均匀 count 个点，第 i 个点，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(float radius, float startAngle, float endAngle, int i, int count)
        {
            Vector3 v = PositionOnCircle(radius, startAngle, endAngle, i, count);
            return new Vector2(v.x, v.y);
        }

        /// <summary>以 origin 为圆心的圆上 [startAngle, endAngle] 均匀 count 个点，第 i 个点，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(Vector2 origin, float radius, float startAngle, float endAngle, int i, int count)
        {
            return origin + PositionOnCircleV2(radius, startAngle, endAngle, i, count);
        }

        /// <summary>圆上按角度取点，angle 为度，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(float radius, float angle)
        {
            return new Vector2(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        /// <summary>以 origin 为圆心的圆上按角度取点，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(Vector2 origin, float radius, float angle)
        {
            return origin + new Vector2(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        /// <summary>以 origin 为圆心的圆上均匀 count 个点，起始角 0°，返回 Vector2</summary>
        public static Vector2 PositionOnCircleV2(Vector2 origin, float radius, int i, int count, bool centered = false)
        {
            return origin + PositionOnCircleV2(radius, 0f, i, count, centered);
        }

        public static float WaitForFrames(int frames) {
            var t = Timing.RunCoroutine(GameManager.WaitForFrames(frames));
            return Timing.WaitUntilDone(t);
        }

        public static BulletMovement GenerateBullet(BulletMovement bullet, Vector3 pos, float dir) {
            var b = Object.Instantiate(bullet, GameManager.Manager.bulletSortingGroup);
            b.transform.position = pos;
            b.transform.rotation = dir.EulerZ();
            b.direction = dir;
            b.GetComponent<State>().initialRotation = dir;
            return b;
        }

        public static Quaternion EulerZ(this float z) => Quaternion.Euler(0, 0, z);
        
        public static bool Equal(this float argument1, float argument2) {
            return Mathf.Abs(argument1 - argument2) <= Epsilon;
        }
        public static bool Equal(this float argument1, float argument2, float epsilon) {
            return Mathf.Abs(argument1 - argument2) <= epsilon;
        }
        
        public static bool Equal(this Vector3 argument1, Vector3 argument2) {
            return Equal(argument1.x, argument2.x) &&
                   Equal(argument1.y, argument2.y) &&
                   Equal(argument1.z, argument2.z);
        }
        public static bool Equal(this Vector3 argument1, Vector3 argument2, float epsilon) {
            return Equal(argument1.x, argument2.x, epsilon) &&
                   Equal(argument1.y, argument2.y, epsilon) &&
                   Equal(argument1.z, argument2.z, epsilon);
        }

        public static Vector2 Deg2Dir(this float degree) {
            return new Vector2(Mathf.Cos(Mathf.Deg2Rad * degree), Mathf.Sin(Mathf.Deg2Rad * degree));
        }
        
        public static Vector3 Deg2Dir3(this float degree) {
            return new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 
                               Mathf.Sin(Mathf.Deg2Rad * degree),
                               0f);
        }
        
        
        public static Vector2 Deg2Dir(this int degree) {
            return new Vector2(Mathf.Cos(Mathf.Deg2Rad * degree), Mathf.Sin(Mathf.Deg2Rad * degree));
        }
        
        public static Vector3 Deg2Dir3(this int degree) {
            return new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 
                Mathf.Sin(Mathf.Deg2Rad * degree),
                0f);
        }
        
        public static Vector3 GetRandomVectorCircle(float start, float end, float rad) {
            var randDegree = Random.Range(start, end);
            var randRadius = Random.Range(0, rad);
            var offset = randDegree.Deg2Dir3() * randRadius;
            return offset;
        }

        public static Vector2 GetRandomVectorNormalized(float start, float end) {
            return GetRandomVectorCircle(start, end, 1).normalized;
        }
        
        public static Vector3 GetRandomVectorRing(float start, float end, float innerRad, float outerRad) {
            var randDegree = Random.Range(start, end);
            var randRadius = Random.Range(innerRad, outerRad);
            var offset = randDegree.Deg2Dir3() * randRadius;
            return offset;
        }
        
        /// <summary>
        /// A function which approach the current value to the target value, the closer the slower.
        /// </summary>
        /// <param name="current">Value type, the current value which is approaching the target value.</param>
        /// <param name="target">the final destination of the approach process.</param>
        /// <param name="rate">the rate of approach process, the bigger the slower.</param>
        /// <returns></returns>
        public static float ApproachValue(this float current, float target, float rate) {
            if (rate <= 0f) {
                throw new System.ArgumentException("Rate should be bigger than 0.");
            }
            if (Mathf.Abs(current - target) >= Epsilon) {
                current -= (current - target) / rate;
            }
            else {
                current = target;
            }

            return current;
        }
        
        public static float ApproachValue(this float current, float target, float rate, float epsilon) {
            if (rate <= 0f) {
                throw new System.ArgumentException("Rate should be bigger than 0.");
            }
            if (Mathf.Abs(current - target) >= epsilon) {
                current -= (current - target) / rate;
            }
            else {
                current = target;
            }

            return current;
        }
        
        public static Color ApproachValue(this Color current, Color target, float rate) {
            current.r = ApproachValue(current.r, target.r, rate);
            current.g = ApproachValue(current.g, target.g, rate);
            current.b = ApproachValue(current.b, target.b, rate);
            current.a = ApproachValue(current.a, target.a, rate);
            return current;
        }
        
        public static Color ApproachValue(this Color current, Color target, Vector4 rate) {
            current.r = ApproachValue(current.r, target.r, rate.x);
            current.g = ApproachValue(current.g, target.g, rate.y);
            current.b = ApproachValue(current.b, target.b, rate.z);
            current.a = ApproachValue(current.a, target.a, rate.w);
            return current;
        }
        
        public static Vector3 ApproachValue(this Vector3 current, Vector3 target, Vector3 rate) {
            current.x = ApproachValue(current.x, target.x, rate.x);
            current.y = ApproachValue(current.y, target.y, rate.y);
            current.z = ApproachValue(current.z, target.z, rate.z);
            return current;
        }
        
        public static Vector3 ApproachValue(this Vector3 current, Vector3 target, float rate) {
            current.x = ApproachValue(current.x, target.x, rate);
            current.y = ApproachValue(current.y, target.y, rate);
            current.z = ApproachValue(current.z, target.z, rate);
            return current;
        }

        public static Vector3 ApproachValue(this Vector3 current, Vector3 target, Vector3 rate, float epsilon) {
            current.x = ApproachValue(current.x, target.x, rate.x, epsilon);
            current.y = ApproachValue(current.y, target.y, rate.y, epsilon);
            current.z = ApproachValue(current.z, target.z, rate.z, epsilon);
            return current;
        }
        
        public static Vector2 ApproachValue(this Vector2 current, Vector2 target, Vector2 rate) {
            current.x = ApproachValue(current.x, target.x, rate.x);
            current.y = ApproachValue(current.y, target.y, rate.y);
            return current;
        }
        public static Vector2 ApproachValue(this Vector2 current, Vector2 target, float rate) {
            current.x = ApproachValue(current.x, target.x, rate);
            current.y = ApproachValue(current.y, target.y, rate);
            return current;
        }
        
        public static float ApproachRef(this ref float current, float target, float rate) {
            return current = ApproachValue(current, target, rate);
        }
        
        public static float ApproachRef(this ref float current, float target, float rate, float epsilon) {
            return current = ApproachValue(current, target, rate, epsilon);
        }
        
        public static Vector3 ApproachRef(this ref Vector3 current, Vector3 target, Vector3 rate) {
            current.x = ApproachValue(current.x, target.x, rate.x);
            current.y = ApproachValue(current.y, target.y, rate.y);
            current.z = ApproachValue(current.z, target.z, rate.z);
            return current;
        }
        
        public static Vector3 ApproachRef(this ref Vector3 current, Vector3 target, float rate) {
            current.x = ApproachValue(current.x, target.x, rate);
            current.y = ApproachValue(current.y, target.y, rate);
            current.z = ApproachValue(current.z, target.z, rate);
            return current;
        }

        public static Vector3 ApproachRef(this ref Vector3 current, Vector3 target, Vector3 rate, float epsilon) {
            current.x = ApproachValue(current.x, target.x, rate.x, epsilon);
            current.y = ApproachValue(current.y, target.y, rate.y, epsilon);
            current.z = ApproachValue(current.z, target.z, rate.z, epsilon);
            return current;
        }
        
        public static Color Fade(this Color current, float rate) {
            current.a = ApproachValue(current.a, 0, rate);
            return current;
        }

        public static Color Appear(this Color current, float rate) {
            current.a = ApproachValue(current.a, 1, rate);
            return current;
        }

        public static Color SetAlpha(this Color current, float alpha) {
            if (alpha > 1f) alpha = 1f;
            if (alpha < 0f) alpha = 0f;
            current.a = alpha;
            return current;
        }
        
        #region SetVector
        
        public static Vector3 SetX(this Vector3 v3, float x) {
            return new Vector3(x, v3.y, v3.z);
        }
        public static Vector3 SetY(this Vector3 v3, float y) {
            return new Vector3(v3.x, y, v3.z);
        }
        public static Vector3 SetZ(this Vector3 v3, float z) {
            return new Vector3(v3.x, v3.y, z);
        }

        public static Vector3 SetXY(this Vector3 v3, Vector3 i) {
            return new Vector3(i.x, i.y, v3.z);
        }
        
        
        public static Vector2 SetX(this Vector2 v2, float x) {
            return new Vector2(x, v2.y);
        }
        public static Vector2 SetY(this Vector2 v2, float y) {
            return new Vector2(v2.x, y);
        }
        
        public static bool IsOutOfField(this Vector3 pos) {
            return Mathf.Abs(pos.x) > 4.2f || pos.y < -4.4f || pos.y > 4.7f;
        }
        
        #endregion

        public static float WaitForOneFrame(int i) {
            throw new System.NotImplementedException();
        }
    }
}
