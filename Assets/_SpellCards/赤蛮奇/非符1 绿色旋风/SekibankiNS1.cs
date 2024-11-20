using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class SekibankiNS1 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet;
    public DoubleSpeedApproach snipBullet;
    public int mainWays;
    public int snipWays;

    IEnumerator<float> Shoot(float initDir, bool isClockwise) {
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        while (timer <= 360) {
            if(head == null) yield break;
            timer++;//30帧再开始变换方向
            //tarDir += isClockwise ? 0.3f : -0.3f;
            tarDir += (isClockwise ? 1.5f : -1.5f) * Mathf.Sin(timer * 3 * Mathf.Deg2Rad);
            if(timer % 30 == 0) tarDir += Random.Range(-10, 10);
            head.direction.ApproachRef(tarDir, 16f);

            if (timer % 3 == 0) {
                var tail = Calc.GenerateBullet(tailBullet, head.transform.position, head.direction);
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    
    IEnumerator<float> Snip(int ways) {
        var initDir = Vector2.SignedAngle(Vector2.right,PlayerCtrl.Player.transform.position - transform.position);
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            Calc.GenerateBullet(snipBullet, transform.position, dir);
            //yield return Calc.WaitForFrames(1);
        }
        yield return Calc.WaitForFrames(20);
        var offset = 360f / ways / 2;//Random.Range(2, 360f / ways - 2);
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i + offset;
            Calc.GenerateBullet(snipBullet, transform.position, dir);
        }

    }

    bool isClockWise = true;
    public IEnumerator<float> ShootClockwise(float initDir) {
        for (var i = 0; i < mainWays; i++) {
            // var dir = initDir + (isClockWise ? 1 : -1) * 360f / mainWays * i;
            var dir = initDir + 360f / mainWays * i;
            Timing.RunCoroutine(Shoot(dir, isClockWise), "Shoot");
            yield return Calc.WaitForFrames(4);
        }
        isClockWise = !isClockWise;
    }
    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(0, 360);
        
        Timing.RunCoroutine(ShootClockwise(initDir), "Shoot");
        
        yield return Calc.WaitForFrames(180);
        
        // Timing.RunCoroutine(ShootClockwise(initDir + 180 + Random,false));
        //
        // yield return Calc.WaitForFrames(90);
        
        Timing.RunCoroutine(Snip(snipWays), "Shoot");
        
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
