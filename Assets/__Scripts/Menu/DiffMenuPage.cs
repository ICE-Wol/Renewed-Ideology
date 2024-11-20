using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DiffMenuPage : MonoBehaviour {
    public Difficulty diff;
    
    public float curAlpha;
    public float tarAlpha;
    public float curHeight;
    public float tarHeight;
    public float curScale;
    public float tarScale;

    public TMP_Text[] diffTexts;
    public Transform parent;

    /// <summary>
    /// given value by DiffSelectManager
    /// </summary>
    public void SetClonedValue() {
        tarHeight = -4f;
        tarScale = 0.4f;
        tarAlpha = 1f;
    }
    public void SetAppearValue() {
        tarHeight = 0f;
        tarScale = 1f;
        tarAlpha = 1f;
    }
    public void ResetValue() {
        tarHeight = -3f;
        tarScale = 0f;
        tarAlpha = 0f;
    }
    
    private void Update() {
        curAlpha.ApproachRef(tarAlpha, 8f);
        for (int i = 0; i < diffTexts.Length; i++) {
            diffTexts[i].color = diffTexts[i].color.SetAlpha(curAlpha);
            if (i == 1)
                diffTexts[i].color = diffTexts[i].color.SetAlpha(curAlpha * 0.1f);
        }
        
        curHeight.ApproachRef(tarHeight, 8f);
        parent.localPosition = new Vector3(0, curHeight, 0);
        
        
        
        
        
        curScale.ApproachRef(tarScale, 8f);
        transform.localScale = Vector3.one * curScale;
    }
}
