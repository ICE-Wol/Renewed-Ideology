using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
//
// public class WakasagihimeNS0 : BulletGenerator
// {
//     public DoubleSpeedApproach scaleBullet;
//     public DoubleSpeedApproach riceBullet;
//     public DoubleSpeedApproach jadeBullet;
//
//     //奇偶换向换色
//     public IEnumerator<float> Shoot(float startAngle,float offset,bool isClockwise) {
//         float angleInterval = 14f;
//         float bulletNum = 10;
//         float innerRadius = 0.5f;
//         int frameInterval = 5;
//         float speedMultiplier = 5f;
//         for (int i = 0; i < bulletNum; i++) {
//             var color = Color.clear;
//             if (isClockwise) color = Color.Lerp(Color.blue, Color.cyan, i / bulletNum);
//             else color = Color.Lerp(Color.cyan, Color.blue, i / bulletNum);
//             var dir = (startAngle + (isClockwise ? 1 : -1) * angleInterval * i);
//             var bullet = Calc.GenerateBullet(scaleBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir);
//             bullet.speed = (Mathf.Sin(((isClockwise ? 1 : -1) * angleInterval * i + startAngle / 4f) * Mathf.Deg2Rad) + 1f) * speedMultiplier;
//             speedMultiplier -= 0.2f;
//             bullet.bulletState.SetColor(color);
//             Timing.RunCoroutine(
//                 GenSubBullet(bullet as DoubleSpeedApproach, i, offset, color).CancelWith(bullet.gameObject), "Shoot");
//             
//             //var bullet2 = Calc.GenerateBullet(jadeBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir + 180f);
//
//             // if (i % 3 == 0) {
//             //     dir = -(startAngle + (isClockwise ? 1 : -1) * angleInterval * i);
//             //     bullet = Calc.GenerateBullet(jadeBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir);
//             //     bullet.speed =
//             //         (Mathf.Sin(((isClockwise ? 1 : -1) * angleInterval * i + startAngle / 4f) * Mathf.Deg2Rad) + 1f) *
//             //         2f;
//             //     if (isClockwise) color = Color.Lerp(Color.cyan, Color.blue, i / bulletNum);
//             //     else color = Color.Lerp(Color.blue, Color.cyan, i / bulletNum);
//             //     bullet.bulletState.SetColor(Color.Lerp(color, Color.white, i/bulletNum));
//             // }
//
//             yield return Calc.WaitForFrames(frameInterval);
//         }
//     }
//
//     public IEnumerator<float> GenSubBullet(DoubleSpeedApproach parent,int num,float offset,Color color) {
//         yield return Timing.WaitForOneFrame;
//         while (true) {
//             if (parent.IsSpeedChangeFinished(0.01f)) {
//                 for (int i = 1; i <= 1; i++) {
//                     var bullet = Calc.GenerateBullet(riceBullet, parent.transform.position,
//                         parent.transform.eulerAngles.z + offset * i);
//                     bullet.speed = i + num / 2f;
//                     //(bullet as DoubleSpeedApproach).endSpeed = 1f - num / 100f;
//                     bullet.bulletState.SetColor(color / 2f);
//                 }
//                 parent.bulletState.SetState(EBulletStates.Destroying);
//                 yield break;
//             }
//             yield return Timing.WaitForOneFrame;
//         }
//     }
//
//     private float _startAngle;
//     public float decDec = 0.10f;
//     private bool _isClockwise;
//     public override IEnumerator<float> ShootSingleWave() {
//         float initDir = 0f;//Random.Range(-30, 30);
//         float decent = 10f;//10f -=0.25f
//         for (int i = 0; i <= 360 / 9/2; i++) {
//             Timing.RunCoroutine(Shoot(_startAngle + i * 9, initDir + i * decent, _isClockwise), "Shoot");
//             decent -= decDec;
//             yield return Calc.WaitForFrames(5);   
//         }
//     } 
//     public IEnumerator<float> ShootSingleWave2() {
//         float initDir = Random.Range(-30, 30);
//         float decent = 10f;//10f -=0.25f
//         for (int i = 0; i <= 360 / 9/2; i++) {
//             Timing.RunCoroutine(Shoot(_startAngle + i * 9 + 180f, initDir + i * decent, _isClockwise), "Shoot");
//             decent -= decDec;
//             yield return Calc.WaitForFrames(5);   
//         }
//     }
//
//     public override IEnumerator<float> AutoShoot() {
//         _startAngle = Random.Range(0, 360);
//         _isClockwise = true;
//         while (true) {
//             _isClockwise = !_isClockwise;
//             _startAngle += 60f;
//             Timing.RunCoroutine(ShootSingleWave(),"Shoot");
//             //yield return Calc.WaitForFrames(30);
//             Timing.RunCoroutine(ShootSingleWave2(),"Shoot");
//             yield return Calc.WaitForFrames(waveFrameInterval);
//         }
//     }
// }

