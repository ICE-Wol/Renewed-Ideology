using System;
using _Scripts.Tools;
using TMPro;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Lunatic,
    Overdrive,
    Reverie
}

public static class DifficultyExtensions
{
    public static Difficulty Next(this Difficulty current)
    {
        int currentIndex = (int)current;
        int nextIndex = (currentIndex + 1) % Enum.GetValues(typeof(Difficulty)).Length;
        return (Difficulty)nextIndex;
    }
    public static Difficulty Prev(this Difficulty current)
    {
        int currentIndex = (int)current;
        int nextIndex = (currentIndex - 1) % Enum.GetValues(typeof(Difficulty)).Length;
        if (currentIndex == 0) nextIndex = (int)Difficulty.Reverie;
        return (Difficulty)nextIndex;
        
    }
}

public class DiffSelectManager : MonoBehaviour
{
    public Difficulty curDiff;
    
    [Header("State")]
    public bool selecting;
    public bool selected;
    public DiffMenuPage currentPage => diffMenuPages[(int)curDiff];
    public Difficulty selectedDiff => curDiff;
    
    
    [Header("Show/Hide")]
    public float curAlpha;
    public float tarAlpha;
    
    
    [Header("Settings")]
    public DiffMenuPage[] diffMenuPages;
    public TMP_Text[] diffTitles;
    public SpriteRenderer diffBottom;
    
    
    
    /// <summary>
    /// true表示在选择难度
    /// </summary>
    private void Start() {
        Application.targetFrameRate = 60;
        TitleAndBottomAppear(true);
        selecting = true;
    }
    
    
    public void SetAllDiffInvisible() {
        foreach (var p in diffMenuPages) {
            p.ResetValue();
        }
    }
    
    public Difficulty GetCurDifficulty() {
        return curDiff;
    }

    public void TitleAndBottomAppear(bool isAppear) {
        tarAlpha = isAppear ? 1f : 0f;
    }
    
    private void Update() {
        if (MenuManager.Manager.menuState == MenuState.DifficultySelect
            || MenuManager.Manager.menuState == MenuState.PlayerSelect) {
            // 表现.
            if (selecting) {
                SetAllDiffInvisible();
                if (!selected) // 放上去.
                {
                    TitleAndBottomAppear(true);
                    if (currentPage) currentPage.SetAppearValue();
                }
                else // 放下来.
                {
                    currentPage.SetClonedValue();
                    TitleAndBottomAppear(false);
                }
            }
            else {
                if (selected) // 选择了.
                {
                    TitleAndBottomAppear(false);
                    currentPage.SetClonedValue();
                }
                else {
                    SetAllDiffInvisible();
                }
            }
        }

        // 显示和消失.
        curAlpha.ApproachRef(tarAlpha, 8f);
        foreach (var t in diffTitles) {
            t.color = t.color.SetAlpha(curAlpha);
        }
        diffBottom.color = diffBottom.color.SetAlpha(curAlpha);
    }
}


// using System;
// using _Scripts.Tools;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// public enum Difficulty
// {
//     Easy,
//     Normal,
//     Hard,
//     Lunatic,
//     Overdrive,
//     Reverie
// }
//
// public class DiffSelectManager : MonoBehaviour
// {
//     public DiffMenuPage[] diffMenuPages;
//     public Difficulty curDiff;
//     public Difficulty selectedDiff;
//     
//     public TMP_Text[] diffTitles;
//     public SpriteRenderer diffBottom;
//     public float curAlpha;
//     public float tarAlpha;
//
//     public DiffMenuPage moveDownPage;
//
//     /// <summary>
//     /// true表示在选择难度
//     /// </summary>
//     public bool isSelecting;
//     private void Start() {
//         Application.targetFrameRate = 60;
//         isSelecting = true;
//         TitleAndBottomAppear(true);
//         UpdateDiffPage();
//     }
//     
//     public Difficulty GetCurDifficulty() {
//         return curDiff;
//     }
//     
//
//     void UpdateDiffPage() {
//         var newDiffNum = (int)curDiff;
//         if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)) {
//             newDiffNum = (int)curDiff - 1;
//             if (newDiffNum < 0) newDiffNum = 5;
//         }
//         if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) {
//             newDiffNum = (int)curDiff + 1;
//             if (newDiffNum > 5) newDiffNum = 0;
//         }
//         curDiff = (Difficulty)newDiffNum;
//         
//         //curDiff = (Difficulty)Mathf.Repeat((int)curDiff + 1, 6);
//         //0 1 2 3 4 5
//         
//         foreach (var p in diffMenuPages) {
//             if (curDiff == p.diff) p.SetAppearValue();
//             else p.ResetValue();
//         }
//     }
//     
//     public void SetAllDiffInvisible() {
//         foreach (var p in diffMenuPages) {
//             p.ResetValue();
//         }
//     }
//
//     public void TitleAndBottomAppear(bool isAppear) {
//         tarAlpha = isAppear ? 1f : 0f;
//     }
//     
//     private void Update() { 
//         //if(MenuManager.Manager.menuState != MenuState.DifficultySelect) return;
//         if (isSelecting && MenuManager.Manager.menuState == MenuState.DifficultySelect)
//             UpdateDiffPage();
//         if (isSelecting && Input.GetKeyDown(KeyCode.Z)) {
//             isSelecting = false;
//             moveDownPage = diffMenuPages[(int)curDiff];
//             moveDownPage.SetClonedValue();
//             selectedDiff = curDiff;
//             TitleAndBottomAppear(false);
//         }
//         if(isSelecting && Input.GetKeyDown(KeyCode.X)) {
//             TitleAndBottomAppear(false);
//             SetAllDiffInvisible();
//         }
//         if (!isSelecting && Input.GetKeyDown(KeyCode.X)) {
//             isSelecting = true;
//             foreach (var p in diffMenuPages) {
//                 if (p.diff == selectedDiff) {
//                     p.ResetValue();
//                 }
//             }
//             TitleAndBottomAppear(true);
//         }
//         
//         curAlpha.ApproachRef(tarAlpha, 8f);
//         foreach (var t in diffTitles) {
//             t.color = t.color.SetAlpha(curAlpha);
//         }
//         diffBottom.color = diffBottom.color.SetAlpha(curAlpha);
//     }
// }
