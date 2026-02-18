using System;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimeLineStage0 : MonoBehaviour
{
    public int timer;
    public bool startFromBoss;
    
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
    public FairyMovement[] st0F5Prefab;
    public int[] st0F5Interval;
    
    [Header("3250 散花大妖精")]
    public FairyMovement st0F6Prefab;
    
    [Header("4000 开花藤蔓")]
    public FairyMovement st0F7Prefab;
    public FairyMovement st0F9Prefab;
    
    [Header("4500 开花藤蔓")]
    public FairyMovement st0F8Prefab;
    public FairyMovement st0F10Prefab;

    [Header("5000 道中boss")] 
    public BossCtrl st0MidBoss;
    
    [Header("关底boss")] 
    public BossCtrl st0Boss;

    IEnumerator<float> GenerateFairy(FairyMovement fairyPrefab, Vector3 offset, int count, int interval)
    {
        for (int i = 0; i < count; i++)
        {
            FairyMovement fairy = Instantiate(fairyPrefab, new Vector3(-10,10,0), Quaternion.identity);
            fairy.offsetPos = offset;
            yield return Calc.WaitForFrames(interval);
        }
    }
    
    IEnumerator<float> GenerateFairy(FairyMovement[] fairyPrefabs, int[] interval)
    {
        for (int i = 0; i < fairyPrefabs.Length; i++)
        {
            FairyMovement fairy = Instantiate(fairyPrefabs[i], new Vector3(-10,10,0), Quaternion.identity);
            yield return Calc.WaitForFrames(interval[i]);
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
    
    IEnumerator<float> GenerateStemFairy(bool type,int count, int interval) {
        for (int i = 0; i < count; i++) {
            FairyMovement fairy = Instantiate(type ? st0F7Prefab : st0F8Prefab, new Vector3(-10, 10, 0),
                Quaternion.identity);
            fairy.GetComponent<St0F7>().stemLayer = i;
            yield return Calc.WaitForFrames(interval);
        }
    }
    
    IEnumerator<float> GenerateGhost(bool type,int count, int interval) {
        for (int i = 0; i < count; i++) {
            FairyMovement ghost = Instantiate(type ? st0F9Prefab : st0F10Prefab, new Vector3(-10, 10, 0), Quaternion.identity);
            ghost.offsetPos = Random.insideUnitCircle;
            yield return Calc.WaitForFrames(interval);
        }
    }

    private bool hasFinalBossActivated = false;
    IEnumerator<float> GenStageBoss(int waitFrame) {
        hasFinalBossActivated = true;
        //yield return Calc.WaitForFrames(waitFrame);
        st0Boss = BossManager.instance.GenerateBossWithSpellCardSet("东风谷早苗",
            SpellPracticeManager.Difficulty.Demo, 0, 1);
        yield return 0;
    }

    private void Start() {
        if (startFromBoss) timer = 5000;
    }

    private void Update() {
        
        if(PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode)
            return;
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
            Timing.RunCoroutine(GenerateFairy(st0F5Prefab, st0F5Interval));
        }

        if (timer == 2800 + 450) {
            Instantiate(st0F6Prefab);
        }
        
        if (timer == 4000) {
            Timing.RunCoroutine(GenerateStemFairy(true, 3, 60));
            Timing.RunCoroutine(GenerateGhost(true, 5, 100));
        }
        
        if (timer == 4500) {
            Timing.RunCoroutine(GenerateStemFairy(false, 3, 60));
            Timing.RunCoroutine(GenerateGhost(false, 5, 100));
        }
        
        if (timer == 5000) {
            st0MidBoss =
                BossManager.instance.GenerateBossWithSpellCardSet("琪露诺",
                    SpellPracticeManager.Difficulty.Demo, 0, 0);
        }

        if (timer > 5000 && st0MidBoss == null && st0Boss == null && !hasFinalBossActivated) {
            Timing.RunCoroutine(GenStageBoss(100));
        }
        
        if(timer > 5103 && hasFinalBossActivated && st0Boss == null) {
            Timing.RunCoroutine(PauseManager.instance.TriggerEndPause(true));
        }
        
        timer++;
    }
}
