using _Scripts.Tools;
using UnityEngine;

public class TransformApproacher : MonoBehaviour
{
    [Header("位置跟随")]
    public bool activatePosApproach;
    public Vector3 targetPos;
    public Vector3 posApproachRate = 16 * Vector3.one;
    
    [Header("旋转跟随")]
    public bool activateRotApproach;
    public Vector3 targetRot;
    public Vector3 rotApproachRate = 16 * Vector3.one;
    private Vector3 _curRot;
    
    [Header("缩放跟随")]
    public bool activateScaleApproach;
    public Vector3 targetScale;
    public Vector3 scaleApproachRate = 16 * Vector3.one;

    public void Update() {
        if (activatePosApproach) {
            transform.localPosition = transform.localPosition.ApproachValue(targetPos, posApproachRate);
        }

        if (activateRotApproach) {
            _curRot.ApproachRef(targetRot, rotApproachRate);

            transform.localRotation = Quaternion.Euler(_curRot);
        }

        if (activateScaleApproach) {
            transform.localScale = transform.localScale.ApproachValue(targetScale, scaleApproachRate);
        }
    }

}
