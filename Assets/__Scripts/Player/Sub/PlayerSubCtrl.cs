using System;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class PlayerSubCtrl : MonoBehaviour {
        public PlayerSub playerSub;
        public GameObject playerSubSet;

        public float radius;
        public float slowRadius;
        
        
        private Queue<PlayerSub> _playerSubs;
        private int _timer;

        public int GetCurSubNum() => _playerSubs.Count;

        public void ChangeSubNum(int num) {
            int cnt = _playerSubs.Count;
            while (num > cnt) {
                var sub = Instantiate(playerSub,transform);
                sub.transform.parent = playerSubSet.transform;
                _playerSubs.Enqueue(sub);
                cnt++;
            }

            while (num < cnt) {
                _playerSubs.Dequeue().DestroyPlayerSub();
                cnt--;
            }
        }

        private void RefreshSubPos() {
            //Debug.Log(_playerSubs);
            int cnt = _playerSubs.Count;
            float startAngle, endAngle;
            if(Input.GetKey(KeyCode.LeftShift)) {
                startAngle = 60f;
                endAngle = 120f;
            }
            else {
                startAngle = 30f;
                endAngle = 150f;
            }

            //float splitAngle = (endAngle - startAngle) / (cnt - 1);
            for (int i = 0; i < cnt; i++) {
                var curSub = _playerSubs.Dequeue();
                var tarPos = transform.position + (Input.GetKey(KeyCode.LeftShift) ? slowRadius : radius) *
                    ((cnt == 1)
                        ? (startAngle + endAngle) / 2f
                        : ( /*_timer * 1.5f + */(endAngle - startAngle) / (cnt - 1) * i + startAngle))
                    .Deg2Dir3();
                curSub.transform.position = curSub.transform.position.ApproachValue(tarPos, 6f * Vector3.one);
                _playerSubs.Enqueue(curSub);
            }
        }

        private void Update() {
            RefreshSubPos();
            _timer++;
        }

        private void Awake() {
            _playerSubs = new Queue<PlayerSub>();
            ChangeSubNum(1);
        }
    }
}
