
using UnityEngine;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;

public class FairyCtrl : MonoBehaviour {
    public int lastSegment;
    public int beginSegment;
    public int endSegment;
    public Path path;
    public PathFollower pathFollower;
    public BulletGenerator spawner;
    

    public enum BulletPattern {
        SingleWaveInPoints,
        AutoShootInPeriod,
    }

    public BulletPattern bulletPattern;
    
    
    private void Update() {
        if (lastSegment != pathFollower.currentSegment) {
            lastSegment = pathFollower.currentSegment;
            if (bulletPattern == BulletPattern.AutoShootInPeriod) {
                if (lastSegment == beginSegment) {
                    Timing.RunCoroutine(spawner.AutoShoot().CancelWith(gameObject),"curGenerator");
                    
                }
                else if (lastSegment == endSegment) {
                    Timing.KillCoroutines("curGenerator");
                }
            }else if (bulletPattern == BulletPattern.SingleWaveInPoints) {
                if (lastSegment >= beginSegment && lastSegment <= endSegment) {
                    Timing.RunCoroutine(spawner.ShootSingleWave().CancelWith(gameObject),"curGenerator");
                }
            }

        }
        //if (pathFollower.currentSegment == path.waypoints.Count - 1) {
        if (lastSegment == path.waypoints.Count - 1) {
            Destroy(gameObject);
        }

    }
}
