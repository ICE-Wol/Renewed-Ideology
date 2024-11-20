using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;

public class Follow : BulletMovement
{
    public Transform followTarget;
    public State state;
    public float approachRate;

    // Update is called once per frame
    void Update() {
        Movement(transform);
    }


    private Vector2 oldPos;
    public override void Movement(Transform transform) {
        if (followTarget != null) {
            oldPos = transform.position;
            
            transform.position = transform.position.ApproachValue(followTarget.position, approachRate);
            
            direction = Vector2.SignedAngle((Vector2)transform.position - oldPos, Vector2.right);
        }
        else {
            state.SetState(EBulletStates.Destroying);
        }
    }
}
