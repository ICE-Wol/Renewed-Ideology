using System;
using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadingCtrl : MonoBehaviour {
    public TMP_Text textChi;
    public TMP_Text textEng;

    public int timer;
    private void Start() {
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in meshRenderer) {
            mr.sortingLayerName = "UI";
        }
    }

    private void Update() {
        var alpha = (Mathf.Sin(Mathf.Deg2Rad * timer * 10f) + 1f) / 2f;
        textChi.color = textChi.color.SetAlpha(alpha);
        textEng.color = textEng.color.SetAlpha(alpha);
        if (timer != 0) timer++;
    }
}
