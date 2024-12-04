
using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.EnemyBullet {
    public enum EBulletStates {
        Template,
        Spawning,
        Activated,
        Destroying
    }

    public class State : MonoBehaviour
    { 
        [Header("已在代码内获取索引")]
        public Highlight highlight;
        public bool isSelfMovement;
        public BulletMovement movement;
        
        [Header("自行拖动获取索引")]
        public SpriteRenderer spriteRenderer;
        public Detect bulletDetector;
        public Config bulletConfig;
        
        [SerializeField] private EBulletStates state;
        
        public static HashSet<State> bulletSet = new(); 
        public event Action OnBulletDestroy;
        //储存方法列表，在销毁时调用回调（Action是委托类型）中的一系列方法
        
        

        public bool hasNoCollisionCheck;
        public float hitRadius;
        public bool isGrazed;
        public float grazeRadius;
        
        [Header("当速度为0时，是否反转默认方向")]
        public bool revZeroDir;

        /// <summary>
        /// 弹雾角度随机，防止初始角度丢失，因此提前存好，在生成时赋值，激活时复位
        /// </summary>
        [Header("用于弹雾生成角度随机时存储初始角度")]
        public float initialRotation;
        private int _timer;
        
        private static readonly int PropHueID = Shader.PropertyToID("_Hue");
        private static readonly int PropSatID = Shader.PropertyToID("_Saturation");
        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            bulletDetector = GetComponent<Detect>();
            if (isSelfMovement) movement = GetComponent<BulletMovement>();
            
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
            SetBasicParam();
            SetState(EBulletStates.Spawning);
            bulletSet.Add(this);
           
        }

        private void SetBasicParam() {
            spriteRenderer.sprite
                = bulletConfig.basic.GetBulletSprite(bulletConfig.type);
            highlight = GetComponent<Highlight>();
            spriteRenderer.material
                = bulletConfig.basic.GetBulletMaterial(bulletConfig.isGlowing ||
                                                       (highlight != null && highlight.enabled));
            if (highlight != null && highlight.enabled) {
                if (highlight.isColorSameAsParentBullet) SetColor(bulletConfig.color);
                else SetColorAsHighLight();
            }
            else SetColor(bulletConfig.color);
        }

        /// <summary>
        /// 这个设置函数不会覆盖配置中的颜色为高光色
        /// </summary>
        private void SetColorAsHighLight() {
            float H = 0, S = 0, V = 0;
            Color.RGBToHSV(highlight.HighlightColor, out H, out S, out V);
            spriteRenderer.material.SetFloat(PropHueID, H);
            spriteRenderer.material.SetFloat(PropSatID, S);
        }

        /// <summary>
        /// 这个设置函数会覆盖配置中的颜色为设定色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color) {
            float H = 0, S = 0, V = 0;
            Color.RGBToHSV(color, out H, out S, out V);
            spriteRenderer.material.SetFloat(PropHueID, H);
            spriteRenderer.material.SetFloat(PropSatID, S);
            bulletConfig.color = color;
        }
        
        private float _fogScale;
        private float _fogAlpha;
        
        public EBulletStates GetState() => state;
        public void SetState(EBulletStates state) {
            //防止反复重置fog scale数值.
            if (this.state == state) return;
            this.state = state;
            
            switch (state) {
                case EBulletStates.Spawning: 
                    //Debug.Log("Sprite reset" + bulletConfig.basic.enemyBulletGenerateSprite);
                    //Debug.Log("check size" + bulletConfig.basic.bulletBasics[(int)bulletConfig.type].size);
                    if (bulletConfig.basic.bulletBasics[(int)bulletConfig.type].size == BulletSize.Small || bulletConfig.basic.bulletBasics[(int)bulletConfig.type].size == BulletSize.Medium) {
                        spriteRenderer.sprite = bulletConfig.basic.enemyBulletGenerateSprite;
                        //Debug.Log("in 1");
                    }
                    else {
                        spriteRenderer.sprite = bulletConfig.basic.GetBulletSprite(bulletConfig.type);
                        //Debug.Log("in 2");
                    }
                    spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
                    
                    //弹雾方向随机
                    //todo:没做完，只在激光生效
                    transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                    switch (bulletConfig.size) {
                        default:
                            _fogScale = 6f;
                            break;
                        case BulletSize.Small:
                            _fogScale = 6f;
                            break;
                        case BulletSize.Medium:
                            _fogScale = 8f;
                            break;
                        case BulletSize.Large: 
                            _fogScale = 10f;
                            break;
                    }
                    _fogAlpha = 0f;
                    break;
                case EBulletStates.Activated:
                    transform.rotation = Quaternion.Euler(0, 0, initialRotation);
                    
                    spriteRenderer.sprite = bulletConfig.basic.GetBulletSprite(bulletConfig.type);
                    if(bulletConfig.isLaserNode) spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
                    else spriteRenderer.color = spriteRenderer.color.SetAlpha(1f);
                    
                    //若被高光脚本替换为高光材质，换回材质后需重设颜色
                    spriteRenderer.material
                        = bulletConfig.basic.GetBulletMaterial(bulletConfig.isGlowing);
                    SetColor(bulletConfig.color);
                    
                    if (highlight != null) {
                        highlight.curAlpha = 1f;
                        highlight.tarAlpha = 1f;
                    }
                    break;
                case EBulletStates.Destroying:
                    _fogScale = 1f;
                    _fogAlpha = 1f;
                    OnBulletDestroy?.Invoke();
                    GameManager.Manager.CreateBulletDestroyParticle(transform.position, bulletConfig.color,
                        bulletConfig.size, spriteRenderer.sortingOrder);
                    Destroy(gameObject);
                    break;
            }
        }

        private float _curRotation;
        private void Update() {
            //当子弹超出边界且不允许超出边界时，销毁
            if (!bulletConfig.isOutOfBoundFree && bulletDetector.IsOutOfBound(transform.position) && state != EBulletStates.Template) {
                bulletSet.Remove(this);
                DestroyImmediate(gameObject);
                return;
            }
            

            if (bulletConfig.selfRotSpdInDegree != 0)
                spriteRenderer.transform.localRotation =
                    Quaternion.Euler(0, 0, bulletConfig.selfRotSpdInDegree * (_timer++));
            switch (state) {
                case EBulletStates.Spawning:
                    float fogScaleTarget;
                    switch (bulletConfig.size) {
                        case BulletSize.Small:
                            fogScaleTarget = 1f;
                            break;
                        case BulletSize.Medium:
                            fogScaleTarget = 2f;
                            break;
                        case BulletSize.Large:
                            fogScaleTarget = 3f;
                            break;
                        default:
                            fogScaleTarget = 1f;
                            break;
                    }
                    _fogScale.ApproachRef(fogScaleTarget, 4f);
                    _fogAlpha.ApproachRef(1f, 16f);

                    transform.localScale = _fogScale * Vector3.one;
                    spriteRenderer.color = spriteRenderer.color.SetAlpha(_fogAlpha);

                    if (_fogScale.Equal(fogScaleTarget, 0.1f)) {
                        transform.localScale = Vector3.one;
                        SetState(EBulletStates.Activated);
                    }

                    if(bulletConfig.moveWhenSpawning)
                        movement.Movement(transform);
                    break;
                case EBulletStates.Activated:
                    if (isSelfMovement) {
                        movement.Movement(transform);
                    }

                    if (bulletConfig.faceMoveDirection) {
                        _curRotation.ApproachRef(movement.direction, 32f);

                        spriteRenderer.transform.rotation = movement.speed < 0
                            ? Quaternion.Euler(0, 0, movement.direction + 180)
                            : Quaternion.Euler(0, 0, movement.direction);


                        if (movement.speed == 0) {
                            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0,
                                revZeroDir ? movement.direction + 180 : movement.direction);
                        }
                    }

                    if (!isGrazed) {
                        isGrazed = bulletDetector.CheckPlayerGraze(grazeRadius);
                        if (isGrazed) {
                            var g = PlayerCtrl.Player.state.graze++;
                        }
                    }
                    
                    //碰撞半径与localScale正相关
                    hitRadius = bulletConfig.collideRadius * transform.localScale.x;
                    if (!hasNoCollisionCheck && bulletDetector.CheckPlayerCollision(hitRadius)) {
                        if(!bulletConfig.indestructible)
                            SetState(EBulletStates.Destroying);
                        if (!PlayerCtrl.Player.CheckInvincibility()) {
                            PlayerCtrl.Player.GetHit();
                        }
                    }
                    
                    break;
                case EBulletStates.Destroying :

                    break;
                
            }
        }

        private void OnDestroy() {
            bulletSet.Remove(this);
        }

        private void OnDrawGizmos() {
            Gizmos.color = !hasNoCollisionCheck ? Color.green : Color.grey;
            if (state == EBulletStates.Activated)
                Gizmos.DrawSphere(transform.position, hitRadius);
        }
    }
}
