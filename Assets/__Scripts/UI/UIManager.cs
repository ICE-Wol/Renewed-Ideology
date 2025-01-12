using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.Player;
using _Scripts.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }
    
    [Header("ReferenceRegion")]
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

    [Header("BonusPointRegion")] 
    public TMP_Text bonusPointText;
    public int curBonusPoint;

    private void UpdateTimeText() {
        var damageable = BossManager.instance.curBoss.damageable;
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
                      "<size=40>" + (power % 100 < 10 ? "0" : "") + (int)(power % 100) + "</size>" +
                      "/4.<size=40>00</size></color>";
        powerText.text = powerString;
    }
    
    private void UpdateGrazeText() {
        curGraze = PlayerCtrl.instance.state.graze;
        grazeText.text = "<color=#CFCFCF>" + (int)curGraze + "</color>";
    }

    private void UpdateBonusPointText() {
        curBonusPoint = BossManager.instance.curBoss.bonusPoints;
        bonusPointText.text = BossManager.instance.curBoss.hasBonus
            ? curBonusPoint.ToString("#,0", CultureInfo.InvariantCulture)
            : "Failed";
    }

    private void Update() {
        var damageable = BossManager.instance.curBoss.damageable;
        if(damageable != null) UpdateTimeText();
        UpdatePowerText();
        UpdateGrazeText();
        if(damageable != null) UpdateBonusPointText();
    }
}
