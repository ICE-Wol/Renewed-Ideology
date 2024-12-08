using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;

public class MainBullet : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public Material spriteAddMaterial;
    
    public float hitRadius;
    public bool isHit;

    public float speed;

    private float _curScaleX;
    private float _curScaleY;
    private float _curAlpha;

    private void Start() {
        _curAlpha = 0.6f;
        _curScaleX = 1f;
        _curScaleY = 1f;
        isHit = false;
    }

    void Update() {
        if (!isHit) {
            transform.position += speed * Time.fixedDeltaTime * Vector3.up;
            
            // var damageableSet = Damageable.damageableSet;
            // foreach (var obj in damageableSet) {
            //     if (obj == null) continue;
            //     var rad = obj.hitRadius;
            //     if (((Vector2)obj.transform.position - (Vector2)transform.position).sqrMagnitude <
            //         (rad + hitRadius) * (rad + hitRadius)) {
            //         if(!obj.isInvincible) obj.TakeDamage(100);
            //         isHit = true;
            //         break;
            //     }
            // }
            var damageableSet = Damageable.damageableSet;
            Vector2 lineStart = transform.position; // 线段起点
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
            // for (int i = 0; i < damageableSet.Count; i++) {
            //     if (damageableSet[i] == null)
            //         damageableSet.Remove(damageableSet[i]);
            // }

            damageableSet.RemoveWhere(x => x == null);
        }

        if (isHit) {
            transform.position += speed / 20f * Time.fixedDeltaTime * Vector3.up;    
            
            _curScaleY.ApproachRef(3f, 8f);
            _curScaleX.ApproachRef(0f, 32f);
            _curAlpha.ApproachRef(0f, 4f);

            transform.localScale = new Vector3(_curScaleX, _curScaleY, 1f);
            spriteRenderer.color = spriteRenderer.color.SetAlpha(_curAlpha);
        }

        if (_curAlpha.Equal(0f) || transform.position.y >= 6f) {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Vector2 lineStart = transform.position; // 线段起点
        Vector2 lineEnd = transform.position + 0.5f * Vector3.down;
        Gizmos.DrawLine(lineStart, lineEnd);
        Gizmos.DrawLine(lineStart + 0.1f * Vector2.right, lineEnd + 0.1f * Vector2.right);
        Gizmos.DrawLine(lineStart - 0.1f * Vector2.right, lineEnd - 0.1f * Vector2.right);
    }
}
