using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooter : MonoBehaviour {
    public int intervalFrame;
    public int direction;
    
    public ArrowCtrl arrowPrefab;
    
    private int timer = 0;
    
    void Update() {
        if (timer % intervalFrame == 0) {
            var arrow = Instantiate(arrowPrefab, transform);
            arrow.direction = direction;
        }
        timer++;
    }
}
