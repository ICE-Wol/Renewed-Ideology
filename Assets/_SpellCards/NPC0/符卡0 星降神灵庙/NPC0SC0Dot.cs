using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;

public class NPC0SC0Dot : MonoBehaviour
{
    public bool isClockwise;
    public Vector3 centerPoint;
    public State state;
    public DoubleSpeedApproach selfMovement;
    public DoubleSpeedApproach ricePrefab;
    public DoubleSpeedApproach bacPrefab;

    private void Start() {
        state.OnBulletDestroy += () => {
            // 把方法加入到OnBulletDestroy事件中
            GenerateChildBullet();
        };
        
        //state.OnBulletDestroy +=GenerateRice;
    }

    public void GenerateChildBullet() {
        DoubleSpeedApproach bullet;
        bullet = Instantiate(isClockwise ? ricePrefab : bacPrefab, transform.position, Quaternion.identity);

        bullet.transform.position = transform.position; // + 0.5f * rice.direction.Deg2Dir3();
        bullet.direction = -Vector2.SignedAngle(bullet.transform.position - centerPoint,Vector2.right) + 90;

        // if (!isClockwise) {
        //     DoubleSpeedApproach bullet1 = Instantiate(ricePrefab, transform.position, Quaternion.identity);
        //     bullet1.transform.position = transform.position; // + 0.5f * rice.direction.Deg2Dir3();
        //     bullet1.direction = -Vector2.SignedAngle(((Vector2)bullet.transform.position - new Vector2(0,2)),Vector2.right) + 81;
        //     bullet1.endSpeed -= 0.1f;
        //     
        //     DoubleSpeedApproach bullet2 = Instantiate(ricePrefab, transform.position, Quaternion.identity);
        //     bullet2.transform.position = transform.position; // + 0.5f * rice.direction.Deg2Dir3();
        //     bullet2.direction = -Vector2.SignedAngle(((Vector2)bullet.transform.position - new Vector2(0,2)),Vector2.right) + 91;
        //     bullet2.endSpeed -= 0.1f;
        // }
    }

    // private void Update() {
        //     if(!hasSpawnedRice && selfMovement.IsSpeedChangeFinished(0.1f)){//state.GetState() == EBulletStates.Destroying) {
        //         hasSpawnedRice = true;
        //         print("Spawned");
        //         for (int i = 0; i < 360; i += 30) {
        //             DoubleSpeedApproach rice = Instantiate(ricePrefab, transform);
        //             rice.direction = i;
        //             rice.transform.position = transform.position + 0.5f * rice.direction.Deg2Dir3();
        //         }
        //     }
        // }
}
