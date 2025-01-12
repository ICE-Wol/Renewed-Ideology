using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC0NS0 : BulletGenerator
{
    public TripleSpeedApproach ringJadePrefab;
    public DoubleSpeedApproach ricePrefab;
    public int ways;
    public int layers;

    public IEnumerator<float> WheatShoot(bool isClockwise, float basicDir) {
        Vector3 center = transform.position;
        float radius = 0;
        float deg = Random.Range(0, 360); 
        while (true) {
            radius += 0.02f;
            if (isClockwise) deg -= 10;
            else deg += 10;
            for (int i = 0; i < 6; i++) {
                var bullet = Instantiate(ricePrefab, center + radius * deg.Deg2Dir3(), Quaternion.identity);
                bullet.speed = 4f;
                bullet.direction = Vector2.SignedAngle(Vector2.right,
                    PlayerCtrl.instance.transform.position - bullet.transform.position) + 360f / 6f * i + basicDir;
            }

            if(radius > 2f) break;
            
            yield return Timing.WaitForOneFrame;
        }
    }
    
    public IEnumerator<float> RotateRingJade(bool isClockwise) {
        var bulletSet = new TripleSpeedApproach[ways, layers];
        for (int i = 0; i < ways; i++) {
            for (int j = 0; j < layers; j++) {
                var bullet = Instantiate(ringJadePrefab, transform.position, Quaternion.identity);
                bulletSet[i, j] = bullet;
                bullet.speed = 2f + 0.5f * j;
                bullet.direction = 360f / ways * i;
            }
        }

        for (int k = 0; k < 70; k++) {
            for (int i = 0; i < ways; i++) {
                for (int j = 0; j < layers; j++) {
                    var bullet = bulletSet[i, j];
                    bullet.direction += (isClockwise ? 1 : -1) * (4 + 0.015f * j); //1 + 0.5f * j;


                }
            }
            yield return Timing.WaitForOneFrame;
        }
        
    }
    
    public override IEnumerator<float> ShootSingleWave() {
        waveCount++;
        if (waveCount % 2 == 0) {
            Timing.RunCoroutine(RotateRingJade(true));
            yield return Calc.WaitForFrames(60);
            Timing.RunCoroutine(RotateRingJade(false));
            yield return Calc.WaitForFrames(60);
            Timing.RunCoroutine(WheatShoot(true,0));
            Timing.RunCoroutine(WheatShoot(false,30f));
        }
        else {
            Timing.RunCoroutine(RotateRingJade(false));
            yield return Calc.WaitForFrames(60);
            Timing.RunCoroutine(RotateRingJade(true));
            yield return Calc.WaitForFrames(60);
            Timing.RunCoroutine(WheatShoot(false,0));
            Timing.RunCoroutine(WheatShoot(true,30f));
        }
        
        
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }

    private void Start() {
        Timing.RunCoroutine(AutoShoot());
    }
}
