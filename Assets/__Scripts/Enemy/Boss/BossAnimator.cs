using System;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts.Enemy {
    public class BossAnimator : MonoBehaviour {
        [SerializeField] private Sprite[] idleSpriteSequence;
        [SerializeField] private Sprite[] moveLeftSequence;
        [SerializeField] private Sprite[] moveRightSequence;
        [SerializeField] private Sprite[] actionSequence;

        [SerializeField] private Sprite[] animEnemyMove;
        [SerializeField] private Sprite[] animEnemyEnchant;
        

        public SpriteRenderer spriteRenderer;

        public int frameSpeed;
        protected int timer;
        protected int idlePointer;
        protected int movePointer;
        protected int enchantPointer;
        protected Vector3 prePosition;
        protected Vector2 direction;

        [SerializeField] protected bool isEnchanting;
        public bool IsEnchanting {
            set => isEnchanting = value;
        }

        private Sprite GetSpriteMovePointer(int movePointer) {
            return animEnemyMove[(movePointer < 0)
                ? ((movePointer < -moveFrameCount) ? 0 : (movePointer + moveFrameCount))
                : ((movePointer > moveFrameCount) ? 
                    moveFrameCount * 2 + idleFrameCount - 1 : 
                    (movePointer + moveFrameCount + idleFrameCount - 1))];
        }

        // 新增变量
        [SerializeField]
        private int idleFrameCount;
        [SerializeField]
        private int moveFrameCount;
        protected void PlayAnim() {
            direction = (transform.position - prePosition).normalized;
            if (timer % frameSpeed == 0) {
                if (isEnchanting) {
                    if (enchantPointer == actionSequence.Length) enchantPointer = 0;
                    spriteRenderer.sprite = animEnemyEnchant[enchantPointer];
                    enchantPointer++;
                }
                else {
                    float horVector = direction.x;
                    bool hasHorizontalMovement = !horVector.Equal(0f);
                    if (hasHorizontalMovement) {
                        movePointer += (int)Mathf.Sign(horVector);
                        spriteRenderer.sprite = GetSpriteMovePointer(movePointer);
                        bool movePointerReachEdges = (Math.Abs(movePointer) == moveFrameCount);
                        if (movePointerReachEdges) movePointer = Math.Sign(movePointer) * moveFrameCount;

                    }
                    else {
                        bool remainSideAnimation = (movePointer != 0);
                        if (remainSideAnimation) {
                            movePointer -= Math.Sign(movePointer);
                            spriteRenderer.sprite = GetSpriteMovePointer(movePointer);
                        }
                        else {
                            idlePointer++;
                            if (idlePointer >= idleFrameCount) idlePointer = 0;
                            spriteRenderer.sprite = idleSpriteSequence[idlePointer];
                        }
                    }
                }
            }

            prePosition = transform.position;
        }

        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            timer = 0;
            frameSpeed = 8;
            
        }

        // Update is called once per frame
        void Update() {
            //if (!Input.GetMouseButton(0)) test++;
            //isEnchanting = Input.GetMouseButton(2);
            //timer++;
            PlayAnim();
            timer++;
        }
    }
}