using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class St0F7 : BulletGenerator
{
    public DoubleSpeedApproach stemBullet;
    public DoubleSpeedApproach petalBullet;
    public Color petalStartColor;
    public Color petalEndColor;
    public int stemLayer;
    
    public int startTime;
    public int endTime;
    public int timer;
    

    public override IEnumerator<float> ShootSingleWave() {
        float initDir = Random.Range(0, 360);
        int genInterval = 3;
        float dirInterval = 10;
        while (true) {
            ShootStem(0.5f, initDir, stemLayer);
            initDir += dirInterval;
            yield return Calc.WaitForFrames(genInterval);
        }
    }

    public void ShootStem(float radius, float dir, int layer) {
        var b1 = Calc.GenerateBullet(stemBullet, transform.position + (radius + 0.1f * layer) * dir.Deg2Dir3(), -dir);
        var b2 = Calc.GenerateBullet(stemBullet, transform.position - (radius + 0.1f * layer) * dir.Deg2Dir3(), dir);
        (b1 as DoubleSpeedApproach).endSpeed += 0.5f * layer;
        (b2 as DoubleSpeedApproach).endSpeed += 0.5f * layer;
    }
    
    public void ShootFlower() {
        int bulletNum = 72;
        int petalNum = 6;
        int petalBulletNum = bulletNum / petalNum;
        int layer = 3;
        float radius = 0.5f;
        float initDir = Random.Range(0, 360);
        Vector3 randPos = Random.insideUnitCircle * 0.5f; 
        for (int k = 0; k < layer; k++) {
            for (int i = 0; i < bulletNum; i++) {
                var dir = initDir + 360f / bulletNum * i;
                var order = petalBulletNum / 2 - Mathf.Abs(i % petalBulletNum - petalBulletNum / 2);
                var b = Calc.GenerateBullet(petalBullet,
                    (radius + order / 6f - k * 0.3f) * dir.Deg2Dir3() + transform.position + randPos,
                    dir);
                (b as DoubleSpeedApproach).endSpeed += 0.5f * k;
                b.bulletState.SetColor(Calc.LerpColorInRGB(petalStartColor,petalEndColor, k / (layer - 1f)));
            }
        }

    }

    public override IEnumerator<float> AutoShoot() {
        Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject),"Shoot");
        // while (true) {
        //     yield return Calc.WaitForFrames(30);
        //     ShootFlower();
        //     yield return Calc.WaitForFrames(waveFrameInterval);
        // }
        yield break;
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}

