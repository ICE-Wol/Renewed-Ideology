using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using UnityEngine;

public class NPC0SC2Rice : MonoBehaviour
{
    public bool isPositive;
    public int upLimit;
    public float speed;
    public DoubleSpeedApproach doubleSpeedApproach;

    public int maxOrder;
    public int curOrder;
    
    public int timer;
    public bool flag;
    void Update()
    {
        if (timer * speed < upLimit) {
            doubleSpeedApproach.direction += (isPositive ? 1 : -1) * speed;
        }
        if(timer * speed >= upLimit && flag == false) {
            doubleSpeedApproach.direction += Random.Range(-18f, 18f) * curOrder / maxOrder;
            doubleSpeedApproach.speed -= 1f;
            flag = true;
        }

        timer++;
    }
}
