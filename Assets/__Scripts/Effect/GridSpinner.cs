using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpinner : MonoBehaviour {
    public int timer;
    private void Update() {
        transform.rotation = Quaternion.Euler(0, 0, 9f * Mathf.Sin(timer / 500f));
        timer++;
    }
}
