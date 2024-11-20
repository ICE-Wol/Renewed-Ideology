using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;
public class TitleCircleCtrl : MonoBehaviour {
    public LineRenderer lineRenderer;
    public Vector3 centerPos;
    public float radius;
    public int pointNum;
    public float alpha;

    public bool isAppearing;
    public float curRadius = 0f;
    private void Start() {
        lineRenderer.startColor = lineRenderer.startColor.SetAlpha(alpha);
        lineRenderer.endColor = lineRenderer.endColor.SetAlpha(alpha);
        lineRenderer.positionCount = pointNum;
        
        centerPos = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height / 2));
        for (int i = 0; i < pointNum; i++) {
            float angle = 2 * Mathf.PI * i / pointNum;
            lineRenderer.SetPosition(i, new Vector3(centerPos.x + Mathf.Cos(angle) * curRadius,
                centerPos.y + Mathf.Sin(angle) * curRadius, -5f));
        }
    }

    private void Update() {
        centerPos = transform.position;
        if (isAppearing) {
            curRadius = curRadius.ApproachValue(radius, 16f);
        }
        for (int i = 0; i < pointNum; i++) {
            float angle = 2 * Mathf.PI * i / pointNum;
            lineRenderer.SetPosition(i, new Vector3(centerPos.x + Mathf.Cos(angle) * curRadius,
                centerPos.y + Mathf.Sin(angle) * curRadius, -centerPos.z));
        }
    }

}
