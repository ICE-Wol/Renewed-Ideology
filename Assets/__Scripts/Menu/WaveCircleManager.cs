using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class WaveCircleManager : MonoBehaviour {
    public WaveCircleCtrl linePrefab;
    public WaveCircleCtrl[] lines;
    
    public int count;
    public float outerRad;
    public float innerRad;

    public bool isMirror;

    private void Change() {
        for (int i = 0; i <= count; i++) {
            lines[i].tarDegree = 180f - lines[i].tarDegree;
        }
    }
    private void Start() {
        lines = new WaveCircleCtrl[count + 1];
        for (int i = 0; i <= count; i++) {
            lines[i] = Instantiate(linePrefab, transform);
            lines[i].center = transform.position;
            lines[i].tarDegree = 180f / count * i;
            lines[i].innerRad = innerRad;
            lines[i].outerRad = Mathf.Sin(Time.time) + outerRad;
            lines[i].alpha = 0.1f;
        }
    }

    private void Update() {
        if(Input.anyKeyDown) Change();
        
        for (int i = 0; i <= count; i++) {
            lines[i].outerRad = Mathf.Sin(Time.time * 2 + i) / 4f + outerRad;
            lines[i].innerRad = -Mathf.Sin(Time.time * 2 + i) / 8f + innerRad;
            if (isMirror) {
                lines[i].outerRad = -(Mathf.Sin(Time.time * 2 + i) / 4f + outerRad);
                lines[i].innerRad = -(-Mathf.Sin(Time.time * 2 + i) / 8f + innerRad);
            }

            lines[i].alpha = Mathf.Sin(Time.time * 2 + i) / 10f + 0.1f;
            
            if (i >= count / 2) {
                lines[i].outerRad = lines[count - i].outerRad;
                lines[i].innerRad = lines[count - i].innerRad;
                lines[i].alpha = lines[count - i].alpha;
            }
        }
    }
}
