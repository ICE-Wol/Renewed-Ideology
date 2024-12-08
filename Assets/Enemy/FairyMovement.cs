using System;
using UnityEngine;
using UnityEngine.Splines;

public class FairyMovement : MonoBehaviour
{
    public SplineContainer splineContainer;

    public AnimationCurve positionCurve;

    [Header("需要填写")] 
    public bool isPathReverse;
    public Vector3 offsetPos;
    public float totFrames;

    public bool useStartEndPoints;
    public Vector3 startPoint;
    public Vector3 endPoint;
    
    private int _timer;

    private void Start() {
        transform.position = new Vector3(-10, 10, 0);
    }

    private void Update() {

        var splinePos =
            splineContainer.transform.TransformPoint(
                splineContainer.Spline.EvaluatePosition(
                    positionCurve.Evaluate(isPathReverse ? (1 - _timer / totFrames) : _timer / totFrames)));
        transform.position = splinePos + offsetPos;

        if (useStartEndPoints) {
            transform.position = offsetPos + Vector3.Lerp(startPoint, endPoint,
                positionCurve.Evaluate(isPathReverse ? (1 - _timer / totFrames) : _timer / totFrames));
        }
        
        
        if(_timer >= totFrames) {
            DestroyImmediate(gameObject);
        }
        
        _timer++;
        
        // Debug.DrawLine(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes)),
        //     (Vector3)splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(timer / totframes)) + Vector3.right,
        //     Color.red,1000f);
    }
    
    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(0)), 0.1f);
    //     Gizmos.DrawSphere(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(1)), 0.1f);
    //     Gizmos.DrawSphere(splineContainer.Spline.EvaluatePosition(positionCurve.Evaluate(0.5f)), 0.1f);
    // }
}
