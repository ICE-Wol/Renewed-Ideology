using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterOrbParticle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector3 parentPos;
    public float scale;
    public float direction;
    
    public float maxRad;
    public bool isArrived;

    private void Start() {
        scale = 0f;
        maxRad = 0.3f;
        direction = Random.Range(0f, 360f);
        transform.position = parentPos + Random.Range(0f, 1f) * direction.Deg2Dir3();
        transform.localScale = scale * Vector3.one;
        spriteRenderer.color = Color.HSVToRGB(Random.Range(0.4f, 0.6f), Random.Range(0.8f, 1f),
            Random.Range(0.8f, 1f));
        spriteRenderer.color = spriteRenderer.color.SetAlpha(0.5f);
    }
    private void Update() {
        transform.position += Time.deltaTime * direction.Deg2Dir3();
        if (!isArrived) {
            scale.ApproachRef(maxRad, 16f);
            if(scale.Equal(maxRad,0.1f)) isArrived = true;
        }
        else {
            scale.ApproachRef(0f, 32f);
            if(scale.Equal(0f,0.001f)) 
                Destroy(gameObject);
        }
        transform.localScale = scale * Vector3.one;
    }
}
