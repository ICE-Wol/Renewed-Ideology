using System.Collections.Generic;
using UnityEngine;
using _Scripts.EnemyBullet;

public class BulletEraser : MonoBehaviour
{
    public bool isInstant;
    public float radius;
    public float frameSpeed = 0.1f;
    
    private void EraseBulletsWithinRange() {
        // var copySet = new HashSet<State>();
        // foreach (var b in State.bulletSet) {
        //     copySet.Add(b);
        // }
        // foreach (var b in copySet) {
        //     if (b == null) {
        //         State.bulletSet.Remove(b);
        //         continue;
        //     }
        //     if (b.GetState() == EBulletStates.Template) continue;
        //     if (Vector2.Distance(b.transform.position, transform.position) < radius) {
        //         b.SetState(EBulletStates.Destroying);
        //         State.bulletSet.Remove(b);
        //     }
        // }
        var copyList = new List<BulletRuntimeState>();
        foreach (var b in TimelineRunner.instance.snapshot.states) {
            copyList.Add(b);
        }
        foreach (var b in copyList) {
            if (b.isAlive && Vector2.Distance(b.position, transform.position) < radius) {
                b.isAlive = false;
            }
        }
    }
    
    public void Update() {
        if (isInstant) {
            radius = 10f;
            EraseBulletsWithinRange();
            Destroy(gameObject);
        }else {
            radius += frameSpeed;
            EraseBulletsWithinRange();
            if (radius > 10f) {
                Destroy(gameObject);
            }
        }
    }
    
    
}
