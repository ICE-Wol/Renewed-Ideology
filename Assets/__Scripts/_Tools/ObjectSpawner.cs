using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    public GameObject target;
    public int framePeriod;

    private int _timer;

    private void Update() {
        if (_timer % framePeriod == 0) {
            Instantiate(target, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        }
        _timer++;
    }
}
