using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class SekibankiNS0 : BulletGenerator
{
    public DoubleSpeedApproach headBullet;
    public DoubleSpeedApproach tail1Bullet;
    public DoubleSpeedApproach tail2Bullet;
    public TripleSpeedApproach snip1Bullet;
    public TripleSpeedApproach snip2Bullet;
    public int ways;

    IEnumerator<float> Shoot(float initDir, bool isTail1) {
        var head = Instantiate(headBullet, transform.position + (isTail1 ? 1f : 0.5f) * initDir.Deg2Dir3(),
            Quaternion.Euler(0, 0, initDir));
        head.direction = initDir;
        var tarDir = initDir;

        var timer = 0;
        while (true) {
            if(head == null) yield break;
            timer++;//30帧再开始变换方向
            if(timer % 30 == 0) tarDir += Random.Range(-20, 20);
            head.direction.ApproachRef(tarDir, 16f);
            var tail = Instantiate(isTail1 ? tail1Bullet : tail2Bullet, head.transform.position, Quaternion.Euler(0, 0, head.direction));
            tail.direction = head.direction;
            yield return Calc.WaitForFrames(1);
        }
    }

    
    /// <summary>
    /// 扇形狙击弹幕
    /// </summary>
    /// <param name="sideWays">类似于 a b c d e子弹，c为中间值，d e为边路数量</param>
    /// <param name="isSnip1">snip1 snip2颜色不同，作为区分</param>
    /// <param name="t">时间戳，按外层循环变量i取值，用于整理子弹z值</param>
    /// <returns></returns>
    IEnumerator<float> Snip(int sideWays,float degInterval,float startSpeed,float endSpeed,float speedInterval,bool isSnip1,int t) {
        var initDir = Vector2.SignedAngle(Vector2.right,PlayerCtrl.instance.transform.position - transform.position);
        var bullet = Instantiate(isSnip1 ? snip1Bullet : snip2Bullet, transform.position, Quaternion.Euler(0, 0, initDir));
        bullet.direction = initDir;
        bullet.speed = startSpeed;
        bullet.endSpeed = endSpeed;
        bullet.GetComponent<SpriteRenderer>().sortingOrder = t;
        for (var i = 0; i < sideWays; i++) {
            var dir = initDir + (i + 1) * degInterval;
            var b = Instantiate(isSnip1 ? snip1Bullet : snip2Bullet, transform.position, Quaternion.Euler(0, 0, dir));
            b.speed = startSpeed;
            b.direction = dir;
            b.endSpeed = endSpeed - (i + 1) * speedInterval; 
            //b.transform.position.SetZ(t);
            b.GetComponent<SpriteRenderer>().sortingOrder = t;
            
            dir = initDir - (i + 1) * degInterval;
            b = Instantiate(isSnip1 ? snip1Bullet : snip2Bullet, transform.position, Quaternion.Euler(0, 0, dir));
            b.speed = startSpeed;
            b.direction = dir;
            b.endSpeed = endSpeed - (i + 1) * speedInterval;
            //b.transform.position.SetZ(t);
            b.GetComponent<SpriteRenderer>().sortingOrder = t;
        }
        yield return Calc.WaitForFrames(15);
    }

    
    bool isSnip1 = true;
    public override IEnumerator<float> ShootSingleWave() {
        var initDir = Random.Range(0, 360);
        for (var i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways * i;
            Timing.RunCoroutine(Shoot(dir, true), "Shoot");
        }
        
        yield return Calc.WaitForFrames(15);
        
        for (var i = 0; i < ways; i++) {
            var dir = initDir + 360f / ways / 2f + 360f / ways * i;
            Timing.RunCoroutine(Shoot(dir, false), "Shoot");
        }

        for (int i = 0; i < 5; i++) {
            Timing.RunCoroutine(Snip(5, 10, 5 - i / 2f, 3 - 0.2f * i, 0.2f + 0.02f * i, isSnip1 = !isSnip1,i), "Shoot");
            yield return Calc.WaitForFrames(5);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
