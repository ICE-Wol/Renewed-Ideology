using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class St0F6 : BulletGenerator
{
    public DoubleSpeedApproach midJade;
    public DoubleSpeedApproach rice;
    public DoubleSpeedApproach jade;
    
    public int startTime;
    public int endTime;
    public int timer;
    
    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot().CancelWith(gameObject),"Shoot");
        yield break;
    }
    
    IEnumerator<float> Shoot() {
        yield return Calc.WaitForFrames(120);

        var startDir = Random.Range(0, 360f);
        var radius = 0.5f;
        var bulletNum = 7;
        var waitTime = 15;

        var curDir = startDir;
        for (int i = 0; i < bulletNum; i++) {
            var bullet = (DoubleSpeedApproach)Calc.GenerateBullet(midJade,
                transform.position + radius * curDir.Deg2Dir3(), curDir);
            Timing.RunCoroutine(GenSub(bullet, startDir).CancelWith(bullet.gameObject), "Shoot");
            bullet.direction = curDir + 360f / bulletNum * i;

            bullet.trigger = true;
            yield return Calc.WaitForFrames(waitTime);
        }
    }
    
    IEnumerator<float> GenSub(DoubleSpeedApproach bullet,float initDir) {
        int timer = 0;
        float dir = 0;
        float initRad = 1f;
        Color initColor = bullet.bulletConfig.color;
        while (initRad > 0) {
            var speedVector = bullet.endSpeed * bullet.direction.Deg2Dir();
            speedVector -= new Vector2(0, 0.02f);
            bullet.SetSpeed(speedVector.magnitude);
            bullet.direction = Vector2.SignedAngle(Vector2.right, speedVector);
            
            bullet.bulletState.SetColor(Calc.LerpColorInRGB(initColor, Color.blue, dir));

            var subBullet = (DoubleSpeedApproach)Calc.GenerateBullet(rice,
                bullet.transform.position + initRad * (initDir + dir * 1000).Deg2Dir3(), initDir + dir * 1000);
            subBullet.direction = initDir + dir * 1000;
            initRad -= 0.01f;
            
            timer++;
            dir += 0.01f;
            yield return Timing.WaitForOneFrame;
        }

        var initDir2 = Random.Range(0, 360);
        var maxBulletNum = 20;
        for (int i = 0; i < maxBulletNum; i++) {
            var deg = initDir2 + 360f / maxBulletNum * i;
            var subBullet =
                (DoubleSpeedApproach)Calc.GenerateBullet(jade, bullet.transform.position + 0.5f * deg.Deg2Dir3(),
                    deg);
            if(i % 2 == 0)subBullet.approachRate *= 0.5f;
        }
        bullet.bulletState.SetState(EBulletStates.Destroying);
    }

    public override IEnumerator<float> AutoShoot() {
        Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject), "Shoot");
        yield return Calc.WaitForFrames(waveFrameInterval);
        Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject), "Shoot");
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}
