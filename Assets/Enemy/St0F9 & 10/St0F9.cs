using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
public class St0F9 : BulletGenerator
{
    public DoubleSpeedApproach petalBullet;
    public Color petalStartColor;
    public Color petalEndColor;
    
    public int startTime;
    public int endTime;
    public int timer;
    

    public override IEnumerator<float> ShootSingleWave() {
        yield break;
    }

    public override IEnumerator<float> AutoShoot() {
        yield break;
    }

    public void ShootFlower() {
        int bulletNum = 36;
        int petalNum = 6;
        int petalBulletNum = bulletNum / petalNum;
        int layer = 3;
        float radius = 0.5f;
        float initDir = Random.Range(0, 360);
        Vector3 randPos = Random.insideUnitCircle * 0.5f; 
        for (int k = 0; k < layer; k++) {
            for (int i = 0; i < bulletNum; i++) {
                var dir = initDir + 360f / bulletNum * i;
                var order = petalBulletNum / 2 - Mathf.Abs(i % petalBulletNum - petalBulletNum / 2);
                var b = Calc.GenerateBullet(petalBullet,
                    (radius + order / 3f - k * 0.3f) * dir.Deg2Dir3() + transform.position + randPos,
                    dir);
                (b as DoubleSpeedApproach).endSpeed += 0.5f * k;
                b.bulletState.SetColor(Calc.LerpColorInRGB(petalStartColor,petalEndColor, k / (layer - 1f)));
            }
        }

    }

    private void OnDisable() {
        ShootFlower();
    }


    // Update is called once per frame
    void Update() {
        timer++;
    }
}
