using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player {
    public class PlayerState : MonoBehaviour {
        public Config config;
        
        public int life;
        public int bomb;
        public GameObject bombPrefab;
        public GameObject bombObject;
        
        public int maxLifeFrag;
        public int maxLife;
        public int maxBomb;
        private int _lifeFrag;

        public int LifeFrag {
            set {
                if (value == maxLifeFrag) {
                    print("Extend!");
                    if(life + 1 <= maxLife)
                        life += 1;
                    _lifeFrag = 0;
                    return;
                }
                else {
                    _lifeFrag = value;
                }
                //PlayerStatusManager.instance.RefreshSlot();
            }
            get => _lifeFrag;
        }

        public int maxBombFrag;
        private int _bombFrag;

        public int BombFrag {
            set {
                if (value == maxBombFrag) {
                    if(bomb + 1 <= maxBomb)
                        bomb += 1;
                    _bombFrag = 0;
                    return;
                }
                else {
                    _bombFrag = value;
                }
                //PlayerStatusManager.instance.RefreshSlot();
            }

            get => _bombFrag;
        }

        public PlayerSubCtrl subCtrl;
        public int maxPower;
        private int _power;

        public int Power {
            set {
                _power = value;
                if (_power >= maxPower) {
                    _power = maxPower;
                    point += 1;
                }
                if(_power < 100) _power = 100;
                
                if (_power / 100 != subCtrl.GetCurSubNum())
                    subCtrl.ChangeSubNum(_power / 100);
            }
            get => _power;
        }
        
        public bool IsPowerFull => Power == maxPower;

        public int point;
        public int gold;
        public int graze;
        public int maxPoint;
        public int score;
        public int maxScore;

        public float hitRadius;
        public float grazeRadius;
        public float itemRadius;
        
        public float slowRate;
        public float moveSpeed;
        
        private void Start() {
            life = 2;
            LifeFrag = 0;
            bomb = 3;
            BombFrag = 0;
            
            maxPower = config.maxPower;
            _power = 100;

            graze = 0;
            maxPoint = 10000;
            score = 0;
            maxScore = config.maxScore;

            slowRate = config.slowRate;
            moveSpeed = config.moveSpeed;

            hitRadius = config.hitRadius;
            grazeRadius = config.grazeRadius;
            itemRadius = config.itemRadius;

            Power = 200;
        }
        

        private void Update() {
            if (Input.GetKeyDown(KeyCode.X)) {
                if (bomb > 0) {
                    if(BossManager.instance.curBoss != null)
                        BossManager.instance.curBoss.hasBonus = false;
                    if (bombObject == null)
                        bombObject = Instantiate(bombPrefab, transform.position, Quaternion.identity);
                    bomb -= 1;
                    //PlayerStatusManager.instance.RefreshSlot();
                    PlayerCtrl.instance.InvincibleTimer = 300;
                }
            }
            
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E)) {
                Power += 100;
            }
            
            if (Input.GetKeyDown(KeyCode.Q)) {
                Power += 100;
            }
            #endif
        }
    }
}
