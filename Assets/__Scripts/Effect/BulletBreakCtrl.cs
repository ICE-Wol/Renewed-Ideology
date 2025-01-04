using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletBreakCtrl : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public float initScale;
    public float initAlpha;
    public float shrinkSpeed;
    private static readonly int Color = Shader.PropertyToID("_Color");

    //
    // private void OnEnable() {
    //     transform.SetAsFirstSibling();
    // }

    public void SetColor(Color c) {
        spriteRenderer.material.SetColor(Color,c);
    }
    
    public void SetScale(BulletSize size) {
        initScale = ((int)size + 1) * 0.5f;
    }

    void Start() {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        spriteRenderer.color = spriteRenderer.color.SetAlpha(initAlpha);
        transform.localScale = initScale * Vector3.one;
    }
    
    void Update() {
        spriteRenderer.color = spriteRenderer.color.SetAlpha(spriteRenderer.color.a - shrinkSpeed/2f);
        
        var scale = transform.localScale.x;
        scale -= shrinkSpeed;
        transform.localScale = scale * Vector3.one;
        if(scale <= 0.01)
            Destroy(gameObject);
    }
}
