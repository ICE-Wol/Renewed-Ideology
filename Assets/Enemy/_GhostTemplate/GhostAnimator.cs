using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GhostParticleCtrl particlePrefab;

    public Color color;
    public float scale;
    public float offset;
    public float speed;

    public int particleGenerateInterval;
    public int timer;
    
    public void SetColor(Color color) {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        spriteRenderer.material.SetFloat(Shader.PropertyToID("_Hue"), H);
        spriteRenderer.material.SetFloat(Shader.PropertyToID("_Saturation"), S);
    }
    private void Start() {
        SetColor(color);
    }
    private void Update() {
        var x = Mathf.Sin(Time.time * speed / 10f) * offset * 5f;
        var y = Mathf.Sin(Time.time * speed / 10f) * offset * 5f;
        transform.position = new Vector3(x, y, 0);
        
        
        if(timer % particleGenerateInterval == 0) {
            var particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particle.SetColor(color);
        }
        
        scale = Mathf.Sin(Time.time * speed) * offset + 1.0f;
        transform.localScale = new Vector3(scale, scale, scale);

        timer++;
    }
}
