using System;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class ReverseColorCtrl : MonoBehaviour
{
    public static Vector3 center;
    public static Vector3[] circles = new Vector3[6];
    private static readonly int Center = Shader.PropertyToID("_Center");
    
    private readonly Vector3[] _posOffSetAndRadius = new Vector3[6] {
        new(0, 0, -1),
        new(1, 0, -1),
        new(-1, 0, -1),
        new(0, 1, -1),
        new(0, -1, -1),
        new(0, 0, -1),
    };

    Material mat => GetComponent<Renderer>().material;
    
    
    // void Update() {
    //     center =  GameManager.Manager.curBoss.transform.position;
    //     
    //     mat.SetVector("_Circle" + 0, Calc.SetZ(center + Vector2.zero,2f));
    //     mat.SetVector("_Circle" + 1, ((Vector3)(center + Vector2.right)).SetZ(2f));
    //     mat.SetVector("_Circle" + 2, ((Vector3)(center + Vector2.left)).SetZ(2f));
    //     mat.SetVector("_Circle" + 3, ((Vector3)(center + Vector2.up)).SetZ(2f));
    //     mat.SetVector("_Circle" + 4, ((Vector3)(center + Vector2.down)).SetZ(2f));
    //     
    // }
    
    IEnumerator<float> StartExpandCircle(int num,float spdPerFrame){
        if (num < 0 || num > 6) yield break;
        var radius = 0f;
        while (true) {
            radius += spdPerFrame;
            spdPerFrame += 0.005f;
            mat.SetVector("_Circle" + num, (center + _posOffSetAndRadius[num]).SetZ(radius));
            yield return Timing.WaitForOneFrame;
        }
    }
    
    IEnumerator<float> StartReverseColorEffect() {
        Timing.RunCoroutine(StartExpandCircle(0, 0.1f),"ReverseColorEffect");
        yield return Timing.WaitForSeconds(0.3f);
        for (int i = 1; i < 5; i++) {
            Timing.RunCoroutine(StartExpandCircle(i, 0.1f),"ReverseColorEffect");
        }
        yield return Timing.WaitForSeconds(0.2f);
        Timing.RunCoroutine(StartExpandCircle(5, 0.1f),"ReverseColorEffect");
        yield return Timing.WaitForSeconds(1.5f);
        Timing.KillCoroutines("ReverseColorEffect");
        for (int i = 0; i < 6; i++) {
            mat.SetVector("_Circle" + i, (center + _posOffSetAndRadius[i]).SetZ(0f));           
        }
    }

    public void StartReverseColorEffectAtCenter(Vector3 initCenter) {
        center = initCenter;
        Timing.RunCoroutine(StartReverseColorEffect());
    }
    

    void OnDestroy()
    {
        Destroy(mat);
    }
}
