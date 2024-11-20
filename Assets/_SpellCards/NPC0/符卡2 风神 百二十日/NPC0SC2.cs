using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0SC2 : MonoBehaviour
{
    public int waveLength;
    public int ways;
    public DoubleSpeedApproach riceBullet1;
    public DoubleSpeedApproach riceBullet2;

    public IEnumerator<float> Shoot(int num, int ways) {
        for (int i = 0; i < num; i++) {
            for (int j = 0; j < ways; j++) {
                var bullet = Instantiate(riceBullet1, transform.position, Quaternion.identity);
                bullet.direction = (float)j / ways * 360;
                bullet.GetComponent<NPC0SC2Rice>().curOrder = i;
                bullet.GetComponent<NPC0SC2Rice>().maxOrder = num;

                bullet = Instantiate(riceBullet2, transform.position, Quaternion.identity);
                bullet.direction = (float)j / ways * 360;
                bullet.GetComponent<NPC0SC2Rice>().curOrder = i;
                bullet.GetComponent<NPC0SC2Rice>().maxOrder = num;
            }

            yield return Calc.WaitForFrames(5);
        }
    }

    public void Start() {
        Timing.RunCoroutine(Shoot(waveLength, ways));
    }
}
