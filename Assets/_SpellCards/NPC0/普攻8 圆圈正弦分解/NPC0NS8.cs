using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0NS8 : MonoBehaviour
{
    public DoubleSpeedApproach spellBullet;
    public int ways1;
    
    public void Shoot() {
        DoubleSpeedApproach[] SpellBullets = new DoubleSpeedApproach[ways1];
        for (int i = 0; i < ways1; i++) {
            var bullet = Instantiate(spellBullet, transform.position, Quaternion.identity);
            SpellBullets[i] = bullet;
            bullet.direction = (float)i / ways1 * 360;
            //bullet.endSpeed = Mathf.Sin(bullet.direction * 6f * Mathf.Deg2Rad)/2f + 1.5f;
            
            Color color = Color.HSVToRGB(0f, 0f, 1f);
            bullet.GetComponent<Config>().color = color;
            bullet.GetComponent<State>().SetColor(color);
        }
        Timing.RunCoroutine(ShootSubBullet(SpellBullets));
    }

    public IEnumerator<float> ShootSubBullet(DoubleSpeedApproach[] bullets) {
        
        yield return Calc.WaitForFrames(30);
        

        for (int i = 0; i < ways1; i++) {
            if (bullets[i] == null) continue;
            bullets[i].direction *= 7f;
            if (i % 2 == 0) bullets[i].direction *= -1f;
        }
        
        //yield return Calc.WaitForFrames(30);
        
        // for (int i = 0; i < ways1; i++) {
        //     if (bullets[i] == null) continue;
        //     bullets[i].direction *= 4;
        // }
        int timer = 0;
        while (timer < 60) {
            timer++;
            for (int i = 0; i < ways1; i++) {
                if (bullets[i] == null) continue;
                bullets[i].direction += Mathf.Sin(i * 18 * Mathf.Deg2Rad);
                //bullets[i].endSpeed = (-Mathf.Sin(bullets[i].direction*6f* Mathf.Deg2Rad) + 1f) + 1f;
                
            }
            
            yield return Calc.WaitForFrames(1);
        }

    }

    public int t = 0;
    public void Update() {
        t++;
        if(t % 120 == 0) Shoot();
    }
}
