using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class St0F5 : BulletGenerator
{
    public DoubleSpeedApproach riceBullet;
    
    public int startTime;
    public int endTime;
    public int timer;
    

    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(0, 360);
        var addDir = 28;
        var bulletCnt = 12;
        var waveCount = 0;
        while (waveCount < 50) {
            for (int i = 0; i < bulletCnt; i++) {
                var b = Calc.GenerateBullet(riceBullet, transform.position, initDir + 360f / bulletCnt * i);
                b.transform.position = transform.position;
            }
            initDir += addDir;
            waveCount++;
            yield return Calc.WaitForFrames(10);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            if (endTime != 0 && timer > endTime) break;
            if (timer >= startTime) Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}
