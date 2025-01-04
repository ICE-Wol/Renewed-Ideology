using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Scripts.Tools;
using UnityEngine;
using MEC;

public class LaserSpawner : BulletGenerator
{
    public CurveLaserHead head;

    public CurveLaserHead[] lasers;
    
    public int ways = 10;
    public float genRadius = 3f;
    

    public IEnumerator<float> Shoot(float initDir) {
        lasers = new CurveLaserHead[10];
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            lasers[i] = Instantiate(head, transform.position + genRadius * (dir).Deg2Dir3(), Quaternion.identity);
            lasers[i].initDir = dir;
            lasers[i].color = Color.HSVToRGB(i / 10f, 1, 1);
            lasers[i].changeInterval = 40 - i;
            yield return Calc.WaitForFrames(19);
        }
        lasers = new CurveLaserHead[10];
    }
    
    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot(0));
        yield return Calc.WaitForFrames(7);
        Timing.RunCoroutine(Shoot(10));
        yield return Calc.WaitForFrames(7);
        Timing.RunCoroutine(Shoot(20));
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}



