using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class SekibankiSC2 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet1;
    public DoubleSpeedApproach tailBullet2;
    public int mainWays;
    public float degInterval;

    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(-30, 30);
        
        Timing.RunCoroutine(ShootClockwise(initDir, degInterval), "Shoot");
        
        yield return Calc.WaitForFrames(180);
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
    
    bool isClockWise = true;
    public IEnumerator<float> ShootClockwise(float initDir,float degInterval) {
        for (var i = 0; i < mainWays; i++) {
            var dir = initDir + degInterval * i;
            Timing.RunCoroutine(Shoot(dir,false), "Shoot");
            yield return Calc.WaitForFrames(15);
        }
        isClockWise = !isClockWise;
        for (var i = 0; i < mainWays; i++) {
            var dir = initDir + degInterval * i;
            Timing.RunCoroutine(Shoot(dir,true), "Shoot");
            yield return Calc.WaitForFrames(15);
        }
        
    }
    
    IEnumerator<float> Shoot(float initDir,bool isClockWise) {
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        var tScale = 360f;
        while (timer <= 360) {
            if(head == null) yield break;
            timer++;

            if (isClockWise) {
                head.transform.position = Calc.GetCubicBezierPoint(transform.position,
                    transform.position + 5f * (initDir + 40f).Deg2Dir3(),
                    transform.position + 5f * (initDir + 10f).Deg2Dir3(),
                    transform.position + 8f * (-initDir - 70f).Deg2Dir3(),
                    timer, tScale);
            }
            else {
                head.transform.position = Calc.GetCubicBezierPoint(transform.position,
                    transform.position + 5f * (180f - (initDir + 40f)).Deg2Dir3(),
                    transform.position + 5f * (180f - (initDir + 10f)).Deg2Dir3(),
                    transform.position + 9f * (180f - (-initDir - 70f)).Deg2Dir3(),
                    timer, tScale);
            }
            
            if (timer % 20 >= 5) {
                DoubleSpeedApproach b =
                    (DoubleSpeedApproach)Calc.GenerateBullet(isClockWise ? tailBullet1 : tailBullet2,
                        head.transform.position, head.direction + (isClockWise ? 90f : -90f));
            }

            yield return Timing.WaitForOneFrame;
        }
    }
    
}
