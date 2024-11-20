using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using MEC;
using Unity.Mathematics;
using UnityEngine;

public class CurveLaserHead : MonoBehaviour {
    public State laserNodePrefab;
    public LineRenderer lineRenderer;
    public State[] nodes;
    public int length;
    public float width;

    [Header("是否直接改变位置，false时应用插值函数")]
    public bool isPosChangeDirect = true;
    
    [Header("否时插值函数的逼近速度参数")]
    public float approachRate;
    
    [Header("不必更改，当前生成到的节点序号")]
    public float curGenNodeIndex;
    
    [Header("不必更改，初始生成位置")]
    public Vector3 initPos;


    
    public IEnumerator<float> GenerateLaserWithLength(int len) {
        length = len;
        nodes = new State[len];
        initPos = transform.position;
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, width), new Keyframe(1, width));
        lineRenderer.widthCurve.RemoveKey(0);
        for (int i = 0; i < len; i++) {
            nodes[i] = Instantiate(laserNodePrefab, initPos, quaternion.identity);
            lineRenderer.positionCount = i + 1;
            lineRenderer.SetPosition(i, nodes[i].transform.position);
            lineRenderer.widthCurve.AddKey((float)i / len, width);
            curGenNodeIndex = i;
            yield return Calc.WaitForFrames(3);
        }
    }

    /// <summary>
    /// 只需要让子弹正常移动即可，保证子弹生成运动方式一致的前提下，差速会自动组成弹链
    /// </summary>
    public void UpdateLaser() {
        for (int i = 0; i <= curGenNodeIndex; i++) {
            if (nodes[i] == null) {
                lineRenderer.widthCurve.RemoveKey(i);
                lineRenderer.widthCurve.AddKey((float)i / length, 0f);
                continue;
            }
            
            lineRenderer.SetPosition(i, nodes[i].transform.position);
        }
    }

    public void Awake() {
        Timing.RunCoroutine(GenerateLaserWithLength(length),"Shoot");
    }
    public void Update() {
        UpdateLaser();
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
    }
}