public class WakasagihimeNS0 : BulletGenerator
{
    public DoubleSpeedApproach scaleBullet;
    public DoubleSpeedApproach riceBullet;
    public DoubleSpeedApproach jadeBullet;

    public IEnumerator<float> Shoot(float startAngle, float offset, bool isClockwise)
    {
        float angleInterval = 12f; // 调小角度间隔以增加密集感
        float bulletNum = 16; // 增加子弹数量
        float innerRadius = 0.6f; // 调整初始半径以丰富弹幕层次
        int frameInterval = 3; // 缩短间隔以增加弹幕速度感
        float speedMultiplier = 4f; // 调整速度使整体更协调
        for (int i = 0; i < bulletNum; i++)
        {
            var color = Color.clear;
            if (isClockwise) color = Color.Lerp(Color.magenta, Color.cyan, i / bulletNum); // 调整颜色为更鲜艳的过渡
            else color = Color.Lerp(Color.cyan, Color.magenta, i / bulletNum);
            var dir = (startAngle + (isClockwise ? 1 : -1) * angleInterval * i);
            var bullet = Calc.GenerateBullet(scaleBullet, transform.position + innerRadius * dir.Deg2Dir3(), dir);
            bullet.speed = (Mathf.Sin(((isClockwise ? 1 : -1) * angleInterval * i + startAngle / 4f) * Mathf.Deg2Rad) + 1f) * speedMultiplier;
            speedMultiplier -= 0.15f; // 减小衰减变化以形成更自然的效果
            bullet.bulletState.SetColor(color);
            Timing.RunCoroutine(
                GenSubBullet(bullet as DoubleSpeedApproach, i, offset, color).CancelWith(bullet.gameObject), "Shoot");

            yield return Calc.WaitForFrames(frameInterval);
        }
    }

    public IEnumerator<float> GenSubBullet(DoubleSpeedApproach parent, int num, float offset, Color color)
    {
        yield return Timing.WaitForOneFrame;
        while (true)
        {
            if (parent.IsSpeedChangeFinished(0.01f))
            {
                for (int i = 1; i <= 1; i++) // 减少分裂子弹的数量至原来的1/5
                {
                    var bullet = Calc.GenerateBullet(riceBullet, parent.transform.position,
                        parent.transform.eulerAngles.z + offset * i);
                    bullet.speed = 1.5f + num / 4f; // 保持合理的分裂子弹速度
                    bullet.bulletState.SetColor(color * 0.8f); // 颜色保持稍淡的主色
                }
                parent.bulletState.SetState(EBulletStates.Destroying);
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
    }

    private float _startAngle;
    public float decDec = 0.05f; // 调小衰减步长
    private bool _isClockwise;
    public IEnumerator<float> ShootSingleWave(float startAngle)
    {
        float initDir = 0f;
        float decent = 12f; // 初始递减值
        for (int i = 0; i <= 360 / 8 / 2; i++) // 循环步长保持一致
        {
            // 正向旋转波次
            Timing.RunCoroutine(Shoot(startAngle + i * 8, initDir + i * decent, _isClockwise), "Shoot");
            decent -= decDec;
            yield return Calc.WaitForFrames(4); // 帧间隔一致
        }
    }

    public IEnumerator<float> ShootSingleWave2()
    {
        float initDir = 0f; // 使用与正向相同的初始偏移
        float decent = 12f; // 初始递减值保持一致
        for (int i = 0; i <= 360 / 8 / 2; i++)
        {
            // 反向旋转波次，与正向形成镜像对称
            Timing.RunCoroutine(Shoot(_startAngle - i * 8, initDir - i * decent, !_isClockwise), "Shoot");
            decent -= decDec;
            yield return Calc.WaitForFrames(4); // 帧间隔保持一致
        }
    }

    public override IEnumerator<float> ShootSingleWave() {
        yield return 0;
    }

    public override IEnumerator<float> AutoShoot()
    {
        _startAngle = Random.Range(0, 360);
        _isClockwise = true;
        while (true)
        {
            _isClockwise = !_isClockwise;
            Timing.RunCoroutine(ShootSingleWave(_startAngle), "Shoot");
            _startAngle += 180f; 
            Timing.RunCoroutine(ShootSingleWave(_startAngle), "Shoot");
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }

}

