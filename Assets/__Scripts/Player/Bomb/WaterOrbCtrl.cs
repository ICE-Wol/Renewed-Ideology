using System;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;

public class WaterOrbCtrl : MonoBehaviour {
    public WaterOrb[] subOrbs;
    
    [Header("运动参数")]
    public float ang;
    public float initAng;
    public float speed;
    public float angSpd;

    [Header("状态参数")]
    public bool isRotating = true;
    public bool isCircling = false;
    public bool isExploded = false;
    public int explodeTimer;
    public void RefreshPos() {
        //转一定圈数之后进入追踪阶段
        if(Mathf.Abs(initAng - ang) > 360f) {
            isRotating = false;
            transform.parent = null;
        }
        
        if (isRotating) {
            //speed.ApproachRef(0.2f, 16f);
            speed = 0.15f;                                      //这里用ApproachRef会导致水球运动轨迹大幅度变形
            if (!isCircling) {                                  //半径增大阶段
                if (Mathf.Abs(initAng - ang) > 180f) {
                    isCircling = true;
                }
                //angSpd.ApproachRef(10f, 16f);
                angSpd = 5f;
            }                                                   //环绕阶段 
            else angSpd = 2.5f; 
            //angSpd.ApproachRef(5f, 16f);

        } else { //追踪阶段
            //print("追踪阶段");
            Damageable nearestEnemy = Damageable.GetNearestDamageable(transform.position);
            
            if (nearestEnemy == null) {
                var pos = transform.position;
                if (pos.x > 3.4f || pos.x < -3.4f || pos.y > 40f || pos.y < -3.6f) {
                    Explode();
                }
            } else {
                angSpd.ApproachRef(0f, 8f);
                var dist = (nearestEnemy.transform.position - transform.position).sqrMagnitude;
                if(dist > 3f) speed += 0.003f;
                else speed.ApproachRef(0.1f, 8f);
                var targetAngle = Vector2.SignedAngle(Vector2.right,
                    (nearestEnemy.transform.position - transform.position).normalized);
                float delta = Mathf.DeltaAngle(ang, targetAngle);
                ang += 5f * Mathf.Sign(delta);
                //ang.ApproachRef(targetAngle, 8f);
            }
        }
        
        transform.position += speed * ang.Deg2Dir3();
        ang += angSpd;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (Damageable.GetNearestDamageable(transform.position) != null)
            Gizmos.DrawLine(transform.position, Damageable.GetNearestDamageable(transform.position).transform.position);
    }

    private void Explode() {
        //AudioManager.Manager.PlaySound(AudioNames.SeWaterBomb);
        
        isExploded = true;
        
        subOrbs[0].particleSystem.Stop();
        subOrbs[1].particleSystem.Stop();
        
        subOrbs[0].isExploded = true;
        subOrbs[1].isExploded = true;

        //爆炸伤害
        foreach (var target in Damageable.damageableSet) {
            if (target == null) continue;
            if (target.hitRadius * target.hitRadius + damageRad * damageRad >
                (target.transform.position - transform.position).sqrMagnitude) {
                if (!target.isInvincible) target.TakeDamage(burstDamage);
                
            }
        }
        //print("bursted");
        //AudioManager.Manager.PlaySound(AudioNames.SeEnemyExplode1);
    }

    [Header("伤害参数")] 
    public float damageRad;
    public int frameDamage;
    public int damageLimit;
    public int burstDamage;
    public int totDamage;
    
    public void GiveDamage() {
        //持续伤害
        foreach (var target in Damageable.damageableSet) {
            if (target == null) return;
            if (target.hitRadius * target.hitRadius + damageRad * damageRad >
                (target.transform.position - transform.position).sqrMagnitude) {
                if (target.isInvincible) {
                    totDamage += frameDamage;
                }
                else {
                    target.TakeDamage(frameDamage);
                    totDamage += frameDamage;
                }
            }
        }
    }

    public void EraseBullets(float radius){
        foreach (var b in State.bulletSet) {
            if (b == null) continue;
            if(Vector2.Distance(b.transform.position, transform.position) < radius) {
                b.SetState(EBulletStates.Destroying);
            }
        }
    }
    
    
    private void Start() {
        ang = initAng;
    }

    public int fullTimer;

    private void Update() {
        RefreshPos();
        if (!isExploded) {
            fullTimer++;
            GiveDamage();
            EraseBullets(1f);
            if (totDamage > damageLimit) {
                Explode();
            }
        }

        if (fullTimer >= 180) Explode();
        if (isExploded) {
            explodeTimer++;
            speed.ApproachRef(0f, 8f);
            angSpd.ApproachRef(0f, 8f);
            if (explodeTimer == 180) Destroy(gameObject);
        }
    }
}
