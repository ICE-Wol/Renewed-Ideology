using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0NS4 : MonoBehaviour
{
    public DoubleSpeedApproach scalePrefab;

    public float circleRad;
    public float circleDeg;
    public float crossDeg;

    public int ways;
    public int num;
    public float interval;

    public List<DoubleSpeedApproach> scaleList;
    
    public int timer;

    public IEnumerator<float> CrossGen(Vector3 centerPos) {
        for(int j = 0;j < num;j++) {
            for (int i = 0; i < ways; i++) {
                DoubleSpeedApproach scale = Instantiate(scalePrefab, transform.position, Quaternion.identity);
                scaleList.Add(scale);
                
                float deg = crossDeg + 360f / ways * i;
                scale.direction = deg;
                scale.transform.position = centerPos + deg.Deg2Dir3() * j * interval;

                Color c = Color.HSVToRGB((float)j / num, 1f, 1f);
                scale.GetComponent<Config>().color = c;
                scale.GetComponent<State>().SetColor(c);
            }
            yield return Calc.WaitForFrames(5);
        }
    }

    private void Start() {
        scaleList = new List<DoubleSpeedApproach>();
    }

    // Update is called once per frame
    void Update() {
        if(timer % 5 == 0 && circleDeg <= 360f) {
            float rad = circleRad;// * (1.5f + 0.5f * Mathf.Sin(circleDeg * Mathf.Deg2Rad));
            Timing.RunCoroutine(CrossGen(transform.position + circleDeg.Deg2Dir3() * rad));
            circleDeg += 20f;
            //crossDeg = circleDeg/8f;
            crossDeg = circleDeg/4f;
        }

        if (timer == 180) {
            foreach (var b in scaleList) {
                b.trigger = true;
            }
        }
        timer++;
    }
}
