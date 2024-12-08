using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class St0F2 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public Color scaleColor1;
    public Color scaleColor2;
    
    public int startTime;
    public int endTime;
    public int timer;

    public bool isClockWise;
    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot2(isClockWise).CancelWith(gameObject),"Shoot");
        isClockWise = !isClockWise;
        yield break;
    }
    
    IEnumerator<float> Shoot2(bool isClockWise) {
        yield return Calc.WaitForFrames(120);

        var startDir = Random.Range(0, 360f);
        var radius = 0.5f;
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

    public override IEnumerator<float> AutoShoot() {
            while (true) {
                if(endTime != 0 && timer > endTime) break;
                if (timer >= startTime) Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject), "Shoot");
                yield return Calc.WaitForFrames(waveFrameInterval);
            }
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}
