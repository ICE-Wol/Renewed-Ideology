using System;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.EnemyBullet.MoveMethod {
    public class TripleSpeedApproach : BulletMovement {
        public float midSpeed;
        public float endSpeed;
        public int startFrame;
        public int secondFrame;
        public float approachRate;

        public bool trigger;
        public bool secondTrigger;

        private int _timer;
        private float _curSpeed;



        public void Start() {
            _curSpeed = speed;
        }

        public bool IsTriggered() {
            return _timer >= startFrame;
        }

        public Action onTriggerEvent;

        public override void Movement(Transform transform) {
            if (trigger) {
                _curSpeed.ApproachRef(midSpeed, approachRate);
            }else if (secondTrigger) {
                _curSpeed.ApproachRef(endSpeed, approachRate);
            }

            if (_timer == startFrame) {
                trigger = true;
                onTriggerEvent?.Invoke();
            }
            if(_timer == secondFrame) {
                trigger = false;
                secondTrigger = true;
                
            }
            

            transform.position += _curSpeed * Time.fixedDeltaTime * direction.Deg2Dir3();
            _timer++;
        }
    }
}