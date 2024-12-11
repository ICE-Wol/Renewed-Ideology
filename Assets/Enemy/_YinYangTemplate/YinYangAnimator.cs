using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;


public interface ISetAlpha
{
    void SetAlpha(float a);
}
public class YinYangAnimator : MonoBehaviour,ISetAlpha
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer subCircleRendererPrefab;
    public Material bulletGlowMaterial;

    public SpriteRenderer[,] subCircleRenderers;

    public Color color;
    public float alpha;

    public float rotSpd;
    
    public float innerRad;
    public float outerRad;
    public float innerRotSpd;
    public float outerRotSpd;

    
    public void SetColor(Color color) {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        spriteRenderer.material.SetFloat("_Hue", H);
        spriteRenderer.material.SetFloat("_Saturation", S);
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 10; j++) {
                if (j % 2 == 0) {
                    subCircleRenderers[i, j].color = color.SetAlpha(0.3f);
                }
                else {
                    subCircleRenderers[i, j].material.SetFloat("_Hue", H);
                    subCircleRenderers[i, j].material.SetFloat("_Saturation", S);
                }
            }
        }
    }
    
    public void SetAlpha(float a) {
        alpha = a;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 10; j++) {
                if (j % 2 == 0) {
                    subCircleRenderers[i, j].color = subCircleRenderers[i, j].color.SetAlpha(0.3f * alpha);
                }
                else {
                    subCircleRenderers[i, j].material.SetFloat("_Alpha", alpha);
                }
            }
        }
    }
    private void Start() {
        subCircleRenderers = new SpriteRenderer[2, 10];
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 10; j++) {
                var subCircle = Instantiate(subCircleRendererPrefab, transform.position, Quaternion.identity);
                subCircleRenderers[i, j] = subCircle;
                subCircle.transform.SetParent(transform);
                subCircle.transform.localScale = Vector3.one * (i == 0 ? 0.8f : 1.1f);
                if(i == 1) {
                    subCircle.material = bulletGlowMaterial;
                }
            }
        }
        SetColor(color);
    }
    
    private void Update() {
        spriteRenderer.transform.Rotate(0, 0, rotSpd * Time.deltaTime);
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 10; j++) {
                var angle = 360f / 10 * j + (i == 0 ? innerRotSpd : outerRotSpd) * Time.time;
                var rad = (i == 0 ? innerRad : outerRad) * (j % 2 == 0 ? 0.8f : 1f);
                var pos = rad * angle.Deg2Dir3();
                subCircleRenderers[i, j].transform.localPosition = pos;
            }
        }
    }
}
