using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class StageTitleCtrl : MonoBehaviour {
    public RectTransform patternRect;
    public Image patternPic;
    public Image number;
    public Image location;
    public Image stageName;

    public int timer;
    public float patternRectY;
    
    public void Init() {
        patternRect.anchoredPosition = new Vector2(0, 75);
        patternPic.color = patternPic.color.SetAlpha(0f);
        number.color = number.color.SetAlpha(0f);
        location.color = location.color.SetAlpha(0f);
        stageName.color = stageName.color.SetAlpha(0f);

        timer = 0;
        patternRectY = 75f;
    }

    private void Start() {
        Init();
    }

    public void Update() {
        if (!patternRectY.Equal(0f, 1f)) {
            if (timer >= 60) {
                patternPic.color = patternPic.color.Appear(32f);
                patternRectY.ApproachRef(0f, 64f);
                patternRect.anchoredPosition = new Vector2(0, patternRectY);
            }

            if (timer >= 120) {
                number.color = number.color.Appear(32f);
                location.color = location.color.Appear(32f);
            }

            if (timer >= 180) {
                stageName.color = stageName.color.Appear(32f);
            }
        }
        else {
            patternPic.color = patternPic.color.Fade(16f);
            number.color = number.color.Fade(16f);
            location.color = location.color.Fade(16f);
            stageName.color = stageName.color.Fade(16f);

            if (timer >= 600) {
                this.gameObject.SetActive(false);
            }
        }

        timer++;
    }
}
