using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;


public class Spell000200 : BulletGenerator
{
    public CircleSpawner spawner;
    public BulletMovement bulletTemplate;

    public int changeFrame;
    public float degreeOffsetInterval;
    public bool isGreen; 
    
    public IEnumerator<float> ChangeSingleLayerBullet(BulletMovement[] bullets/*, int waveId, int layerId*/) {
        var d = Timing.RunCoroutine(GameManager.WaitForFrames(changeFrame ));
        yield return Timing.WaitUntilDone(d);
        
        for(int i = 0; i < bullets.Length; i++) {
            if(bullets[i] == null) continue;
            
            var j = Instantiate(bulletTemplate);
            j.transform.position = bullets[i].transform.position;
            j.direction = bullets[i].direction;
            j.speed = bullets[i].speed;
            (j as DoubleSpeedApproach).speed = 1f;
            Destroy(bullets[i].gameObject);
            bullets[i] = j;
            bullets[i].gameObject.GetComponent<State>().SetState(EBulletStates.Spawning);
        }
    }
    public override IEnumerator<float> ShootSingleWave(/*int waveId*/) {
        isEnchanting = true;
        for (int i = 0; i < layerCountInWave; i++) {
            var bullets = spawner.Shoot();
            foreach (var b in bullets) {
                b.direction += -190f
                               + (isGreen ? 1 : -1)
                               * 60 * Mathf.Sin(i * Mathf.Deg2Rad * 360 / layerCountInWave);
            }
            Timing.RunCoroutine(ChangeSingleLayerBullet(bullets /*waveId, i*/));
            spawner.degreeOffset += degreeOffsetInterval; 
            //if degreeOffset too big, it will makes the spell easier to dodge
            
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }

        isEnchanting = false;
    }
    
    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave(/*cc++*/));
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public void Start() {
        Timing.RunCoroutine(AutoShoot());
    }
}