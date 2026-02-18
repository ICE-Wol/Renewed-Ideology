using System;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class ShardCircleTransition : MonoBehaviour
{
    public ShardCircleCtrl shardPrefab;
    public List<List<ShardCircleCtrl>> shardList = new List<List<ShardCircleCtrl>>();

    public void InitInstant(int row, int col, bool isOpen) {
        for (int i = 0; i < row; i++) {
            List<ShardCircleCtrl> temp = new List<ShardCircleCtrl>();
            for (int j = 0; j < col + 10 * i; j++) {
                ShardCircleCtrl shard = Instantiate(shardPrefab, transform);
                shard.transform.localPosition = 3f * (i - 0.5f) * (j / (float)(col + 10 * i) * 360).Deg2Dir3();
                shard.tfApproacher.targetPos = 3f * (i - 0.5f) * (j / (float)(col + 10 * i) * 360).Deg2Dir3();
                shard.transform.localRotation = Quaternion.Euler(0, 0, j / (float)(col + 10 * i) * 360);
                shard.tfApproacher.targetRot = new Vector3(0, 0, j / (float)(col + 10 * i) * 360);

                if (isOpen) {
                    shard.transform.localScale = 3f * (Vector3.one + i * 0.2f * Vector3.one);
                }else {
                    shard.transform.localScale = Vector3.zero;
                }
                temp.Add(shard);
                

            }
            shardList.Add(temp);            
        }
    }

    public IEnumerator<float> CloseSlow() {
        for (int i = 0; i < shardList.Count; i++) {
            for (int j = 0; j < shardList[i].Count; j++) {
                shardList[i][j].tfApproacher.targetScale = Vector3.up;

            }

            yield return Calc.WaitForFrames(10);
        }
    }

    public void CloseInstant() {
        for (int i = 0; i < shardList.Count; i++) {
            for (int j = 0; j < shardList[i].Count; j++) {
                shardList[i][j].tfApproacher.targetScale = Vector3.zero;
            }
        }
    }
    
    public IEnumerator<float> OpenSlow() {
        for (int i = 0; i < shardList.Count; i++) {
            for (int j = 0; j < shardList[i].Count; j++) {
                shardList[i][j].transform.localScale = Vector3.up;
                shardList[i][j].tfApproacher.targetScale = 3f * (Vector3.one + i * 0.2f * Vector3.one);
            }
            yield return Calc.WaitForFrames(10);
        }
    }
    
    public void OpenInstant() {
        for (int i = 0; i < shardList.Count; i++) {
            for (int j = 0; j < shardList[i].Count; j++) {
                shardList[i][j].tfApproacher.targetScale = 3f * (Vector3.one + i * 0.2f * Vector3.one);
            }
        }
    }

    private void Start() {
        //Timing.RunCoroutine(InitSlow(8, 5));
        InitInstant(8, 0, false);
    }
    
    public bool isOpen = false;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            isOpen = !isOpen;
            if (isOpen) {
                Timing.RunCoroutine(OpenSlow());
            }
            else {
                Timing.RunCoroutine(CloseSlow());
            }
        }
    }
}
