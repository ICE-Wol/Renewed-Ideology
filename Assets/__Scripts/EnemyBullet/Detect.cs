using _Scripts.Player;
using UnityEngine;

namespace _Scripts.EnemyBullet {
    public class Detect: MonoBehaviour {
        
        public float GetPlayerSqrDis() =>
            ((Vector2)(transform.position - PlayerCtrl.instance.transform.position)).sqrMagnitude;

        public bool CheckPlayerCollision(float radius) {
            var rad = PlayerCtrl.instance.state.hitRadius;
            return GetPlayerSqrDis() < (rad + radius) * (rad + radius);
        }
        
        public bool CheckPlayerGraze(float radius) {
            var rad = PlayerCtrl.instance.state.grazeRadius;
            return GetPlayerSqrDis() < (rad + radius) * (rad + radius);
        }

        public bool IsOutOfBound(Vector3 pos) {
            return (Mathf.Abs(pos.x) > 6f || Mathf.Abs(pos.y) > 6f);
        }
    }
}