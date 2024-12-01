using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0SC4 : BulletGenerator
{
    public DoubleSpeedApproach JadeMBullet;
    public DoubleSpeedApproach ScaleBullet;
    
    
    
    public IEnumerator<float> Shoot(bool isRed) {
        //射击帧间隔
        var frameInterval = 5;
        //初始角度
        var initDir = Calc.GetPlayerDirection(transform.position) + (isRed ? 1 : -1) * 5f;
        //圈的最大way数，高于最大值保持不变
        var maxWay = 16;
        //最大射击圈数
        var maxShootTimes = 48;

        var curWay = 0;
        for (var i = 0; i < maxShootTimes; i++) {
            if(curWay < maxWay) curWay++;
            for (int j = 0; j < curWay; j++) {
                var dir = initDir + 360f / curWay * j;
                var bullet = Calc.GenerateBullet(JadeMBullet, transform.position, dir);
                bullet.GetComponent<State>().SetColor(isRed ? Color.red : Color.blue);
                Timing.RunCoroutine(ChangeBullet((DoubleSpeedApproach)bullet,isRed).CancelWith(bullet.gameObject),"Shoot");
            }
            yield return Calc.WaitForFrames(frameInterval);
        }
    }

    public IEnumerator<float> ChangeBullet(DoubleSpeedApproach bullet,bool isRed) {
        //发射瞬间速度为0 等待一帧后才设置好速度
        yield return Timing.WaitForOneFrame;
        while (true) {
            if (bullet.IsSpeedChangeFinished(0.1f)) {
                bullet.GetComponent<State>().SetState(EBulletStates.Destroying);
                Calc.GenerateBullet(ScaleBullet, bullet.transform.position, bullet.direction + (isRed ? 1 : -1) * 90f);
                yield break;
            }

            yield return Timing.WaitForOneFrame;
        }
        
    }

    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot(true),"Shoot");
        Timing.RunCoroutine(Shoot(false),"Shoot");
        yield return 0;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
