
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Rotator : MonoBehaviour {
    private int _timer;
    
    public bool isOrbit;
    public Transform centerTransform;
    public float radius;
    public bool isRadiusFloat;
    public float sinDivider = 2f;
    public float speedDivider = 2f;
    public float orbitMultiplier;
    
    public bool isRotate;
    public float initDegree;
    public float rotateMultiplier;
    private void Update() {
        if (isOrbit) {
            if (isRadiusFloat) {
                var rad = radius * (Mathf.Sin(Time.time/speedDivider) / sinDivider + 1f);
                transform.position = centerTransform.position
                                     + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (initDegree + orbitMultiplier * _timer)),
                                         Mathf.Sin(Mathf.Deg2Rad * (initDegree + orbitMultiplier * _timer)), 0) * rad;
            }
            else {
                transform.position = centerTransform.position
                                     + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (initDegree + orbitMultiplier * _timer)),
                                         Mathf.Sin(Mathf.Deg2Rad * (initDegree + orbitMultiplier * _timer)), 0) * radius;
            }
        }

        if(isRotate)transform.rotation = Quaternion.Euler(0,0,initDegree + rotateMultiplier * _timer);
        _timer++;
    }
}
