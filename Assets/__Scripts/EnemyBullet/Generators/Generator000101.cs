using System;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.EnemyBullet {
    public class Generator000101 : MonoBehaviour {
        public GameObject[] bulletList;
        private float _timer;

        public int intervalTime;
        
        public float startRadius;
        public int startWays;
        public float startDegrees;
        public float repeatTimes;
        
        [SerializeField] private int _totalWaves;
        public float offsetDegrees;

        private void Start() {
            _totalWaves = 0;
            intervalTime = 10;
            startDegrees = Random.value * 360f;
        }

        private void Update() {
            if (_totalWaves < 15 && _totalWaves >= 0){
                if (_timer % intervalTime == 0) {
                    for (int k = 0; k < repeatTimes; k++) {
                        for (int i = 0; i < startWays; i++) {
                            var curDir = startDegrees + offsetDegrees * _totalWaves + (float)360 / startWays * i;
                            var pos = transform.position + startRadius * curDir.Deg2Dir3();
                            var b = Instantiate(bulletList[0], pos, Quaternion.Euler(0, 0, curDir));
                            
                            var comp = b.GetComponent<DoubleSpeedLinear>();
                            comp.startFrame = 0;
                            comp.endFrame = 30;
                            comp.direction = curDir;
                            comp.speed = 0;
                            comp.endSpeed = 1.5f + k * 0.5f;
                        }
                    }

                    for (int k = 0; k < repeatTimes + 1; k++) {
                        for (int i = 0; i < startWays; i++) {
                            var curDir = -startDegrees - offsetDegrees * _totalWaves + (float)360 / startWays * i;
                            var pos = transform.position + startRadius * curDir.Deg2Dir3();
                            var b = Instantiate(bulletList[1], pos, Quaternion.Euler(0, 0, curDir));
                            
                            var comp = b.GetComponent<DoubleSpeedLinear>();
                            comp.startFrame = 0;
                            comp.endFrame = 30;
                            comp.direction = curDir;
                            comp.speed = 0;
                            comp.endSpeed = 3f - k * 2f;
                        }
                    }
                    _totalWaves++;
                }
            }else if (_totalWaves < 30 && _totalWaves >= 15) {
                if (_timer % intervalTime == 0) {
                    for (int k = 0; k < repeatTimes; k++) {
                        for (int i = 0; i < startWays; i++) {
                            var curDir = startDegrees - offsetDegrees * _totalWaves - (float)360 / startWays * i;
                            var pos = transform.position + 2 * startRadius * curDir.Deg2Dir3();
                            var b = Instantiate(bulletList[0], pos, Quaternion.Euler(0, 0, curDir));
                            
                            var comp = b.GetComponent<DoubleSpeedLinear>();
                            comp.startFrame = 0;
                            comp.endFrame = 30;
                            comp.direction = curDir;
                            comp.speed = 0;
                            comp.endSpeed = 1.5f + k * 0.5f;
                        }
                    }

                    for (int k = 0; k < repeatTimes + 1; k++) {
                        for (int i = 0; i < startWays; i++) {
                            var curDir = -(startDegrees - offsetDegrees * _totalWaves) - (float)360 / startWays * i;
                            var pos = transform.position + 2 * startRadius * curDir.Deg2Dir3();
                            var b = Instantiate(bulletList[1], pos, Quaternion.Euler(0, 0, curDir));
                            
                            var comp = b.GetComponent<DoubleSpeedLinear>();
                            comp.startFrame = 0;
                            comp.endFrame = 30;
                            comp.direction = curDir;
                            comp.speed = 0;
                            comp.endSpeed = 3f - k * 2f;
                        }
                    }
                    _totalWaves++;
                }
            }else if (_totalWaves == 30) {
                if (_timer % intervalTime == 0) {
                    for (int i = 0; i < 20; i++) {
                        var curDir = startDegrees + (float)360 / 20 * i;
                        var pos = transform.position + 2 * startRadius * curDir.Deg2Dir3();
                        var b = Instantiate(bulletList[2], pos, Quaternion.Euler(0, 0, curDir));
                        
                        var comp = b.GetComponent<DoubleSpeedLinear>();
                        comp.startFrame = 0;
                        comp.endFrame = 30;
                        comp.direction = curDir;
                        comp.speed = 0;
                        comp.endSpeed = 1;
                    }
                    _totalWaves++;
                    intervalTime = 60;
                }
            }else if (_totalWaves <= 35) {
                if (_timer % intervalTime == 0) {
                    for (int i = 0; i < 20; i++) {
                        float curDir = Random.Range(-5f, 5f) + (float)360 / 20 * i;
                        var pos = transform.position + 2 * startRadius * curDir.Deg2Dir3();
                        var b = Instantiate(bulletList[3], pos, Quaternion.Euler(0, 0, curDir));
                        
                        var comp = b.GetComponent<DoubleSpeedLinear>();
                        comp.startFrame = 0;
                        comp.endFrame = 30;
                        comp.direction = curDir;
                        comp.speed = 0;
                        comp.endSpeed = 1;
                    }
                    intervalTime = 30;
                    _totalWaves++;
                }
            }
            else if(_totalWaves > 35) {
                Destroy(this.gameObject);
            }
            
            _timer++;
        }
    }
}
