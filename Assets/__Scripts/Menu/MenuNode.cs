using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuNode : MonoBehaviour
{
    public int curMenuIndex;
    public string menuName;
    public TMP_Text menuText;

    public MenuNode[] childNodes;
    public MenuNode parentNode;

    public Vector3 unselectPos;
    public Vector3 selectPos;
    public Vector3 initialPos;
    
    public float curScale;
    public float tarScale;
    
    public bool isMenuOnScreen;

    private void Awake() {
        curScale = 1;
        tarScale = 1;
        menuText.text = menuName[0] + "<size=5>" + menuName.Substring(1) + "</size>";
    }

    private void Update() {
        curScale.ApproachRef(tarScale, 16f);
        transform.localScale = curScale * Vector3.one;
    }

    public void SetAlpha(float alpha) {
        menuText.color = menuText.color.SetAlpha(alpha);
    }
    
    public float GetAlpha() {
        return menuText.color.a;
    }
    
}
