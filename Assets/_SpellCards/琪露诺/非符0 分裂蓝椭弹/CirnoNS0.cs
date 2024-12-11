using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class CirnoNS0 : BulletGenerator
{
    
    public DoubleSpeedApproach ellipse;

    public override IEnumerator<float> ShootSingleWave() {
        isEnchanting = true;
        int bulletNum = 18;
        
        for (int i = 0; i < bulletNum; i++) {
            var b = Calc.GenerateBullet(ellipse, transform.position, 360f / bulletNum * i);
            b.bulletState.SetColor(Color.cyan);
            if(i % 2 == 0) {
                b.bulletState.SetColor(Color.blue);
                b.speed /= 2;
            }
        }
        
        
        isEnchanting = false;
        yield break;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true)
        {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            //所有射击相关的协程都应拥有"Shoot"标签，便于在OnDisable()时统一取消
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
