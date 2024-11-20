using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BulletDestroyParticle : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteSequence;
    public Rotator rot;

    public int frameInterval;
    public Color c;

    public void SetSpriteColorHSV(Color c) {
        spriteRenderer.color = c; 
    }
    
    public IEnumerator<float> PlayAnimation() {
        int index = 0;
        while (true) {
            spriteRenderer.sprite = spriteSequence[index];
            index++;
            if(index >= spriteSequence.Length) Destroy(gameObject);
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(frameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }
    public void Start() {
        Timing.RunCoroutine(PlayAnimation());
        rot.rotateMultiplier = Random.Range(-0.5f, 0.5f);
        rot.initDegree = Random.Range(0, 360);
    }
    
}
