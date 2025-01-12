using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using _Scripts;
using MEC; 


public class WideSniper : BulletGenerator {
    public DoubleSpeedApproach bulletTemplates;
    public DoubleSpeedApproach bullet2Template;
    public float bulletNum;
    public float bulletInterval;

    public float genRadius;
    
    public override IEnumerator<float> ShootSingleWave() {
        for (int c = 0; c < layerCountInWave; c++) {
            float centerDegree = Vector2.SignedAngle(Vector2.right,
                PlayerCtrl.instance.transform.position - transform.position);
            ;
            DoubleSpeedApproach b;
            for (int i = 0; i < bulletNum; i++) {
                if (c == 0) b = Instantiate(bulletTemplates, transform.position, Quaternion.identity);
                else b = Instantiate(bullet2Template, transform.position, Quaternion.identity);
                var deg = centerDegree - bulletInterval * (i - (bulletNum - 1) / 2f);
                var pos = transform.position + genRadius * deg.Deg2Dir3();
                b.direction = deg;
                b.transform.position = pos;
                b.startFrame = 0;
                b.approachRate = 16;
                b.endSpeed = 0.2f + (bulletNum / 2 - Mathf.Abs(i - (bulletNum - 1) / 2)) * 0.2f;
            }
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave().CancelWith(gameObject));
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }
}
