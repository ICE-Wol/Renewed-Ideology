using System;
using _Scripts;
using _Scripts.Player;
using UnityEngine;

public class PracticeManager : MonoBehaviour
{
    public static PracticeManager instance;

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    public SpellPracticeStartInfo spellPracticeStartInfo;

    public void Start() {
        if (spellPracticeStartInfo.isSpellPracticeMode) {
            var boss = BossManager.instance.GenerateBossWithSpellCardInfo(spellPracticeStartInfo.bossName,
                spellPracticeStartInfo.spellCardInfo);
            PlayerCtrl.instance.state.life = 0;
            PlayerCtrl.instance.state.bomb = 0;
            //不要用等号赋值，否则会导致boss的spellCardInfos和spellPracticeStartInfo的spellCardInfo指向同一个地址
            //这样会导致boss的spellCardInfos被修改时，spellPracticeStartInfo的spellCardInfo也会被修改
            //除非你知道自己在做什么，否则不要这样做
        }
    }
}
