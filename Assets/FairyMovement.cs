using System;
using UnityEngine;
using UnityEngine.Splines;

public class FairyMovement : MonoBehaviour
{
    public SplineContainer splineContainer;

    public AnimationCurve positionCurve;
    public float totframes;

    public int timer;

    private void Update() {
        timer++;
        transform.position = splineContainer.transform.TransformPoint(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes)));
        Debug.DrawLine(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes)),
            (Vector3)splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes)) + Vector3.right,
            Color.red,1000f);
    }
}
