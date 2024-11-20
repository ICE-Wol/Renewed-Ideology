using _Scripts.Tools;
using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    public class ZigzagLinear : BulletMovement {
        public float initSpeed;
        public float initDirection;
        public float reduceSpeed;
        public float zigzagAngle;
        
        public override void Movement(Transform transform) {
            transform.position += Time.fixedDeltaTime * speed * direction.Deg2Dir3();
            transform.rotation = Quaternion.Euler(0,0,direction);
            speed -= reduceSpeed;
            if (speed <= 0f) {
                speed = initSpeed;
                direction += zigzagAngle;
                zigzagAngle *= -1;
            }
        }
    }
}