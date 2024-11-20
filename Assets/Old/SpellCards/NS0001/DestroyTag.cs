using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using UnityEngine;

public class DestroyTag : MonoBehaviour { 
    public bool destroyTag;
    public int alarm;
    public int timer;

    
    public void SetBulletStateDestroy() {
        GetComponent<State>().SetState(EBulletStates.Destroying);
    }

    public void Update() {
        if (alarm != 0) timer++;
        if (alarm != 0 && timer == alarm)
            SetBulletStateDestroy();

        if (destroyTag) {
            SetBulletStateDestroy();
            destroyTag = false;
        }
    }
}
