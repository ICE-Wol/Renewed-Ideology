using System;
using System.Collections.Generic;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class WaterBomb : MonoBehaviour
{
    public WaterOrbCtrl waterOrbPrefab;
    public WaterOrbCtrl[] waterOrbs;
    
    public int orbNum = 8; //水球数量
    void Start()
    {
        waterOrbs = new WaterOrbCtrl[8];
        //Timing.RunCoroutine(ShootWaterOrbs());
        for (int i = 0; i < orbNum; i++) {
            var orb = Instantiate(waterOrbPrefab, PlayerCtrl.instance.transform.position, Quaternion.identity);
            orb.initAng = 360f / orbNum * i;
            orb.transform.parent = PlayerCtrl.instance.transform;
            waterOrbs[i] = orb;

            //yield return Calc.WaitForFrames(3);
        }
    }
    
    public IEnumerator<float> ShootWaterOrbs() {
        for (int i = 0; i < orbNum; i++) {
            var orb = Instantiate(waterOrbPrefab, PlayerCtrl.instance.transform.position, Quaternion.identity);
            orb.initAng = 360f / orbNum * i;
            orb.transform.parent = PlayerCtrl.instance.transform;
            waterOrbs[i] = orb;

            yield return Calc.WaitForFrames(3);
        }
    }

    public int nullNum;
    public int timer = 0;
    void Update() {
        timer++;
        //if (timer > 3 * orbNum) {
            nullNum = 0;
            for (int i = 0; i < orbNum; i++) {
                if (waterOrbs[i]) nullNum++;
            }

            if (nullNum == 0) {
                Destroy(gameObject);
            }
        //}
    }
}
