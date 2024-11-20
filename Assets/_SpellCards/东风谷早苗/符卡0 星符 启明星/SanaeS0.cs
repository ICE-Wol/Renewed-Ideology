using System;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using NUnit.Framework;
using UnityEngine;

public class SanaeS0 : BulletGenerator
{
    public LaserFlower laserFlower;
    public StarToCircle starToCircleJade;
    public StarToCircle starToCirclePoint;
    
    //public CoroutineHandle coroutineHandle;

    public override IEnumerator<float> ShootSingleWave() {
        waveCount++;
        var a = Instantiate(laserFlower, new Vector3(0, 2, 0), Quaternion.identity);
        var b = Instantiate(starToCircleJade, new Vector3(0, 2, 0), Quaternion.identity);
        var c = Instantiate(starToCirclePoint, new Vector3(0, 2, 0), Quaternion.identity);
        
        subBulletGenerators.Add(a.gameObject);
        subBulletGenerators.Add(b.gameObject);
        subBulletGenerators.Add(c.gameObject);

        starToCircleJade.centerPos = new Vector3(0, 2, 0);
        starToCirclePoint.centerPos = new Vector3(0, 2, 0);
        
        if (waveCount % 2 == 0) {
            laserFlower.isOdd = false;
            starToCircleJade.isOddWave = false;
            starToCirclePoint.isOddWave = false;
        }
        else {
            laserFlower.isOdd = true;
            starToCircleJade.isOddWave = true;
            starToCirclePoint.isOddWave = true;
        }
        
        yield return Timing.WaitForOneFrame;
    }

    public IEnumerator<float> SetIsEnchantingFlagForFrames(int frames) {
        isEnchanting = true;
        yield return Calc.WaitForFrames(frames);
        isEnchanting = false;
    }

    public override IEnumerator<float> AutoShoot() {
        yield return Calc.WaitForFrames(10);
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            Timing.RunCoroutine(SetIsEnchantingFlagForFrames(240));
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
    
}
