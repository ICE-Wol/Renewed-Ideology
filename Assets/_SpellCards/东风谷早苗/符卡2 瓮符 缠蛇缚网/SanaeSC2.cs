using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class SanaeSC2 : BulletGenerator
{
    public DoubleSpeedApproach jadeSBullet;
    public DoubleSpeedApproach jadeRBullet;
    public DoubleSpeedApproach jadeMBullet;
    public TripleSpeedApproach scaleBullet;
    
    public DoubleSpeedApproach jadeGBullet;
    public DoubleSpeedApproach jadeR2Bullet;

    public IEnumerator<float> ShootFan(int ways,int times, int interval,float offset) {
        yield return Calc.WaitForFrames(210);
        for (int t = 0; t < times; t++) {
            for (int i = 0; i < ways; i++) {
                var dir = i * 360f / ways + t * offset;
                var bullet = Instantiate(i % 2 == 0 ? jadeGBullet : jadeR2Bullet, transform.position + dir.Deg2Dir3(),
                    Quaternion.identity);
                bullet.direction = dir;
            }
            yield return Calc.WaitForFrames(interval);
        }
    }
    public IEnumerator<float> ShootStem(float mainDir, float iDiv, float sinAmp,bool isRev,int liveFrame) {
        var bullet = Instantiate(jadeMBullet, transform.position, Quaternion.identity);
        for (int i = 0; i <= liveFrame; i++) {
            if (bullet == null) yield break;
            bullet.direction = mainDir + (isRev ? 1 : -1) * (i / iDiv + Mathf.Sin(i * 10f * Mathf.Deg2Rad) * sinAmp);
            if (i % 5 == 0) {
                Instantiate(i / 5 % 2 == 0 ? jadeSBullet : jadeRBullet, bullet.transform.position, Quaternion.identity)
                        .direction =
                    bullet.direction + (i / 5 % 2 == 0 ? 1 : -1) * 90f;
            }

            if (bullet.IsSpeedChangeFinished(0.1f)) break;
            yield return Calc.WaitForFrames(1);
        }
        
        Timing.RunCoroutine(ShootFlower(bullet.direction,bullet.transform.position,10,1f), "Shoot");
        
        bullet.GetComponent<State>().SetState(EBulletStates.Destroying);
    }

    IEnumerator<float> ShootFlower(float initDir, Vector3 oriPos,float ways,float offsetAngle) {
        var list = new List<TripleSpeedApproach>();
        for(int i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            var b = Instantiate(scaleBullet, oriPos + 0.3f * dir.Deg2Dir3(), Quaternion.identity);
            b.direction = dir;
            b.midSpeed = 1.8f;
            list.Add(b);
            
            dir = initDir + 360f / ways * i;
            b = Instantiate(scaleBullet, oriPos + 0.3f * dir.Deg2Dir3(), Quaternion.identity);
            b.direction = dir - offsetAngle;
            b.midSpeed = 1.6f;
            list.Add(b);
            
            dir = initDir + 360f / ways * i;
            b = Instantiate(scaleBullet, oriPos + 0.3f * dir.Deg2Dir3(), Quaternion.identity);
            b.direction = dir + offsetAngle;
            b.midSpeed = 1.6f;
            list.Add(b);
            
            b = Instantiate(scaleBullet, oriPos + 0.5f * (dir + 360f / ways / 2).Deg2Dir3(), Quaternion.identity);
            b.direction = dir + 360f / ways / 2;
            list.Add(b);
            
            b = Instantiate(scaleBullet, oriPos + 0.5f * (dir + 360f / ways / 2).Deg2Dir3(), Quaternion.identity);
            b.direction = dir + 360f / ways / 2 + offsetAngle;
            b.midSpeed = 1.3f;
            list.Add(b);
            
            b = Instantiate(scaleBullet, oriPos + 0.5f * (dir + 360f / ways / 2).Deg2Dir3(), Quaternion.identity);
            b.direction = dir + 360f / ways / 2 - offsetAngle;
            b.midSpeed = 1.3f;
            list.Add(b);
            yield return Calc.WaitForFrames(5);
        }
        
        yield return Calc.WaitForFrames(30);
        foreach (var b in list) {
            b.trigger = true;
        }
        yield return Calc.WaitForFrames(40);
        foreach (var b in list) {
            b.secondTrigger = true;
            b.trigger = false;
        }
    }

    public override IEnumerator<float> ShootSingleWave() {
        var mainDir = Calc.GetPlayerDirection(transform.position) + 90f;
        print(mainDir);
        Timing.RunCoroutine(ShootFan(18, 10, 15, 17), "Shoot");
        
        Timing.RunCoroutine(ShootStem(mainDir + 150,1.4f,20f,false,45), "Shoot");
        Timing.RunCoroutine(ShootStem(mainDir + 40,1.3f,60f,false,240), "Shoot");
        Timing.RunCoroutine(ShootStem(mainDir + 0,1.2f,40f,false,240), "Shoot");
        Timing.RunCoroutine(ShootStem(mainDir - 30,1.1f,20f,false,240), "Shoot");
        
        Timing.RunCoroutine(ShootStem(mainDir + 180 - 150,1.4f,20f,true,45), "Shoot");
        Timing.RunCoroutine(ShootStem(mainDir + 180 - 40,1.3f,60f,true,240), "Shoot");
        Timing.RunCoroutine(ShootStem(mainDir + 180,1.2f,40f,true,240),"Shoot");
        Timing.RunCoroutine(ShootStem(mainDir + 180 + 30,1.1f,20f,true,240), "Shoot");
        yield return 0;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true)
        {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            //所有射击相关的协程都应拥有"Shoot"标签，便于在OnDisable()时统一取消
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
