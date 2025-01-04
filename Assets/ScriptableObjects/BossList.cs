using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BossList", menuName = "BossList", order = 0)]
[Serializable]
public class BossList : ScriptableObject
{
    /*
     * boss是可以释放一系列符卡的集合
     * 以下情况时boss会释放不同的符卡
     * 难度
     * 位置（道中/关底）（不同面数）
     */
    [Serializable]
    public struct BossInfo
    {
        public string bossName;
        public SpellCardSet[] spellCardSets;

        [Serializable]
        public struct SpellCardSet
        {
            public SpellPracticeManager.Difficulty difficulty;
            public int stageIndex;
            public int bossOrder;
            public SpellCardInfo[] spellCardInfos;
        }
    }
    
    public BossInfo[] bossInfos;
}
