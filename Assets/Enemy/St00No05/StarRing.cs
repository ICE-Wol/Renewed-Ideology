using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;

public class StarRing : BulletGenerator
{
    public DoubleSpeedApproach bulletPrefab;
    public int bulletCount = 36;
    public float innerRadius = 0.5f;
    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i < bulletCount; i++) {
            var dir = 2 * Mathf.PI * i / bulletCount;
            var pos = transform.position + innerRadius* new Vector3(
                Mathf.Cos(2 * Mathf.PI * i / bulletCount), 
                Mathf.Sin(2 * Mathf.PI * i / bulletCount), 0);
            var bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            bullet.direction = (float)i / bulletCount * 360;
            if (i % 2 == 0) {
                bullet.speed = 1.5f;
            }
            else {
                bullet.speed = 1f;
                bullet.GetComponent<State>().SetColor(Color.green);
                bullet.GetComponent<Config>().color = Color.green;
            }

            bullet.endSpeed = 2f;

        }
        yield return Timing.WaitForOneFrame;
    }

    public override IEnumerator<float> AutoShoot() {
        throw new System.NotImplementedException();
    }
}
