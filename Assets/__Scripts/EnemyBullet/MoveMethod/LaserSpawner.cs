using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Scripts.Tools;
using UnityEngine;
using MEC;

public class LaserSpawner : BulletGenerator
{
    public CurveLaserHead head;

    public CurveLaserHead[] lasers;
    

    private void Awake() {
        lasers = new CurveLaserHead[10];
        for(int i = 0;i<10;i++) {
            lasers[i] = Instantiate(head);
        }
        //Timing.RunCoroutine(AutoShoot());
       Timing.RunCoroutine( ShootSingleWave());
    }

    private float timer = 0;
    private void Update() {

        if (timer % 50== 0) {
            for (var i = 0; i < 10; i++) {
                lasers[i].transform.position = transform.position + 2 * (timer + 36 * i).Deg2Dir3();
            }
        }

        timer++;
    }

    public override IEnumerator<float> ShootSingleWave() {
        yield return Timing.WaitForOneFrame;
    }

    public override IEnumerator<float> AutoShoot() {
        yield return Timing.WaitForOneFrame;
    }
}



