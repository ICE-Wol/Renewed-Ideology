using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class TriFragCtrl : MonoBehaviour {
    public TriFragCtrl triFragPrefab;
    public SpriteRenderer triFragBlur;
    
    public bool isHead;
    public TriFragCtrl head;

    public int childOrder;
    public TriFragCtrl child;
    public TriFragCtrl parent;

    public float actualScale;
    public float curYScale;
    public float tarYScale;
    public bool isFlipping;
    
    public bool isCounterClockwise;
    public float degSpeed;
    public float curRadius;
    public float tarRadius;
    public float direction;
    public float oriDirection;
    public float rotation;
    
    public SpriteRenderer circlePrefab;
    public bool isCircleMode;
    public float circleScale;
    
    public int timer;

    public void Start() {
        if(isHead) CreateChild();
        direction = oriDirection;
    }

    public void CreateChild() {
        if (actualScale <= 0.1f) return;
        child = Instantiate(triFragPrefab, transform.position + 0.1f * Vector3.left, Quaternion.identity);
        
        child.isHead = false;
        child.head = head;
        child.childOrder = childOrder + 1;
        child.parent = this;
        
        child.actualScale = actualScale - 0.1f;
        child.curYScale = curYScale;
        child.tarYScale = tarYScale;
        child.CreateChild();
    }

    private void Update() {
        if (isHead && !isFlipping && Input.GetKeyDown(KeyCode.F)) {
            tarYScale *= -1;
            isFlipping = true;
        }
        
        if(isFlipping && Mathf.Abs(curYScale) <= 0.3f) {
            isFlipping = false;
            if (child != null) {
                child.tarYScale *= -1;
                child.isFlipping = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            isCircleMode = !isCircleMode;
        }
        
        if (!isCircleMode) {
            //curYScale.ApproachRef(tarYScale, 36f);

            transform.localScale =
                new Vector3(actualScale * 2f, curYScale * (3f * (1f - Mathf.Cos(actualScale * Mathf.PI * 0.5f))), 1);
            
            transform.localRotation = Quaternion.Euler(0, 0, rotation); 

            if (!isHead && parent != null) {
                var newPos = transform.position.ApproachValue(parent.transform.position, 32f * Vector3.one);
                rotation = Vector2.SignedAngle(Vector3.right, newPos - transform.position);
                transform.position = newPos;
                
                direction.ApproachRef(parent.direction, 32f);
                curYScale.ApproachRef(parent.curYScale, 32f);
            }
        }

        if (isCircleMode) {
            if (isHead) transform.position = curRadius * direction.Deg2Dir3();
            else {
                var targetPos = head.transform.position + curRadius * 0.3f * (direction + childOrder * 360f / 10f).Deg2Dir3();
                transform.position = transform.position.ApproachValue(targetPos, 32f * Vector3.one);
            }
        }
        
        if (isHead) {
            direction += (isCounterClockwise ? -1 : 1) * (degSpeed + 0.1f * Mathf.Sin(timer * Mathf.Deg2Rad));

            var stretchTag = Mathf.Sin(timer * Mathf.Deg2Rad) > 0f;
            tarYScale = stretchTag ? 0.6f : 1.2f;
            curYScale.ApproachRef(tarYScale, 48f);
            
            curRadius.ApproachRef(tarRadius + 0.5f * Mathf.Sin(timer * Mathf.Deg2Rad), 32f);
            
            var newPos = curRadius * direction.Deg2Dir3();
            rotation = Vector2.SignedAngle(Vector3.right, newPos - transform.position);
            transform.position = newPos;
        }
        
        triFragBlur.color = triFragBlur.color.SetAlpha(0.5f + 0.5f * Mathf.Sin((timer + childOrder * 30f) * Mathf.Deg2Rad));
        

        timer++;
    }
}
