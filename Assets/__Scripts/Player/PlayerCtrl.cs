using System;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player {
    public class PlayerCtrl : MonoBehaviour {
        public static PlayerCtrl Player;
        public PlayerState state;
        private void Awake() {
            if (Player == null) {
                Player = this;
            }
            else {
                Destroy(this.gameObject);
            }
        }
        
        public SpriteRenderer spriteRenderer;
        
        private int _timer;
        private int _invincibleTimer;
        private int _invincibleTimerMax;
        public int GetHitInvTime;

        public int InvincibleTimer {
            get => _invincibleTimer;
            set {
                _invincibleTimer = value;
                _invincibleTimerMax = value;
            }
        }

        #region Movement
        
        private Vector3 _direction;
        private Vector3 GetDirectionVectorNormalized() {
            var direction = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow)) direction.x -= 1;
            if (Input.GetKey(KeyCode.RightArrow)) direction.x += 1;
            if (Input.GetKey(KeyCode.DownArrow)) direction.y -= 1;
            if (Input.GetKey(KeyCode.UpArrow)) direction.y += 1;
            _direction = direction;

            return direction.normalized;
        }
        
        private void Movement() {
            var dirVector = GetDirectionVectorNormalized();
            var slowMultiplier = 
                (Input.GetKey(KeyCode.LeftShift)) ? state.slowRate : 1f;

            var targetPos = slowMultiplier * state.moveSpeed * Time.fixedDeltaTime * dirVector
                            + transform.position;
            
            if (targetPos.x > 3.9f) targetPos.x = 3.9f;
            if (targetPos.x < -3.9f) targetPos.x = -3.9f;
            if (targetPos.y > 4.4f) targetPos.y = 4.4f;
            if (targetPos.y < -4.0f) targetPos.y = -4.0f;
            
            transform.position = targetPos;
        }
        
        #endregion


        #region Animation
        
        public int frameSpeed;
        private int _idlePointer;
        private int _movePointer;
        
        [SerializeField] private Sprite[] animPlayerIdle;
        [SerializeField] private Sprite[] animPlayerLeft;
        [SerializeField] private Sprite[] animPlayerRight;

        private void PlayAnim() {
            if (_timer % frameSpeed == 0) {
                //get the direction
                int hor = (int)_direction.x;
                if (hor == 0) {
                    //Only when move pointer returned to zero can idle animation being played.
                    if (_movePointer == 0) {
                        _idlePointer++;
                        if (_idlePointer == 8) _idlePointer = 0;
                        spriteRenderer.sprite = animPlayerIdle[_idlePointer];
                    }
                    //If move pointer is not zero then make it naturally back to zero.
                    else {
                        _movePointer -= Math.Sign(_movePointer);
                        spriteRenderer.sprite = _movePointer >= 0
                            ? animPlayerRight[_movePointer]
                            : animPlayerLeft[-_movePointer];
                    }
                }
                else {
                    _movePointer += hor;
                    if (_movePointer == 8) _movePointer = 4;
                    if (_movePointer == -8) _movePointer = -4;

                    spriteRenderer.sprite =
                        _movePointer >= 0 ? animPlayerRight[_movePointer] : animPlayerLeft[-_movePointer];
                }
            }
        }
        
        #endregion

        
        public GameObject playerHitEffect;
        public void GetHit() {
            if(GameManager.Manager.isHitEffectOn) Instantiate(playerHitEffect, transform.position, Quaternion.identity);
            GameManager.Manager.hits++;
            if (!GameManager.Manager.isCheatModeOn) {
                //Instantiate(playerHitEffect, transform.position, Quaternion.identity);
                GameManager.Manager.reverseColorCtrl.StartReverseColorEffectAtCenter(transform.position);
                InvincibleTimer = GetHitInvTime;
                Timing.RunCoroutine(GetHitCoroutine());
            }
        }
        
        public IEnumerator<float> GetHitCoroutine() {
            var dist = 5f;
            var maxDist = 3f;
            transform.position = dist * Vector2.down;
            while (true) {
                dist.ApproachRef(maxDist, 16f);
                transform.position = dist * Vector2.down;
                if (dist.Equal(maxDist,0.05f)) break;
                yield return Timing.WaitForOneFrame;
            }
        }

        public bool CheckInvincibility() => _invincibleTimer > 0;
        
        private void SetInvincibleEffect() {
            if (InvincibleTimer > 0) {
                _invincibleTimer--;
            }

            if (InvincibleTimer == 0) { }
        }
        
        void Update() {
            _timer++;
            SetInvincibleEffect();
            Movement();
            PlayAnim();
            if (Input.GetKey(KeyCode.Z));
        }

        private void OnDrawGizmos() {
            // Gizmos.color = Color.green;
            // Gizmos.DrawWireSphere(transform.position, playerState.grazeRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, state.hitRadius);
        }
    }
}