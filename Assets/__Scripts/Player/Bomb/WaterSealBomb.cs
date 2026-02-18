using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaterSealBomb : MonoBehaviour
{
    public enum BombState
    {
        Expanding,
        AccRotating,
        DecRotating,
        Hitting,
    }
    
    public GameObject waterOrb;
    public int waterOrbCount;
    public GameObject[] waterOrbs;
    
    public float[] radius;
    public float[] degree;
    public Vector3[] endPos;
    public int pathTimer;
    public int startHittingTimer;
    public float startHittingInterval;

    public float degChangeRate;

    public float scale;
    
    public BombState bombState;
    public int explodeTimer;
    public int explodeCount;
    
    private void Start()
    {
        waterOrbs = new GameObject[waterOrbCount];
        radius = new float[waterOrbCount];
        degree = new float[waterOrbCount];
        endPos = new Vector3[waterOrbCount];

        for (int i = 0; i < waterOrbCount; i++) {
            waterOrbs[i] = Instantiate(waterOrb, PlayerCtrl.instance.transform.position, Quaternion.identity);
            waterOrbs[i].transform.localScale = 0f * Vector3.one;
            radius[i] = 0f;
            degree[i] = 360f / waterOrbCount * i;
        }

        scale = 0f;
        bombState = BombState.Expanding;
    }

    private void Update() {
        
        scale.ApproachRef(0.7f, 16f);
        
        if (bombState == BombState.AccRotating) {
            degChangeRate.ApproachRef(3f, 16f);
            if (degChangeRate.Equal(3f, 0.1f)) {
                bombState = BombState.DecRotating;
                //去除减速旋转的入口
                //bombState = BombState.Hitting;
                //for (int i = 0; i < waterOrbCount; i++) {
                //    if (waterOrbs[i] == null) continue;
                //    endPos[i] = waterOrbs[i].transform.position;
                //}
            }
        }
        if (bombState == BombState.DecRotating) {
            degChangeRate.ApproachRef(0f, 16f);
            if (degChangeRate.Equal(0f, 0.1f)) {
                bombState = BombState.Hitting;
                for (int i = 0; i < waterOrbCount; i++) {
                    if (waterOrbs[i] == null) continue;
                    endPos[i] = waterOrbs[i].transform.position;
                }
            }
        }
        if(bombState == BombState.Hitting) {
            startHittingTimer++;
            startHittingInterval -= 0.3f;
        }

        explodeCount = 0;
        for (int i = 0; i < waterOrbCount; i++) {
            if (waterOrbs[i] == null) {
                explodeCount++;
            }
        }
        if (explodeCount == waterOrbCount) {
            Destroy(gameObject);
        }
        
        for (int i = 0; i < waterOrbCount; i++) {
            if(waterOrbs[i] == null) continue;
            waterOrbs[i].transform.localScale = scale * Vector3.one;

            foreach (var b in State.bulletSet) {
                if(Vector2.Distance(b.transform.position, waterOrbs[i].transform.position) < 1f) {
                    b.SetState(EBulletStates.Destroying);
                }
            }
            
            if(bombState != BombState.Hitting)
                waterOrbs[i].transform.position =
                    PlayerCtrl.instance.transform.position + radius[i] * (degree[i].Deg2Dir3());
            
            switch (bombState) {
                case BombState.Expanding:
                    radius[i].ApproachRef(2.5f, 16f);
                    if (radius[i].Equal(2.5f,0.1f)) {
                        bombState = BombState.AccRotating;
                    }
                    break;
                
                case BombState.AccRotating:
                    degree[i] += degChangeRate;
                    radius[i] += 0.01f;
                    break;
                case BombState.DecRotating:
                    degree[i] += degChangeRate;
                    radius[i] += 0.01f;
                    break;
                case BombState.Hitting:
                    //startHittingTimer++;
                    if (startHittingTimer <= i * startHittingInterval && Damageable.damageableSet.Count != 0) continue;
                    //startHittingInterval -= 0.1f;
                    var minDist = 10000f;
                    var tarPos = Vector3.zero;
                    var pathPos = Vector3.zero;
                    foreach (var target in Damageable.damageableSet) {
                        if (target == null) continue;
                        var dist = Vector2.Distance(target.transform.position, waterOrbs[i].transform.position);
                        if (dist < minDist) {
                            minDist = dist;
                            tarPos = target.transform.position;
                        }
                    }

                    if (Damageable.damageableSet.Count == 0) {
                        RaycastHit2D hit = Physics2D.Raycast(waterOrbs[i].transform.position, degree[i].Deg2Dir());

                        // 如果射线碰撞到物体
                        if (hit.collider != null) {
                            tarPos = hit.point;
                        }

                        explodeTimer++;
                        if (explodeTimer >= 120) {
                            waterOrbs[i].GetComponentInChildren<WaterOrb>().isExploded = true;
                            //waterOrbs[i].GetComponentInChildren<WaterOrb>().waterOrbPair.isExploded = true;
                        }
                    }
                    
                    pathPos = Calc.GetQuadBezierPoint(endPos[i], endPos[i] + 2.5f * degree[i].Deg2Dir3(),tarPos, pathTimer,150f);
                    waterOrbs[i].transform.position = waterOrbs[i].transform.position.ApproachValue(pathPos, 8f);
                    if(pathTimer < 150f) pathTimer++;
                    break;
            }
        }
    }
}
