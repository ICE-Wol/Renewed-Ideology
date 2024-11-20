using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEngine;

public class Spell000101 : MonoBehaviour {
    public DoubleSpeedApproach bullet;
    public DoubleSpeedApproach subBullet1;
    public DoubleSpeedApproach subBullet2;
    public DoubleSpeedApproach subBullet3;

    public IEnumerator<float> ShootTailWave() {

        float randomUpLimit = 0.2f;
        while (true) {
            if (bullet == null) continue;
            if (randomUpLimit < 0.5f) randomUpLimit += 0.05f;
            
            int bulletNum = Random.Range(3, 5);
            for (int bulletIndex = 0; bulletIndex < bulletNum; bulletIndex++) {
                var offset = Calc.GetRandomVectorCircle(0, 360, randomUpLimit);
                var b = Instantiate(subBullet1, bullet.transform.position + offset, Quaternion.identity);
                b.direction = bullet.direction;
                b.direction = bullet.direction + Random.Range(3f, -3f);
                b.speed = Random.Range(0.25f, -0.25f);
            }

            bulletNum = Random.Range(3, 5);
            for (int bulletIndex = 0; bulletIndex < bulletNum; bulletIndex++) {
                var offset = Calc.GetRandomVectorCircle(0, 360, randomUpLimit);
                var b = Instantiate(subBullet2, bullet.transform.position + offset, Quaternion.identity);
                b.direction = bullet.direction + Random.Range(4f, -4f);
                b.speed = Random.Range(0.25f, -0.5f);
            }

            bulletNum = Random.Range(1, 3);
            for (int bulletIndex = 0; bulletIndex < bulletNum; bulletIndex++) {
                var offset = Calc.GetRandomVectorCircle(0, 360, randomUpLimit);
                var b = Instantiate(subBullet3, bullet.transform.position + offset, Quaternion.identity);
                b.direction = bullet.direction + Random.Range(4f, -4f);
                b.speed = Random.Range(0.5f, -0.25f);
            }

            var d = Timing.RunCoroutine(GameManager.WaitForFrames(20));
            yield return Timing.WaitUntilDone(d);

        }
    }

    public void Start() {
        Timing.RunCoroutine(ShootTailWave());
    }
}

