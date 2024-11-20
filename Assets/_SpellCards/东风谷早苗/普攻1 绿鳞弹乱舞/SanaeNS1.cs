using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class SanaeNS1 : BulletGenerator
{
    
    public DoubleSpeedApproach ScalePrefab;
    public DoubleSpeedApproach[,,] bulletsA;
    public DoubleSpeedApproach[,,] bulletsB;
    public float[,,] bulletDir;

    public bool isClockwise;
    

    public void Start() {
        bulletsA = new DoubleSpeedApproach[5,15,5];
        bulletsB = new DoubleSpeedApproach[5,15,5];
        
        
        bulletDir = new float[5,15,5];
        Timing.RunCoroutine(AutoShoot(),"Shoot");
    }

    private void Update() {
        if (isClockwise) {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 15; j++) {
                    for (int k = 0; k < 5; k++) {
                        if (bulletsA[i, j, k] == null) continue;
                        if (isClockwise) {
                            if (bulletsA[i, j, k].direction - bulletDir[i, j, k] < 360f + 10f * i + 4f * j)
                                bulletsA[i, j, k].direction += 1 - i * 0.25f + j * 0.1f;
                        }
                        else {
                            if (bulletDir[i, j, k] - bulletsA[i, j, k].direction < 360f + 10f * i + 4f * j)
                                bulletsA[i, j, k].direction -= 1 - i * 0.25f + j * 0.1f;
                        }
                    }
                }
            }
        }
        else {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 15; j++) {
                    for (int k = 0; k < 5; k++) {
                        if (bulletsB[i, j, k] == null) continue;
                        if (isClockwise) {
                            if (bulletsB[i, j, k].direction - bulletDir[i, j, k] < 360f + 10f * i + 4f * j)
                                bulletsB[i, j, k].direction += 1 - i * 0.25f + j * 0.1f;
                        }
                        else {
                            if (bulletDir[i, j, k] - bulletsB[i, j, k].direction < 360f + 10f * i + 4f * j)
                                bulletsB[i, j, k].direction -= 1 - i * 0.25f + j * 0.1f;
                        }
                    }
                }
            }
        }

    }

    public override IEnumerator<float> ShootSingleWave() {
        isEnchanting = true;
        var initDir = Random.Range(0, 360);
        var centerPos = transform.position;
        for (int k = 0; k < 5; k++) {// layer in single wave
            for (int j = 0; j < 15; j++) {
                for (int i = 0; i < 5; i++) {
                    var b = (DoubleSpeedApproach)Calc.GenerateBullet(ScalePrefab, centerPos, 0f);
                    if (isClockwise) bulletsA[k, j, i] = b;
                    else bulletsB[k, j, i] = b;
                    b.GetComponent<SpriteRenderer>().sortingOrder = j;
                    Color c = Color.green;
                    if(!isClockwise) c = (Color.green + Color.yellow)/2f;
                    Color.RGBToHSV(c, out float h, out float s, out float v);

                    Color bulletColor = Color.HSVToRGB(Mathf.Lerp(h + 0.1f, h - 0.1f, j / 15f), (j + 1) / 15f, v);
                    b.GetComponent<State>().SetColor(bulletColor);
                    b.direction = initDir + 360f / 5 * i + (isClockwise ? 1 : -1) * j * 0.1f;// + 360 / 47f * k;
                    bulletDir[k,j,i] = b.direction;
                    b.speed = 4f + 0.2f * (15 - j);
                    b.endSpeed = 1f + 0.1f * j;
                }

                yield return Timing.WaitForOneFrame;
            }
            yield return Calc.WaitForFrames(10);
        }
        isEnchanting = false;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true)
        {
            Timing.RunCoroutine(ShootSingleWave(), "Shoot");
            //所有射击相关的协程都应拥有"Shoot"标签，便于在OnDisable()时统一取消
            yield return Calc.WaitForFrames(waveFrameInterval);
            isClockwise = !isClockwise;
        }
    }
}
