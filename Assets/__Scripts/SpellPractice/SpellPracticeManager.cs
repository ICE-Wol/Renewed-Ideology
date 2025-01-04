using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpellPracticeManager : MonoBehaviour
{
    [Serializable]
    public struct SpellPracticeData
    {
        public string characterName;
        public Difficulty difficulty;
        public SpellCardInfo[] spellCardInfos;
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

    public SpellPracticeData[] spellPracticeData;

    /*试着做更加高级的抽象
     例如一个界面有打开和关闭两个状态
     就抽象出打开和关闭两个状态
     改的时候建立在这个双状态的逻辑的基础上
    */

    public int currentCharacterIndex;
    public int currentSpellIndex;

    /// <summary>
    /// true 为角色选单，false 为符卡选单
    /// </summary>
    public bool isCharacterMode;

    /*
     * 左右切换角色选单/符卡选单
     * 若在角色选单，上下切换角色
     * 若在符卡选单，上下切换符卡
     */

    /// <summary>
    /// 存储角色序号，只有角色序号变化时才更新符卡选单
    /// </summary>
    private int _oldCharacterIndex = 0;

    public void SwitchMode() {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
            isCharacterMode = !isCharacterMode;
            nameMenu.isOperable = isCharacterMode;

            if (!isCharacterMode) {
                for (int i = 0; i < spellCardMenus.Count; i++) {
                    spellCardMenus[i].isOperable = i == nameMenu.centerElementIndex;
                }
            }
        }

        if (nameMenu.centerElementIndex != _oldCharacterIndex) {
            for (int i = 0; i < spellCardMenus.Count; i++) {
                spellCardMenus[i].isOperable = false;
                if (i == nameMenu.centerElementIndex) {
                    spellCardMenus[i].UpdateElementsTargetValues();
                }
                else {
                    spellCardMenus[i].DisappearElements();
                }
            }
            _oldCharacterIndex = nameMenu.centerElementIndex;
        }
    }



// if(isCharacterMode) {
        //     if(Input.GetKeyDown(KeyCode.DownArrow)) {
        //         currentCharacterIndex = (currentCharacterIndex + 1) % spellPracticeData.Length;
        //     } else if(Input.GetKeyDown(KeyCode.UpArrow)) {
        //         currentCharacterIndex = (currentCharacterIndex - 1 + spellPracticeData.Length) % spellPracticeData.Length;
        //     }
        // } else {
        //     int curSpellNum = spellPracticeData[currentCharacterIndex].spellCardInfos.Length;
        //     if(Input.GetKeyDown(KeyCode.DownArrow)) {
        //         currentSpellIndex = (currentSpellIndex + 1) % curSpellNum;
        //     } else if(Input.GetKeyDown(KeyCode.UpArrow)) {
        //         currentSpellIndex = (currentSpellIndex - 1 + curSpellNum) % curSpellNum;
        //     }
        // }

    public SCPMenuManager scpMenuManagerPrefab;
    public SCPMenuManager nameMenu;
    public List<SCPMenuManager> spellCardMenus;
    private void Start() {
        currentCharacterIndex = 0;
        currentSpellIndex = 0;
        isCharacterMode = true;
        
        InitNameMenu();
    }

    public void InitNameMenu() {
        spellCardMenus = new List<SCPMenuManager>();

        var characterNames = new List<TMP_Text>();
        for (int i = 0; i < spellPracticeData.Length; i++) {
            //新建角色名TMP对象，并加入列表
            var go = Instantiate(characterNamePrefab, characterNameParent);
            //go.transform.localPosition = new Vector3(0, -i * characterNameWidth, 0);
            go.name = spellPracticeData[i].characterName;
            go.text = spellPracticeData[i].characterName;
            characterNames.Add(go);
            
            var spellCards = new List<TMP_Text>();
            for (int j = 0; j < spellPracticeData[i].spellCardInfos.Length; j++) {
                var spell = spellPracticeData[i].spellCardInfos[j];
                var sc = Instantiate(spellCardPrefab, spellCardParent);
                //sc.transform.localPosition = new Vector3(0, -j * spellCardWidth, 0);
                if (spell.isSpellCard) {
                    sc.text = spell.spellName + " " + spell.cardName;
                } else {
                    sc.text = "Non-Spell " + spell.spellName;
                }
                spellCards.Add(sc);
            }
            var scMenu = Instantiate(scpMenuManagerPrefab,transform);
            scMenu.name = spellPracticeData[i].characterName + "MenuManager";
            spellCardMenus.Add(scMenu);
            scMenu.InitElements(spellCards);
            scMenu.isOperable = false;
            
            //只显示第一个角色的符卡选单
            if (i != 0) scMenu.DisappearElements();
            

        }
        
        //使用列表初始化角色名菜单
        nameMenu = Instantiate(scpMenuManagerPrefab,transform);
        nameMenu.name = "NameMenuManager";
        nameMenu.InitElements(characterNames);
        nameMenu.isOperable = true;
    }

    public void Update() {
        SwitchMode();
    }
    

    [Header("Character Name")]
    public Transform characterNameParent;
    public TMP_Text characterNamePrefab;
    public int characterNameWidth;

    [Header("Spell Card")]
    public Transform spellCardParent;
    public TMP_Text spellCardPrefab;
    public int spellCardWidth;
    // public void InitCharacterName() {
    //     characterNames = new TMP_Text[spellPracticeData.Length];
    //     for (int i = 0; i < spellPracticeData.Length; i++) {
    //         var go = Instantiate(characterNamePrefab, characterNameParent);
    //         go.transform.localPosition = new Vector3(0, -i * characterNameWidth, 0);
    //         go.name = spellPracticeData[i].characterName;
    //         go.text = spellPracticeData[i].characterName;
    //         characterNames[i] = go;
    //     }
    //     
    //     /*
    //      * 视觉上分开的obj在场景树里也要分开
    //      */
    //     for (int i = 0; i < spellPracticeData.Length; i++) {
    //         var go = new GameObject();
    //         go.name = spellPracticeData[i].characterName;
    //         go.transform.SetParent(spellCardParent);
    //         go.transform.localPosition = new Vector3(0, 0, 0);
    //         
    //         for(int j = 0; j < spellPracticeData[i].spellCardInfos.Length; j++) {
    //             var spell = spellPracticeData[i].spellCardInfos[j];
    //             
    //             //todo 后续改成符卡名专门prefab
    //             var sc = Instantiate(characterNamePrefab, go.transform);
    //             
    //             sc.transform.localPosition = new Vector3(0, -j * spellCardWidth, 0);
    //             if(spell.isSpellCard) {
    //                 sc.text = spell.spellName + " " + spell.cardName;
    //             } else {
    //                 sc.text = "Non-Spell " + spell.spellName;
    //             }
    //         }
    //         
    //         go.SetActive(false);
    //     }
    //     spellCardParent.GetChild(0).gameObject.SetActive(true);
    // }
    //
    // public void UpdateCharacterName() {
    //     for (int i = 0; i < spellPracticeData.Length; i++) {
    //         characterNames[i].color = i == currentCharacterIndex ? Color.red : Color.white;
    //     }
    // }
    //
    // public void UpdateSpellCard() {
    //     for (int i = 0; i < spellPracticeData.Length; i++) {
    //     }
    // }
    //
    //
    // public void Update() {
    //     SwitchMode();
    //     UpdateCharacterName();
    // }
    
    /// <summary>
    /// 调试用的GUI
    /// </summary>
    public void OnGUI() {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 24; // 设置字体大小为24（根据需要调整）
        if (isCharacterMode) {
            GUI.Label(new Rect(10, 10, 500, 100), "Character Number " + currentCharacterIndex, labelStyle);
            GUI.Label(new Rect(10, 30, 500, 100), spellPracticeData[currentCharacterIndex].characterName, labelStyle);
        } else {
            GUI.Label(new Rect(10, 10, 500, 100), "Spell Number " + currentSpellIndex, labelStyle);
            if(spellPracticeData[currentCharacterIndex].spellCardInfos.Length == 0) {
                GUI.Label(new Rect(10, 30, 500, 100), "No Spell Card", labelStyle);
            }
            else {
                var spell = spellPracticeData[currentCharacterIndex].spellCardInfos[currentSpellIndex];
                if (spellPracticeData[currentCharacterIndex].spellCardInfos[currentSpellIndex].isSpellCard) {
                    GUI.Label(new Rect(10, 30, 500, 100), spell.spellName + " " + spell.cardName, labelStyle);
                } else {
                    GUI.Label(new Rect(10, 30, 500, 100), "Non-Spell" + spell.spellName, labelStyle);
                }
            }
        }

    }
}
