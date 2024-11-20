using UnityEngine;

//[ExecuteAlways]
public class PointLaserManager : MonoBehaviour
{
    public bool isLoop;
    public LaserPoint[] laserPoints;
    public LaserPoint laserPointPrefab;
    public Color laserColor;
    public LaserState laserState;
    public float laserHitRadius;

    private void Start() {
        laserPoints = GetComponentsInChildren<LaserPoint>(false);
        for(int i = 0; i < laserPoints.Length; i++) {
            laserPoints[i].nextPoint = laserPoints[(i + 1) % laserPoints.Length];
            laserPoints[i].hitRadius = laserHitRadius;
        }
        SetColor(laserColor);
    }
    
    public void Update() {
        foreach (var p in laserPoints) {
            p.SetState(laserState);
        }
    }

    public void SetState(LaserState state) {
        laserState = state;
        foreach (var p in laserPoints) {
            p.SetState(laserState);
        }
    }
    

    public void SetColor(Color color) {
        laserColor = color;
        for (int i = 0; i < laserPoints.Length; i++) {
            laserPoints[i].lineRenderer.material.SetColor("_Color", color);   
        }
        
        foreach (var p in laserPoints) {
            p.SetColor(color);
        }
    }
    
    public void SetClose() {
        foreach (var p in laserPoints) {
            p.isNodeActive = false;
            p.SetState(LaserState.Inactive);
        }
    }
}
