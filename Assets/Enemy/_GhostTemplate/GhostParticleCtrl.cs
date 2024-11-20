using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostParticleCtrl : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float scale;
    public float alpha;

    public void SetColor(Color color) {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        spriteRenderer.material.SetFloat(Shader.PropertyToID("_Hue"), H);
        spriteRenderer.material.SetFloat(Shader.PropertyToID("_Saturation"), S);
    }
    
    private void Start() {
        alpha = 1.0f;
        scale = Random.Range(0.5f, 1.0f);
        transform.position += new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0);
    }

    public void Update() {
        scale -= 0.01f;
        scale.ApproachRef(0f, 128f);
        alpha.ApproachRef(0f, 64f);
        
        if (scale.Equal(0f, 0.01f)) {
            Destroy(gameObject);
        }

        spriteRenderer.material.SetFloat(Shader.PropertyToID("_Alpha"), alpha);
        transform.localScale = new Vector3(scale, scale, scale);
        transform.position += new Vector3(0, 0.01f, 0);
    }
}
