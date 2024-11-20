using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SanaeNS3 : BulletGenerator
{
    public DoubleSpeedApproach jadeOddPrefab;
    public DoubleSpeedApproach jadeEvenPrefab;
    public DoubleSpeedApproach pointPrefab;

    private void Start() {
        Timing.RunCoroutine(AutoShoot().CancelWith(gameObject));
        //Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject));
    }

    public override IEnumerator<float> ShootSingleWave() {
        var radius = 0f;
        var degree = Random.Range(0f,360f);
        var spdDeg = 5f;
        var centerPos = transform.position;

        radius = 1.5f;
        while (radius > 0f) {
            DoubleSpeedApproach b1;
            if(waveCount % 2 == 0) b1 = Instantiate(jadeOddPrefab, centerPos, Quaternion.identity);
            else b1 = Instantiate(jadeEvenPrefab, centerPos, Quaternion.identity);
            b1.transform.position = centerPos + new Vector3(
                Mathf.Sin(degree * Mathf.Deg2Rad) * radius,
                Mathf.Cos(degree * Mathf.Deg2Rad) * radius,
                0
            );
            b1.direction = -degree + 180f;
            b1.speed = 3f;
            b1.startFrame = 0;
            if(waveCount % 2 == 0) b1.endSpeed = 2f + 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            else b1.endSpeed = 2f - 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            
            
            if(waveCount % 2 == 0) b1 = Instantiate(jadeOddPrefab, centerPos, Quaternion.identity);
            else b1 = Instantiate(jadeEvenPrefab, centerPos, Quaternion.identity);
            b1.transform.position = centerPos + new Vector3(
                Mathf.Sin((degree + 180f) * Mathf.Deg2Rad) * radius,
                Mathf.Cos((degree + 180f) * Mathf.Deg2Rad) * radius,
                0
            );
            b1.direction = -(degree + 180f) + 180f;
            b1.speed = 3f;
            b1.startFrame = 0;
            if(waveCount % 2 == 0) b1.endSpeed = 2f + 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            else b1.endSpeed = 2f - 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            
            if(waveCount % 2 == 0) b1 = Instantiate(pointPrefab, centerPos, Quaternion.identity);
            else b1 = Instantiate(pointPrefab, centerPos, Quaternion.identity);
            b1.transform.position = centerPos + new Vector3(
                Mathf.Sin(degree * Mathf.Deg2Rad) * radius,
                Mathf.Cos(degree * Mathf.Deg2Rad) * radius,
                0
            );
            b1.direction = -degree + 180f;
            b1.speed = 2f;
            b1.startFrame = 0;
            if(waveCount % 2 == 0) b1.endSpeed = 2f + 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            else b1.endSpeed = 2f - 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            
            
            if(waveCount % 2 == 0) b1 = Instantiate(pointPrefab, centerPos, Quaternion.identity);
            else b1 = Instantiate(pointPrefab, centerPos, Quaternion.identity);
            b1.transform.position = centerPos + new Vector3(
                Mathf.Sin((degree + 180f) * Mathf.Deg2Rad) * radius,
                Mathf.Cos((degree + 180f) * Mathf.Deg2Rad) * radius,
                0
            );
            b1.direction = -(degree + 180f) + 180f;
            b1.speed = 2f;
            b1.startFrame = 0;
            if(waveCount % 2 == 0) b1.endSpeed = 2f + 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);
            else b1.endSpeed = 2f - 1.5f * Mathf.Sin(degree * 6f * Mathf.Deg2Rad);

            if (waveCount % 2 == 0)
                degree -= spdDeg;
            else degree += spdDeg;
            spdDeg += 0.01f;
            
            radius -= 0.01f;
            yield return Timing.WaitForOneFrame;
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            waveCount++;
            Timing.RunCoroutine(ShootSingleWave());
            
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}