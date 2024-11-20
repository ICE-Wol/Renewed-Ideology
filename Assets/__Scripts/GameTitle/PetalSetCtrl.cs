using System.Collections;
using System.Collections.Generic;
using System.Timers;
using _Scripts.Tools;
using UnityEngine;

public class PetalSetCtrl : MonoBehaviour
{
    public SpriteRenderer petalPrefab;
    public int petalNum;
    public int maxPetalNum;

    public SpriteRenderer[] petalsSet;
    public int[] petalNums;

    public float totalDegree;
    
    public int timer = 0;
    void Start() {
        petalsSet = new SpriteRenderer[petalNum];
        for (int i = 0; i < petalNum; i++) {
            petalsSet[i] = Instantiate(petalPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            //petalsSet[i].color = petalsSet[i].color.SetAlpha(0f);
        }
    }
    
    void Update() {
        
        for(int i = 0; i < petalNum; i++) {
            var curDegree = totalDegree / petalNum * i;
            petalsSet[i].transform.localPosition = curDegree.Deg2Dir3();
            petalsSet[i].transform.localRotation = Quaternion.Euler(0, 0, curDegree + timer);
            //petalsSet[i].color = petalsSet[i].color.SetAlpha(1f);
        }

        transform.localRotation = Quaternion.Euler(0,0,timer / 2f);
        timer++;
    }
}
