using System;
using System.Collections.Generic;
using UnityEngine;


namespace _Scripts.Enemy {
    public class Damageable : MonoBehaviour {
        public static HashSet<Damageable> damageableSet = new();
        public FairyBreakEffectCtrl breakEffect;
        
        [Header("血量")]
        public float maxHealth;
        public float curHealth;
        
        [Header("时间(仅用于Boss)")]
        public bool isBoss;
        [Tooltip("仅Boss状态机处在战斗状态中,时间才会流逝")]
        public bool inBattle;
        public float maxTime;
        public float curTime;
        
        [Header("无敌时间")]
        public bool isInvincible;
        public int initInvincibleTime;
        public int curInvincibleTime;
        
        [Header("碰撞半径")]
        public float hitRadius;
        private float _curScale;

        [Header("死亡事件")]
        public bool isOnDeadInvoked;

        
        
        //public event Action OnDead;
        
        public void TakeDamage(int damageValue) {
            curHealth -= damageValue;
        }

        public void SetInvincibleTime(int time) {
            if (time <= 0) return;
            else {
                initInvincibleTime = time;
                curInvincibleTime = time;
                isInvincible = true;
            }
        }

        private void Start() {
            curHealth = maxHealth;
            curTime = maxTime;
            _curScale = 1f;
            SetInvincibleTime(initInvincibleTime);
            damageableSet.Add(this);
        }

        public void DeadEvents() {
            //Debug.Log("DeadEvents");
            if (!isOnDeadInvoked) {
                if (!isBoss) {
                    if (breakEffect != null)
                        Instantiate(breakEffect, transform.position, Quaternion.Euler(0, 0, 0));
                    damageableSet.Remove(this);
                    Destroy(gameObject);
                }
                else {
                    GameManager.Manager.StartEraseBullets(transform.position);
                }
                //OnDead?.Invoke();

                
                
                isOnDeadInvoked = true;
            }
        }
        
        private void Update() {
            if (curInvincibleTime > 0) {
                curInvincibleTime--;
                if (curInvincibleTime == 0)
                    isInvincible = false;
            }
            
            if(inBattle && curTime > 0) curTime -= Time.deltaTime;
            
            if (curHealth <= 0) {
                DeadEvents();
            }
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, hitRadius);
        }
        
    }
}
