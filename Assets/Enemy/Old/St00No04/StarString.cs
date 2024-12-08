using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Rendering;
using MEC;

public class GlowJadeMCircle : BulletGenerator
{
    //public TripleSpeedApproach bulletPrefab;
    public BulletGroup bulletGroup;
    public float radius;
    public float degree;
    public float initSpeed;
    public float endSpeed;
    public override IEnumerator<float> ShootSingleWave() {
        //transform.localPosition = Vector3.zero;
        for (int i = 0; i < 20; i++) {
            var pos = transform.position + radius * Calc.Deg2Dir3(i * 50);
            var b = Instantiate(bulletGroup, pos, Quaternion.identity);
            var bullet = b.mainBullet.GetComponent<TripleSpeedApproach>();
            bullet.direction = i * 50;
            bullet.speed = initSpeed;
            bullet.midSpeed = 0;
            bullet.endSpeed = endSpeed;

            pos = transform.position + radius * Calc.Deg2Dir3(i * 50 + 180f);
            b = Instantiate(bulletGroup, pos, Quaternion.identity);
            bullet = b.mainBullet.GetComponent<TripleSpeedApproach>();
            bullet.direction = i * 50 + 180f;
            bullet.speed = initSpeed;
            bullet.midSpeed = 0;
            bullet.endSpeed = endSpeed;
            
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
