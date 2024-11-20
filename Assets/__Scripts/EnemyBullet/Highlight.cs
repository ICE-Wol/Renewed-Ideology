using System;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Config tarBulletConfig;
    public SpriteRenderer highlightSprite;
    public SpriteRenderer bottomSprite;
    
    public float tarAlpha;
    public float curAlpha;

    public void Appear() {
        tarAlpha = 1f;  
    }

    public void Fade() {
        tarAlpha = 0f;
    }
    
    private void Start() {
        highlightSprite.sprite = tarBulletConfig.basic.GetBulletSprite(tarBulletConfig.type);
    }

    private void Update() {
        curAlpha.ApproachRef(tarAlpha, 32f);
        var color = tarBulletConfig.color;
        Color.RGBToHSV(color, out var H, out var S, out var V);
        highlightSprite.material.SetFloat("_Hue", H);
        highlightSprite.material.SetFloat("_Saturation", S);
        
        highlightSprite.color = highlightSprite.color.SetAlpha(curAlpha);
        bottomSprite.color = bottomSprite.color.SetAlpha(1f - curAlpha);
    }
}
