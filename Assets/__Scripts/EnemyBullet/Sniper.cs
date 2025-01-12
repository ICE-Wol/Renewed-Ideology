using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;
using MEC;

public class Sniper : BulletGenerator
{
    public BulletMovement bulletTemplate;
    

    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i <= layerCountInWave; i++) {
            var b = Instantiate(bulletTemplate);
            b.transform.position = transform.position + Calc.GetRandomVectorCircle(0,360,1f);
            b.direction =
                -Vector2.SignedAngle((PlayerCtrl.instance.transform.position - transform.position), Vector2.right);
            b.direction += Random.Range(-30, 30);
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public void Start() {
        Timing.RunCoroutine(AutoShoot());
    }
}
