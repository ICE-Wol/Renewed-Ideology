using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDownCtrl : MonoBehaviour {
    public Damageable targetBoss;
    public Image image;

    public bool isCircular;
    public bool isTime;
    public RectTransform rt;

    private void Start() {
        image = GetComponent<Image>();
        if (isCircular) rt = GetComponent<RectTransform>();
    }

    private void Update() {
        if (targetBoss == null) {
            foreach (var d in Damageable.damageableSet) {
                if (d.isBoss) {
                    targetBoss = d;
                    if (isTime) {
                        image.fillAmount = targetBoss.curTime / targetBoss.maxTime;
                    }
                    else image.fillAmount = targetBoss.curHealth / targetBoss.maxHealth;
                    break;
                }
            }

            if (targetBoss == null) {
                image.fillAmount = image.fillAmount.ApproachValue(0f, 16f);
            }
        }
        else {
            if (isTime) {
                image.fillAmount = targetBoss.curTime / targetBoss.maxTime;
            }
            else image.fillAmount = targetBoss.curHealth / targetBoss.maxHealth;
            if (isCircular) {
                rt.position = Camera.main.WorldToScreenPoint(targetBoss.transform.position);
            }
        }
        
    }
}
