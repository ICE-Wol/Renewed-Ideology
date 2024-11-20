using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC0SC1JadeG : MonoBehaviour
{
    public DoubleSpeedApproach dartPrefab;

    //public float deg;
    //public float rad;

    public float speed;
    public float dir;

    public float dirAdd;
    public float dirAcc;

    public int timer;
    
    public void GenerateChildBullet(int childNum, float initDeg) {
        DoubleSpeedApproach bullet;

        for (int i = 0; i < childNum; i++) {

            bullet = Instantiate(dartPrefab, transform.position, Quaternion.identity);

            bullet.transform.position = transform.position; // + 0.5f * rice.direction.Deg2Dir3();
            bullet.direction = initDeg + 360f / childNum * i;
        }
    }

    private void Update() {
        timer++;
        //deg += 5f + Mathf.Sin(timer *( 10f) * Mathf.Deg2Rad);
        //rad += 0.01f; //deg * Mathf.Deg2Rad;
        //transform.position = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);

        dir += dirAdd;
        dirAdd += dirAcc;
        transform.position += speed * 0.01f * dir.Deg2Dir3();
        
        
        
        if (timer % 20 <= 12 && timer % 2 == 0) {
            GenerateChildBullet(6, timer / 100f);
        }
    }
}
