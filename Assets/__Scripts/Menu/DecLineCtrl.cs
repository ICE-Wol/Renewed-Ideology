using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecLineCtrl : MonoBehaviour {
    public LineRenderer lineRenderer;
    public bool isMirrored;
    public float basicLength;
    public float maxExtraLength;
    public float speedMultiplier;
    
    public float extraLength;

    public GameObject circle;
    
    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, 
            transform.position + basicLength * (isMirrored ? Vector3.up : Vector3.down));
    }
    
    private void Update() {
        extraLength = Mathf.Sin(Mathf.Deg2Rad * (Time.time * speedMultiplier) + transform.position.x) * maxExtraLength;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position
                                    + (basicLength + extraLength) * (isMirrored ? Vector3.up : Vector3.down));
        circle.transform.position =
            transform.position + (basicLength + extraLength) * (isMirrored ? Vector3.up : Vector3.down);
    }

}
