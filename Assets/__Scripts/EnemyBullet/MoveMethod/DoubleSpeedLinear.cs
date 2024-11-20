using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.EnemyBullet.MoveMethod {
    public class DoubleSpeedLinear : BulletMovement {
        public float endSpeed;
        public int startFrame;
        public int endFrame;

        private int _timer;
        private float _curSpeed;

        public void Start() {
            _curSpeed = speed;
            _timer = 0;
        }
        
        public override void Movement(Transform transform) {
            transform.position += _curSpeed * Time.fixedDeltaTime * direction.Deg2Dir3();
            if (_timer >= startFrame && _timer <= endFrame) {
                _curSpeed = Mathf.Lerp(speed, endSpeed, _timer);
            }
            _timer++;
        }
    }
}