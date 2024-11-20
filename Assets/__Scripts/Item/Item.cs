using _Scripts.Tools;
using UnityEngine;

namespace _Scripts.Item {
    public enum ItemType{
        Power,
        Point,
        Gold,
        Full,
        Faith,
        BombFrag,
        LifeFrag,
        Bomb,
        Life,
        Card,
    }
    public class Item : MonoBehaviour {
        //public static int cnt = 0;
        
        public SpriteRenderer spriteRenderer;
        public Sprite[] itemSprites;
        public ItemType type;
        public Vector3 direction = Vector3.down;
        public float rotation;
        public float speed;
        public float scale;

        public bool isCollected;
        public float collectRadius;
        
        private int _timer;

        public void SetType(ItemType t) {
            type = t;
            spriteRenderer.sprite = itemSprites[(int)type];
        }
        private void Start() {
            collectRadius = 0.1f;
            isCollected = false;
            _timer = 0;
            speed = -1.5f;
            rotation = 1080f;
            if ((int)type > 4) {
                rotation = 360f;
            }
            if (Random.Range(0, 1f) < 0.5f) {
                rotation *= -1;
            }
            scale = 0;
        }

        void Update() {
            speed.ApproachRef(1.5f, 32f);
            rotation.ApproachRef(0f, 32f);
            scale.ApproachRef(1f, 32f);
            
            transform.localScale = scale * Vector3.one;
            transform.rotation = Quaternion.Euler(0,0,rotation);
            
            if(!isCollected)
                transform.position += Time.fixedDeltaTime * speed * direction;
            
            if (transform.position.y <= -4.8f) {
                Destroy(gameObject);
            }

            if (!isCollected) {
                isCollected = CheckCollect();
                
            }
            else {
                CollectBehaviour();
            }

            _timer++;
        }

        public bool CheckCollect() {
            var distance 
                = Vector2.Distance(transform.position, 
                    Player.PlayerCtrl.Player.transform.position);
            var targetRadius = Player.PlayerCtrl.Player.state.itemRadius;
            return (distance <= collectRadius + targetRadius) || 
                   (Player.PlayerCtrl.Player.transform.position.y >= 2.5f);
        }

        public virtual void TriggerEffect() {
            var state = Player.PlayerCtrl.Player.state;
            switch (type) {
                case ItemType.Power:
                    state.Power += 1;

                    break;
                case ItemType.Point:
                    state.point++;
                    var pointPercentage = (transform.position.y + 2.5f) / 5f;
                    if (pointPercentage >= 1) pointPercentage = 1;
                    if (pointPercentage <= 0) pointPercentage = 0;
                    state.score += (int)((0.5f * pointPercentage + 1f) * state.maxPoint);
                    //GameManager.Manager.CurScoreText.text
                    //    = GameManager.NumToCommaStr(state.score);
                    break;
                case ItemType.Gold:
                    state.gold++;
                    break;
                case ItemType.Full:
                    state.Power = state.maxPower;
                    break;
                case ItemType.BombFrag:
                    state.BombFrag += 1;
                    PlayerStatusManager.Manager.RefreshSlot();
                    break;
                case ItemType.LifeFrag:
                    state.LifeFrag += 1;
                    PlayerStatusManager.Manager.RefreshSlot();
                    break;
                case ItemType.Bomb:
                    state.bomb += 1;
                    PlayerStatusManager.Manager.RefreshSlot();
                    break;
                case ItemType.Life:
                    state.life += 1;
                    PlayerStatusManager.Manager.RefreshSlot();
                    break;
            }
            //DestroyImmediate(gameObject);
        }
        
        /// <summary>
        /// when collected, move to player and trigger effect
        /// </summary>
        public void CollectBehaviour() {
            var targetPosition = Player.PlayerCtrl.Player.transform.position;
            transform.position = transform.position.ApproachValue(targetPosition, 
                8f * Vector3.one, 0.1f);
            if (transform.position.Equal(targetPosition, 0.1f)) {
                TriggerEffect();
                Destroy(gameObject);
            }
        }
    }
}