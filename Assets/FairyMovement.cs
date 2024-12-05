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
        transform.position = splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes));
    }
}
