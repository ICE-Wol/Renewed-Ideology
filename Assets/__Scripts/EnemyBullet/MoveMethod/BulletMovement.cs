using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    public abstract class BulletMovement : MonoBehaviour {
        public float speed;
        public float direction;
        public abstract void Movement(Transform transform);
    }
}