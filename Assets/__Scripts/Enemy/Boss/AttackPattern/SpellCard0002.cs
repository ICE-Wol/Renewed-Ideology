using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Scripts.Enemy.Boss;
using UnityEngine;

public class SpellCard0002 : AttackPattern {
    public Generator000201 generator;
    public int interval;
    public int timer;
    void Update()
    {
        if (timer % interval == 0) {
            for (int i = 0; i < 12; i++) {
                var g = Instantiate(generator, transform.position, Quaternion.Euler(0, 0, 0));
                g.targetSpeed = 10f + Random.Range(0f, 4f);
                g.targetDir = 360f / 12f * i;
            }
        }

        timer++;
    }


}
