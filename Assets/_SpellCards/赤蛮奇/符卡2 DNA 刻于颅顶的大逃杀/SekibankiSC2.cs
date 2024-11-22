using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using State = _Scripts.EnemyBullet.State;

public class SekibankiSC2 : BulletGenerator
{
    [Header("网格限位射击")] public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet1;
    public DoubleSpeedApproach tailBullet2;
    public int mainWays;
    public float degInterval;

    [Header("增way风车射击")]
    public DoubleSpeedApproach scaleBullet;

    public Color scaleColor1;
    public Color scaleColor2;
    
    [Header("自机相关圆圈射击")]
    public DoubleSpeedApproach pointBullet;

    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(-30, 30);

        Timing.RunCoroutine(ShootClockwise(initDir, degInterval), "Shoot");

        yield return Calc.WaitForFrames(180);
    }

    public override IEnumerator<float> AutoShoot() {
        var isClockWise = true;
        while (true) {
            //Timing.RunCoroutine(ShootSingleWave(), "Shoot");

            //Timing.RunCoroutine(Shoot2(isClockWise), "Shoot");
            //isClockWise = !isClockWise;
            Timing.RunCoroutine(Shoot3(true), "Shoot");
            Timing.RunCoroutine(Shoot3(false), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }

    public IEnumerator<float> ShootClockwise(float initDir, float degInterval) {
        for (var i = 0; i < mainWays; i++) {
            var dir = initDir + degInterval * i;
            Timing.RunCoroutine(Shoot(dir, false), "Shoot");
            yield return Calc.WaitForFrames(15);
        }

        for (var i = 0; i < mainWays; i++) {
            var dir = initDir + degInterval * i;
            Timing.RunCoroutine(Shoot(dir, true), "Shoot");
            yield return Calc.WaitForFrames(15);
        }

    }

    /// <summary>
    /// 上一刻贝塞尔曲线的准确位置,直接用transform.position会溢出到奇怪的位置导致子弹错位
    /// </summary>
    private Vector3 _prePos;

    IEnumerator<float> Shoot(float initDir, bool isClockWise) {
        _prePos = transform.position;
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        var tScale = 360f;
        DoubleSpeedApproach[] tails = new DoubleSpeedApproach[20];
        while (timer <= 360) {
            if (head == null) yield break;
            timer++;

            if (isClockWise) {
                var nxtPos = Calc.GetCubicBezierPoint(transform.position,
                    transform.position + 5f * (initDir + 40f).Deg2Dir3(),
                    transform.position + 5f * (initDir + 10f).Deg2Dir3(),
                    transform.position + 8f * (-initDir - 70f).Deg2Dir3(),
                    timer, tScale);
                head.direction = Calc.GetDirection(_prePos, nxtPos);
                _prePos = nxtPos;
            }
            else {
                var nxtPos = Calc.GetCubicBezierPoint(transform.position,
                    transform.position + 5f * (180f - (initDir + 40f)).Deg2Dir3(),
                    transform.position + 5f * (180f - (initDir + 10f)).Deg2Dir3(),
                    transform.position + 9f * (180f - (-initDir - 70f)).Deg2Dir3(),
                    timer, tScale);
                head.direction = Calc.GetDirection(_prePos, nxtPos);
                // print(head.transform.position +" "+ nxtPos +" "+  head.direction);
                // Debug.DrawLine(head.transform.position,nxtPos,Color.red,100f);
                _prePos = nxtPos;
                // Debug.DrawLine(nxtPos, nxtPos + 0.01f * Vector3.right, Color.green, 100f);
            }

            head.transform.position = _prePos;


            if (timer % 20 >= 5 && timer % 2 == 0) {
                DoubleSpeedApproach b =
                    (DoubleSpeedApproach)Calc.GenerateBullet(isClockWise ? tailBullet1 : tailBullet2,
                        head.transform.position, head.direction + (isClockWise ? 90f : -90f));
                if (timer / 20 % 2 == 0) b.endSpeed = 2f;
                else b.endSpeed = -2f;
                Timing.RunCoroutine(Fade(b, 120).CancelWith(b.gameObject), "Shoot");
                // tails[timer % 20] = b;
                // if (timer % 20 == 19 && timer >= 260) { 
                //     var arr = tails.Clone();
                //     Timing.RunCoroutine(MoveTail((DoubleSpeedApproach[])arr,120), "Shoot");
                // }
                //b.direction = Calc.GetPlayerDirection(b.transform.position);
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    IEnumerator<float> Fade(DoubleSpeedApproach b, int time) {
        yield return Calc.WaitForFrames(time);
        b.GetComponent<Highlight>().Fade();
    }

    IEnumerator<float> MoveTail(DoubleSpeedApproach[] arr, int time) {
        yield return Calc.WaitForFrames(time);
        foreach (var b in arr) {
            if (b == null) continue;
            b.trigger = true;
            b.GetComponent<Highlight>().Fade();
        }
    }

    IEnumerator<float> Shoot2(bool isClockWise) {
        yield return Calc.WaitForFrames(210);

        var startDir = Random.Range(0, 360f);
        var radius = 1f;
        var smallInterval = 3f;
        var bigInterval = 12f;
        var bulletSetSize = 5;
        var bulletSizeNum = 15;
        var waitTime = 15;

        var curDir = startDir;
        for (int i = 0; i < bulletSizeNum; i++) {
            var maxSet = i; // > 10 ? 20 - i : i;
            for (int k = 0; k <= maxSet; k++) {
                for (int j = 0; j < bulletSetSize; j++) {
                    var bullet = (DoubleSpeedApproach)Calc.GenerateBullet(scaleBullet,
                        transform.position + radius * curDir.Deg2Dir3(), curDir);
                    Timing.RunCoroutine(ChangeColor(bullet).CancelWith(bullet.gameObject), "Shoot");
                    bullet.direction = curDir;
                    bullet.endSpeed = 1f + (3 - Mathf.Abs(j - 2)) / 5f;
                    bullet.trigger = true;
                    curDir -= smallInterval;
                }

                curDir -= bigInterval;
            }

            curDir = startDir + (isClockWise ? 1 : -1) * 2 * (i + 1) * bigInterval;
            yield return Calc.WaitForFrames(waitTime);
        }
    }

    IEnumerator<float> ChangeColor(DoubleSpeedApproach bullet) {
        for (int t = 0; t < 100; t++) {
            var color = Calc.LerpColorInHSV(scaleColor1, scaleColor2, t / 100f);
            bullet.GetComponent<State>().SetColor(color);
            yield return Timing.WaitForOneFrame;
        }

    }

    IEnumerator<float> Shoot3(bool isClockWise) {
        var radius = 0f;
        var circleWaveInterval = 10;
        var rotateInterval = 20;
        var circleNum = 72;
        var bulletNumInCicle = 24;
        
        
        //求出玩家距离和方向，方便后续绕发射器旋转目标点
        var playerPoint = PlayerCtrl.Player.transform.position;
        var playerDist = Vector2.Distance(transform.position, playerPoint);
        if(playerDist < 4f) playerDist = 4f;
        var playerDir = Calc.GetPlayerDirection(transform.position);
        for (int i = 0; i < circleNum; i++) {
            radius = Mathf.Sin(i * 30 * Mathf.Deg2Rad) / 2f + 1f; //i / 4f;
            var newPoint = transform.position
                           + playerDist * (playerDir + (isClockWise ? 180 : 0) + i * 13).Deg2Dir3();
            for (int j = 0; j < bulletNumInCicle; j++) {
                var dir = 360f / bulletNumInCicle * j;
                DoubleSpeedApproach bullet;
                if (j % 2 == 0) {
                    bullet = (DoubleSpeedApproach)Calc.GenerateBullet(scaleBullet,
                        transform.position + radius * dir.Deg2Dir3(), 0);
                }
                else {
                    bullet = (DoubleSpeedApproach)Calc.GenerateBullet(pointBullet,
                        transform.position + radius / 3f * dir.Deg2Dir3(), 0);
                }

                //0.5f - 0.9f
                var lerpValue = 0.7f + 0.2f * Mathf.Sin(i * 30 * Mathf.Deg2Rad);
                lerpValue = 0.8f;
                bullet.direction =
                    Calc.GetDirection(bullet.transform.position, Calc.Lerp(transform.position, newPoint, lerpValue));
                bullet.endSpeed = 1f;
                bullet.trigger = true;
            }

            yield return Calc.WaitForFrames(circleWaveInterval);
        }
    }
}