using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleSetCtrl : MonoBehaviour
{
    public SpriteRenderer circlePrefab;
    public SpriteRenderer[] circleList;
    public int circleNum;
    public float circleInterval;
    public float circleScale;

    public int timer;
    private void Start() {
        circleList = new SpriteRenderer[circleNum];
        for (int i = 0; i < circleNum; i++) {
            var circle = Instantiate(circlePrefab, transform);
            circle.transform.position += circleInterval * i * Vector3.down;
            circleList[i] = circle;
        }
    }

    private void Update() {
        for (int i = 0; i < circleNum; i++) {
            circleList[i].transform.localScale =
                circleScale * (Mathf.Sin((timer + i * 45) * Mathf.Deg2Rad) + 0.6f) / 2f * Vector3.one;
        }

        timer++;
    }
}
