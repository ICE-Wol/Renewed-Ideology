using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Item;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardInfo", menuName = "SpellCardInfo", order = 0)]
[Serializable]
public class SpellCardInfo : ScriptableObject {
    public BulletGenerator bulletGenerator;
    public bool isSpellCard;
    public string spellName;
    public string cardName;
    
    public int maxHealth;
    public float maxTime;
    
    public int maxBonusPoints;

    //todo
    [Header("使用默认掉落物列表：\n" +
            "非符：收取掉一雷碎片，默认不掉落\n" +
            "符卡：收取掉一残碎片，默认掉50p")]
    public bool useDefaultItems;

    public ItemSpawnEntry[] itemSequence;
    public ItemSpawnEntry[] bonusItemSequence;

    public bool hasFixedPos;
    public bool hasInitPos;
    public Vector3 initPos;
}
