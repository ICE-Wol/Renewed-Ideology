using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class WakasagihimeNS1 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach riceBullet;
    public DoubleSpeedApproach jadeBullet;

    //奇偶换向换色
    public IEnumerator<float> Shoot(float startAngle,float offset,bool isClockwise) {
        float angleInterval = 14f; //21f
        float bulletNum = 10;
        float innerRadius = 0.5f;
        int frameInterval = 5;
        float speedMultiplier = 4f;
        for (int i = 0; i < bulletNum; i++) {
            var color = Color.clear;
            if (isClockwise) color = Color.Lerp(Color.red, Color.magenta, i / bulletNum);
            else color = Color.Lerp(Color.magenta, Color.red, i / bulletNum);
            var dir = (startAngle + angleInterval * i);
            var bullet = Calc.GenerateBullet(scaleBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir);
            bullet.speed = (Mathf.Sin(i * Mathf.Deg2Rad) + 1f) * speedMultiplier;
            speedMultiplier -= 0.4f;
            bullet.bulletState.SetColor(color);
            Timing.RunCoroutine(
                GenSubBullet(bullet as DoubleSpeedApproach, i, offset, color).CancelWith(bullet.gameObject), "Shoot");

            if (i % 2 == 0) {
                var dir2 = -dir - 180f;
                bullet = Calc.GenerateBullet(jadeBullet, transform.position + innerRadius * dir2.Deg2Dir3(), dir2);
                bullet.bulletState.SetColor(color / 4f + Color.white * 3f / 4f);
                Timing.RunCoroutine(RotateSubBullet(bullet as DoubleSpeedApproach, -0.2f).CancelWith(bullet.gameObject),
                    "Shoot");
            }

            yield return Calc.WaitForFrames(frameInterval);
        }
    }

    public IEnumerator<float> GenSubBullet(DoubleSpeedApproach parent,int num,float offset,Color color) {
        yield return Timing.WaitForOneFrame;
        while (true) {
            if (parent.IsSpeedChangeFinished(0.1f)) {
                for (int i = 1; i <= 3; i++) {
                    var bullet = Calc.GenerateBullet(riceBullet, parent.transform.position,
                        parent.transform.eulerAngles.z);
                    bullet.speed = i * 1.5f + num * 1.5f - 1f;
                    (bullet as DoubleSpeedApproach).endSpeed = 2f - num / 100f;
                    bullet.bulletState.SetColor(color / 2f);
                    Timing.RunCoroutine(RotateSubBullet(bullet as DoubleSpeedApproach,0.2f).CancelWith(bullet.gameObject),
                        "Shoot");
                }
                parent.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
    }

    public IEnumerator<float> RotateSubBullet(DoubleSpeedApproach bullet,float rotDir) {
        var curRotDir = 5f;
        while (true) {
            curRotDir.ApproachRef(rotDir, 8f);
            bullet.direction -= curRotDir;
            yield return Timing.WaitForOneFrame;
        }
    }

    private float _startAngle;
    public float decDec = 0.10f;
    private bool _isClockwise;
    public override IEnumerator<float> ShootSingleWave() {
        float initDir = 0f;//Random.Range(-30, 30);
        float decent = 10f;//10f -=0.25f
        for (int i = 0; i <= 360 / 15; i++) {
            Timing.RunCoroutine(Shoot(_startAngle + i * 15, initDir + i * decent, _isClockwise), "Shoot");
            decent -= decDec;
            yield return Calc.WaitForFrames(5);   
        }
    } 
    
    public IEnumerator<float> ShootSingleWave2() {
        float initDir = 0f;//Random.Range(-30, 30);
        float decent = 10f;//10f -=0.25f
        for (int i = 0; i <= 360 / 15; i++) {
            Timing.RunCoroutine(Shoot(_startAngle + 180f + i * 15, initDir + i * decent, _isClockwise), "Shoot");
            decent -= decDec;
            yield return Calc.WaitForFrames(5);   
        }
    } 

    public override IEnumerator<float> AutoShoot() {
        _startAngle = Random.Range(0, 360);
        _isClockwise = true;
        while (true) {
            _isClockwise = !_isClockwise;
            _startAngle += 60f;
            Timing.RunCoroutine(ShootSingleWave(),"Shoot");
            Timing.RunCoroutine(ShootSingleWave2(),"Shoot");
            
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
