using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spell000000 : BulletGenerator
{
    public CircleSpawner spawner;
    public BulletMovement bulletTemplate;

    public int changeFrame;
    public int degreeOffsetInterval;
    
    public IEnumerator<float> ChangeSingleLayerBullet(BulletMovement[] bullets/*, int waveId, int layerId*/) {
        var d = Timing.RunCoroutine(GameManager.WaitForFrames(changeFrame ));
        yield return Timing.WaitUntilDone(d);
        
        //Debug.LogWarning($"Remove! {waveId} {layerId} {bullets[0].gameObject.GetInstanceID()}::{Time.frameCount}\n{changeFrame}!~{time}");
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
        print("ShootSingleWave");
        isEnchanting = true;
        for (int i = 0; i < layerCountInWave; i++) {
            //Debug.LogWarning($"Shoot!{waveId} : {this.GetNamePath()} :: {Time.frameCount}");
            //Debug.Log(this.gameObject.name + " ?? " + spawner.bulletAmount + " ?? " + spawner.bullets.Length);
            var bullets = spawner.Shoot();
            foreach (var b in bullets) {
               b.direction += -190f;
            }
            Timing.RunCoroutine(ChangeSingleLayerBullet(bullets /*waveId, i*/));
            spawner.degreeOffset += degreeOffsetInterval;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            
            yield return Timing.WaitUntilDone(d);
        }

        isEnchanting = false;
    }
    
    public override IEnumerator<float> AutoShoot() {
        while (true) {
            print("isAutoShooting");
            Timing.RunCoroutine(ShootSingleWave(/*cc++*/));
            yield return Calc.WaitForFrames(waveFrameInterval);

        }
    }

    public void Start() {
        print("Start"); 
        Timing.RunCoroutine(AutoShoot());
    }
}
