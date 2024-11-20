using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;

public class NPC0NS3BulletRing : MonoBehaviour
{
    public DoubleSpeedApproach jadePrefab;
    public DoubleSpeedApproach dotPrefab;

    public int bulletNum;
    //public bool isClockwise;
    public bool isJade;
    
    public struct bulletInfo
    {
        public DoubleSpeedApproach bullet;
        public float num;
        public float deg;
        public float rad;
    }
    public bulletInfo[] bullets;
    
    public int timer = 0;

    public void Init() {
        bullets = new bulletInfo[bulletNum];
        for (int i = 0; i < bulletNum; i++) {
            // if (isJade) {
            //     bullets[i].bullet = Instantiate(jadePrefab, transform.position, Quaternion.identity);
            // } else {
            //     bullets[i].bullet = Instantiate(dotPrefab, transform.position, Quaternion.identity);
            // }
            bullets[i].bullet = Instantiate(i % 2 == 0 ? jadePrefab : dotPrefab, transform.position, Quaternion.identity);
            bullets[i].num = i;
            bullets[i].deg = 360f / bulletNum * i;
            bullets[i].rad = 0;
        }
    }

    public void Refresh() {
        bool isAllNull = true;
        for (int i = 0; i < bulletNum; i++) {
            if(bullets[i].bullet == null) continue;
            isAllNull = false;
           bullets[i].deg += 0.5f;
           bullets[i].bullet.direction = bullets[i].deg;
            
            
            //bullets[i].rad += (0.1f - timer / 500f) > 0.01f ? (0.1f - timer / 500f) : 0.01f;// * Mathf.Cos(timer * Mathf.Deg2Rad);
            
            bullets[i].bullet.transform.position = transform.position + (bullets[i].rad 
                                                                         + 0.5f + Mathf.Sin(timer * Mathf.Deg2Rad)
                                                                         + 0.8f * Mathf.Cos(timer * Mathf.Deg2Rad + 360f / bulletNum * i) ) * bullets[i].deg.Deg2Dir3();
        }
        
        if (isAllNull) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        Init();
    }
    
    private void Update() {
        timer++;
        Refresh();
    }
    
}
