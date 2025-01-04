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
        // Color.RGBToHSV(color, out var h, out var s, out var v);
        // spriteRenderer.material.SetFloat("_Hue",h);
        // spriteRenderer.material.SetFloat("_Saturation",s);
        spriteRenderer.material.SetColor("_Color",color);
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
        
        SetColor(color,0.5f);
        
    }

    void Update() {
        if (!isHit) {
            transform.position += speed * Time.fixedDeltaTime * direction.Deg2Dir3();//Vector3.up;
            
            var damageableSet = Damageable.damageableSet;
            Vector2 lineStart = transform.position + 0.5f * Vector3.up; // 线段起点
            Vector2 lineEnd = transform.position + 0.5f * Vector3.down;    // 线段终点

            foreach (var obj in damageableSet) {
                if (obj == null) continue;

                Vector2 circleCenter = obj.transform.position; // 圆心
                float circleRadius = obj.hitRadius;           // 圆半径

                // 计算线段与圆的相交
                Vector2 d = lineEnd - lineStart;
                Vector2 f = lineStart - circleCenter;

                float a = Vector2.Dot(d, d);
                float b = 2 * Vector2.Dot(f, d);
                float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

                float discriminant = b * b - 4 * a * c;

                if (discriminant >= 0) { // 有交点
                    float t1 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
                    float t2 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);

                    if ((t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1)) { // 交点在线段范围内
                        if (!obj.isInvincible) obj.TakeDamage(100);
                        isHit = true;
                        break;
                    }
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
        Vector2 lineStart = transform.position + 0.5f * Vector3.up; // 线段起点
        Vector2 lineEnd = transform.position + 0.5f * Vector3.down;
        Gizmos.DrawLine(lineStart, lineEnd);
        Gizmos.DrawLine(lineStart + 0.1f * Vector2.right, lineEnd + 0.1f * Vector2.right);
        Gizmos.DrawLine(lineStart - 0.1f * Vector2.right, lineEnd - 0.1f * Vector2.right);
    }
}
