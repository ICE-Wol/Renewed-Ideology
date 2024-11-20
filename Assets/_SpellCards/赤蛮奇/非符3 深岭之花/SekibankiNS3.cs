using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class SekibankiNS3 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tailBullet;
    public DoubleSpeedApproach snipBullet;
    public Color tailColor1;
    public Color tailColor2;
    public int mainWays;
    public int snipWays;

    IEnumerator<float> Shoot(float initDir, float initSpd, bool isClockwise, bool isColor1)
    {
        var head = Instantiate(headBullet, transform.position + 0.5f * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        head.speed = initSpd;

        var tarDir = initDir;
        var timer = 0;
        var offset = 3f;
        while (timer <= 180)
        {
            if (head == null) yield break;
            timer++; //30帧再开始变换方向
            if (timer < 90)
            {
                tarDir += (isClockwise ? -1.5f : 1.5f) * Mathf.Sin(timer * 3 * Mathf.Deg2Rad);
                tarDir += (isClockwise ? 1f : 0.5f) * (1f - initSpd / 6f);
            }
            else if (timer == 90)
            {
                head.endSpeed += 2f;
            }
            else
            {
                tarDir += isClockwise ? offset : -offset;
                offset.ApproachRef(offset * 3, 64f);
                if (offset >= offset * 3)
                {
                    Destroy(head.gameObject);
                    yield break;
                }
            }
            if (timer % 30 == 0) tarDir += Random.Range(-10, 10);
            head.direction.ApproachRef(tarDir, 16f);

            if (timer % 1 == 0)
            {
                Calc.GenerateBullet(tailBullet, head.transform.position, head.direction).GetComponent<State>()
                    .SetColor(isColor1 ? tailColor1 : tailColor2);
            }

            yield return Calc.WaitForFrames(1);
        }
    }

    IEnumerator<float> Snip(int ways, int order, bool isMagenta)
    {
        var initDir = Vector2.SignedAngle(Vector2.right, PlayerCtrl.Player.transform.position - transform.position);
        DoubleSpeedApproach[,] bullets = new DoubleSpeedApproach[ways, 2];
        var color = Color.HSVToRGB((isMagenta ? 300 : 250) / 360f, (4 - order) / 3f, 1f);

        for (int i = 0; i < ways; i++)
        {
            var dir = initDir + 360f / ways * i;
            bullets[i, 0] = (DoubleSpeedApproach)Calc.GenerateBullet(snipBullet, transform.position, dir);
            bullets[i, 0].GetComponent<State>().SetColor(color);
        }

        yield return Calc.WaitForFrames(30);

        for (int i = 0; i < ways; i++)
        {
            bullets[i, 0].endSpeed = (4 - Mathf.Abs(i % 7 - 3)) / (4f + order) + 1f;
        }

        for (int t = 0; t < 90; t++)
        {
            for (int i = 0; i < ways; i++)
            {
                if (bullets[i, 0] == null) continue;
                bullets[i, 0].direction += (i % 7 - 3) * (0.1f - order * 0.02f);
            }

            yield return Calc.WaitForFrames(3);
        }
    }

    bool isClockWise = true;
    public IEnumerator<float> ShootClockwise(float initDir, float initSpd)
    {
        for (var i = 0; i < mainWays; i++)
        {
            var dir = initDir + 72f * i; // 360f / mainWays * i;
            Timing.RunCoroutine(Shoot(dir, initSpd /*i % 2 == 0 ? 6f : 3f*/, isClockWise, i % 2 == 0), "Shoot");
            yield return Calc.WaitForFrames(37); //4);
        }

        mainDir += 72f * (mainWays);
    }

    private float mainDir;
    public override IEnumerator<float> ShootSingleWave()
    {
        mainDir = Random.Range(0, 360);

        Timing.RunCoroutine(ShootClockwise(mainDir, 6f), "Shoot");
        yield return Calc.WaitForFrames(13);
        Timing.RunCoroutine(ShootClockwise(mainDir + 14, 4f), "Shoot");
        yield return Calc.WaitForFrames(13);
        Timing.RunCoroutine(ShootClockwise(mainDir + 28, 2f), "Shoot");
        yield return Calc.WaitForFrames(13);
        Timing.RunCoroutine(ShootClockwise(mainDir + 42, 0.5f), "Shoot");

        yield return Calc.WaitForFrames(180);

        Timing.RunCoroutine(Snip(snipWays, 0, isClockWise), "Shoot");
        yield return Calc.WaitForFrames(30);
        Timing.RunCoroutine(Snip(snipWays, 1, isClockWise), "Shoot");
        yield return Calc.WaitForFrames(30);
        Timing.RunCoroutine(Snip(snipWays, 2, isClockWise), "Shoot");
        yield return Calc.WaitForFrames(30);
        Timing.RunCoroutine(Snip(snipWays, 3, isClockWise), "Shoot");
        yield return Calc.WaitForFrames(30);
        Timing.RunCoroutine(Snip(snipWays, 4, isClockWise), "Shoot");

        isClockWise = !isClockWise;

    }

    public override IEnumerator<float> AutoShoot()
    {
        while (true)
        {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            //所有射击相关的协程都应拥有"Shoot"标签，便于在OnDisable()时统一取消
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
