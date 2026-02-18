using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Item;
using _Scripts.Player;
using UnityEngine;


namespace _Scripts.Enemy {
    public class Damageable : MonoBehaviour {
        public static HashSet<Damageable> damageableSet = new();
        public FairyBreakEffectCtrl breakEffect;
        public Color breakEffectColor;
        
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

        [Header("死亡事件")]
        public bool isOnDeadInvoked;
        public Detect bulletDetector;

        
        public static Damageable GetNearestDamageable(Vector2 pos) {
            //print(damageableSet.Count);
            Damageable nearest = null;
            float minDist = float.MaxValue;
            if (damageableSet.Count == 0) return null;
            foreach (var damageable in damageableSet) {
                if(damageable == null) continue;
                float dist = (damageable.transform.position - (Vector3)pos).sqrMagnitude;
                if (dist < minDist) {
                    minDist = dist;
                    nearest = damageable;
                }
            }
            return nearest;
        }
        
        
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
            SetInvincibleTime(initInvincibleTime);
            damageableSet.Add(this);

            bulletDetector = gameObject.AddComponent<Detect>();
        }

        public void DeadEvents() {
            //Debug.Log("DeadEvents");
            if (!isOnDeadInvoked) {
                if (!isBoss) {
                    if (breakEffect != null) {
                        var eff = Instantiate(breakEffect, transform.position, Quaternion.Euler(0, 0, 0));
                        eff.color = breakEffectColor;
                    }

                    damageableSet.Remove(this);
                    GetComponent<ItemSpawner>().CreateItem();
                    Destroy(gameObject);
                }
                else {
                    //GameManager.Manager.StartEraseBullets(transform.position);
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
            
            if (hitRadius > 0 && bulletDetector.CheckPlayerCollision(hitRadius)) {
                if (!PlayerCtrl.instance.CheckInvincibility()) {
                    PlayerCtrl.instance.GetHit();
                }
            }
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, hitRadius);
        }
        
    }
}
