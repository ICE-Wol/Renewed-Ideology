

using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CirnoSC0 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach iceBullet;
    public DoubleSpeedApproach jadeBullet;
    public DoubleSpeedApproach jadeMBullet;

    public DoubleSpeedApproach[,] BulletSet;
    public float[,] OrderValue;

    private void Start() {
        BulletSet = new DoubleSpeedApproach[6, 72];
        OrderValue = new float[6, 72];
        Timing.RunCoroutine(AutoShoot().CancelWith(gameObject),"Shoot");
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        // 在物体位置显示动态数字
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 72; j++) {
                if (BulletSet[i, j] != null)
                    Handles.Label(BulletSet[i, j].transform.position + Vector3.forward * 0.1f,OrderValue[i, j].ToString(),new GUIStyle
                    {
                        fontSize = 20,
                        normal = {textColor = Color.red}
                    });
            }
        }
    }
    
    
    public int bulletNum = 72;
    public float initDir;
    public int bulletRings = 6;
    public int edges = 6;
    public float radius = 3f;
    public int pointsOnEdge;
    public IEnumerator<float> Shoot() {
        initDir = Random.Range(0, 360);
        pointsOnEdge = bulletNum / edges;
        float dirOffset = 0;
        var t = 0;
        while (t < bulletRings) {
            //计算多边形顶点
            DoubleSpeedApproach[] bullets = new DoubleSpeedApproach[bulletNum];
            Vector3[] points = new Vector3[edges];
            for(int i = 0; i < edges; i++) {
                points[i] = (radius - 0.5f * t)* (360f / edges * i + initDir + t * 30f).Deg2Dir3();
            }
        
            //计算多边形上的点
            Vector3[] bulletPos = new Vector3[bulletNum];
            for (int i = 0; i < bulletNum; i++) {
                if (i / pointsOnEdge + 1 == edges)
                    bulletPos[i] = Calc.Lerp(points[i / pointsOnEdge], points[0],
                        (float)i % pointsOnEdge / pointsOnEdge);
                else {
                    //print("connecting " + i / pointsOnEdge + " and " + (i / pointsOnEdge + 1));
                    bulletPos[i] = Calc.Lerp(points[i / pointsOnEdge], points[i / pointsOnEdge + 1],
                        (float)i % pointsOnEdge / pointsOnEdge);
                }
            }
            for (int i = 0; i < bulletNum; i++) {
                var dir = i * 360f / bulletNum + initDir + t * 30f + dirOffset;
                var bullet = Calc.GenerateBullet(i % pointsOnEdge != 0 ? jadeBullet : jadeMBullet,
                    dir.Deg2Dir3() + transform.position, dir);
                bullet.speed = 3f - 0.5f * t;
                bullet.bulletState.SetColor(Color.Lerp(Color.blue, Color.cyan, (float)t / (bulletRings-1)));
                bullet.direction = Calc.GetDirection(Vector3.zero, bulletPos[i]);
                bullet.highlight.Appear();
                //abs后，不用pointsOnEdge / 2减是顶点为0
                //abs后，用pointsOnEdge / 2减是中点为0
                var order = i % pointsOnEdge - pointsOnEdge / 2;
                
                //bullet.highlight.tarAlpha = (Mathf.Abs(order) + 3) / (pointsOnEdge / 2f);
                
                BulletSet[t,i] = bullet as DoubleSpeedApproach;
                OrderValue[t,i] = order;
                
                Timing.RunCoroutine(
                     ChangeBullet(bullet as DoubleSpeedApproach, bulletPos[i] + transform.position, order,t,pointsOnEdge)
                         .CancelWith(bullet.gameObject), "Shoot");
                //Timing.RunCoroutine(GenSubBullet(bullet as DoubleSpeedApproach, i, Color.Lerp(Color.red, Color.magenta, (float)i / bulletNum)).CancelWith(bullet.gameObject), "Shoot");
                AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
            }

            dirOffset += 5f;
            t++;
            yield return Calc.WaitForFrames(5);
        }
        
        yield return Calc.WaitForFrames(120);
        
        Timing.RunCoroutine(ChangeBullet2().CancelWith(gameObject),"Shoot");
    }

    IEnumerator<float> ChangeBullet(DoubleSpeedApproach bullet, Vector3 tarPos, int order,int layer, int pointsOnEdge) {
        AudioManager.Manager.PlaySound(AudioNames.SeShootBoon1);
        while (true) {
            if (bullet.IsSpeedChangeFinished(0.1f)) {
                bullet.transform.position = bullet.transform.position.ApproachValue(tarPos, 16f);
                if (bullet.transform.position.Equal(tarPos, 0.01f)) {
                    //yield return Calc.WaitForFrames(150);
                    //bullet.endSpeed = (pointsOnEdge / 2f - (float)Mathf.Abs(order))/(layer + 3f) + 2f;
                    //bullet.direction += order;
                    //Timing.RunCoroutine(
                    //    ChangeBullet2(bullet, order/10f).CancelWith(bullet.gameObject), "Shoot");
                    //bullet.direction -= order;
                    //bullet.endSpeed = 1f;
                    break;
                }
            }

            yield return Timing.WaitForOneFrame;
        }
    }
    
    IEnumerator<float> ChangeBullet2() {
        int interval = 30;
        int numInCircle = 18;
        for (int i = 0; i < bulletRings; i++) {
            for (int j = 0; j < bulletNum; j++) {
                if (BulletSet[i, j] != null) {
                    BulletSet[i, j].endSpeed = (pointsOnEdge / 2f - Mathf.Abs(OrderValue[i, j])) / (i + 3f) + 2f;
                    BulletSet[i, j].highlight.Fade();
                    if (j % pointsOnEdge == 0) {
                        BulletSet[i, j].bulletState.SetState(EBulletStates.Destroying);
                        for(int k = 0; k < numInCircle - i*2; k++) {
                            var bullet = Calc.GenerateBullet(iceBullet, BulletSet[i, j].transform.position,
                                BulletSet[i, j].direction + k * 360f / (numInCircle - i*2)+initDir);
                            bullet.bulletState.SetColor(BulletSet[i, j].bulletConfig.color / 2f);
                        }
                        AudioManager.Manager.PlaySound(AudioNames.SeShootLaser2);
                    }

                    BulletSet[i, j].direction -= OrderValue[i, j] * 10f;
                }
            }
            AudioManager.Manager.PlaySound(AudioNames.SeShootBoon0);
            yield return Calc.WaitForFrames(interval);
        }
    }

    public IEnumerator<float> GenSubBullet(DoubleSpeedApproach parent,int num,Color color) {
        yield return Timing.WaitForOneFrame;
        while (true) {
            if (parent.IsSpeedChangeFinished(0.001f)) {
                for (int i = 1; i <= 1; i++) {
                    var bullet = Calc.GenerateBullet(iceBullet, parent.transform.position,
                        parent.direction * 50f);
                    bullet.bulletState.SetColor(color / 2f);
                }
                parent.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
    }

    public IEnumerator<float> RotateSubBullet(DoubleSpeedApproach bullet,float rotDir) {
        var curRotDir = 5f;
        while (true) {
            curRotDir.ApproachRef(rotDir, 8f);
            bullet.direction -= curRotDir;
            yield return Timing.WaitForOneFrame;
        }
    }
    
    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot(),"Shoot");
        yield break;
    } 
    

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
