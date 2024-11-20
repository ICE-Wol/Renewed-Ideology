using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;

public class Spell000600 : MonoBehaviour {
    
    [Serializable]
    public class BulletGeneration {
        public EnemyBulletAPIs bullet;
        public int timer;
        public int gen;
    }
    public EnemyBulletAPIs[] bulletTemplates;
    
    [SerializeField]
    public List<BulletGeneration> bullets;

    public float direction;
    public float speed;
   
    public float spdIncrease;
    public float dirIncrease;
    
    public int shootFrameInterval;
    
    public int bulletChangeTime;

    public int destroyTimer;
    public int timer;
    private void Start() {
        bullets = new List<BulletGeneration>();
    }
    private void Update() {
        //transform.position = new Vector3(Mathf.Sin(Time.time) * 3, Mathf.Cos(Time.time) * 3, 0);
        transform.position += speed / 60f * direction.Deg2Dir3();
        speed += spdIncrease;
        direction += dirIncrease;

        if (timer % shootFrameInterval == 0) {
            BulletGeneration b = new BulletGeneration();
            b.bullet = Instantiate(bulletTemplates[0], transform.position, Quaternion.identity);
            b.timer = 0;
            b.gen = 0;
            bullets.Add(b);
            b.bullet.movement.direction = direction + 90f;
            b.bullet.movement.speed = Mathf.Sin(timer/6f)/3f;
            
            BulletGeneration b2 = new BulletGeneration();
            b2.bullet = Instantiate(bulletTemplates[0], transform.position, Quaternion.identity);
            b2.timer = 0;
            b2.gen = 0;
            bullets.Add(b2);
            b2.bullet.movement.direction = direction - 90f;
            b2.bullet.movement.speed = Mathf.Sin(timer/6f)/3f;
        }

        if (timer > destroyTimer) {
            for (int i = 0; i < bullets.Count; i++) {
                var s = bullets[i];
                s.bullet.state.SetState(EBulletStates.Destroying);
                bullets.Remove(s);
            }

            Destroy(gameObject);
        }
        timer++;

        for (int i = 0;i < bullets.Count;i++) {
            var s = bullets[i];
            if (s.bullet == null) {
                bullets.Remove(s);
                continue;
            }
            s.timer++;
            
            if (s.timer >= bulletChangeTime) {
                s.timer = 0;
                s.gen++;
                if (s.gen < 3) {
                    var old = s.bullet.movement;
                    s.bullet = Instantiate(bulletTemplates[s.gen],transform.position,Quaternion.identity);
                    s.bullet.movement.direction = old.direction;
                    s.bullet.movement.speed = old.speed;
                    s.bullet.transform.position = old.transform.position;
                    DestroyImmediate(old.gameObject);
                }
                else {
                    s.bullet.state.SetState(EBulletStates.Destroying);
                    bullets.Remove(s);
                }
            }
        }
    }
}
