using System;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class SanaeNS0 : BulletGenerator
{
    public DoubleSpeedApproach ringJadePrefab;
    public DoubleSpeedApproach pointPrefab;
    public DoubleSpeedApproach ricePrefab;
    
    public StarParent starParentPrefab;
    
    public int starNum;
    public int bulletNum;
    public float radius;
    public Vector3 centerPos;
    public Vector3[] starPos;
    public StarParent[] starParents;
    public List<DoubleSpeedApproach> bullets;
    
    public IEnumerator<float> StarShoot() {
        BossManager.instance.curBoss.movement.GoToNextPos(new Vector3(0, 1f,0));
        BossManager.instance.curBoss.movement.stayFrames = 540;
        
        yield return Calc.WaitForFrames(60);
        
        starPos = new Vector3[5];
        starParents = new StarParent[5];
        for (int i = 0; i < 5; i++) {
            starPos[i] = centerPos + new Vector3(
                Mathf.Sin(Mathf.PI * 2 / 5 * i) * radius,
                Mathf.Cos(Mathf.PI * 2 / 5 * i) * radius,
                0
            );
        }
        
        bullets = new List<DoubleSpeedApproach>();
        for (int k = 0; k < 5; k++) {
            starParents[k] = Instantiate(starParentPrefab, centerPos, Quaternion.identity);
            starParents[k].transform.position = starParents[k].transform.position.SetZ(0f);
            
            Timing.RunCoroutine(CreateStar(k));
        }
        yield return Calc.WaitForFrames(120);
        
        for (var spd = 0.1f; spd >= 0; spd -= 0.002f) {
            for (int i = 0; i < 5; i++) {
                if(starParents[i] == null) continue;
                starParents[i].transform.position += (360f / 5f * i + 90f).Deg2Dir3() * spd;
            }
            yield return Calc.WaitForFrames(1);
        }
        
        foreach (var b in bullets) {
            b.trigger = true;
        }
    }

    public IEnumerator<float> WindShoot() {
        for (int k = 0; k < 5; k++) {
            for (int j = 0; j < 15; j++) {
                for (int i = 0; i < 5; i++) {
                    var b = Instantiate(ricePrefab, centerPos, Quaternion.identity);
                    b.GetComponent<SpriteRenderer>().sortingOrder = j;
                    b.direction = 360f / 5 * i + j * 1.5f + 360 / 30f * k;
                    b.speed = 4f + 0.2f * (15 - j);
                    b.endSpeed = 1f + 0.1f * j;
                }

                yield return Timing.WaitForOneFrame;
            }
            yield return Calc.WaitForFrames(10);
        }
    }
    
    public IEnumerator<float> WindShoot2() {
        for (int k = 0; k < 5; k++) {
            for (int j = 0; j < 15; j++) {
                for (int i = 0; i < 5; i++) {
                    var b = Instantiate(ricePrefab, centerPos, Quaternion.identity);
                    b.GetComponent<SpriteRenderer>().sortingOrder = i;
                    b.direction = 360f / 5 * i - j * 1.5f - 360 / 30f * k;
                    b.speed = 4f + 0.2f * (15 - j);
                    b.endSpeed = 1f + 0.1f * j;
                }

                yield return Timing.WaitForOneFrame;
            }
            yield return Calc.WaitForFrames(10);
        }
        isEnchanting = false;
    }

    public IEnumerator<float> CreateStar(int starIndex) {
        var edgeNum = bulletNum / 5;
        for (int i = 0, cnt = 0; cnt < 5; i += 2, cnt++) {
            var midPos = Vector3.Lerp(starPos[i % 5], starPos[(i + 2) % 5], 0.5f);
            var midDir = Vector2.SignedAngle(Vector2.right, midPos - centerPos);
            for (int j = 0; j <= edgeNum; j++) {
                var pos = Vector3.Lerp(
                    starPos[i % 5],
                    starPos[(i + 2) % 5],
                    (float)j / edgeNum) + centerPos;
                pos = pos.SetZ(0f);
                var b = Instantiate(ringJadePrefab, pos, Quaternion.identity);
                var b2 = Instantiate(pointPrefab, pos, Quaternion.identity);
                var dir = Vector2.SignedAngle(Vector2.right, pos - centerPos);
                b.direction = midDir + (j - edgeNum / 2f) * 8f; //dir + 180f;
                b2.direction = midDir + (j - edgeNum / 2f) * 8f; //dir + 180f;
                b2.endSpeed = b.endSpeed / 1.2f;
                b.transform.parent = starParents[starIndex].transform;
                b2.transform.parent = starParents[starIndex].transform;
                bullets.Add(b);
                bullets.Add(b2);

                yield return Timing.WaitForOneFrame;
            }
        }
    }

    private void Update() {
        centerPos = transform.position;
    }

    public override IEnumerator<float> ShootSingleWave() {
        yield return Calc.WaitForFrames(10);
        isEnchanting = true;
        Timing.RunCoroutine(StarShoot().CancelWith(gameObject),"Shoot");
        yield return Calc.WaitForFrames(480);
        Timing.RunCoroutine(WindShoot().CancelWith(gameObject),"Shoot");
        yield return Calc.WaitForFrames(120);
        Timing.RunCoroutine(WindShoot2().CancelWith(gameObject),"Shoot");
        yield return Calc.WaitForFrames(60);
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
