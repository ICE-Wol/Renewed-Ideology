using _Scripts.Tools;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class LoadingCtrl : MonoBehaviour {
    public TMP_Text textChi;
    public TMP_Text textEng;

    public bool isDestroyed;
    public float memAlpha;
    
    private void Start() {
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in meshRenderer) {
            mr.sortingLayerName = "UI";
        }

        if (isDestroyed) memAlpha = (Mathf.Sin(Mathf.Deg2Rad * Time.time * 500f) + 1f) / 2f;
        textChi.color = textChi.color.SetAlpha(curAlpha);
        textEng.color = textEng.color.SetAlpha(curAlpha);
    }

    public float tarAlpha;
    public float curAlpha;

    public int timer = 0;
    
    private void Update() {
        if (!isDestroyed) {
            tarAlpha = (Mathf.Sin(Mathf.Deg2Rad * Time.time * 500f) + 1f) / 2f;
            curAlpha.ApproachRef(tarAlpha, 4f);
        }
        else {
            // if (!isTweened) {
            //     tarAlpha = (Mathf.Sin(Mathf.Deg2Rad * Time.time * 500f) + 1f) / 2f;
            //     curAlpha = tarAlpha;
            //     isTweened = true;
            //     DOTween.To(() => tarAlpha, x => tarAlpha = x, 0, 1f).OnComplete(() => Destroy(gameObject));
            // }
            curAlpha = Mathf.SmoothStep(memAlpha, 0, timer / 30f);
            if(timer > 30) Destroy(gameObject);
            timer++;
        }
        
        textChi.color = textChi.color.SetAlpha(curAlpha);
        textEng.color = textEng.color.SetAlpha(curAlpha);
        //if (timer != 0) timer++;
    }
}
