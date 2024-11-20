using System;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewTitleCircle : MonoBehaviour
{
    [Header("需要设置的参数")] 
    public NewTitleCircle circleLinePrefab;
    public LineRenderer lineRenderer;

    [Header("是否为根节点")] 
    public bool isRoot;
    
    [Header("使圆的部分可见")]
    public bool isCircleVisible;
    [Header("使线段的部分可见")]
    public bool isLineVisible;
    
    public int pointNum;
    public float radiusToParent;
    public float radiusToSelf;
    public float angleSpdInFrame;


    [Header("不需要设置的参数")] 
    [Header("线段索引")] 
    public NewTitleCircle subLine;
    public NewTitleCircle parentLine;
    public float angle;
    public Vector3 endPos;
    [Header("是否是圆")]
    public bool isCircle;
    
    private void Start()
    {
        if (isCircle) {
            lineRenderer.positionCount = pointNum;
            lineRenderer.loop = true;
            angle = Random.Range(0, 360);
            
            subLine = Instantiate(circleLinePrefab, transform);
            subLine.angle = angle;
            subLine.isCircle = false;
            subLine.isRoot = false;
            subLine.name = "SubLine";
            subLine.angleSpdInFrame = angleSpdInFrame;
            subLine.radiusToParent = radiusToParent;
            subLine.radiusToSelf = radiusToSelf;
            
            if(!isCircleVisible) lineRenderer.enabled = false;
            if(!isLineVisible) subLine.lineRenderer.enabled = false;
        }
        else {
            
            lineRenderer.positionCount = 2;
            lineRenderer.loop = false;
        }
        
        if (isRoot) {
            for (int i = 0; i < 3 * Random.value; i++) {
                GenerateSub(0);
            }
        }
        
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
        
    }

    public void GenerateSub(int layer) {
        var sub = Instantiate(circleLinePrefab, transform);
        sub.isRoot = false;
        sub.isCircle = true;
        sub.parentLine = this;
        sub.radiusToParent = Random.Range(0.5f, 2f);
        sub.radiusToSelf = Random.Range(0.01f, 0.01f);
        sub.angleSpdInFrame = Random.Range(1f, 1f);
        var upLimit = 3 * Random.value;
        if (layer < 3) {
            for (int i = 0; i < upLimit; i++) {
                if (Random.value < 0.5f) {
                    sub.GenerateSub(layer + 1);
                }
            }
        }
    }

    private void Update() {
        angle += angleSpdInFrame;
        //transform.localPosition = angle.Deg2Dir3() * radiusToParent; 
        endPos = (parentLine == null ? Vector3.zero : parentLine.endPos)
                 + angle.Deg2Dir3() * radiusToParent;

        if (!isCircle) {
            lineRenderer.SetPosition(0, endPos);
            lineRenderer.SetPosition(1, parentLine == null ? Vector3.zero : parentLine.endPos);
        } else {
            for (int i = 0; i < pointNum; i++) {
                Vector2 pos = endPos + (360f * i / pointNum).Deg2Dir3() * radiusToSelf;
                lineRenderer.SetPosition(i, pos);
            }
        }
    }
}
