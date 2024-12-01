using System;
using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    public abstract class BulletMovement : MonoBehaviour {
        [Header("已在代码中获取索引")]
        public Config bulletConfig;
        public State bulletState;

        private void Awake() {
            bulletConfig = GetComponent<Config>();
            bulletState = GetComponent<State>();
        }

        
        public float speed;
        public float direction;
        public abstract void Movement(Transform transform);
    }
}