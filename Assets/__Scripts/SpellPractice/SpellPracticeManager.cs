using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tools;
using MEC;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpellPracticeManager : MonoBehaviour
{
    public BossList bossList;
    public SpellPracticeStartInfo spellPracticeStartInfo;
    public SCPMenuManager scpMenuManagerPrefab;

    public enum SpellPracticeMenuType
    {
        Character,
        Difficulty,
        SpellCard
    }
    
    public List<SpellPracticeData> spellPracticeData;

    [Serializable]
    public struct SpellPracticeData
    {
        public string characterName;
        
        [Serializable]
        public struct SpellCardSetOnlyWithDifficulty
        {
            //todo 压缩成字典
            public Difficulty difficulty;
            public List<SpellCardInfo> spellCardInfos;
        }
        
        public List<SpellCardSetOnlyWithDifficulty> spellCardSets;
    }
    
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Lunatic,
        Overdrive,
        Extra,
        Reverie,
        Demo
    }

    /*试着做更加高级的抽象
     例如一个界面有打开和关闭两个状态
     就抽象出打开和关闭两个状态
     改的时候建立在这个双状态的逻辑的基础上
    */

    public int oldCharacterIndex;
    public Difficulty oldDifficultyIndex;
    
    
    public SpellPracticeMenuType spellPracticeMenuType;
    public SCPMenuManager[] scpMenuManagers;
    
    /*
     * 左右切换角色选单/难度选单/符卡选单
     * 上下切换具体元素
     */

    public void SwitchMode() {
        //getKeyDown的判断在一帧中是持续的，所以一帧内多次调用结果是一致的，不会“吞掉”按键
        //左右切换角色选单、难度选单、符卡选单
        //todo 改成一个叫prev和next的字典，然后用字典来切换
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            spellPracticeMenuType = (SpellPracticeMenuType)(((int)spellPracticeMenuType - 1 + 3) % 3);
            SetBlinkingMenu(spellPracticeMenuType);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            spellPracticeMenuType = (SpellPracticeMenuType)(((int)spellPracticeMenuType + 1) % 3);
            SetBlinkingMenu(spellPracticeMenuType);
        }

        //上下切换选单中的元素
        //用父级控制器控制菜单元素上下移动而非菜单元素自己控制
        //避免频繁改变菜单元素的可操作性
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            scpMenuManagers[(int)spellPracticeMenuType].MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            scpMenuManagers[(int)spellPracticeMenuType].MoveUp();
        }

        switch (spellPracticeMenuType) {
            case SpellPracticeMenuType.Character:
                if (oldCharacterIndex != scpMenuManagers[(int)SpellPracticeMenuType.Character].centerElementIndex) {
                    oldCharacterIndex = scpMenuManagers[(int)SpellPracticeMenuType.Character].centerElementIndex;
                    scpMenuManagers[(int)SpellPracticeMenuType.Difficulty] =
                        GenerateDifficultyMenuFromCharacter(oldCharacterIndex);
                    scpMenuManagers[(int)SpellPracticeMenuType.SpellCard] =
                        GenerateSpellCardMenuFromCharacterAndDifficulty(oldCharacterIndex);
                }
                
                if(Input.GetKeyDown(KeyCode.Z)) {
                    spellPracticeMenuType = SpellPracticeMenuType.Difficulty;
                    SetBlinkingMenu(spellPracticeMenuType);
                }
                if(Input.GetKeyDown(KeyCode.X)) {
                    SceneManager.LoadScene(0);
                }

                break;
            case SpellPracticeMenuType.Difficulty:
                /*
                 * 注意并不是每个boss在所有难度都有符卡
                 * 因此符卡的索引并不是单纯按顺序来的
                 * 因此有了以下的调用
                 *
                 * 目标是获取难度
                 * 从角色的难度符卡集出发
                 * 找到菜单元素顺序在符卡集中对应的难度枚举值
                 * 
                 */
                var characterData = spellPracticeData[oldCharacterIndex];
                var diffMenu = scpMenuManagers[(int)SpellPracticeMenuType.Difficulty];
                var diff = characterData.spellCardSets[diffMenu.centerElementIndex].difficulty;
                if (oldDifficultyIndex != diff) {
                    oldDifficultyIndex = diff;
                    scpMenuManagers[(int)SpellPracticeMenuType.SpellCard] =
                        GenerateSpellCardMenuFromCharacterAndDifficulty(oldCharacterIndex,
                            oldDifficultyIndex);
                }
                
                if(Input.GetKeyDown(KeyCode.Z)) {
                    spellPracticeMenuType = SpellPracticeMenuType.SpellCard;
                    SetBlinkingMenu(spellPracticeMenuType);
                }
                if(Input.GetKeyDown(KeyCode.X)) {
                    spellPracticeMenuType = SpellPracticeMenuType.Character;
                    SetBlinkingMenu(spellPracticeMenuType);
                }

                break;
            case SpellPracticeMenuType.SpellCard:
                if (Input.GetKeyDown(KeyCode.Z)) {
                    spellPracticeStartInfo.isSpellPracticeMode = true;
                    spellPracticeStartInfo.bossName = spellPracticeData[oldCharacterIndex].characterName;
                    
                    var centerElementIndex = scpMenuManagers[(int)SpellPracticeMenuType.SpellCard].centerElementIndex;

                    //使用前记得清空
                    spellPracticeStartInfo.spellCardInfo.Clear();
                    spellPracticeStartInfo.spellCardInfo.Add(spellPracticeData[oldCharacterIndex].spellCardSets
                        .Find(x => x.difficulty == oldDifficultyIndex).spellCardInfos[centerElementIndex]);
                    SceneManager.LoadScene("GameScene");
                }
                if(Input.GetKeyDown(KeyCode.X)) {
                    spellPracticeMenuType = SpellPracticeMenuType.Difficulty;
                    SetBlinkingMenu(spellPracticeMenuType);
                }
                break;
        }
    }

    
    private void Start() {
        scpMenuManagers = new SCPMenuManager[3];
        InitSpellPracticeData();
        InitMenus();
    }
    
    public void Update() {
        SwitchMode();
        
        //由于插值跟随的原因指针会抖动，所以暂时不用
        //cursor.transform.position = cursor.transform.position.ApproachValue(tarPos, 32f);
        //tarPos = scpMenuManagers[(int)spellPracticeMenuType].elements[scpMenuManagers[(int)spellPracticeMenuType].centerElementIndex].tmpComp.rectTransform.position;
    }

    //public Image cursor;
    //public Vector3 tarPos;
    
    

    [Header("Character Name")]
    public Transform characterNameParent;
    public TMP_Text characterNamePrefab;

    [Header("Spell Card")]
    public Transform spellCardParent;
    public TMP_Text spellCardPrefab;
    
    [Header("Difficulty")]
    public Transform difficultyParent;

    /// <summary>
    /// 将BossList中的数据转换为SpellPracticeData
    /// 其中，保留难度索引，去除位置索引
    /// </summary>
    public void InitSpellPracticeData() {
        spellPracticeData = new List<SpellPracticeData>();
        foreach (var boss in bossList.bossInfos) {
            var spd = new SpellPracticeData();
            spd.characterName = boss.bossName;
            spd.spellCardSets = new List<SpellPracticeData.SpellCardSetOnlyWithDifficulty>();
            
            //遍历带有关卡位置索引的符卡集合
            foreach (var scs in boss.spellCardSets) {
                //遍历只有难度索引的符卡集合
                //如果难度相同，将符卡集合合并
                bool hasDifficulty = false;
                foreach (var t in spd.spellCardSets) {
                    if(t.difficulty == scs.difficulty) {
                        t.spellCardInfos.AddRange(scs.spellCardInfos);
                        hasDifficulty = true;
                        break;
                    }
                }

                //如果没有相同难度的符卡集合，新建一个
                if (!hasDifficulty) {
                    spd.spellCardSets.Add(new SpellPracticeData.SpellCardSetOnlyWithDifficulty() {
                        difficulty = scs.difficulty,
                        spellCardInfos = new List<SpellCardInfo>(scs.spellCardInfos)
                    });
                }
            }
            
            //将新建的spd加入列表
            spellPracticeData.Add(spd);
        }
    }
    
    
    public void SetBlinkingMenu(SpellPracticeMenuType type) {
        for (int i = 0; i < scpMenuManagers.Length; i++) {
            scpMenuManagers[i].isBlinking = i == (int)type;
            scpMenuManagers[i].timer = 0;
        }
    }

    public void InitMenus() {
        scpMenuManagers[(int)SpellPracticeMenuType.Character] = InitNameMenu();
        scpMenuManagers[(int)SpellPracticeMenuType.Difficulty] = GenerateDifficultyMenuFromCharacter(0);
        scpMenuManagers[(int)SpellPracticeMenuType.SpellCard] = GenerateSpellCardMenuFromCharacterAndDifficulty(0);

        SetBlinkingMenu(SpellPracticeMenuType.Character);
        oldDifficultyIndex = spellPracticeData[scpMenuManagers[(int)SpellPracticeMenuType.Character].centerElementIndex]
            .spellCardSets[0].difficulty;
    }

    public SCPMenuManager GenerateSpellCardMenuFromCharacterAndDifficulty(int characterIndex, Difficulty difficulty) {
        if(scpMenuManagers[(int)SpellPracticeMenuType.SpellCard] != null)
            Timing.RunCoroutine(scpMenuManagers[(int)SpellPracticeMenuType.SpellCard].DestroySelf());
        
        var spellCardInfos = spellPracticeData[characterIndex].spellCardSets.Find(x => x.difficulty == difficulty).spellCardInfos;
        var spellCardTexts = new List<TMP_Text>();
        foreach (var spellCardInfo in spellCardInfos) {
            var go = Instantiate(spellCardPrefab, spellCardParent);
            if(spellCardInfo.isSpellCard)
                go.text = spellCardInfo.spellName + " " + spellCardInfo.cardName;
            else go.text = "Non-Spell" + spellCardInfo.spellName;
            spellCardTexts.Add(go);
        }
        
        var scMenu = Instantiate(scpMenuManagerPrefab,transform);
        scMenu.name = spellPracticeData[characterIndex].characterName + " " + difficulty + " " + "MenuManager";
        scMenu.InitElements(spellCardTexts);
        //scMenu.isOperable = false;

        return scMenu;
    }
    
    /// <summary>
    /// 无diff参数时默认为第一个难度
    /// </summary>
    /// <param name="characterIndex"></param>
    /// <returns></returns>
    public SCPMenuManager GenerateSpellCardMenuFromCharacterAndDifficulty(int characterIndex) {
        if(scpMenuManagers[(int)SpellPracticeMenuType.SpellCard] != null)
            Timing.RunCoroutine(scpMenuManagers[(int)SpellPracticeMenuType.SpellCard].DestroySelf());
        
        var spellCardInfos = spellPracticeData[characterIndex].spellCardSets[0].spellCardInfos;
        var spellCardTexts = new List<TMP_Text>();
        foreach (var spellCardInfo in spellCardInfos) {
            var go = Instantiate(spellCardPrefab, spellCardParent);
            if(spellCardInfo.isSpellCard)
                go.text = spellCardInfo.spellName + " " + spellCardInfo.cardName;
            else go.text = "Non-Spell" + spellCardInfo.spellName;
            spellCardTexts.Add(go);
        }
        
        var scMenu = Instantiate(scpMenuManagerPrefab,transform);
        scMenu.name = spellPracticeData[characterIndex].characterName + " " + spellPracticeData[characterIndex].spellCardSets[0].difficulty + " " + "MenuManager";
        scMenu.InitElements(spellCardTexts);
        //scMenu.isOperable = false;

        return scMenu;
    }
    
    public SCPMenuManager GenerateDifficultyMenuFromCharacter(int characterIndex) {
        if(scpMenuManagers[(int)SpellPracticeMenuType.Difficulty] != null)
            Timing.RunCoroutine(scpMenuManagers[(int)SpellPracticeMenuType.Difficulty].DestroySelf());
        
        var difficulties = spellPracticeData[characterIndex].spellCardSets;
        var difficultyTexts = new List<TMP_Text>();
        foreach (var difficulty in difficulties) {
            var go = Instantiate(spellCardPrefab, difficultyParent);
            go.text = difficulty.difficulty.ToString();
            difficultyTexts.Add(go);
        }
        
        var diffMenu = Instantiate(scpMenuManagerPrefab,transform);
        diffMenu.name = spellPracticeData[characterIndex].characterName + "DiffMenuManager";
        diffMenu.InitElements(difficultyTexts);
        //diffMenu.isOperable = false;

        return diffMenu;
    }
    
    public SCPMenuManager InitNameMenu() {
        var characterNames = new List<TMP_Text>();
        for (int i = 0; i < spellPracticeData.Count; i++) {
            //新建角色名TMP对象，并加入列表
            //把这部分逻辑交给SCPMenuManager的初始化函数来做，直接传stringList就行了
            var go = Instantiate(characterNamePrefab, characterNameParent);
            //go.transform.localPosition = new Vector3(0, -i * characterNameWidth, 0);
            go.name = spellPracticeData[i].characterName;
            go.text = spellPracticeData[i].characterName;
            characterNames.Add(go);
        }

        //使用Linq表达式简化上述代码
        //var characterNames = spellPracticeData.Select(x => x.characterName).ToList();
        
        //使用列表初始化角色名菜单
        var nameMenu = Instantiate(scpMenuManagerPrefab,transform);
        nameMenu.name = "NameMenuManager";
        nameMenu.InitElements(characterNames);//, characterNameParent);
        //nameMenu.isOperable = true;

        return nameMenu;
    }
}
