using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slashes : BulletGenerator {
    public TripleSpeedApproach flowerBulletTemplate;
    public int bulletsPerCircle;
    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i < layerCountInWave; i++) {
            var randomDir = Random.Range(0, 360);
            for (int j = 0; j < bulletsPerCircle; j++) {
                var pos = transform.position
                          + Mathf.Sin(Mathf.Deg2Rad * (360f / bulletsPerCircle * j + i * 100 + randomDir)) * Vector3.right
                          + Mathf.Cos(Mathf.Deg2Rad * (360f / bulletsPerCircle * j + i * 200 + randomDir)) * Vector3.up;
                var bullet = Instantiate(flowerBulletTemplate, pos, Quaternion.identity);
                var dir = 360f / bulletsPerCircle * j + i * 10;
                bullet.direction = dir;
                bullet.speed = 8f;
                bullet.startFrame = 0;
                bullet.secondFrame = 180;
                bullet.endSpeed = 2f;
                bullet.midSpeed = 0.1f;
            }
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }
    
}
