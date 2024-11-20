using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;
using MEC;

public class Flower : BulletGenerator
{
    public DoubleSpeedApproach bulletTemplate;
    public int bulletsPerCircle;

    public float dirOffset;
    
    public override IEnumerator<float> ShootSingleWave() {
        dirOffset = Random.Range(-30f, 30f);
        for (int i = 0; i < layerCountInWave; i++) {
            for (int j = 0; j < bulletsPerCircle; j++) {
                var basicRadius = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (360f / bulletsPerCircle * j * 5)));
                var radius = basicRadius + 0.1f * layerCountInWave;
                var pos = transform.position
                          + radius * Mathf.Sin(Mathf.Deg2Rad * (360f / bulletsPerCircle * j)) * Vector3.right
                          + radius * Mathf.Cos(Mathf.Deg2Rad * (360f / bulletsPerCircle * j)) * Vector3.up;
                var bullet = Instantiate(bulletTemplate, pos, Quaternion.identity);
                var dir = Vector2.SignedAngle(Vector2.right, pos);//360f / bulletsPerCircle * j;
                if (j % 2 == 0) {
                    bullet.speed = 3f;
                    bullet.endSpeed = 2.5f;
                    bullet.direction = dir + dirOffset;
                } else {
                    bullet.speed = 1.5f;
                    bullet.endSpeed = 1f;
                    bullet.direction = dir - dirOffset;
                }
            }
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        throw new System.NotImplementedException();
    }

    private void Start() {
        Timing.RunCoroutine(ShootSingleWave());
    }
}
