using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class MenuDecoManager : MonoBehaviour {
    public bool isTitleFaded;
    public SpriteRenderer titleRenderer;
    
    public TitleCircleCtrl[] circles;
    
    public TitleLineManager[] lines;

    IEnumerator<float> FadeTitle() {
        while (titleRenderer.color.a > 0.001f) {
            var color = titleRenderer.color;
            titleRenderer.color = color.Fade(16f);
            yield return Timing.WaitForOneFrame;
        }
        Destroy(titleRenderer.gameObject);
        lines[0].gameObject.SetActive(true);
    }
    
    IEnumerator<float> AppearTitle() {
        while(!titleRenderer.color.a.Equal(1f,0.01f)) {
            var color = titleRenderer.color;
            titleRenderer.color = color.Appear(32f);
            yield return Timing.WaitForOneFrame;
        }
    }
    
    private IEnumerator<float> MakeAllCircleAppear() {
        List<TitleCircleCtrl> list = new List<TitleCircleCtrl>();
        foreach (var c in circles) {
            c.GetComponentsInChildren<TitleCircleCtrl>(list);
            foreach (var t in list) {
                t.isAppearing = true;
            }
            
            yield return Timing.WaitForSeconds(0.5f);
        }
    }
    
    public void MoveCirclePos (float target) {
        tarPos = target;
    }

    public float tarPos;
    
    private void Start() { 
        Timing.RunCoroutine(MakeAllCircleAppear());
        Timing.RunCoroutine(AppearTitle(),"Appear");
    }
    private void Update() {
        var pos = transform.position;
        pos.x = pos.x.ApproachValue(tarPos, 16f);
        transform.position = pos;

        if (Input.anyKeyDown && !isTitleFaded) {
            Timing.KillCoroutines("Appear");
            Timing.RunCoroutine(FadeTitle()); 
            MoveCirclePos(-8f);
            isTitleFaded = true;
        }
    }
}
