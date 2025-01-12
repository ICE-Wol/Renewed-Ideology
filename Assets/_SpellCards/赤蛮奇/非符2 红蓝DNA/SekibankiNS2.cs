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

public class SekibankiNS2 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet1;
    public DoubleSpeedApproach tailBullet2;
    public DoubleSpeedApproach snipBullet1;
    public DoubleSpeedApproach snipBullet2;
    public int mainWays;
    public int snipWays;

    IEnumerator<float> Shoot(float initDir) {
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        while (timer <= 360) {
            if(head == null) yield break;
            timer++;
            if(timer % 30 == 0) tarDir += Random.Range(-10, 10);
            head.direction.ApproachRef(tarDir, 16f);

            if (timer % 3 == 0) {
                Calc.GenerateBullet(tailBullet1, head.transform.position + 0.3f * Mathf.Sin(timer * 3 * Mathf.Deg2Rad)*(tarDir + 90).Deg2Dir3(), head.direction);
                Calc.GenerateBullet(tailBullet2, head.transform.position + 0.3f * Mathf.Sin((timer * 3 + 45) * Mathf.Deg2Rad)*(tarDir - 90).Deg2Dir3(), head.direction);
            }

            yield return Calc.WaitForFrames(1);
        }
    }

    bool isRed = true;
    IEnumerator<float> Snip(int ways) {
        var initDir = Vector2.SignedAngle(Vector2.right,PlayerCtrl.instance.transform.position - transform.position);
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
        }
        isRed = !isRed;
        yield return Calc.WaitForFrames(20);
        var offset = 360f / ways / 2;
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i + offset;
            Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
        }
        isRed = !isRed;
        yield return Calc.WaitForFrames(20);
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
        }
        isRed = !isRed;

    }

    IEnumerator<float> Snip2(int ways,float offsetDeg) {
        var initDir = Vector2.SignedAngle(Vector2.right,PlayerCtrl.instance.transform.position - transform.position);
        for (int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            var b0 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            isRed = !isRed;
            
            var b1 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            var b2 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            isRed = !isRed;
            
            var b3 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            var b4 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            isRed = !isRed;
            
            var b5 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            var b6 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            isRed = !isRed;
            
            var b7 = Calc.GenerateBullet(isRed ? snipBullet1 : snipBullet2, transform.position, dir);
            isRed = !isRed;
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b1, offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.1f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b2, -offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.1f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b3, 2*offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.2f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b4, -2*offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.2f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b5, offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.3f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b6, -offsetDeg, 60, ((DoubleSpeedApproach)b0).endSpeed-0.3f).CancelWith(b1.gameObject), "Shoot");
            Timing.RunCoroutine(Trigger((DoubleSpeedApproach)b7, 0, 60, ((DoubleSpeedApproach)b0).endSpeed-0.4f).CancelWith(b1.gameObject), "Shoot");
        }

        yield return 0;
    }

    IEnumerator<float> Trigger(DoubleSpeedApproach bullet, float offsetDeg, int waitTime, float endSpeed) {
        yield return Calc.WaitForFrames(waitTime);
        bullet.direction += offsetDeg;
        bullet.endSpeed = endSpeed;
    }

    bool isClockWise = true;
    public IEnumerator<float> ShootClockwise(float initDir) {
        for (var i = 0; i < mainWays; i++) {
            // var dir = initDir + (isClockWise ? 1 : -1) * 360f / mainWays * i;
            var dir = initDir + 360f / mainWays * i;
            Timing.RunCoroutine(Shoot(dir), "Shoot");
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
        
        Timing.RunCoroutine(Snip2(snipWays,4f), "Shoot");
        
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
