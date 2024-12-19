using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class WakasagihimeNS0 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach riceBullet;

    //奇偶换向换色
    public IEnumerator<float> Shoot(float startAngle,float offset,bool isClockwise) {
        float angleInterval = 14f;
        float bulletNum = 10;
        float innerRadius = 0.5f;
        int frameInterval = 5;
        float speedMultiplier = 5f;
        for (int i = 0; i < bulletNum; i++) {
            var color = Color.clear;
            if (isClockwise) color = Color.Lerp(Color.blue, Color.cyan, i / bulletNum);
            else color = Color.Lerp(Color.cyan, Color.blue, i / bulletNum);
            var dir = (startAngle + (isClockwise ? 1 : -1) * angleInterval * i);
            var bullet = Calc.GenerateBullet(scaleBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir);
            bullet.speed = (Mathf.Sin(((isClockwise ? 1 : -1) * angleInterval * i + startAngle / 4f) * Mathf.Deg2Rad) + 1f) * speedMultiplier;
            speedMultiplier -= 0.2f;
            bullet.bulletState.SetColor(color);
            Timing.RunCoroutine(
                GenSubBullet(bullet as DoubleSpeedApproach, i, offset, color).CancelWith(bullet.gameObject), "Shoot");
            yield return Calc.WaitForFrames(frameInterval);
        }
    }

    public IEnumerator<float> GenSubBullet(DoubleSpeedApproach parent,int num,float offset,Color color) {
        yield return Timing.WaitForOneFrame;
        while (true) {
            if (parent.IsSpeedChangeFinished(0.01f)) {
                for (int i = 1; i <= 1; i++) {
                    var bullet = Calc.GenerateBullet(riceBullet, parent.transform.position,
                        parent.transform.eulerAngles.z + offset * i);
                    bullet.speed = i + num / 2f;
                    //(bullet as DoubleSpeedApproach).endSpeed = 1f - num / 100f;
                    bullet.bulletState.SetColor(color / 2f);
                }
                parent.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
    }

    private float _startAngle;
    public float decDec = 0.10f;
    private bool _isClockwise;
    public override IEnumerator<float> ShootSingleWave() {
        float initDir = 0f;//Random.Range(-30, 30);
        float decent = 10f;//10f -=0.25f
        for (int i = 0; i <= 360 / 9/2; i++) {
            Timing.RunCoroutine(Shoot(_startAngle + i * 9, initDir + i * decent, _isClockwise), "Shoot");
            decent -= decDec;
            yield return Calc.WaitForFrames(5);   
        }
    } 
    public IEnumerator<float> ShootSingleWave2() {
        float initDir = Random.Range(-30, 30);
        float decent = 10f;//10f -=0.25f
        for (int i = 0; i <= 360 / 9/2; i++) {
            Timing.RunCoroutine(Shoot(_startAngle + i * 9 + 180f, initDir + i * decent, _isClockwise), "Shoot");
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
            //yield return Calc.WaitForFrames(30);
            Timing.RunCoroutine(ShootSingleWave2(),"Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
