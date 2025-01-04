using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class WakasagihimeSC1 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach riceBullet;
    public DoubleSpeedApproach jadeBullet;

    public Color scaleColor1;
    public Color scaleColor2;


    public bool[] trigger;
    public bool triggerMove;
    

    public IEnumerator<float> Trigger(DoubleSpeedApproach bullet,Color color, int order,int bulletSetNum,int waitTime) {
        yield return Calc.WaitForFrames(waitTime);
        
        var rice = (DoubleSpeedApproach)Calc.GenerateBullet(riceBullet, bullet.transform.position, bullet.direction);
        rice.bulletState.SetColor(color);
        
        int orderInSet = order % bulletSetNum - bulletSetNum / 2;
        rice.direction += orderInSet * 5f + 180f;
        rice.endSpeed += (1f + bulletSetNum / 2f - Mathf.Abs(orderInSet)) * 0.5f;
        
        bullet.bulletState.SetState(EBulletStates.Destroying);

        rice.direction += orderInSet * 0.5f;

        yield return Calc.WaitForFrames(1);
        rice.approachRate = 4f;

        for(int i = 0; i < 360; i++) {
            rice.endSpeed = 0;//Mathf.Sin(Mathf.Deg2Rad * (i + 90f)) * 2f;
            yield return Timing.WaitForOneFrame;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t">layer</param>
    /// <returns></returns>
    public IEnumerator<float> ShootSingleLayer(int t, bool isClockwise) {
        var dir = 180f;
        for (int i = 0; i < bulletNum / 2; i++) {
            dir += (isClockwise ? 1 : -1) * 360f / bulletNum;
            //状态变量不要放在循环里面，否则continue的时候状态不会按预期变化
            if (i <= 5 || bulletNum / 2 - 1 - i <= 6) continue;
            var bullet = (DoubleSpeedApproach)Calc.GenerateBullet(scaleBullet, transform.position, -dir);
            bullet.speed = (5 - t / 5f) * radius * Mathf.Sin(Mathf.Deg2Rad * dir);
            var color = Color.Lerp(scaleColor2,scaleColor1,t / (float)(maxLayer));
            bullet.bulletState.SetColor(color);
            Timing.RunCoroutine(Trigger(bullet,color, i + 9/2 * t, 9, 200 - i + t * 15).CancelWith(bullet.gameObject), "Trigger");
            yield return Timing.WaitForOneFrame;
        }
    }
    
    
    float radius = 1.5f;
    int bulletNum = 360;
    int maxLayer = 10;
    int layerInterval = 10;
    public IEnumerator<float> Shoot() {
        
        //好看的图形，可以考虑留着当非符用
        
        // for (int i = 0; i < 180; i++) {
        //     var bullet =
        //         (DoubleSpeedApproach)Calc.GenerateBullet(scaleBullet, transform.position + radius * 1.5f * dir.Deg2Dir3(),
        //             dir);
        //     bullet.speed = 2 * radius * Mathf.Cos(dir);
        //     bullet.endSpeed = 0;
        //     dir += 360f / bulletNum;
        //     yield return Timing.WaitForOneFrame;
        // }
        for (int t = 0; t < maxLayer; t++) {
            Timing.RunCoroutine(ShootSingleLayer(t,t % 2 == 0),"Shoot");
            AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
            yield return Calc.WaitForFrames(layerInterval);
        }
        yield return 0;
    }

    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot(),"Shoot");
        yield return 0;
    } 


    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}