using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.Player;
using _Scripts.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [Header("ReferenceRegion")]
    public Damageable damageable;
    public PlayerState player;
    
    [Header("TimeRegion")]
    public TMP_Text timeTextWhite;
    public TMP_Text timeTextRed;
    public float curTime;
    public string timeString;

    [Header("PowerRegion")]
    public TMP_Text powerText;
    public float power;
    public string powerString;
    
    [Header("GrazeRegion")] 
    public TMP_Text grazeText;
    public float curGraze;

    private void UpdateTimeText() {
        curTime = damageable.curTime;
        var dec = (int)(curTime * 100) % 100;

        timeString = "<size=96>" + (int)curTime + "</size>" +
                     "<size=72>." + (dec < 10 ? ("0" + dec) : dec) + "</size>";

        timeTextWhite.text = timeString;
        if (curTime < 5 && damageable != null) {
            timeTextRed.text = timeString;
            timeTextRed.color = Color.red.SetAlpha(Mathf.Abs(Mathf.Sin(curTime * 5)));
        } else timeTextRed.color = Color.red.SetAlpha(0);
        
    }
    
    private void UpdatePowerText() {
        power = player.Power;
        powerString = "<color=#EECBAD>" + (int)(power / 100) + "." +
                      "<size=25>" + (power % 100 < 10 ? "0" : "") + (int)(power % 100) + "</size>" +
                      "/4.<size=25>00</size></color>";
        powerText.text = powerString;
    }
    
    private void UpdateGrazeText() {
        curGraze = PlayerCtrl.Player.state.graze;
        grazeText.text = "<color=#CFCFCF>" + (int)curGraze + "</color>";
    }

    private void Update() {
        UpdateTimeText();
        UpdatePowerText();
        UpdateGrazeText();
    }
}
