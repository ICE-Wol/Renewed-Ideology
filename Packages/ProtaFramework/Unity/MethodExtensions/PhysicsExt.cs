using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
		// ============================================================================================================
        // Physics Operation
        // ============================================================================================================
        
		public static Rigidbody2D RandomVelocityOnCircle(this Rigidbody2D rd, float speed)
		{
			var angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
			return rd;
		}
		
		public static Rigidbody2D RandomVelocityInCircle(this Rigidbody2D rd, float speed)
		{
			var angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
			var radius = UnityEngine.Random.Range(0f, speed);
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
			return rd;
		}
		
		public static Rigidbody2D RandomVelocityOnFan(this Rigidbody2D rd, float speed, float angleLeft, float angleRight)
		{
			if(speed < 0) throw new ArgumentException("speed must be positive");
			if(angleLeft > angleRight) throw new ArgumentException("angleLeft must be less than angleRight");
			var angle = UnityEngine.Random.Range(angleLeft, angleRight) * Mathf.Deg2Rad;
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
			return rd;
		}

		public static Rigidbody2D RandomVelocityInFan(this Rigidbody2D rd, float speed, float angleLeft, float angleRight)
		{
			if(speed < 0) throw new ArgumentException("speed must be positive");
			if(angleLeft > angleRight) throw new ArgumentException("angleLeft must be less than angleRight");
			var angle = UnityEngine.Random.Range(angleLeft, angleRight) * Mathf.Deg2Rad;
			var radius = UnityEngine.Random.Range(0f, speed);
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
			return rd;
		}
		
		public static Rigidbody2D RandomVelocityInCircle(this Rigidbody2D rd, float minSpeed, float maxSpeed)
		{
			if (minSpeed < 0 || maxSpeed < 0) throw new ArgumentException("Speeds must be positive");
			if (minSpeed > maxSpeed) throw new ArgumentException("minSpeed must be less than or equal to maxSpeed");

			var angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
			var speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
			return rd;
		}

		public static Rigidbody2D RandomVelocityInFan(this Rigidbody2D rd, float minSpeed, float maxSpeed, float angleLeft, float angleRight)
		{
			if (minSpeed < 0 || maxSpeed < 0) throw new ArgumentException("Speeds must be positive");
			if (minSpeed > maxSpeed) throw new ArgumentException("minSpeed must be less than or equal to maxSpeed");
			if (angleLeft > angleRight) throw new ArgumentException("angleLeft must be less than angleRight");

			var angle = UnityEngine.Random.Range(angleLeft, angleRight) * Mathf.Deg2Rad;
			var speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
			rd.linearVelocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
			return rd;
		}
		
		
        // ============================================================================================================
        // Physics Material
        // ============================================================================================================
        
        public static PhysicsMaterial2D Clone(this PhysicsMaterial2D mat)
        {
            var res = new PhysicsMaterial2D();
            res.bounciness = mat.bounciness;
            res.friction = mat.friction;
            res.name = mat.name;
            res.frictionCombine = mat.frictionCombine;
            return res;
        }
        
        // ============================================================================================================
        // Consts
        // ============================================================================================================
        
        const float collisionSizeReduction = 1f / 256;
        
        static RaycastHit2D[] rayCastBuffer = new RaycastHit2D[128];
        
        static readonly LayerMask maskOfAll = (LayerMask)(-1);
        
        // ============================================================================================================
        // MoveAndCollide
        // ============================================================================================================
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, maskOfAll);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, LayerMask layer)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, layer);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime)
        {
            return MoveAndCollide(c, velocity, deltaTime, maskOfAll);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime, LayerMask layer)
        {
            // 速度太慢, 当做没移动.
            if(velocity.sqrMagnitude < 1e-8f) return Vector2.zero;
            
            var move = velocity * deltaTime;
            move = move.WithLength(move.magnitude);
            
            switch(c)
            {
                default:
                case BoxCollider2D box:
                {
                    var n = Physics2D.BoxCast(c.bounds.center, c.bounds.size, c.transform.rotation.z, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    
                    #if UNITY_EDITOR
                    var color = Color.yellow;
                    var min = c.bounds.min + (Vector3)move;
                    var max = c.bounds.max + (Vector3)move;
                    Debug.DrawLine(min, min.WithX(max.x), color);
                    Debug.DrawLine(min, min.WithY(max.y), color);
                    Debug.DrawLine(min.WithX(max.x), max, color);
                    Debug.DrawLine(min.WithY(max.y), max, color);
                    #endif
                    
                    move = ClosestCollide(n, c, move);
                }
                break;
                
                case CircleCollider2D circ:
                {
                    var n = Physics2D.CircleCast(circ.bounds.center, circ.radius, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    move = ClosestCollide(n, c, move);
                }
                break;
                
                case CapsuleCollider2D cap:
                {
                    var n = Physics2D.CapsuleCast(cap.bounds.center, cap.size, cap.direction, c.transform.rotation.z, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    move = ClosestCollide(n, c, move);
                }
                break;
            }
            
            move = move.WithLength(move.magnitude);
            return move;
        }

        class RaycastHit2DDistanceComparer : IComparer<RaycastHit2D>
        {
            public int Compare(RaycastHit2D x, RaycastHit2D y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
        static readonly RaycastHit2DDistanceComparer raycastHit2DDistanceComparer = new RaycastHit2DDistanceComparer();

        static Vector2 ClosestCollide(int n, Collider2D c, Vector2 move)
        {
            if(n == 0) return move;
            
            Array.Sort(rayCastBuffer, 0, n, raycastHit2DDistanceComparer);
            for(int i = 0; i < n; i++)
            {
                // 自己不算.
                if(rayCastBuffer[i].collider == c) continue;
                // 法线和移动方向相同, 不属于碰撞.
                if(Vector2.Dot(rayCastBuffer[i].normal, move) >= 0) continue;
                
                return move.WithLength(rayCastBuffer[i].distance);
            }
            
            // 啥都没碰到.
            return move;
        }
        
        
        
        static Collider2D[] collidersCache = new Collider2D[64];
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, float deltaTime, LayerMask layer)
        {
            var n = rd.GetAttachedColliders(collidersCache);
            var res = velocity * deltaTime;
            for(int i = 0; i < n; i++)
            {
                var r = collidersCache[i].MoveAndCollide(velocity, deltaTime, layer);
                if(r.sqrMagnitude < res.sqrMagnitude) res = r; 
            }
            return res;
        }
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, float deltaTime)
        {
            return rd.MoveAndCollide(velocity, deltaTime, maskOfAll);
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, LayerMask layer)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, layer);
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, maskOfAll);
        }
        
        public static Rigidbody2D MoveRelative(this Rigidbody2D rd, Vector2 move)
        {
            rd.MovePosition(rd.position + move);
            return rd;
        }
        
        public static Rigidbody2D RotateRelative(this Rigidbody2D rd, float move)
        {
            rd.MoveRotation(rd.rotation + move);
            return rd;
        }
        
        
        // ====================================================================================================
        // collider as geometry
        // ====================================================================================================

        public static Vector2 RandomLocalPoint(this CircleCollider2D c)
        {
            var angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * c.radius;
        }

        public static Vector2 RandomWorldPoint(this CircleCollider2D c)
        {
            return c.transform.position.ToVec2() + c.RandomLocalPoint();
        }

        public static Vector2 RandomLocalPoint(this BoxCollider2D c)
        {
            Debug.Assert(c.isActiveAndEnabled, "CircleCollider2D is not active and enabled, this will cause bounds invalid");
            return c.transform.InverseTransformPoint(c.RandomWorldPoint());
        }

        public static Vector2 RandomWorldPoint(this BoxCollider2D c)
        {
            Debug.Assert(c.isActiveAndEnabled, "BoxCollider2D is not active and enabled, this will cause bounds invalid");
            var bounds = c.bounds;
            var min = bounds.min;
            var max = bounds.max;
            var x = UnityEngine.Random.Range(min.x, max.x);
            var y = UnityEngine.Random.Range(min.y, max.y);
            return new Vector2(x, y);
        }

        public static Vector2 RandomLocalPoint(this PolygonCollider2D c)
        {
            Debug.Assert(c.isActiveAndEnabled, "PolygonCollider2D is not active and enabled, this will cause bounds invalid");
            return c.transform.InverseTransformPoint(c.RandomWorldPoint());
        }

        public static Vector2 RandomWorldPoint(this PolygonCollider2D c)
        {
            Debug.Assert(c.isActiveAndEnabled, "PolygonCollider2D is not active and enabled, this will cause bounds invalid");
            var bounds = c.bounds;
            var min = bounds.min;
            var max = bounds.max;
            for(int i = 0; i < 100; i++)
            {
                var point = new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
                // Debug.LogFormat("RandomWorldPoint: {0}, {1}, {2}", point, min, max);
                if(c.OverlapPoint(point)) return point;
            }
            Debug.LogWarning("RandomWorldPoint: failed to get random point in polygon collider");
            return c.transform.position;
        }

        // ====================================================================================================
        // ====================================================================================================
        #region Fast get or set
        
        public static Rigidbody2D SetVx(this Rigidbody2D rd, float x)
        {
            rd.linearVelocity = rd.linearVelocity.WithX(x);
            return rd;
        }
        
        public static Rigidbody2D SetVy(this Rigidbody2D rd, float y)
        {
            rd.linearVelocity = rd.linearVelocity.WithY(y);
            return rd;
        }
        
        public static Rigidbody2D GetVx(this Rigidbody2D rd, out float x)
        {
            x = rd.linearVelocity.x;
            return rd;
        }
        
        public static Rigidbody2D GetVy(this Rigidbody2D rd, out float y)
        {
            y = rd.linearVelocity.y;
            return rd;
        }
        
        public static Vector2 GetPosition(this Collider2D c)
        {
            if(c.attachedRigidbody == null) return c.transform.position.ToVec2();
            return c.attachedRigidbody.position;
        }
        

        public static Vector2 GetContactPoint(this Collider2D a, Collider2D b)
        {
            // assume a collides into b, that is a is the moving collider
            // and collision point must be on b.
            var centerOfA = a.bounds.center;
            return b.ClosestPoint(centerOfA);
        }
        #endregion
    }
}
