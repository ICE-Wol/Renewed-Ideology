using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class FullMoonShoot : MonoBehaviour {
    public DoubleSpeedApproach[] bulletTemplate;
    public List<(DoubleSpeedApproach,int)> bullets;

    public bool isMirror;
    public int bulletPerCircle;
    public int circleDegreeOffset;
    public int initDegreeOffset;
    public int layers;

    public float basicSpdAfterChange;
    public float extraSpdAfterChange;
    
    
    
    public IEnumerator<float> Shoot() {
        for (int k = 0; k < layers; k++) {
            for (int i = 0; i < bulletPerCircle; i++) {
                var b = Instantiate(bulletTemplate[0], transform.position, Quaternion.identity);
                var dir = (isMirror ? -1 : 1)
                          * (360f / bulletPerCircle * i
                             + circleDegreeOffset
                             + initDegreeOffset);
                b.direction = dir;
                b.speed = 3f;
                b.endSpeed = 0f;
                bullets.Add((b, circleDegreeOffset / 90));
            }
            circleDegreeOffset += 10;
            yield return Timing.WaitForOneFrame;
        }
        
        var d = Timing.RunCoroutine(GameManager.WaitForFrames(30));
        yield return Timing.WaitUntilDone(d);

        foreach (var b in bullets) {
            if (b.Item1 == null) continue;

            var j = Instantiate(bulletTemplate[1]);
            j.transform.position = b.Item1.transform.position;
            j.direction = b.Item1.direction + (isMirror ? -1 : 1) * 30f;
            j.speed = basicSpdAfterChange + extraSpdAfterChange * b.Item2;
            Destroy(b.Item1.gameObject);
        }
        
        Destroy(this.gameObject);
    }
    
    public void Start() {
        bullets = new List<(DoubleSpeedApproach, int)>();
        initDegreeOffset = Random.Range(0, 360);
        Timing.RunCoroutine(Shoot());
    }

    
}
