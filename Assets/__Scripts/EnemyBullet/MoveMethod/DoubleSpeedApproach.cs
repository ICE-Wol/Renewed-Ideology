using System;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    public class DoubleSpeedApproach : BulletMovement {
        public float endSpeed;
        public int startFrame;
        public float approachRate;
        public bool destroyOnFinish;
        public int destroyOnTime;
        private bool _isDestroyed;
        
        public bool trigger;

        private int _timer;
        private float _curSpeed;
        
        public void Start() {
            _curSpeed = speed;
        }
        
        public bool IsTriggered() {
            return _timer >= startFrame;
        }
        
        public bool IsSpeedChangeFinished(float epsilon) {
            return _curSpeed.Equal(endSpeed,epsilon);
        }
        
        public void SetSpeed(float speed) {
            _curSpeed = speed;
        }

        public Action onTriggerEvent;
        
        public override void Movement(Transform transform) {
            if (trigger) {
                _curSpeed.ApproachRef(endSpeed, approachRate);
            }
            if(destroyOnFinish && !_isDestroyed && _curSpeed.Equal(endSpeed,0.1f) || 
               (destroyOnTime > 0 && _timer >= destroyOnTime)) {
                //print(_isDestroyed);
                _isDestroyed = true;
                gameObject.GetComponent<State>().SetState(EBulletStates.Destroying);
            }
            if (_timer == startFrame) {
                trigger = true;
                onTriggerEvent?.Invoke();
            }
            //if (moveOnlyWhenTriggered && !trigger) return;
            transform.localPosition += _curSpeed * Time.fixedDeltaTime * direction.Deg2Dir3();
            //Debug.LogError($"updatePos { Time.frameCount }");
            _timer++;
        }
    }
}