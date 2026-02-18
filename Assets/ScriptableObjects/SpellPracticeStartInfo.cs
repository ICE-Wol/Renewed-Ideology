using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellPracticeStartInfo", menuName = "SpellPracticeStartInfo", order = 0)]
public class SpellPracticeStartInfo : ScriptableObject
{
    public bool isSpellPracticeMode;
    public string bossName;
    public List<SpellCardInfo> spellCardInfo;
}
