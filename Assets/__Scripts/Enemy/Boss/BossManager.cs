using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public BossList bossList;
    public BossCtrl curBoss;

    public BossCtrl GenerateBossWithSpellCardSet(string bossName, SpellPracticeManager.Difficulty difficulty,
        int stageIndex, int bossOrder) {
        foreach (var bossInfo in bossList.bossInfos) {
            if (bossInfo.bossName == bossName) {
                foreach (var spellCardSet in bossInfo.spellCardSets) {
                    if (spellCardSet.difficulty == difficulty && spellCardSet.stageIndex == stageIndex &&
                        spellCardSet.bossOrder == bossOrder) {
                        var go = Instantiate(bossInfo.bossPrefab, BossManager.instance.transform);
                        go.spellCardInfos = new List<SpellCardInfo>();
                        go.spellCardInfos.AddRange(spellCardSet.spellCardInfos);
                        go.maxScNumber = spellCardSet.spellCardInfos.Count;
                        return go;
                    }
                }
            }
        }

        Debug.LogError("No corresponding Boss");
        return null;
    }

    public BossCtrl GenerateBossWithSpellCardInfo(string bossName, List<SpellCardInfo> info) {
        foreach (var bossInfo in bossList.bossInfos) {
            if (bossInfo.bossName == bossName) {
                var go = Instantiate(bossInfo.bossPrefab, instance.transform);
                //boss的spellCardInfos是空的
                //虽然会被自动序列化，但是保险起见还是新建一个吧
                //所以要手动初始化
                go.spellCardInfos = new List<SpellCardInfo>();
                go.spellCardInfos.AddRange(info);
                go.maxScNumber = go.spellCardInfos.Count;
                return go;
            }
        }

        Debug.LogError("No corresponding Boss");
        return null;
    }

    public BossCtrl GenerateBossWithSpellCardIndex(string bossName, SpellPracticeManager.Difficulty difficulty,
        int stageIndex, int bossOrder, int cardNum) {
        foreach (var bossInfo in bossList.bossInfos) {
            if (bossInfo.bossName == bossName) {
                foreach (var spellCardSet in bossInfo.spellCardSets) {
                    if (spellCardSet.difficulty == difficulty && spellCardSet.stageIndex == stageIndex &&
                        spellCardSet.bossOrder == bossOrder) {
                        if (cardNum < spellCardSet.spellCardInfos.Count) {
                            var go = Instantiate(bossInfo.bossPrefab, BossManager.instance.transform);
                            go.spellCardInfos = new List<SpellCardInfo> { spellCardSet.spellCardInfos[cardNum] };
                            go.maxScNumber = 1;
                            return go;
                        }
                        else {
                            Debug.LogError("CardNum is out of range");
                            return null;
                        }

                    }
                }
            }
        }

        Debug.LogError("No corresponding Boss");
        return null;
    }

    bool _isPaused = false;
    public void Update() {
        if (PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode && curBoss == null && !_isPaused) {
            _isPaused = true;
            Timing.RunCoroutine(PauseManager.instance.TriggerEndPause(true));
        }
    }
}
