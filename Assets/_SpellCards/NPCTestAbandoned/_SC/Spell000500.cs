using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spell000500 : BulletGenerator {
    public float longAxis;
    public float shortAxis;
    public float rotateDegree;
    public float beginDeg;
    public float endDeg;
    public bool isClockwise;

    public EnemyBulletAPIs bulletTemplate;
    public EnemyBulletAPIs[,] bullets;

    public void SetRotateDegree(float deg) {
        rotateDegree = deg;
        transform.localRotation = Quaternion.Euler(0, 0, rotateDegree);
    }
    public override IEnumerator<float> ShootSingleWave() {
        var sign = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        var randomSeed = sign * Random.Range(3f, 6f);
        for (float i = beginDeg; i < endDeg; i+=3) {
            for (int j = 0; j < layerCountInWave; j++) {
                var deg = (isClockwise ? 1 : -1) * i;
                Vector3 localPos = (longAxis + j/5f) * Mathf.Cos(deg * Mathf.Deg2Rad) * Vector3.right
                                   + (shortAxis + j/5f) * Mathf.Sin(deg * Mathf.Deg2Rad) * Vector3.up;

                var b = Instantiate(bulletTemplate, transform);
                b.transform.localPosition = localPos;
                b.transform.localRotation = Quaternion.Euler(0, 0, deg);
                b.movement.direction = (deg - rotateDegree) * randomSeed;
                var c = Color.green + (Color.blue - Color.green) / 4 * (j + 1);
                b.config.color = c;
                b.state.SetColor(c);
                bullets[(int)(i - beginDeg) / 5, j] = b;


            }

            float time = Time.frameCount;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }

        /*for (;;) {
            foreach (var b in bullets) {
                if (b != null) {
                    if (b.movement.IsTriggered()) {
                        if (b.movement.direction > 180)
                            b.movement.direction++;
                        else b.movement.direction--;
                    }

                }
            }
            int time = Time.frameCount; 
            yield return new WaitUntil(() => Time.frameCount - time >= layerFrameInterval);
        }*/

    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
            float time = Time.frameCount;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }
    

    public void Start() {
        bullets = new EnemyBulletAPIs[(int)(endDeg - beginDeg) / 5, layerCountInWave];
        SetRotateDegree(rotateDegree);
        Timing.RunCoroutine(AutoShoot());
    }
}
