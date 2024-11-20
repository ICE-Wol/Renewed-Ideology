using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class SekibankiSC0 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet1;
    public DoubleSpeedApproach tailBullet2;
    public int mainWays;
    public List<DoubleSpeedApproach> tailBullets1;

    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(0, 360);
        
        Timing.RunCoroutine(ShootClockwise(initDir), "Shoot");
        
        yield return Calc.WaitForFrames(180);
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
    
    bool isClockWise = true;
    public IEnumerator<float> ShootClockwise(float initDir) {
        //var list = new List<DoubleSpeedApproach>();
        for (var i = 0; i < mainWays; i++) {
            // var dir = initDir + (isClockWise ? 1 : -1) * 360f / mainWays * i;
            var dir = initDir + 360f / mainWays * i;
            Timing.RunCoroutine(Shoot(dir,1f,0.0025f,3f), "Shoot");
            yield return Calc.WaitForFrames(4);
        }
        isClockWise = !isClockWise;
        
    }
    
    IEnumerator<float> Shoot(float initDir,float initRad,float spdRadShrink,float spdMultiplier) {
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        while (timer <= 360 && initRad > 0) {
            if(head == null) yield break;
            timer++;
            tarDir += 1f;
            initRad -= spdRadShrink;
            //head.direction.ApproachRef(tarDir, 16f);
            head.direction = tarDir;
            
            if (timer % 20 >= 5) {
                //Calc.GenerateBullet(tailBullet1, head.transform.position + initRad * Mathf.Sin(timer * 5 * Mathf.Deg2Rad)*(tarDir + timer).Deg2Dir(), head.direction);
                var dir1 = tarDir + timer * spdMultiplier + 90f;
                var dir2 = tarDir - timer * spdMultiplier - 90f;
                DoubleSpeedApproach b1 = (DoubleSpeedApproach)Calc.GenerateBullet(tailBullet1, head.transform.position + 0.5f * initRad * dir1.Deg2Dir3(), dir1);
                tailBullets1.Add(b1);
                b1.endSpeed += timer / 360f / 2f;
                //b1.direction = Vector2.SignedAngle(b1.transform.position - transform.position, Vector2.right) + timer/10f;
                Timing.RunCoroutine(Trigger(b1, b1.direction / 10f, 150, 1f), "Shoot");
                DoubleSpeedApproach b2 = (DoubleSpeedApproach)Calc.GenerateBullet(tailBullet2, head.transform.position + 1.3f * initRad * dir2.Deg2Dir3(), dir2);
                b2.endSpeed -= timer / 360f / 2f;
                if (timer % 20 >= 13 || timer % 20 < 7) {
                    b1.destroyOnFinish = true;
                }
                if (timer % 20 >= 15 || timer % 20 < 10) {
                    b2.destroyOnFinish = true;
                }
            }

            yield return Timing.WaitForOneFrame;
        }
    }


    IEnumerator<float> Trigger(DoubleSpeedApproach bullet, float offsetDeg, int waitTime, float endSpeed) {
        //Debug.LogError("start");
        yield return Calc.WaitForFrames(waitTime);
        var oriDir = bullet.direction;
        // while(!bullet.direction.Equal(oriDir + offsetDeg,0.1f)){
        //      bullet.direction.ApproachRef(oriDir + offsetDeg, 4f);
        //      yield return Timing.WaitForOneFrame;
        // }
        //print("Triggered");
        //bullet.direction = oriDir + offsetDeg;
        bullet.endSpeed = 0f;
        yield return Calc.WaitForFrames(waitTime);
        bullet.endSpeed = endSpeed;
    }
}
