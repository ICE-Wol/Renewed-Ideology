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
            
            var damageableSet = Damageable.damageableSet;
            foreach (var obj in damageableSet) {
                if (obj == null) continue;
                var rad = obj.hitRadius;
                if (((Vector2)obj.transform.position - (Vector2)transform.position).sqrMagnitude <
                    (rad + hitRadius) * (rad + hitRadius)) {
                    if(!obj.isInvincible) obj.TakeDamage(100);
                    isHit = true;
                    
                    //spriteRenderer.material = spriteAddMaterial;
                    break;
                }
            }
            //
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
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
