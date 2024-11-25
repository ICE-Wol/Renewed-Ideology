using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CurveLaserHead : MonoBehaviour
{
    private static readonly int Hue = Shader.PropertyToID("_Hue");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");
    public MeshFilter meshFilter;
    public State laserNodePrefab;
    public MeshRenderer meshRenderer;

    [Header("测试区域")] 
    public float initDir = 0;

    public float dirAddValue = 0;    
    
    
    [Header("激光颜色")] public Color color;
    [Header("激光节点数")] public int length;
    [Header("激光整体宽度因数")] public float width;
    [Header("激光边缘节点数")]public int edgeNodeCount  = 10;

    [Header("不必更改")] 
    [Header("是否由其他激光断开得到")]public bool isLaserGeneratedFromLaser;
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

    private void Awake() {
        Color.RGBToHSV(color, out var H, out var S, out var V);
        meshRenderer.material.SetFloat(Hue, H);
        meshRenderer.material.SetFloat(Saturation, S);
        laserNodePrefab.bulletConfig.color = color;

        nodesCondition = new int[length];
        radius = new float[length];
        upPos = new Vector3[length];
        downPos = new Vector3[length];
        nodes = new Transform[length];
        nodesCondition = new int[length];
    }
    
    private void Start() {
        if (!isLaserGeneratedFromLaser) {
            Timing.RunCoroutine(GenerateLaserWithLength(this,0, length).CancelWith(gameObject), "Shoot");
            print("start");
        }
    }

    public void Update() {
        //求bullet Condition功能与RefreshLaser合并
        //原有的求宽度功能因为三角片会产生折线的原因取消
        //UpdateLaserWidth();
        
        //销毁后代码依旧会顺着跑，所以销毁后要提前终止
        if (RefreshLaser()) return;
        
        meshFilter.mesh = GenerateMeshFromNodes(nodes, radius,edgeNodeCount );
        initPos = transform.position;
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
    }

    public IEnumerator<float> GenerateLaserWithLength(CurveLaserHead head,int startPos,int remLen) {
        head.length = startPos + remLen;
        head.initPos = head.transform.position;
        for (int i = startPos; i < head.length; i++) {
            head.nodesCondition[i] = 1;
            head.nodes[i] = Instantiate(head.laserNodePrefab, head.initPos, quaternion.identity).transform;
            //head.nodes[i].GetComponent<Config>().laserParent = head;
            Timing.RunCoroutine(ChangeBulletDir(head.nodes[i].GetComponent<DoubleSpeedApproach>(),head.dirAddValue)
                .CancelWith(head.nodes[i].gameObject));
            head.curGenNodeIndex = i + 1;
            yield return Calc.WaitForFrames(3);
        }
    }
    
    //需要做的事
    //0 对于一条完整的、有剩余点未生成的激光
    //0.1 每一条激光被拆分后，剩下的激光都视为完整的激光，且仅有最后一条激光需要接续管理剩下的未生成节点
    //0.2 每一个没有未生成节点的激光都可以看做未生成节点个数为0的激光
    //0.3 新激光属性列表:开头节点位置 新激光节点数 剩余未生成节点数
    //1 判断激光节点是否改变，也就是说一旦出现断点就视为改变
    //2 将剩下的激光节点分成新的连续段落
    //3 对于剩下的连续段落，**大于一定长度**则生成新的激光
    //4 清理没有被任何激光管理的激光节点
    //5 最后的激光节点需要接续管理剩下的未生成节点

    public struct LaserBlock{
        public int startIndex;
        public int length;
        public int remainNodeCount;
    }
    public bool RefreshLaser() {
        //string res2 = " ";
        for (int i = 0; i < this.length; i++) {
             nodesCondition[i] = (nodes[i] == null) ? 0 : 1;
             //res2 += "i:" + i + " " + nodesCondition[i] + ",";
        }
        //print(res2);
        
        for (int i = 0; i < this.length; i++) {
            radius[i] = /*tmp[i] / 5f */width;
        }
        
        var isFullZero = true;
        for(int i = 0; i < curGenNodeIndex; i++) {
            //print(nodesCondition.Length + " " + curGenNodeIndex);
            if(nodesCondition[i] == 1) {
                isFullZero = false;
                break;
            }
        }

        if (isFullZero) {
            Debug.Log("All nodes are zero, destroy laser");
            Destroy(gameObject);
            return true;
        }
        
        //1
        var isLaserNodeChanged = false;
        for(int i = 0; i < curGenNodeIndex; i++) {
            if(nodesCondition[i] == 0) {
                isLaserNodeChanged = true;
                break;
            }
        }

        if (!isLaserNodeChanged) return false;
        
        List<LaserBlock> laserBlocks = new List<LaserBlock>();
        //string res = " ";
        //for (int i = 0; i < this.length; i++) {
        //    res += "i:" + i + " (" + nodesCondition[i] + "),";
        // }
        //print(res);
        //2
        int isHead = -1;
        int length = 0;
        for(int i = 0; i < curGenNodeIndex; i++) {
            
            if(nodesCondition[i] == 1) {
                if(i == 0) isHead = i;
                else if(nodesCondition[i - 1] == 0) isHead = i;
            }
            
            if(nodesCondition[i] == 0 || i == curGenNodeIndex - 1) {
                LaserBlock block = new LaserBlock();
                block.startIndex = isHead;
                block.length = length;
                if (i != curGenNodeIndex - 1) {
                    block.remainNodeCount = 0;
                }else {
                    block.remainNodeCount = this.length - curGenNodeIndex;
                }
                if(isHead == -1) continue;
                laserBlocks.Add(block);
                //print(block.startIndex + " " + block.length + " "+ block.remainNodeCount);
                isHead = -1;
                length = 0;
            }

            if (isHead != -1) {
                length++;
            }
        }

        //3,4,5
        for(int i = 0; i < laserBlocks.Count; i++) {
            LaserBlock block = laserBlocks[i];
            //4
            if(block.length <= 10) {
                print("length <= 10");
                for(int j = 0; j < block.length; j++) {
                    Destroy(nodes[block.startIndex + j].gameObject);
                }
                continue;
            }
            //oldLaser使用restart函数新建laser
            //这里一定要注意是使用谁的restart函数
            //print(block.startIndex + " " + block.length + " "+ block.remainNodeCount
            //+"full length: " + this.length);
            ReStart(block);
        }
        //print(laserBlocks.Count);
        //print(gameObject.name);
        Destroy(gameObject);
        return true;

    }
    
    private void ReStart(LaserBlock block) {
        //print("restarted");
        //print(block.startIndex + " " + block.length + " "+ block.remainNodeCount);
        var newLaser = Instantiate(this);
        var fullLength = block.length + block.remainNodeCount;
        newLaser.length = fullLength;
        newLaser.isLaserGeneratedFromLaser = true;
        newLaser.nodesCondition = new int[fullLength];
        newLaser.radius = new float[fullLength];
        newLaser.upPos = new Vector3[fullLength];
        newLaser.downPos = new Vector3[fullLength];
        newLaser.nodes = new Transform[fullLength];
        newLaser.curGenNodeIndex = block.length;
        for (int i = 0; i < block.length; i++) {
            if(nodes[block.startIndex + i] == null) {
                Debug.Log("NULL :start index:" + block.startIndex + " i:" + i);
                continue;
            }
            newLaser.nodes[i] = nodes[block.startIndex + i];
            //newLaser.nodes[i].GetComponent<Config>().laserParent = newLaser;
            
            //Debug.DrawLine(newLaser.nodes[i].position, newLaser.nodes[i].position + Vector3.up * 0.1f, new Color(block.length/30f > 0.5f? 1f:0f,1,1), 1000f);
             //print(newLaser.nodes[i].position);
        }
        foreach (var node in newLaser.nodes) {
            //Debug.DrawLine(node.position, node.position + Vector3.up * 0.1f, Color.red, 1000f);
        }
        if (block.remainNodeCount != 0) {
            Timing.RunCoroutine(
                GenerateLaserWithLength(newLaser, block.length, block.remainNodeCount).CancelWith(newLaser.gameObject),
                "Shoot");
        }


    }

    public IEnumerator<float> ChangeBulletDir(DoubleSpeedApproach bullet,float dirAddValue) {
        var timer = 0;
        while (true) {
            bullet.direction = 180 * Mathf.Sin(Mathf.Deg2Rad * (timer * 3 + initDir));
            bullet.endSpeed -= 0.01f;
            timer++;
            yield return Timing.WaitForOneFrame;
            
        }
    }

    private Mesh GenerateMeshFromNodes(Transform[] nodes, float[] radius, int edgeNodeCount) {
        int length = curGenNodeIndex;

        // 检查并调整 edgeNodeCount
        if (length < 3) {
            Debug.LogWarning("节点数量过少，无法生成中间部分。直接使用线性UV分布。");
            edgeNodeCount = 0; // 所有节点线性分布UV
        }
        else if (edgeNodeCount * 2 >= length) {
            edgeNodeCount = (length - 1) / 2; // 限制 edgeNodeCount
            Debug.LogWarning($"edgeNodeCount 太大，已调整为 {edgeNodeCount} 以确保正确的分布。");
        }

        Vector3[] vertices = new Vector3[length * 2]; // 每个节点有 upPos 和 downPos 两个顶点
        Vector2[] uv = new Vector2[length * 2]; // 每个顶点对应一个UV
        int[] triangles = new int[(length - 1) * 6]; // 每段用两个三角形连接

        float dir = 0;

        // 计算UV跨度
        float edgeUVStep = 0.1f / edgeNodeCount; // 边缘UV线性跨度
        float middleUVStep = 0.8f / (length - edgeNodeCount * 2 - 1); // 中间部分UV跨度

        // 当前物体的 Transform
        Transform thisTransform = this.transform;

        // 计算顶点和UV
        for (int i = 0; i < length; i++) {
            if (i != length - 1) {
                // if (nodes[i] == null || nodes[i + 1] == null) {
                //     print("null"+i + " " + (i + 1) +" length: " +length);
                //     continue;
                // }
                dir = Calc.GetDirection(nodes[i].position, nodes[i + 1].position);
            }

            // 世界坐标
            Vector3 worldUpPos = nodes[i].position + radius[i] * (dir + 90).Deg2Dir3();
            Vector3 worldDownPos = nodes[i].position + radius[i] * (dir - 90).Deg2Dir3();

            // 转换为局部坐标 因为mesh的顶点是相对于物体的
            vertices[i * 2] = thisTransform.InverseTransformPoint(worldUpPos);
            vertices[i * 2 + 1] = thisTransform.InverseTransformPoint(worldDownPos);

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


