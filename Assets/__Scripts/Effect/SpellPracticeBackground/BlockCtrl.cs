using System;
using _Scripts.Tools;
using UnityEngine;

public class BlockCtrl : MonoBehaviour
{
    /// <summary>
    /// 所有方块的父物体，作为旋转基底
    /// </summary>
    public Transform fullBgRotate;
    public Vector3 tarRotate;
    
    public Block blockPrefab;
    
    public Vector2Int size = new Vector2Int(10, 10);
    
    public float blockWidth = 0.2f;

    public Vector3 startPos = new Vector3(-10, -10, 0);

    public Block[,] Blocks;
    
    public Transform lookAtTarget;
    
    public void Init() {
        Blocks = new Block[size.x, size.y];
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                // 生成方块
                Block block = Instantiate(blockPrefab, fullBgRotate);
                block.transform.localPosition = startPos + new Vector3(i * blockWidth, j * blockWidth, 0);
                block.targetPos = block.transform.localPosition;//初始化目标tarPos，否则第一次会聚在一起
                block.transform.localScale = new Vector3(blockWidth, blockWidth, 1);
                Blocks[i, j] = block;
                
                
                // 设置方块的颜色
                block.spriteRenderer.color = Color.HSVToRGB(i / (float)size.x, j / (float)size.y, 1f);
            }
        }
        
        //lookAtTarget = new GameObject("LookAtTarget").transform;
        lookAtTarget.SetParent(fullBgRotate);
        
    }
    private void Start() {
        Init();
    }


    public bool isCircleMode = false;
    
    public float speed = 0.5f; // 移动速度

    private void Update() {
        lookAtTarget.localPosition = (10f * Time.time).Deg2Dir3() + 3f * Mathf.Sin(Time.time) * Vector3.forward;
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                Blocks[i, j].transform.LookAt(lookAtTarget);
            }
        }

        fullBgRotate.localRotation =
            Quaternion.Euler(fullBgRotate.localRotation.eulerAngles.ApproachValue(tarRotate, 32f));
        
        if(Input.anyKeyDown) {
            isCircleMode = !isCircleMode;
            //非圆形模式时必须重设方块位置
            if(!isCircleMode) {
                for (int i = 0; i < size.x; i++) {
                    for (int j = 0; j < size.y; j++) {
                        Blocks[i, j].targetPos = startPos + new Vector3(i * blockWidth, j * blockWidth, 0);
                        //Blocks[i, j].transform.localPosition = Blocks[i, j].targetPos;
                    }
                }
                tarRotate = Vector3.zero;
            }
            else {
                tarRotate = new Vector3(90f, 0, 0);
            }
        }
        
        if (!isCircleMode) {
            float totalWidth = size.x * blockWidth; // 方块铺满屏幕的总宽度

            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    // 更新每个方块的位置
                    Vector3 targetPos = Blocks[i, j].targetPos;
                    Vector3 currentPosition = Blocks[i, j].transform.localPosition;
                    targetPos.x += Time.deltaTime * speed;

                    // 使用模运算实现无缝衔接
                    if (targetPos.x > totalWidth / 2) {
                        targetPos.x -= totalWidth; // 超出右边界则移到左边界
                        currentPosition.x -= totalWidth;
                        Blocks[i, j].transform.localPosition = currentPosition; 
                        //只有出界确实触发时才更新实际位置，否则会回拉
                    }
                    else if (targetPos.x < -totalWidth / 2) {
                        targetPos.x += totalWidth; // 超出左边界则移到右边界
                        currentPosition.x += totalWidth;
                        Blocks[i, j].transform.localPosition = currentPosition;
                    }

                    Blocks[i, j].targetPos = targetPos;
                }
            }
        }
        else {
            // 圆形模式
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    // 更新每个方块的位置
                    float angle = Time.time * (i/2f + 5f) + 360f / size.y * j;

                    Blocks[i, j].targetPos =
                        (0.5f + 0.1f * i) * angle.Deg2Dir3() + (i % 2 == 0 ? 1 : -1) * Mathf.Sin((size.x - i) / 5f) * Vector3.forward;

                }
            }
        }
    }


}
