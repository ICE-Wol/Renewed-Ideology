using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorApproacher : MonoBehaviour
{
    public Image image;
    
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
        var curAlpha = image.color.a;
        
        if (activateColorApproach) {
            image.color = image.color.ApproachValue(targetColor, colorApproachRate);
        }
        
        if (activateAlphaApproach) {
            curAlpha.ApproachRef(targetAlpha, alphaApproachRate);
            image.color = image.color.SetAlpha(curAlpha);
        }
    }
    void Start()
    {
        image = GetComponent<Image>();
        if(image == null) {
            Debug.LogError("image not found");
        }
        
        if (activateAlphaApproach) {
            image.color.SetAlpha(initAlpha);
        }
    }
}
