using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageTitleCtrl2 : MonoBehaviour {
    public RectTransform patternRect;
    public Image patternPic;
    public TMP_Text number;
    public TMP_Text location;
    public TMP_Text stageName;
    public TMP_Text stageEnglishName;

    public int timer;
    public float patternRectY;
    public float patternRectX;
    public float patternRotZ;

    public float engNameTarPosX = 130;
    public float engNameCurPosX = 50;
    
    public void Init() {
        patternRect.anchoredPosition = new Vector2(0, 75);
        patternPic.color = patternPic.color.SetAlpha(0f);
        number.color = number.color.SetAlpha(0f);
        location.color = location.color.SetAlpha(0f);
        stageName.color = stageName.color.SetAlpha(0f);
        stageEnglishName.color = stageEnglishName.color.SetAlpha(0f);

        timer = 0;
        patternRectY = 75f;
        patternRotZ = -180f;
    }

    private void Start() {
        Init();
    }

    public void Update() {
        patternRotZ.ApproachRef(15f, 32f);
        patternRect.localEulerAngles = new Vector3(patternRect.rotation.x,
            patternRect.rotation.y,
            patternRotZ);
        
        if (!patternRectY.Equal(0f, 10f)) {
            if (timer >= 60) {
                patternPic.color = patternPic.color.Appear(32f);
                patternRectY.ApproachRef(0f, 128f);
                patternRect.anchoredPosition = new Vector2(patternRectX, patternRectY);
            }

            if (timer >= 120) {
                number.color = number.color.Appear(32f);
                location.color = location.color.Appear(32f);
            }

            if (timer >= 180) {
                stageName.color = stageName.color.Appear(32f);
            }

            if (timer >= 220) {
                stageEnglishName.color = stageEnglishName.color.Appear(32f);
                engNameCurPosX.ApproachRef(engNameTarPosX, 32f);
                stageEnglishName.rectTransform.anchoredPosition = new Vector2(engNameCurPosX, stageEnglishName.rectTransform.anchoredPosition.y);
            }
        }
        else {
            patternPic.color = patternPic.color.Fade(16f);
            number.color = number.color.Fade(16f);
            location.color = location.color.Fade(16f);
            stageName.color = stageName.color.Fade(16f);
            stageEnglishName.color = stageEnglishName.color.Fade(16f);

            if (timer >= 1000) {
                gameObject.SetActive(false);
            }
        }

        timer++;
    }
}