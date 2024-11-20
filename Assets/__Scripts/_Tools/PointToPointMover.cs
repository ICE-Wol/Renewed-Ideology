using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;

public class PointToPointMover : MonoBehaviour {
    public bool useCurrentPoint;
    public Vector3 PointA;
    public Vector3 PointB;

    public float approachSpeed;

    public void Start() {
        if (!useCurrentPoint) {
            transform.position = PointA;
        }
        
    }
    public void Update() {
        transform.position = transform.position.ApproachValue(PointB, approachSpeed);
        if (transform.position.Equal(PointB, 0.01f)) {
            GetComponent<Damageable>().curHealth = 0;
        }
    }
}
