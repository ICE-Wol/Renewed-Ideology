using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using ScriptableObjects;
using UnityEngine;

public class BulletHitRadCheckRoom : MonoBehaviour
{
    public Config bulletConfig;
    public Config[] bullets;
    public float[] radius;

    
    public Vector3 startPos;
    public float gap;

    public int bulletTotNum;
    private void Start() {
        bulletTotNum = Enum.GetValues(typeof(BulletType)).Length;
        //rad = new BulletHitRad[bulletTotNum];
        //radius = new float[bulletTotNum];
        bullets = new Config[bulletTotNum];
        
        for (int i = 0; i < bulletTotNum; i++) {
            var b = Instantiate(bulletConfig, transform.position, Quaternion.identity);
            bullets[i] = b;
            b.type = (BulletType) i;
            b.transform.position = new Vector3((i % 5) * gap, (i / 5) * gap, 0) + startPos;
        }
        
        var basic = Resources.Load<EnemyBulletBasics>("EnemyBulletBasics");
        for (int i = 0; i < bulletTotNum; i++) {
            basic.bulletBasics[i].checkRadius = radius[i];
        }
    }
    
    // private void Update() {
    //     for (int i = 0; i < bulletTotNum; i++) {
    //         bullets[i].collideRadius = radius[i];
    //     }
    // }
}
