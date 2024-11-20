using System.Collections;
using System.Collections.Generic;
using _Scripts.Item;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardInfo", menuName = "SpellCardInfo", order = 0)]
public class SCInfo : ScriptableObject {
    public bool isSpellCard;
    public string spellName;
    public string cardName;
    
    public int maxHealth;
    public float maxTime;
    
    public int maxBonusPoints;

    public ItemSpawnEntry[] itemSequence;

    public bool hasFixedPos;
    public bool hasInitPos;
    public Vector3 initPos;
}
