using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class PlayerSub : MonoBehaviour {
        public NeedleBullet bullet;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer shade;
        public Color color;
        
        public float tarScale;
        public float curScale;
        
        public bool isDestroyed;

        public int fireInterval;
        
        private int _timer;
        void Start() {
            _timer = 0;
            
            spriteRenderer.material.SetColor("_Color",color);
            shade.material.SetColor("_Color",color);
        }

        public void Fire() {
            if (Input.GetKey(KeyCode.LeftShift)) {
                var pos = transform.position + Vector3.back * 4f;
                var b1 = Instantiate(bullet, pos + Vector3.up + 0.08f * Vector3.left, Quaternion.Euler(0, 0, 0));
                var b2 = Instantiate(bullet, pos + Vector3.up - 0.08f * Vector3.left, Quaternion.Euler(0, 0, 0));
                b1.direction = 90f;
                b2.direction = 90f;
                b1.color = Color.cyan;
                b2.color = Color.cyan;
                b1.SetColor(Color.cyan, 0.3f);
                b2.SetColor(Color.cyan, 0.3f);
            }
            else {
                var offset = 3f;
                var pos = transform.position + Vector3.back * 4f;
                var b1 = Instantiate(bullet, pos + Vector3.up + 0.08f * Vector3.left, Quaternion.Euler(0, 0, offset));
                var b2 = Instantiate(bullet, pos + Vector3.up - 0.08f * Vector3.left, Quaternion.Euler(0, 0, -offset));
                b1.direction = 90f + offset;
                b2.direction = 90f - offset;
                b1.color = Color.blue;
                b2.color = Color.blue;
                b1.SetColor(Color.blue, 0.3f);
                b2.SetColor(Color.blue, 0.3f);
            }
        }

        public void DestroyPlayerSub() {
            isDestroyed = true;
            tarScale = 0f;
        }
        //when generated, increase scale
        
        void Update() {

            if (_timer % fireInterval == 0 && Input.GetKey(KeyCode.Z)) {
                Fire();
            }
            
            transform.rotation = Quaternion.Euler(0f,0f,_timer * 3f);
            shade.transform.rotation = Quaternion.Euler(0f,0f,-_timer * 3f);
            
            curScale.ApproachRef(tarScale, 16f);
            transform.localScale = curScale * Vector3.one;
            shade.transform.localScale = curScale * Vector3.one;

            if (PlayerCtrl.Player.state.IsPowerFull) {
                var alpha = shade.color.a;
                alpha.ApproachRef(0.2f, 8f);
                shade.color = shade.color.SetAlpha(alpha);
            }
            else {
                var alpha = shade.color.a;
                alpha.ApproachRef(0f, 8f);
                shade.color = shade.color.SetAlpha(alpha);
            }
            
            if(isDestroyed && curScale.Equal(0f))
                Destroy(gameObject);
            _timer++;
        }
    }
}
