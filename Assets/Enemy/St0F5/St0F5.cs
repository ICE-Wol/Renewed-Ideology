using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class St0F5 : BulletGenerator
{
    public DoubleSpeedApproach riceBullet;
    
    public int startTime;
    public int endTime;
    public int timer;
    

    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(0, 360);
        var addDir = 37;
        var bulletCnt = 12;
        var waveCount = 0;
        while (waveCount < 20) {
            for (int i = 0; i < bulletCnt; i++) {
                var dir = initDir + 360f / bulletCnt * i;
                dir = dir.GetDirBetweenPosAndNeg180() + 90f;
                var b = (DoubleSpeedApproach)Calc.GenerateBullet(riceBullet, transform.position, dir);
                b.transform.position = transform.position;
                Timing.RunCoroutine(ChangeBulletDir(b, dir / 10f, i).CancelWith(b.gameObject));
            }
            initDir += addDir;
            waveCount++;
            yield return Calc.WaitForFrames(5);
        }
    }
    
    public IEnumerator<float> ChangeBulletDir(DoubleSpeedApproach bullet, float maxDir,int i) {
        // var dirPerFrame = 0f;
        // var totDir = 0f;
        // while (totDir <= maxDir) {
        //     bullet.direction = bullet.direction.GetDirBetweenPosAndNeg180();
        //     if (bullet.direction > -90f && bullet.direction < 90f) {
        //         bullet.direction -= dirPerFrame;
        //         yield return Timing.WaitForOneFrame;
        //     }
        //     else {
        //         bullet.direction += dirPerFrame;
        //         yield return Timing.WaitForOneFrame;
        //     }
        //     totDir += dirPerFrame;
        //     dirPerFrame += 0.005f;
        // }
        int timer = 0;
        float dir = 0;
        Color initColor = bullet.bulletConfig.color;
        while (true /*&& dir <= maxDir*/) {
            var speedVector = bullet.endSpeed * bullet.direction.Deg2Dir();
            speedVector -= new Vector2(0, 0.006f);
            bullet.SetSpeed(speedVector.magnitude);
            bullet.direction = Vector2.SignedAngle(Vector2.right, speedVector);
            bullet.bulletState.SetColor(Calc.LerpColorInRGB(initColor, i % 2 == 0 ? Color.blue : Color.gray, dir));
            timer++;
            dir += 0.006f;
            yield return Timing.WaitForOneFrame;
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            if (endTime != 0 && timer > endTime) break;
            if (timer >= startTime) Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }

    // Update is called once per frame
    void Update() {
        timer++;
    }
}
