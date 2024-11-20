using System;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveCircleCtrl : MonoBehaviour {
    public LineRenderer lineRenderer;
    public SpriteRenderer tailStar;
    public Vector3 center;
    public float curDegree;
    public float tarDegree;
    public float innerRad;
    public float outerRad;
    //   (outerRad)------(innerRad)------0
    public float alpha;
    private void Update() {
        curDegree.ApproachRef(tarDegree, 64f);
        
        lineRenderer.positionCount = 2;
        var pos1 = center + innerRad * curDegree.Deg2Dir3();
        var pos2 = center + outerRad * curDegree.Deg2Dir3();
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
        
        lineRenderer.startColor = lineRenderer.startColor.SetAlpha(alpha);
        lineRenderer.endColor = lineRenderer.endColor.SetAlpha(alpha);
        
        var pos3 = center + (outerRad + 1) * curDegree.Deg2Dir3();
        tailStar.transform.position = pos3;
    }
}
