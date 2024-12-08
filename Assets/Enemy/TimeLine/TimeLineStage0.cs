using System;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class TimeLineStage0 : MonoBehaviour
{
    public int timer;
    
    [Header("180 开幕7way狙")]
    public FairyMovement st0F0Prefab;
    public FairyMovement st0F1Prefab;
    
    [Header("1500 面标题")]
    public StageTitleCtrl2 stageTitle;
    
    [Header("1800 增way三角片 + 狙")]
    public FairyMovement st0F2Prefab;
    public FairyMovement st0F3Prefab;
    public FairyMovement st0F4Prefab;
    
    [Header("2800 散花")]
    public FairyMovement st0F5Prefab;
    

    IEnumerator<float> GenerateFairy(FairyMovement fairyPrefab, Vector3 offset, int count, int interval)
    {
        for (int i = 0; i < count; i++)
        {
            FairyMovement fairy = Instantiate(fairyPrefab, new Vector3(-10,10,0), Quaternion.identity);
            fairy.offsetPos = offset;
            yield return Calc.WaitForFrames(interval);
        }
    }

    IEnumerator<float> GenerateOpeningFairy(Vector3 offset, int count, int interval,bool isReverse) {
        for (int i = 0; i < count; i++) {
            FairyMovement fairy = Instantiate(i % 2 == 0 ? st0F0Prefab : st0F1Prefab, new Vector3(-10, 10, 0), Quaternion.identity);
            fairy.offsetPos = offset;
            fairy.isPathReverse = isReverse;
            yield return Calc.WaitForFrames(interval);
        }
        
    }
    private void Update() {
        if(timer == 180) {
            Timing.RunCoroutine(GenerateOpeningFairy(Vector3.zero, 15, 30,false));
        }

        if (timer == 180 + 120 + 30 * 15) {
            Timing.RunCoroutine(GenerateOpeningFairy(Vector3.zero, 15, 30,true));
        }
        
        if (timer == 1500) {
            stageTitle.gameObject.SetActive(true);
        }

        if (timer == 1800) {
            Instantiate(st0F2Prefab);
            Timing.RunCoroutine(GenerateFairy(st0F3Prefab, Vector3.zero, 10, 40));
        }
        
        if (timer == 1800 + 10 * 30) {
            Timing.RunCoroutine(GenerateFairy(st0F4Prefab, Vector3.zero, 10, 40));
        }

        if (timer == 2800) {
            Timing.RunCoroutine(GenerateFairy(st0F5Prefab, Vector3.zero, 1, 10));
        }
        
        timer++;
    }
}
