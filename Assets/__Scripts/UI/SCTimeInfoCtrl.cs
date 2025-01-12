using System;
using _Scripts.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SCTimeInfoCtrl : MonoBehaviour
{
    public static SCTimeInfoCtrl instance;

    private void Awake() {
        if(instance != null) Destroy(gameObject);
        else instance = this;
    }

    public TMP_Text infoText;
    public TMP_Text breakTime;
    public TMP_Text actualTime;

    public float tarAlpha;
    public float curAlpha;

    public int appearTimer = 0;
    public int appearTime = 120;
    
    public void AppearAll() {
        tarAlpha = 1f;
        appearTimer = 1;
    }

    public void DisappearAll() {
        tarAlpha = 0f;
    }

    private void Update() {
        curAlpha.ApproachRef(tarAlpha, 16f);
        infoText.color = infoText.color.SetAlpha(curAlpha);
        breakTime.color = breakTime.color.SetAlpha(curAlpha);
        actualTime.color = actualTime.color.SetAlpha(curAlpha);
        if (appearTimer > 0) appearTimer++;
        if (appearTimer >= appearTime) {
            appearTimer = 0;
            DisappearAll();
        }
    }

}
