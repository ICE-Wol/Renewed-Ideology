using System;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserFlower : MonoBehaviour
{
    public SingleLineLaserCtrl laserCtrlPrefab;
    public SingleLineLaserCtrl[] lasers;
    public float randDir;
    public int laserNum;
    public bool isOdd;
    public Color laserColor;
    public Color evenColor;
    public int laserLiveTime;
    public int laserActiveTime;
    public int timer;

    private void Start() {
        randDir = UnityEngine.Random.Range(-10, 20);
        lasers = new SingleLineLaserCtrl[laserNum];
        for (int i = 0; i < laserNum; i++) {
            var laser = Instantiate(laserCtrlPrefab,transform);
            laser.transform.position = new Vector3(0, 2, 0);
            laser.laserRoot.transform.position = new Vector3(0, 2, 0);
            laser.dir = 90f;
            lasers[i] = laser;
            
            laser.SetHeadState(LaserState.Inactive);
            laser.SetHeadActive(false);
            laser.SetColor(laserColor);
            if(!isOdd) laser.SetColor(evenColor);
            laser.laserRoot.isLaserThin = true;
        }
    }

    private void Update() {
        if(timer == laserActiveTime / 2) {
            foreach (var laser in lasers) {
                laser.SetHeadState(LaserState.WarnLine);
                laser.length = 0;
            }
            
        }

        if (timer >= laserActiveTime / 2 && timer <= laserActiveTime) {
            for (int i = 0; i < laserNum; i++) {
                lasers[i].length += 0.2f;

                var tarDir = randDir - 90f + 360f / laserNum * i;
                lasers[i].dir.ApproachRef(tarDir, 16f);
            }
        }

        if (timer == laserActiveTime) {
            foreach (var laser in lasers) {
                laser.SetRootState(LaserState.Active);
                laser.SetHeadState(LaserState.Inactive);
            }
        }
        
        if (timer == laserLiveTime) {
            foreach (var laser in lasers) {
                laser.SetRootState(LaserState.Inactive);
                laser.SetHeadState(LaserState.Inactive);
                laser.SetRootActive(false);
            }
        }

        if(timer - laserLiveTime > 60) {
            Destroy(gameObject);
        }
        timer++;
        
        
    }
}
