using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0NS7 : MonoBehaviour
{
    public DoubleSpeedApproach spellBullet;
    public int ways1;
    public int ways2;
    public float range;

    public DoubleSpeedApproach[] SpellBullets;
    public void Shoot() {
        for (int i = 0; i < ways1; i++) {
            var bullet = Instantiate(spellBullet, transform.position, Quaternion.identity);
            SpellBullets[i] = bullet;
            bullet.direction = (float)i / ways1 * 360;
            
            Color color = Color.HSVToRGB(0f, 0f, 1f);
            bullet.GetComponent<Config>().color = color;
            bullet.GetComponent<State>().SetColor(color);
        }
    }

    public IEnumerator<float> ShootSubBullet(float range,float startSpeed,float endSpeed,float approachRate) {
        float[,] subBulletsDir = new float[ways1, ways2];
        float[,] subBulletsSat = new float[ways1, ways2];
        DoubleSpeedApproach[,] subBullets = new DoubleSpeedApproach[ways1, ways2];
        int timer = 0;
        for (int i = 0; i < ways1; i++) {
            if (SpellBullets[i] == null) continue;
            for (int j = 0; j < ways2; j++) {
                var bullet = Instantiate(spellBullet, SpellBullets[i].transform.position, Quaternion.identity);
                subBullets[i,j] = bullet;
                subBulletsDir[i,j] = SpellBullets[i].direction - range / 2 + (float)j / (ways2 - 1) * range;
                bullet.direction = SpellBullets[i].direction;
                bullet.transform.rotation = Quaternion.Euler(0, 0, SpellBullets[i].direction);
                bullet.SetSpeed(startSpeed);
                bullet.endSpeed = endSpeed;
                bullet.approachRate = approachRate;

                
                Color color = Color.HSVToRGB(Mathf.Abs(j - ways2 / 2) / (ways2 / 2f), 0f, 1f);
                bullet.GetComponent<Config>().color = color;
                bullet.GetComponent<State>().SetColor(color);
                
                bullet.GetComponent<State>().SetState(EBulletStates.Activated);
            }
        }

        for (int i = 0; i < ways1; i++)
            Destroy(SpellBullets[i].gameObject);

        while (timer < 60) {
            timer++;
            for (int i = 0; i < ways1; i++) {
                for (int j = 0; j < ways2; j++) {
                    if (subBullets[i, j] == null) continue;
                    //subBullets[i, j].direction.ApproachRef(subBulletsDir[i, j], 64f);
                    subBullets[i, j].direction = Mathf.SmoothStep((float)i / ways1 * 360f, subBulletsDir[i, j], (float)timer / 60);
                    subBullets[i,j].SetSpeed(Mathf.SmoothStep(startSpeed,endSpeed,(float)timer / 60));
                    subBulletsSat[i, j].ApproachRef(1f - (32f - approachRate) / 16f, approachRate / 2f);
                    Color color = Color.HSVToRGB(Mathf.Abs(j - ways2 / 2) / (ways2 / 2f), subBulletsSat[i, j], 1f);
                    subBullets[i, j].GetComponent<Config>().color = color;
                    subBullets[i, j].GetComponent<State>().SetColor(color);
                }
            }
            
            yield return Calc.WaitForFrames(1);
        }

    }
    
    public void Start() {
        SpellBullets = new DoubleSpeedApproach[ways1];
        Shoot();
    }
    
    bool isSubGenerated = false;
    public void Update() {
        bool flag = true;
        for (int i = 0; i < ways1; i++) {
            if (SpellBullets[i] == null) continue;
            if (!SpellBullets[i].IsSpeedChangeFinished(0.5f)) {
                flag = false;
                break;
            }
        }

        if (flag && !isSubGenerated) {
            isSubGenerated = true;
            Timing.RunCoroutine(ShootSubBullet(range, 0.5f, 1f, 16f));
            Timing.RunCoroutine(ShootSubBullet(range, 0.5f, 1.5f, 24f));
            Timing.RunCoroutine(ShootSubBullet(range, 0.5f, 1.75f, 32f));
            Timing.RunCoroutine(ShootSubBullet(range, 0.5f, 2.0f, 32f));
            Timing.RunCoroutine(ShootSubBullet(range + 50f, 0.5f, -1f, 16f));
            Timing.RunCoroutine(ShootSubBullet(range + 50f, 0.5f, -1.5f, 24f));
        }
    }
}
