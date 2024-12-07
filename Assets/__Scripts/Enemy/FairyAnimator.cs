using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using UnityEngine;

[ExecuteAlways]
public class FairyAnimator : MonoBehaviour {
    public enum FairyType{
        FairyBig,
        FairyBigDark,
        FairyMidRed,
        FairyMidBlue,
        FairyLeadRed,
        FairyLeadBlue,
        FairySmallRed,
        FairySmallBlue,
        FairySmallGreen,
        FairySmallGold,
    }
    
    public FairyType fairyType;
    
    public SpriteRenderer spriteRenderer;
    public Sprite[] animSequence;
    
    public Vector3 prePosition;
    public Vector3 direction;

    public int movePointer;
    public int idlePointer;
    
    public int frameSpeed = 8;
    public int timer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        prePosition = transform.position;
        animSequence = GameManager.Manager.GetFairyAnimSequences(fairyType);
        spriteRenderer.sprite = animSequence[0];
        
    }

    private void Update() {
        PlayAnim();
        timer++;
    }

    protected void PlayAnim() {
        direction = (transform.position - prePosition).normalized;
        if (timer % frameSpeed == 0) {
            float horVector = direction.x;
            bool hasHorizontalMovement = !horVector.Equal(0f);
            if (hasHorizontalMovement) {
                movePointer += (int)Mathf.Sign(horVector);
                bool movePointerReachEdges = (Math.Abs(movePointer) == 8);
                if (movePointerReachEdges) movePointer -= 4 * Math.Sign(movePointer);
                spriteRenderer.sprite = animSequence[4 + Math.Abs(movePointer)];
                spriteRenderer.flipX = (movePointer < 0);
            }
            else {
                bool remainSideAnimation = (movePointer != 0);
                if (remainSideAnimation) {
                    movePointer -= Math.Sign(movePointer);
                    spriteRenderer.sprite = animSequence[4 + Math.Abs(movePointer)];
                    spriteRenderer.flipX = (movePointer < 0);
                }
                else {
                    idlePointer++;
                    if (idlePointer == 4) idlePointer = 0;
                    spriteRenderer.sprite = animSequence[idlePointer];
                }
            }
        }
        prePosition = transform.position;
    }
}
