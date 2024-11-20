using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC0NS1 : BulletGenerator
{
    public DoubleSpeedApproach ricePrefab;
    public DoubleSpeedApproach ringJadePrefab;
    public int bulletsInCircle = 10;

    public float initDeg;

    private void Start() {
        Timing.RunCoroutine(AutoShoot());
        //Timing.RunCoroutine(ShootSingleWave());
    }

    public override IEnumerator<float> ShootSingleWave() {
        var deg = Random.Range(0f, 360f);

        for (int i = 0; i <= 4; i++) {
            Timing.RunCoroutine(FlowerShoot(ricePrefab, deg, 4f,20));
            yield return Calc.WaitForFrames(20);
        }

        //yield return Calc.WaitForFrames(30);
        Timing.RunCoroutine(FlowerShoot(ringJadePrefab, deg + 180f, 6f,15));
        
    }

    public IEnumerator<float> FlowerShoot(DoubleSpeedApproach bullet,float initDeg/*, Color color*/, float initSpeed,float rotLimit) {
        DoubleSpeedApproach[,] bullets = new DoubleSpeedApproach[layerCountInWave, bulletsInCircle];
        Timing.RunCoroutine(RotateBullets(bullets, 10, rotLimit, 0));
        //yield return Calc.WaitForFrames(30);
        for (int i = 0; i < layerCountInWave; i++) {
            for (int j = 0; j < bulletsInCircle; j++) {
                var b = Instantiate(bullet, transform.position, Quaternion.identity);
                //b.GetComponent<Config>().color = color;
                //b.GetComponent<State>().SetColor(color);
                bullets[i, j] = b;
                b.direction = initDeg + 360f / bulletsInCircle * j;
                b.speed = initSpeed;
            }

            yield return Calc.WaitForFrames(10);
        }
    }

    public IEnumerator<float> RotateBullets(DoubleSpeedApproach[,] bullets, float rotateDir, float rotateLimit,
        int waitFrame) {
        var rotateCount = 0;
        yield return Calc.WaitForFrames(waitFrame);
        while (rotateCount <= rotateLimit) {
            for (int i = 0; i < layerCountInWave; i++) {
                for (int j = 0; j < bulletsInCircle; j++) {
                    if (bullets[i, j] == null) continue;
                    if (j % 2 == 0) bullets[i, j].direction -= rotateDir;
                    else bullets[i, j].direction += rotateDir;
                }

                yield return Calc.WaitForFrames(1);
            }

            //yield return Calc.WaitForFrames(10);
            rotateCount++;
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
}
