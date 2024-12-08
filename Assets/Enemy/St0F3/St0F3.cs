using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class St0F3 : BulletGenerator
{
    public DoubleSpeedApproach ringBullet;
    
    public int startTime;
    public int endTime;
    public int timer;
    

    public override IEnumerator<float> ShootSingleWave() {
        var b = Instantiate(ringBullet);
        b.transform.position = transform.position;
        b.direction = Calc.GetPlayerDirection(transform.position);
        yield break;
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
