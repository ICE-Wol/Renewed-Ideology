using System;
using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum Player
{
    Reimu,
    Marisa
}

public class PlayerSelectManager : MonoBehaviour
{
    public PlayerMenuPage[] playerMenuPages;
    public Player curPlayer;
    public Player selectedPlayer;
    
    public TMP_Text[] playerTitles;
    public SpriteRenderer playerBottom;
    public float curAlpha;
    public float tarAlpha;

    public PlayerMenuPage moveDownPage;

    /// <summary>
    /// true表示在选择角色
    /// </summary>
    public bool isSelecting;
    private void Start() {
        Application.targetFrameRate = 60;
        isSelecting = true;
        TitleAndBottomAppear(true);
        UpdatePlayerPage();
    }
    
    public Player GetCurPlayer() {
        return curPlayer;
    }
    

    void UpdatePlayerPage() {
        var newPlayerNum = (int)curPlayer;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)) {
            newPlayerNum = (int)curPlayer - 1;
            if (newPlayerNum < 0) newPlayerNum = 1;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) {
            newPlayerNum = (int)curPlayer + 1;
            if (newPlayerNum > 1) newPlayerNum = 0;
        }
        curPlayer = (Player)newPlayerNum;
        
        foreach (var p in playerMenuPages) {
            if (curPlayer == p.player) p.SetAppearValue();
            else p.ResetValue();
        }
    }
    
    public void SetAllPlayersInvisible() {
        foreach (var p in playerMenuPages) {
            p.ResetValue();
        }
    }

    public void TitleAndBottomAppear(bool isAppear) {
        tarAlpha = isAppear ? 1f : 0f;
    }
    
    private void Update() {
        if (isSelecting && Input.GetKeyDown(KeyCode.Z)) {
            isSelecting = false;
            moveDownPage = playerMenuPages[(int)curPlayer];
            moveDownPage.SetClonedValue();
            selectedPlayer = curPlayer;
            //黑底不必关掉
            //TitleAndBottomAppear(false);
        }
        // if (!isSelecting && Input.GetKeyDown(KeyCode.X)) {
        //     isSelecting = true;
        //     foreach (var p in playerMenuPages) {
        //         if (p.player == selectedPlayer) {
        //             p.ResetValue();
        //         }
        //     }
        //     TitleAndBottomAppear(true);
        // }
        if(MenuManager.Manager.menuState == MenuState.PlayerSelect) {
            UpdatePlayerPage();
        }
        if(isSelecting && Input.GetKeyDown(KeyCode.X)) {
            SetAllPlayersInvisible();
            TitleAndBottomAppear(false);
        }
        curAlpha.ApproachRef(tarAlpha, 8f);
        foreach (var t in playerTitles) {
            t.color = t.color.SetAlpha(curAlpha);
        }
        playerBottom.color = playerBottom.color.SetAlpha(curAlpha);
    }
}
