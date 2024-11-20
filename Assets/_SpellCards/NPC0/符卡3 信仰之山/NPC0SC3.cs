using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0SC3 : MonoBehaviour
{
    public DoubleSpeedApproach spellBullet;
    public int ways;
    public float rad;
    
    public Vector3[] layer1TopPos;
    public Vector3[] layer2TopPos;

    public void Move(DoubleSpeedApproach[] bullets) {
        // for (int i = 0; i < bullets.Length; i++) {
        //     for (int j = 0; j < 4; j++) {
        //         var bullet = Instantiate(spellBullet, bullets[i].transform.position, Quaternion.identity);
        //         bullet.GetComponent<State>().SetState(EBulletStates.Activated);
        //         bullet.direction += bullets[i].direction + j * 3;
        //         bullet.transform.rotation = Quaternion.Euler(0, 0, bullet.direction);
        //         bullet.trigger = true;
        //     }
        //     Destroy(bullets[i].gameObject);
        // }
        float[] dir = new float[bullets.Length];
        for (int i = 0; i < bullets.Length; i++) {
            dir[i] = bullets[i].direction;
        }

        for (int i = 0; i < bullets.Length; i++) {
            bullets[i].direction = 15f + dir[i / 4 * 4] + (i % 4 - 2) * 1.5f;
            bullets[i].trigger = true;
        }
    }
    public IEnumerator<float> ShootLayer1(int ord,float initDir) {
        DoubleSpeedApproach[] bullets = new DoubleSpeedApproach[ways];
        for (int i = 0; i < ways; i++) {
            var dir = initDir + ord * 72f - 144f + (float)i / ways * 288f;
            var pos = transform.position + rad * (initDir + ord * 72f).Deg2Dir3() + rad * dir.Deg2Dir3();
            var bullet = Instantiate(spellBullet, pos, Quaternion.identity);
            bullet.direction = dir - 180;// + (i % 4 - 2) * 4;
            if (i == ways / 2) {
                layer1TopPos[ord] = pos;
                if (ord == 0) layer1TopPos[5] = pos;
            }
            bullets[i] = bullet;
            yield return Calc.WaitForFrames(1);
        }

        var center = (layer1TopPos[ord] + layer1TopPos[ord + 1]) / 2;
        var rad2 = Vector3.Distance(layer1TopPos[ord], layer1TopPos[ord + 1]) / 2;
        var angle = Vector2.SignedAngle(Vector2.right, (center - transform.position));
        //注意向量逆时针转动才是正角度
        
        DoubleSpeedApproach[] bullets2 = new DoubleSpeedApproach[ways];

        for (int i = 0; i < ways; i++) {
            var dir = angle - 90f + (float)i / ways * 180f;
            var pos = center + rad2 * dir.Deg2Dir3();
            var bullet = Instantiate(spellBullet, pos, Quaternion.identity);
            bullet.GetComponent<Config>().color = Color.magenta;
            bullet.GetComponent<State>().SetColor(Color.magenta);
            bullet.direction = dir - 180;// + (i % 4 - 2) * 4;
            if (i == ways / 2) {
                layer2TopPos[ord] = pos;
                if (ord == 0) layer2TopPos[5] = pos;
            }
            bullets2[i] = bullet;
            yield return Calc.WaitForFrames(1);
        }
       
        
        DoubleSpeedApproach[] bullets3 = new DoubleSpeedApproach[ways];

        center = (layer2TopPos[ord] + layer2TopPos[ord + 1]) / 2;
        rad2 = Vector3.Distance(layer2TopPos[ord], layer2TopPos[ord + 1]) / 2;
        angle = Vector2.SignedAngle(Vector2.right, (center - transform.position));
        //注意向量逆时针转动才是正角度
        
        for (int i = 0; i < ways; i++) {
            var dir = angle - 90f + (float)i / ways * 180f;
            var pos = center + rad2 * dir.Deg2Dir3();
            var bullet = Instantiate(spellBullet, pos, Quaternion.identity);
            bullet.GetComponent<Config>().color = Color.blue;
            bullet.GetComponent<State>().SetColor(Color.blue);
            bullet.direction = dir - 180; //+ (i % 4 - 2) * 4;
            bullets3[i] = bullet;
            yield return Calc.WaitForFrames(1);
        }
        
        yield return Calc.WaitForFrames(60);
        Move(bullets);
        yield return Calc.WaitForFrames(120);
        Move(bullets2);
        yield return Calc.WaitForFrames(120);
        Move(bullets3);
        
    }
    
    public void Start() {
        layer1TopPos = new Vector3[6];
        layer2TopPos = new Vector3[6];
        for (int i = 0; i < 5; i++) {
            Timing.RunCoroutine(ShootLayer1(i,90f));
        }
    }
}
