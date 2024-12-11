using System;
using UnityEngine;
using UnityEngine.Splines;

public class FairyMovement : MonoBehaviour
{
    public SplineContainer splineContainer;
    public AnimationCurve positionCurve;
    private ISetAlpha _alphaSetter;
    

    [Header("需要填写")] 
    public bool isPathReverse;
    public Vector3 offsetPos;
    public float totFrames;

    public bool useStartEndPoints;
    public Vector3 startPoint;
    public Vector3 endPoint;

    [Header("半透明阶段的路径比例，0-1")] 
    [Range(0f,1f)]
    public float alphaNode;
    
    private int _timer;

    private void Start() {
        transform.position = new Vector3(-10, 10, 0);
        _alphaSetter = GetComponent<ISetAlpha>();
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
        
        if(_alphaSetter != null) {
            _alphaSetter.SetAlpha(_timer / totFrames < alphaNode ? _timer / totFrames / alphaNode : 1);
        }else {
            Debug.LogWarning("No Alpha Setter Found");
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
