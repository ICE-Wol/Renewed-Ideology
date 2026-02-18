using _Scripts.Tools;
using UnityEngine;

public class SpriteColorApproacher : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public bool activateColorApproach;
    public Color targetColor;
    public Vector4 colorApproachRate = 16 * Vector4.one;
    
    /// <summary>
    /// 开启独立的透明度变化会覆盖颜色值中的透明度变化
    /// </summary>
    public bool activateAlphaApproach;
    public float initAlpha;
    public float targetAlpha;
    public float alphaApproachRate = 16;
    
    public void Update() {
        var curAlpha = spriteRenderer.color.a;
        
        if (activateColorApproach) {
            spriteRenderer.color = spriteRenderer.color.ApproachValue(targetColor, colorApproachRate);
        }
        
        if (activateAlphaApproach) {
            curAlpha.ApproachRef(targetAlpha, alphaApproachRate);
            spriteRenderer.color = spriteRenderer.color.SetAlpha(curAlpha);
        }
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not found");
        }
        
        if (activateAlphaApproach) {
            spriteRenderer.color.SetAlpha(initAlpha);
        }
    }
}
