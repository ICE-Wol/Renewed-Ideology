using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class St0F0Atk0 : BulletGenerator
{
    public DoubleSpeedApproach bulletSlow;
    public DoubleSpeedApproach bulletFast;
    public int startTime;
    public int timer;
    public override IEnumerator<float> ShootSingleWave() {
        var dir = Calc.GetPlayerDirection(transform.position);
        var b = Instantiate(bulletSlow, transform.position, Quaternion.identity);
        b.direction = dir;
        
        b = Instantiate(bulletSlow, transform.position, Quaternion.identity);
        b.direction = dir + 10f;
        
        b = Instantiate(bulletSlow, transform.position, Quaternion.identity);
        b.direction = dir - 10f;
        
        b = Instantiate(bulletFast, transform.position, Quaternion.identity);
        b.direction = dir + 30f;
        
        b = Instantiate(bulletFast, transform.position, Quaternion.identity);
        b.direction = dir - 30f;
        
        b = Instantiate(bulletFast, transform.position, Quaternion.identity);
        b.direction = dir + 50f;
        
        b = Instantiate(bulletFast, transform.position, Quaternion.identity);
        b.direction = dir - 50f;

        yield break;
    }

    public override IEnumerator<float> AutoShoot() {
            while (true) {
                if(timer > startTime) Timing.RunCoroutine(ShootSingleWave());
                yield return Calc.WaitForFrames(waveFrameInterval);
            }
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}
