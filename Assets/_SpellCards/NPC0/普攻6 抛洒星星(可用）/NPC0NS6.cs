using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


// 好看的形状，interval 50 45 46 47（质数）
// 留意magic number 质数，不被360整除的数，奇怪的数。
public class NPC0NS6 : BulletGenerator
{
    public TripleSpeedApproach starSPrefab;
    public DoubleSpeedApproach starMPrefab;
    public int bulletNum;
    public float degMultiplier;
    public float radius;
    public float interval;
    public float initSpeed;
    public float endSpeed;
    public float endSpeed2;
    
    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i < bulletNum; i++) {
            degMultiplier = Mathf.Lerp(1, 2.5f, (float)i / bulletNum);
            //degMultiplier.ApproachRef(1, 16f);
            
            var pos = transform.position + radius * Calc.Deg2Dir3(i * degMultiplier);
            var bullet = Instantiate(starSPrefab, pos, Quaternion.identity);
            bullet.direction = i * interval;
            bullet.speed = initSpeed;
            bullet.midSpeed = 0;
            bullet.endSpeed = (i % 2 == 0) ? endSpeed : endSpeed2;

            Color c = (i % 2 == 0)
                ? Color.HSVToRGB((float)i / bulletNum, 1f, 1f)
                : Color.HSVToRGB((float)i + (bulletNum - i) / 2f / bulletNum, 1f, 1f);
            
            bullet.GetComponent<State>().SetColor(c);
            bullet.GetComponent<Config>().color = c;
            
            bullet.approachRate = 32f;

            yield return Calc.WaitForFrames(layerFrameInterval);
        }

        for (int i = 0; i < 20; i++) {
            var pos = transform.position + radius * Calc.Deg2Dir3(bulletNum * degMultiplier)
                                         + radius / 2f * Calc.Deg2Dir3(i * 18);
            var bullet = Instantiate(starMPrefab, pos, Quaternion.identity);
            bullet.direction = i * 18;
            
            Color c = Color.HSVToRGB((float)i / 20, 1f, 1f);
            bullet.GetComponent<State>().SetColor(c);
            bullet.GetComponent<Config>().color = c;
            
            pos = transform.position + radius * Calc.Deg2Dir3(bulletNum * degMultiplier)
                                     + radius / 2f * Calc.Deg2Dir3(i * 18);
            bullet = Instantiate(starMPrefab, pos, Quaternion.identity);
            bullet.direction = i * 18 + 9f;
            bullet.endSpeed *= 1.5f;
            
            c = Color.HSVToRGB((float)i / 20, 0f, 1f);
            bullet.GetComponent<State>().SetColor(c);
            bullet.GetComponent<Config>().color = c;
            
            
        }
    }

    public override IEnumerator<float> AutoShoot() {
        throw new System.NotImplementedException();
    }

    private void Start() {
        Timing.RunCoroutine(ShootSingleWave());
    }
}
