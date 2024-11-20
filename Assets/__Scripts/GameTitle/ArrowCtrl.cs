using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    public float direction;
    public float speed;
    
    public int timer = 0;
    void Update() {
        direction = 0;
        var actualSpeed = speed;// * Mathf.Abs(Mathf.Sin(timer * Mathf.Deg2Rad * 3f) - 0.2f);
        transform.localPosition += Time.fixedDeltaTime * actualSpeed * direction.Deg2Dir3();
        transform.localRotation = Quaternion.Euler(0,0,direction);
        
        if(transform.position.x < -15f || transform.position.x > 15f || transform.position.y < -15f || transform.position.y > 15f) {
            Destroy(gameObject);
        }

        timer++;
    }
}
