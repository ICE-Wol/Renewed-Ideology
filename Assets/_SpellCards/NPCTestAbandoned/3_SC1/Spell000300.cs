using  System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;
public class Spell000300 : BulletGenerator {
    public CircleSpawner spawner;
    public BulletMovement[] bullets;

    public bool isMirror;


    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i <= layerCountInWave; i ++) {
            spawner.degreeOffset
                = 90f + i / 2f * ((spawner.degreeEnd - spawner.degreeStart)
                                 / spawner.bulletAmount + 20);// + i;
            if (isMirror)
                spawner.degreeOffset = -90f - i / 2f * ((spawner.degreeEnd - spawner.degreeStart)
                    / spawner.bulletAmount + 20);
            
            
            bullets = spawner.Shoot();

            var maxSpeed = 5f;
            for (int j = 0; j < bullets.Length; j++) {
                var color = Color.blue + (Color.red - Color.blue) / bullets.Length * (j + 1);
                color.a = 1f;
                bullets[j].GetComponent<State>().SetColor(color);
                
                var movement = (bullets[j] as DoubleSpeedApproach);
                movement.endSpeed = maxSpeed - maxSpeed / bullets.Length * (j + 1);
                movement.endSpeed += Random.Range(0.5f, 1f);
                movement.onTriggerEvent += () => {
                    movement.direction += Random.Range(-15f, 15f);
                };
                //basicSpeed += 0.01f;
            }
            
            //todo spell card;
            /*foreach (var b in bullets) {
                var movement = (b as MultiSpeedApproach);
                movement.endSpeed = basicSpeed;
                movement.onTriggerEvent += () => {
                    movement.direction += Random.Range(-10f, 10f);
                };
                basicSpeed += 0.01f;
            }*/
            // shoot angle
            
            float time = Time.frameCount;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
        
        isMirror = !isMirror;
        
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
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
