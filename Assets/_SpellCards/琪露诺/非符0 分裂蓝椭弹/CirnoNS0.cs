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
    public DoubleSpeedApproach ice;

    public override IEnumerator<float> ShootSingleWave() {
        isEnchanting = true;
        int bulletNum = 10;
        float initDir = Random.Range(0, 360);
        
        for (int i = 0; i < bulletNum; i++) {
            var b = Calc.GenerateBullet(ellipse, transform.position, initDir + 360f / bulletNum * i);
            b.bulletState.SetColor(Color.cyan);
            if(i % 2 == 0) {
                b.bulletState.SetColor(Color.blue);
                b.speed /= 2;
            }
            Timing.RunCoroutine(BulletChange(b as DoubleSpeedApproach, 0).CancelWith(b.gameObject), "Shoot");
            
        }
        
        
        isEnchanting = false;
        yield break;
    }
    
    public IEnumerator<float> BulletChange(DoubleSpeedApproach bullet, int gen){
        yield return Timing.WaitForOneFrame;
        while(true){
            if(gen >= 3){
                var max = (bullet.bulletConfig.color == Color.blue) ? 5 : 10;
                for(int i = 0;i < max;i++){
                    var b = Calc.GenerateBullet(ice, bullet.transform.position, Mathf.Lerp(bullet.direction - 120f, bullet.direction + 120f, (float)i / (max-1)));
                    b.bulletState.SetColor(bullet.bulletConfig.color);
                }
                bullet.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            if(bullet.IsSpeedChangeFinished(0.1f)){
                //print(bullet.GetSpeed() + " " + bullet.endSpeed);
                var b1 = Calc.GenerateBullet(ellipse, bullet.transform.position, bullet.direction + 30f);
                var b2 = Calc.GenerateBullet(ellipse, bullet.transform.position, bullet.direction - 30f);
                b1.speed = bullet.speed;
                b2.speed = bullet.speed;
                b1.bulletState.SetColor(bullet.bulletConfig.color);
                b2.bulletState.SetColor(bullet.bulletConfig.color);
                Timing.RunCoroutine(BulletChange(b1 as DoubleSpeedApproach, gen + 1).CancelWith(b1.gameObject), "Shoot");
                Timing.RunCoroutine(BulletChange(b2 as DoubleSpeedApproach, gen + 1).CancelWith(b2.gameObject), "Shoot");
                bullet.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
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
