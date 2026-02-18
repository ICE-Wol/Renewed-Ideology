using System;
using _Scripts.Player;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PauseFunction {
    Resume,
    Card,
    Restart,
    ReturnToTitle,
    ReturnToSpellPractice,
    Continue
}

public enum PauseMenuType {
    NormalGamePause,
    NormalGameFailed,
    NormalGameSucceed,
    SpellPracticePause,
    SpellPracticeFailed,
    SpellPracticeSucceed
}

[Serializable]
public struct PauseMenuInfo {
    public PauseMenuType type;
    public PauseMenuCtrl.PauseMenuStruct head;
    public PauseMenuCtrl.PauseMenuStruct[] nodes;
}

public class PauseMenuManager : MonoBehaviour
{
    public PauseMenuCtrl pauseMenuPrefab;
    public PauseMenuCtrl confirmMenuPrefab;
    
    public PauseMenuCtrl currentMenu;
    public PauseMenuCtrl pauseMenu;
    public PauseMenuCtrl confirmMenu;
    public bool isConfirming;
    
    public TransformApproacher flowerStemTransformApproacher;
    public ImageColorApproacher flowerStemColorApproacher;

    public PauseMenuInfo[] pauseMenuInfo;
    
    public void GenerateMenu(PauseMenuType type) {
        pauseMenu = Instantiate(pauseMenuPrefab, transform);
        currentMenu = pauseMenu;
        
        pauseMenu.menuHead = pauseMenuInfo[(int) type].head;
        pauseMenu.menuNodes = pauseMenuInfo[(int) type].nodes;
    }

    public Vector2 leftPos;
    public Vector2 rightPos;
    public Color leftColor;
    public Color rightColor;
    private bool _oldConfirmState = true;
    private int _timer;
    private void ChangeFlowerStem() {
        _timer += 1;
        flowerStemTransformApproacher.targetRot = 10f * Mathf.Sin(Mathf.Deg2Rad * _timer) * Vector3.forward;
        if(_oldConfirmState == isConfirming) return;
        _oldConfirmState = isConfirming;
        if(isConfirming) {
            flowerStemTransformApproacher.targetPos = rightPos;
            flowerStemColorApproacher.targetColor = rightColor;
        }
        else {
            flowerStemTransformApproacher.targetPos = leftPos;
            flowerStemColorApproacher.targetColor = leftColor;
        }
    }
    
    private int _upTimer;
    private int _downTimer;
    private readonly int _activatePeriod = 15;

    public void GetInput() {
        if (Input.GetKey(KeyCode.UpArrow)) {
            if (_upTimer % _activatePeriod == 0) {
                currentMenu.MoveUp();
            }

            _upTimer++;
        }
        else _upTimer = 0;

        if (Input.GetKey(KeyCode.DownArrow)) {
            if (_downTimer % _activatePeriod == 0) {
                currentMenu.MoveDown();
            }

            _downTimer++;
        }
        else _downTimer = 0;

        if (Input.GetKeyDown(KeyCode.Z)) {
            Timing.RunCoroutine(currentMenu.menuNodes[currentMenu.curPointer].node.Select());
            if (currentMenu.menuNodes[currentMenu.curPointer].hasConfirmMenu) {
                isConfirming = true;
                confirmMenu = Instantiate(confirmMenuPrefab, transform);
                currentMenu.LockMenu();
                currentMenu = confirmMenu;
            }
            else {
                if (isConfirming) {
                    if(confirmMenu.curPointer == 0) {
                        Execute(pauseMenu.menuNodes[pauseMenu.curPointer].function);
                    }
                    else {
                        isConfirming = false;
                        confirmMenu.DestroyMenu();
                        currentMenu = pauseMenu;
                        currentMenu.UnlockMenu();
                    }
                }else {
                    Execute(pauseMenu.menuNodes[pauseMenu.curPointer].function);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            if (isConfirming) {
                isConfirming = false;
                confirmMenu.DestroyMenu();
                currentMenu = pauseMenu;
                currentMenu.UnlockMenu();
            }
            else {
                pauseMenu.DestroyMenu();
            }
        }
    }
    
    public void Execute(PauseFunction index) {
        switch (index) {
            case PauseFunction.Resume:
                StartCoroutine(PauseManager.instance.ResetPause());
                break;
            case PauseFunction.Card:
                // todo 浏览卡牌页面
                break;
            case PauseFunction.Restart:
                SceneManager.LoadScene("GameScene");
                Time.timeScale = 1;
                break;    
            case PauseFunction.ReturnToTitle:
                SceneManager.LoadScene("TitleScene");
                Time.timeScale = 1;
                break;
            case PauseFunction.ReturnToSpellPractice:
                SceneManager.LoadScene("SpellPracticeScene");
                Time.timeScale = 1;
                break;
            case PauseFunction.Continue:
                PlayerCtrl.instance.state.life = 2;
                PlayerCtrl.instance.state.bomb = 3;
                PlayerCtrl.instance.state.Power = 400;
                StartCoroutine(PauseManager.instance.ResetPause());
                break;
        }
        
    }

    public void DestroyMenu() {
        flowerStemColorApproacher.targetAlpha = 0;
        if(confirmMenu != null) confirmMenu.DestroyMenu();
        if(pauseMenu != null) pauseMenu.DestroyMenu();
    }
    
    public void Update() {
        ChangeFlowerStem();
        GetInput();
    }
}
