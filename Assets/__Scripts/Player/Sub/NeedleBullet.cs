using _Scripts;
using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;

public class NeedleBullet : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public Sprite spriteFlame;
    public Material spriteAddMaterial;
    
    public float hitRadius;
    public bool isHit;

    public float speed;
    public float direction;
    public float rotation;
    public Color color;

    private float _curScaleX;
    private float _curScaleY;
    private float _curAlpha;
    private float _curRot;

    public void SetColor(Color color, float alpha) {
        Color.RGBToHSV(color, out var h, out var s, out var v);
        spriteRenderer.material.SetFloat("_Hue",h);
        spriteRenderer.material.SetFloat("_Saturation",s);
        spriteRenderer.material.SetFloat("_Alpha",alpha);
    }
    
    public void SetAlpha(float alpha) {
        spriteRenderer.material.SetFloat("_Alpha",alpha);
    }
    
    private void Start() {
        _curAlpha = 0.6f;
        _curScaleX = 1f;
        _curScaleY = 1f;
        isHit = false;
        
        rotation = Random.Range(-15f, 15f);
        
        //SetColor(color,0.5f);
        
    }

    void Update() {
        if (!isHit) {
            transform.position += speed * Time.fixedDeltaTime * direction.Deg2Dir3();//Vector3.up;
            
            var damageableSet = Damageable.damageableSet;
            foreach (var obj in damageableSet) {
                if(obj == null) continue;
                var rad = obj.hitRadius;
                if (((Vector2)obj.transform.position - (Vector2)transform.position).sqrMagnitude <
                    (rad + hitRadius) * (rad + hitRadius)) {
                    if(!obj.isInvincible)obj.TakeDamage(25);
                    isHit = true;
                    spriteRenderer.color = spriteRenderer.color.SetAlpha(1);
                    //spriteRenderer.material = spriteAddMaterial;
                    break;
                }
            }

            damageableSet.RemoveWhere(x => x == null);
        }

        if (isHit) {
            transform.position += speed / 5f * Time.fixedDeltaTime * Vector3.up; 
            spriteRenderer.sprite = spriteFlame;
            spriteRenderer.material = spriteAddMaterial;
            SetColor(color, 0.5f);
            
            _curScaleY.ApproachRef(3f, 8f);
            _curScaleX.ApproachRef(0f, 32f);
            _curAlpha.ApproachRef(0f, 4f);
            _curRot.ApproachRef(rotation, 8f);

            transform.localScale = new Vector3(_curScaleX, _curScaleY, 1f);
            transform.rotation = Quaternion.Euler(0, 0, _curRot);
            SetAlpha(_curAlpha);
        }

        if (_curAlpha.Equal(0f) || transform.position.y >= 6f) {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
