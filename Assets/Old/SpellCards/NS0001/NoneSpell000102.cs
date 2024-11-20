using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class NoneSpell000102 : MonoBehaviour {
    public DoubleSpeedApproach curveScaleBullet;
    public List<DoubleSpeedApproach> bulletList;

    public Vector3 objCenterPoint;
    public float objRotateRadius;
    public float objRotateSpeedMultiplier;
    public float objStartDegree;
    public bool objCounterClockwise;
    
    public float petalRadius;
    public float centerRadius;
    
    public float degree;

    public float rotate;

    public int timer;

    private void Start() {
        bulletList = new List<DoubleSpeedApproach>();
    }

    private void Update() {

        if (rotate >= 360f) {

            if (timer >= 480f) {
                foreach (var bullet in bulletList) {
                    bullet.speed = 0;
                    bullet.endSpeed = 2.5f;
                }
            }

            timer++;
            return;
        }
        
        rotate = timer * objRotateSpeedMultiplier;
        transform.position = objCenterPoint + objRotateRadius * 
            ((objCounterClockwise ? 1 : -1) * rotate + objStartDegree).Deg2Dir3();

        var centerPos = transform.position;
        
        if (timer % 2 == 0) {
            for (int k = 0; k < 2; k++) {
                for (int i = 0; i < 3; i++) {
                    var curPos = centerPos + centerRadius * (rotate * 6f + k * 180f).Deg2Dir3();
                    //var rot = -rotate + 360f / 3f * i;
                    var rot = rotate + 360f / 3f * i;
                    var pos = curPos + petalRadius * rot.Deg2Dir3();
                    var bullet = Instantiate(curveScaleBullet, pos, rot.EulerZ());
                    bullet.direction = rot;
                    if (timer / 2 % 2 == 0) {
                        bullet.GetComponent<DestroyTag>().alarm = 180;
                    }
                    else {
                        bulletList.Add(bullet);
                    }
                }
            }
        }

        timer++;
    }
}
