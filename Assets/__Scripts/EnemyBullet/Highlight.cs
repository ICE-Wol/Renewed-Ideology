using System;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    private static readonly int Color = Shader.PropertyToID("_Color");
    public Config tarBulletConfig;
    public State tarBulletState;
    public SpriteRenderer highlightSprite;
    public SpriteRenderer bottomSprite;

    [Header("高亮颜色，默认与子弹主体颜色相同")]
    public bool isColorSameAsParentBullet = true;

    [SerializeField]
    private Color _color;
    public Color HighlightColor
    {
        get => _color;
        set
        {
            _color = value;
            highlightSprite.material.SetColor(Color, _color);
        }
    }
    
    public float tarAlpha = 1f;
    public float curAlpha = 0f;

    public void Appear() {
        tarAlpha = 1f;  
    }

    public void Fade() {
        tarAlpha = 0f;
    }
    
    private void Start() {
        tarBulletState = GetComponent<State>();
        tarBulletConfig = tarBulletState.bulletConfig;
        highlightSprite.sprite = tarBulletConfig.basic.GetBulletSprite(tarBulletConfig.type);
        highlightSprite.color = highlightSprite.color.SetAlpha(curAlpha);
        if (isColorSameAsParentBullet)
            HighlightColor = tarBulletConfig.color;
        else {
            HighlightColor = _color;
        }
    }

    private void Update() {
        curAlpha.ApproachRef(tarAlpha, 32f);
        
        highlightSprite.color = highlightSprite.color.SetAlpha(curAlpha);
        if(tarBulletState.GetState() != EBulletStates.Spawning)
            bottomSprite.color = bottomSprite.color.SetAlpha(1f - curAlpha);
    }
}
