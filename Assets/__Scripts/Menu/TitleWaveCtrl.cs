using System;
using _Scripts.Tools;
using UnityEngine;

public class TitleWaveCtrl : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float leftX;
    public float rightX;
    public int count;
    public float alpha;

    public float maxY;
    
    public bool flipY;
    private void Start() {
        lineRenderer.startColor = lineRenderer.startColor.SetAlpha(alpha);
        lineRenderer.endColor = lineRenderer.endColor.SetAlpha(alpha);
        lineRenderer.positionCount = count;
    }

    private void Update() {
        for(int i = 0;i < count;i++) {
            float x = Mathf.Lerp(leftX, rightX, (float)i / (count - 1));
            lineRenderer.SetPosition(i, new Vector3(x,  maxY* Mathf.Sin((x + Time.time)), 0f));
            if (flipY) {
                lineRenderer.SetPosition(i, new Vector3(x, -maxY* Mathf.Sin((x + Time.time)), 0f));
            }
        }
    }
}
