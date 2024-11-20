using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;
public class Spell000400 : BulletGenerator {
    public CircleSpawner spawner;
    public BulletMovement[] bullets;
    
    public float basicSpeed = 0.1f;
    public float increaseWays = 8f;
    public float increaseDeg = 20f;
    
    public bool isMirror;


    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i <= layerCountInWave; i ++) {
            spawner.degreeOffset
                = 90f + i / increaseWays * ((spawner.degreeEnd - spawner.degreeStart)
                                 / spawner.bulletAmount + increaseDeg);// + i;
            if (isMirror)
                spawner.degreeOffset = -90f - i / increaseWays * ((spawner.degreeEnd - spawner.degreeStart)
                    / spawner.bulletAmount + increaseDeg);
            
            
            bullets = spawner.Shoot();
            
            
            
            for (int j = 0; j < bullets.Length; j++) {
                var color = Color.blue + (Color.white - Color.blue) / bullets.Length * (j + 1);
                color.a = 1f;
                bullets[j].GetComponent<State>().SetColor(color);
                bullets[j].GetComponent<Config>().color = color;
                var movement = (bullets[j] as DoubleSpeedApproach);
                movement.endSpeed = basicSpeed;
                movement.onTriggerEvent += () => {
                    movement.direction += Random.Range(-10f, 10f);
                };
                basicSpeed += 0.004f;
            }
            // shoot angle
            
            float time = Time.frameCount;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
        
        isMirror = !isMirror;
        basicSpeed = 0.1f;

    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
            float time = Time.frameCount;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    private void Start() {
        bullets = new BulletMovement[spawner.bulletAmount];
        isMirror = true;
        Timing.RunCoroutine(AutoShoot());
    }
}

