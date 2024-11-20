using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using UnityEngine;

public class BulletGroup : MonoBehaviour {
    public GameObject mainBullet;
    public GameObject[] bullets;

    public bool HasBulletLeft() {
        foreach (var b in bullets) {
            if (b != null) {
                return true;
            }
        }

        return false;
    }

    public void Update() {
        if (!HasBulletLeft()) {
            Destroy(gameObject);
        }
    }
}
