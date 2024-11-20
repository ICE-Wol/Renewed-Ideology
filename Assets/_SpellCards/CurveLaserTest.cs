using _Scripts.Tools;
using UnityEngine;

public class CurveLaserTest : MonoBehaviour
{
    private int timer = 0;
    private float dir = 90;
    void Update() {
        transform.position += 0.02f * dir.Deg2Dir3();
        if(timer < 180)
            dir++;
        else if (timer < 360)
            dir--;
        timer++;
    }
}
