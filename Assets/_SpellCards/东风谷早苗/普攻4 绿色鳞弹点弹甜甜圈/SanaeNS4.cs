using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class SanaeNS4 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach pointBullet;
    
    IEnumerator<float> Shoot3(bool isClockWise) {
        var radius = 0f;
        var circleWaveInterval = 10;
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

    public override IEnumerator<float> ShootSingleWave() {
        Timing.RunCoroutine(Shoot3(true), "Shoot");
        Timing.RunCoroutine(Shoot3(false), "Shoot");
        yield break;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
