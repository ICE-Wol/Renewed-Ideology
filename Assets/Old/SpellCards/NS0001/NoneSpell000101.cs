using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class NoneSpell0001 : MonoBehaviour {
    public GameObject scaleBullet;
    public GameObject[,] bulletSet;
    public float[,] bulletSpeed;
    
    //减少段落感测试
    public bool[,] bulletArriveFlag;

    public DoubleSpeedApproach riceBullet;
    public List<DoubleSpeedApproach> riceMainList = new();


    private Dictionary<GameObject, GameObject> follow = new();
    
    

    public int circleCount;
    public int bulletPerCircleCount;
    public float generateInterval;
    public float initSpeed;
    public int curBulletInCircleCount;

    public bool secondMovement;
    public bool thirdMovement;
    public bool fourthMovement;

    public int timer;
    public int thirdTimer;
    public int fourthTimer;

    void Start() {
        bulletSet = new GameObject[circleCount, bulletPerCircleCount];
        bulletSpeed = new float[circleCount, bulletPerCircleCount];
        bulletArriveFlag = new bool[circleCount, bulletPerCircleCount];
        secondMovement = false;
    }

    private void Update() {
        if (curBulletInCircleCount < bulletPerCircleCount) {
            if (timer % generateInterval == 0) {
                for (int i = 0; i < circleCount; i++) {
                    var circleOffsetDegree = 360f / circleCount * i;
                    var curBulletOffsetDegree = (float)curBulletInCircleCount / bulletPerCircleCount * 360f;
                    var bullet = Instantiate(scaleBullet, transform.position,
                        Quaternion.Euler(0, 0, circleOffsetDegree + curBulletOffsetDegree));
                    bulletSet[i, curBulletInCircleCount] = bullet;
                    bulletSpeed[i, curBulletInCircleCount] = initSpeed;
                }

                curBulletInCircleCount++;
            }
        }

        for (int i = 0; i < circleCount; i++) {
            for (int j = 0; j < bulletPerCircleCount; j++) {
                var bullet = bulletSet[i, j];
                if (bullet == null) continue;
                var speed = bulletSpeed[i, j];
                var rot = bullet.transform.rotation.eulerAngles.z;
                bulletSpeed[i, j].ApproachRef(0f, 32f);
                if (!bulletSpeed[i, j].Equal(0f, 0.05f)) {
                    bullet.transform.position += Time.fixedDeltaTime * speed * rot.Deg2Dir3();
                    bullet.transform.rotation = Quaternion.Euler(0, 0, rot + 1f);
                }//else if(bulletArriveFlag[i, j] == false){
                    //bulletArriveFlag[i, j] = true;
                    //bulletSpeed[i, j] = -5f + i * 2.5f;
                //}
            }
        }

        if (curBulletInCircleCount == bulletPerCircleCount && secondMovement == false) {
            bool arriveFlag = true;
            for (int i = 0; i < circleCount; i++) {
                for (int j = 0; j < bulletPerCircleCount; j++) {
                    var bullet = bulletSet[i, j];
                    if (bullet == null) return;
                    arriveFlag = bulletSpeed[i, j].Equal(0f, 0.05f);
                    if (!arriveFlag) return;
                }
            }

            if (arriveFlag) {
                for (int i = 0; i < circleCount; i++) {
                    for (int j = 0; j < bulletPerCircleCount; j++) {
                        //bulletSpeed[i, j] = -5f + i * 2.5f;
                        //bulletSpeed[i, j] = 1f + i * 2.5f;
                        bulletSpeed[i, j] = i * 2.5f;
                    }
                }
                secondMovement = true;
            }
        }

        if (secondMovement) {
            thirdTimer++;
        }

        if (thirdTimer >= 100 && !thirdMovement) {
            for (int i = 0; i < circleCount; i++) {
                for (int j = 0; j < bulletPerCircleCount; j++) {
                    var bullet = bulletSet[i, j];
                    bullet.GetComponent<State>().SetState(EBulletStates.Destroying);

                    var bulletPos = bullet.transform.position;
                    for (int k = 0; k < 3; k++) {
                        var centerDir = Vector2.SignedAngle(Vector2.right, transform.position - bulletPos);
                        var dir = 360f / 3f * k + centerDir;//transform.eulerAngles.z;
                        var childPos = bulletPos + 0.5f * dir.Deg2Dir3();
                        var b = Instantiate(riceBullet, childPos, Quaternion.Euler(0, 0, dir));
                        b.direction = dir;
                        b.approachRate = 32f;
                        b.GetComponent<Config>().color = Color.cyan;
                        riceMainList.Add(b);
                        
                        for (int m = 0; m < 4; m++) {
                            int[] n = new int[4]{ -2, -1, 2, 1 };
                            var childPos2 = bulletPos + 0.5f * (dir + 10 * n[m]).Deg2Dir3();
                            var bsub = Instantiate(riceBullet, childPos2, Quaternion.Euler(0, 0, dir));
                            //bsub.startSpeed = -1;
                            //bsub.endSpeed = 0;
                            bsub.direction = dir;
                            bsub.approachRate = 32f + Mathf.Abs(n[m]) * 8f;
                            bsub.GetComponent<Config>().color = Color.green;
                            follow[bsub.gameObject] = b.gameObject;
                        }
                    }
                }
            }

            thirdMovement = true;

        }
        
        if (thirdMovement) {
            fourthTimer++;
        }

        if (thirdMovement && fourthTimer == 180) {
            foreach (var b in follow) {
                if (b.Key != null && b.Value != null) {
                    b.Key.transform.parent = b.Value.transform;
                    Destroy(b.Key.GetComponent<DoubleSpeedApproach>());
                }
            }
        }

        if (fourthTimer > 180) {
            foreach (var b in riceMainList) {
                if (b != null) {
                    b.direction += 0.2f;
                    b.transform.localRotation = Quaternion.Euler(0, 0, b.direction);
                }
            }
        }

        
        



        timer++;
    }
}
