using System;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    [Serializable]
    public class UniformLinear : BulletMovement {

        public override void Movement(Transform transform) {
            transform.position += Time.fixedDeltaTime * speed * direction.Deg2Dir3();
            transform.rotation = Quaternion.Euler(0,0,direction);
        }
    }
}