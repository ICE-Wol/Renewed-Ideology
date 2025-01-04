using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class WakasagihimeSC0 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach riceBullet;
    public DoubleSpeedApproach jadeBullet;

    public Color scaleColor1;
    public Color scaleColor2;


    public bool[] trigger;
    public bool triggerMove;
    

    public IEnumerator<float> Trigger(DoubleSpeedApproach bullet,int triggerNum, float dir, float speed,int orderi,int orderj,Color oriColor,Color tarColor) {
        while (!trigger[triggerNum]) {
            yield return Timing.WaitForOneFrame;
        }
        //等待子弹变速完毕
        yield return Calc.WaitForFrames(30);
        trigger[triggerNum] = false;
        //bullet.SetSpeed(speed);
        bullet.endSpeed = speed / (2 - orderj/10f);
        var oriDirDiff = Mathf.Abs(dir - bullet.direction);
        float t = 0;
        float maxT = 100;
        while (!t.Equal(maxT,0.1f) /*&& !triggerMove*/){
            bullet.direction = bullet.direction.ApproachValue(dir, 8f);
            bullet.bulletState.SetColor(Color.Lerp(oriColor, tarColor, t / maxT));
            bullet.endSpeed = speed * (Mathf.Sin(Mathf.Deg2Rad * 10 * t) + 1f) / 2f;
            //不用dir差做参量是因为dir差可能是0
            t.ApproachRef(maxT, 8f + /*oriDirDiff*/orderj);
            yield return Timing.WaitForOneFrame;
        }

        //一次振奋人心的消弹，考虑在低难度加入
        if (bullet.bulletConfig.type == BulletType.Scale) {
            bullet.bulletState.SetState(EBulletStates.Destroying);
        }
        else {
            while (!triggerMove) {
                yield return Timing.WaitForOneFrame;
            }

            t = 0;
            //if (orderi == 0) orderi = Random.Range(-30, 30);
            while (t < 200) {
                if (bullet.endSpeed < speed / 2f)
                    bullet.endSpeed += 0.01f;
                //bullet.direction += orderi / 30f;
                t++;
                yield return Timing.WaitForOneFrame;
            }

            triggerMove = false;
        }
        // bullet.endSpeed = speed / 2f;
        // bullet.approachRate = 32f;


    }

    //public bool isClockwise;
    // public IEnumerator<float> Shoot() {
    //     float angleInterval = 3f;
    //     int frameInterval = 1;
    //     //三层之间的时间间隔
    //     int layerInterval = 5;
    //     float angle = 0f;
    //     int layer = 0;
    //     int maxLayer = 10;
    //     bool isLayerIncreasing = true;
    //     for (int t = 0; t < 3; t++) {
    //         for (int i = 0; i < 120; i++) {
    //             if (i % 20 == 0) {
    //                 AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
    //             }
    //
    //             for (int j = 0; j < layer; j++) {
    //                 float finalSpeed = 2f * (t + 1) + 0.2f * j - 0.1f * layer;
    //                 var bullet = (DoubleSpeedApproach)Calc.GenerateBullet(i % 2 == 0 ^ j % 2 == 0 ? scaleBullet : riceBullet,
    //                     -0.3f * angle.Deg2Dir3() + transform.position, angle);
    //                 bullet.speed = finalSpeed;
    //                 bullet.endSpeed = 0f;
    //                 bullet.bulletState.SetColor(Color.Lerp(scaleColor1, scaleColor2, (float)j / layer));
    //                 var tarColor = Color.Lerp(scaleColor1, scaleColor2, 1f - (float)j / layer);
    //
    //                 //与列相关
    //                 Timing.RunCoroutine(Trigger(bullet,t,
    //                     angle + /*(j % 2 == 0 ? 30 : -30) + /*(t % 2 == 0 ? 1f : -1f) * (isClockwise ? 3f : -3f) */5f *
    //                     (isLayerIncreasing ? -1 : 1) *
    //                     (j + 1) / 2f *
    //                     (maxLayer - layer),
    //                     finalSpeed,j, bullet.bulletConfig.color, tarColor).CancelWith(bullet.gameObject));
    //             }
    //
    //             if (layer == 0) {
    //                 isLayerIncreasing = true;
    //             }
    //             else if (layer == maxLayer) {
    //                 isLayerIncreasing = false;
    //             }
    //
    //             layer += isLayerIncreasing ? 1 : -1;
    //             angle += angleInterval;
    //
    //             yield return Calc.WaitForFrames(frameInterval);
    //         }
    //         yield return Calc.WaitForFrames(layerInterval);
    //     }
    //
    //
    //     yield return Calc.WaitForFrames(30);
    //     isClockwise = !isClockwise;
    //     yield return Calc.WaitForFrames(300);
    //     //trigger = true;
    //     
    // }

    public IEnumerator<float> Shoot2(float radius, int bigLayer, bool reverseColor) {
        trigger = new bool[bigLayer];
        float initAngle = Random.Range(0f, 360f);
        float angleInterval = 3f;
        int frameInterval = 1;
        //三层之间的时间间隔
        int layerInterval = 5;
        float angle = 0f;
        int layer = 0;
        int maxLayer = 10;
        
        int[] layers = new int[(int)(360f/angleInterval)];
        bool[] isLayerIncreasings = new bool[(int)(360f/angleInterval)];
        bool isLayerIncreasing = true;
        for (int i = 0; i < (int)(360f/angleInterval); i++) {
            layers[i] = layer;
            if (layer == 0) {
                isLayerIncreasing = true;
            }
            else if (layer == maxLayer) {
                isLayerIncreasing = false;
            }
            isLayerIncreasings[i] = isLayerIncreasing;
            layer += isLayerIncreasing ? 1 : -1;
        }

        for (int t = 0; t < bigLayer; t++) {
            reverseColor = !reverseColor;
            var spreadRange = Random.Range(3f, 5f);
            for (int j = 0; j < maxLayer; j++) {
                for (int i = 0; i < 360f/angleInterval; i++) {
                    if (j >= layers[i]) continue;
                    angle = i * angleInterval + initAngle + 30f * t;
                    //去掉2f 用对称弹形会有奇效，顺便记得把生成半径改大
                    float finalSpeed = 2f * (t + 1) + 0.2f * j - 0.1f * layers[i];
                    var bullet = (DoubleSpeedApproach)Calc.GenerateBullet(
                        i % 2 == 0 ^ j % 2 == 0 ? scaleBullet : riceBullet,
                        -radius * angle.Deg2Dir3() + transform.position, angle);
                    bullet.speed = finalSpeed;
                    bullet.endSpeed = 0f;
                    
                    Color tarColor;
                    if(!reverseColor){
                        bullet.bulletState.SetColor(Color.Lerp(scaleColor1, scaleColor2, (float)j / layers[i]));
                        tarColor = Color.Lerp(scaleColor1, scaleColor2, 1f - (float)j / layers[i]);
                        
                    }
                    else {
                        bullet.bulletState.SetColor(Color.Lerp(scaleColor2, scaleColor1, (float)j / layers[i]));
                        tarColor = Color.Lerp(scaleColor2, scaleColor1, 1f - (float)j / layers[i]);
                    } //angle += angleInterval;


                    int orderi = i % 20 - (int)(360 / angleInterval / 20);
                    //与列相关
                    Timing.RunCoroutine(Trigger(bullet,t,
                        angle + /*(j % 2 == 0 ? 30 : -30) + /*(t % 2 == 0 ? 1f : -1f) * (isClockwise ? 3f : -3f) */
                        /*(bullet.bulletConfig.type == BulletType.Rice ? -1f : 0f) +*/ spreadRange *
                        (isLayerIncreasings[i] ? -1 : 1) *
                        (j + 1) / 2f *
                        (maxLayer - layers[i]),
                        finalSpeed,orderi,j, bullet.bulletConfig.color, tarColor).CancelWith(bullet.gameObject));
                }
                radius += 0.02f;
                AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
                yield return Calc.WaitForFrames(layerInterval);
            }
            //yield return Calc.WaitForFrames(layerInterval * 2);
        }
        
        yield return Calc.WaitForFrames(60);
        for (int i = bigLayer - 1; i >= 0; i--) {
            trigger[i] = true;
            yield return Calc.WaitForFrames(15);
            AudioManager.Manager.PlaySound(AudioNames.SeShootBoon1);
        }
        yield return Calc.WaitForFrames(100);
        triggerMove = true;



    }

    public override IEnumerator<float> ShootSingleWave() {
        //Timing.RunCoroutine(Shoot2(-0.3f,3,false),"Shoot");
        Timing.RunCoroutine(Shoot2(1f,3,false),"Shoot");
        yield return 0;
    } 


    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
