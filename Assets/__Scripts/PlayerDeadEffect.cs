using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerDeadEffect : MonoBehaviour
{
    public SpriteRenderer playerDeadParticle;
    public SpriteRenderer[] particles;
    
    public int particleCount;
    public float[] radius;
    public float[] curRad;
    public float[] degree;
    public float[] scale;

    public float[] spdRad;
    [FormerlySerializedAs("spdDeg")] public float[] spdScale;

    public void Start() {
        particleCount = Random.Range(10, 20);
        
        particles = new SpriteRenderer[particleCount];
        radius = new float[particleCount];
        curRad = new float[particleCount];
        degree = new float[particleCount];
        scale = new float[particleCount];
        spdScale = new float[particleCount];
        spdRad = new float[particleCount];
        
        for (int i = 0; i < particleCount; i++) {
            particles[i] = Instantiate(playerDeadParticle, transform);
            degree[i] = 360f / particleCount * i + Random.Range(-5f,5f);
            scale[i] = 5f + Random.Range(1f,2f);
            radius[i] = Random.Range(3f, 5f);
            spdScale[i] = Random.Range(0.1f, 0.5f);
            
            Color.RGBToHSV(Color.cyan, out var h, out var s, out var v);
            particles[i].material.SetFloat("_Hue", h);
            particles[i].material.SetFloat("_Saturation", s);
            particles[i].material.SetFloat("_Alpha", 0.5f);
            
            
        }
    }
    
    public void Update() {
        bool isAllZero = true;
        for (int i = 0; i < particleCount; i++) {
            scale[i] -= spdScale[i];
            if (scale[i] <= 0) {
                scale[i] = 0;
            }else {
                isAllZero = false;
            }

            curRad[i].ApproachRef(radius[i], 16f);//spdRad[i];
            particles[i].transform.localPosition = curRad[i] * degree[i].Deg2Dir3();
            particles[i].transform.localScale = scale[i] * Vector3.one;
        }
        if(isAllZero) Destroy(gameObject);
    }
}
