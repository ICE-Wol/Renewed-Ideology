using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using MEC;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CurveLaserHead : MonoBehaviour
{
    public MeshFilter meshFilter;
    public State laserNodePrefab;
    
    [Header("激光节点数")] public int length;
    [Header("激光整体宽度因数")] public float width;
    [Header("激光边缘节点数")]public int edgeNodeCount  = 10;

    [Header("不必更改")]
    [Header("当前生成到的节点序号")] public int curGenNodeIndex;

    [Header("初始生成位置")] public Vector3 initPos;

    [Header("激光节点数组")]
    public Transform[] nodes;
    [Header("当前节点的存在状态，0表示被反激活，1表示被激活")]
    public int[] nodesCondition;
    
    [Header("激光在当前节点的半径，用于生成Mesh")]
    public float[] radius;

    [Header("激光在当前节点的上下顶点，用于生成Mesh")]
    public Vector3[] upPos;
    public Vector3[] downPos;
    
    private void Start()
    {
        radius = new float[length];
        upPos = new Vector3[length];
        downPos = new Vector3[length];
        nodes = new Transform[length];
        Timing.RunCoroutine(GenerateLaserWithLength(length),"Shoot");
    }
    public void Update() {
        UpdateLaserWidth();
        meshFilter.mesh = GenerateMeshFromNodes(nodes, radius,edgeNodeCount );
        initPos = transform.position;
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
    }

    public IEnumerator<float> GenerateLaserWithLength(int len) {
        length = len;
        nodes = new Transform[len];
        nodesCondition = new int[len];
        initPos = transform.position;
        for (int i = 0; i < len; i++) {
            nodesCondition[i] = 1;
            nodes[i] = Instantiate(laserNodePrefab, initPos, quaternion.identity).transform;
            curGenNodeIndex = i;
            yield return Calc.WaitForFrames(3);
        }
    }

    private Mesh GenerateMeshFromNodes(Transform[] nodes, float[] radius, int edgeNodeCount) {
        int length = curGenNodeIndex;
        // 检查并调整 edgeNodeCount
        if (length < 3)
        {
            Debug.LogWarning("节点数量过少，无法生成中间部分。直接使用线性UV分布。");
            edgeNodeCount = 0; // 所有节点线性分布UV
        }
        else if (edgeNodeCount * 2 >= length)
        {
            edgeNodeCount = (length - 1) / 2; // 限制 edgeNodeCount
            Debug.LogWarning($"edgeNodeCount 太大，已调整为 {edgeNodeCount} 以确保正确的分布。");
        }
        if (length < 2){// || edgeNodeCount * 2 >= length) {
            //Debug.LogError("节点数量或参数配置有误");
            return null;
        }

        Vector3[] vertices = new Vector3[length * 2]; // 每个节点有 upPos 和 downPos 两个顶点
        Vector2[] uv = new Vector2[length * 2]; // 每个顶点对应一个UV
        int[] triangles = new int[(length - 1) * 6]; // 每段用两个三角形连接

        float dir = 0;

        // 计算UV跨度
        float edgeUVStep = 0.1f / edgeNodeCount; // 边缘UV线性跨度，前后各占0.1
        float middleUVStep = 0.8f / (length - edgeNodeCount * 2 - 1); // 中间部分UV跨度

        // 计算顶点和UV
        for (int i = 0; i < length; i++) {
            if (i != length - 1) {
                dir = Calc.GetDirection(nodes[i].position, nodes[i + 1].position);
            }

            // 顶点计算
            vertices[i * 2] = nodes[i].position + radius[i] * (dir + 90).Deg2Dir3(); // upPos
            vertices[i * 2 + 1] = nodes[i].position + radius[i] * (dir - 90).Deg2Dir3(); // downPos

            // UV计算
            float t;
            if (i < edgeNodeCount) // 前边缘
            {
                t = edgeUVStep * i; // 从0线性增加
            }
            else if (i >= length - edgeNodeCount) // 后边缘
            {
                t = 0.9f + edgeUVStep * (i - (length - edgeNodeCount)); // 从0.9线性增加
            }
            else // 中间部分
            {
                t = 0.1f + middleUVStep * (i - edgeNodeCount); // 从0.1线性增加
            }

            uv[i * 2] = new Vector2(t, 1); // upPos UV
            uv[i * 2 + 1] = new Vector2(t, 0); // downPos UV
        }

        // 构建三角形索引
        for (int i = 0; i < length - 1; i++) {
            int idx = i * 6;

            triangles[idx] = i * 2; // upPos[i]
            triangles[idx + 1] = (i + 1) * 2; // upPos[i+1]
            triangles[idx + 2] = i * 2 + 1; // downPos[i]

            triangles[idx + 3] = (i + 1) * 2; // upPos[i+1]
            triangles[idx + 4] = (i + 1) * 2 + 1; // downPos[i+1]
            triangles[idx + 5] = i * 2 + 1; // downPos[i]
        }

        // 创建Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 自动计算法线

        return mesh;
    }

    public void UpdateLaserWidth() {
        for (int i = 0; i < length; i++) {
            nodesCondition[i] = (nodes[i] == null) ? 0 : 1;
        }
        
        int[] tmp = new int[length]; 
        int[] tmp2 = new int[length]; 
        for(int i = 0; i < length; i++) {
            tmp[i] = nodesCondition[i];
            tmp2[i] = nodesCondition[i];
        }
        
        for (int i = 1; i < length; i++) {
            if (tmp[i] == nodesCondition[i - 1] && tmp[i] == 1)
                tmp[i] = tmp[i - 1] + 1;
            else tmp[i] = 0;
        }

        
        for (int i = length - 2; i >= 0; i--) {
            if (tmp2[i] == nodesCondition[i + 1] && tmp2[i] == 1)
                tmp2[i] = tmp2[i + 1] + 1;
            else tmp2[i] = 0;
        }
        
        for (int i = 0; i < length; i++) {
            tmp[i] = Mathf.Min(tmp[i], tmp2[i]);
            if (tmp[i] >= 5) tmp[i] = 5;
        }


        for (int i = 0; i < length; i++) {
            radius[i] = /*tmp[i] / 5f */width;
        }
        
        
    }

    
}


