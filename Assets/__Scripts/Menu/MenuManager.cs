using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public enum MenuState {
    PressAnyKey,
    MainMenu,
    DifficultySelect,
    PlayerSelect
}

public class MenuManager : MonoBehaviour {
    public static MenuManager Manager;

    private void Awake() {
        if (Manager != null)
            Destroy(this.gameObject);
        else {
            Manager = this;
        }
        
        Application.targetFrameRate = 60;
    }

    public MenuState menuState;
    
    public MenuDecoManager menuDecoManager;
    public DiffSelectManager diffSelect;
    public PlayerSelectManager playerSelect;
    public MenuCtrl menuCtrl;
    public TriangleTransitionManager transition;

    private void Start() {
        SetState(MenuState.PressAnyKey);
        AudioManager.Manager.PlaySound(AudioNames.MusTitle);
    }
    

    public void SetState(MenuState state) {
        menuState = state;
        switch (state) {
            case MenuState.PressAnyKey:
                Timing.RunCoroutine(menuCtrl.SetMenuAppearOrVanish(false));
                Timing.RunCoroutine(menuCtrl.FadeNodes());
                break;
            case MenuState.MainMenu:
                Timing.RunCoroutine(menuCtrl.SetMenuAppearOrVanish(true));
                Timing.RunCoroutine(menuCtrl.AppearNodes());
                diffSelect.SetAllDiffInvisible();
                diffSelect.TitleAndBottomAppear(false);
                

                break;
            case MenuState.DifficultySelect:
                menuDecoManager.MoveCirclePos(0f);
                Timing.RunCoroutine(menuCtrl.SetMenuAppearOrVanish(false));
                //todo效果不好
                Timing.RunCoroutine(menuCtrl.FadeNodes());

                diffSelect.gameObject.SetActive(true);
                diffSelect.selecting = true;

                menuDecoManager.lines[0].gameObject.SetActive(true);
                menuDecoManager.lines[1].gameObject.SetActive(true);
                menuDecoManager.lines[2].gameObject.SetActive(true);
                break;
            case MenuState.PlayerSelect:
                diffSelect.SetAllDiffInvisible();
                playerSelect.gameObject.SetActive(true);
                playerSelect.TitleAndBottomAppear(true);
                break;

        }
        
    }
    
    private void ChangeMenu() {
        switch (menuState) {
            case MenuState.PressAnyKey:
                if (Input.anyKeyDown) {
                    AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                    SetState(MenuState.MainMenu);
                }

                break;
            case MenuState.MainMenu:
                if (Input.GetKeyDown(KeyCode.Z)) {
                    switch (menuCtrl.curMenuPointer) {
                        case 0:
                            AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                            Debug.Log("Start Game");
                            SetState(MenuState.DifficultySelect);

                            break;
                        case 1:
                            AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                            SceneManager.LoadScene("SpellPracticeScene");
                            break;
                        case 7:
                            AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                            Debug.Log("Exit Game");
                            Application.Quit();
                            break;
                        default:
                            Debug.Log("Invalid Menu Pointer");
                            break;

                    }
                }
                break;
            case MenuState.DifficultySelect:
                var curDiff = diffSelect.curDiff;
                if (Input.GetKeyDown(KeyCode.A)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.D)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.W)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.S)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeMoveUpAndDown);
                    curDiff = curDiff.Next();
                }

                if (Input.anyKeyDown) {
                    diffSelect.curDiff = curDiff;
                }
                
                if (Input.GetKeyDown(KeyCode.Z)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                    SetState(MenuState.PlayerSelect);
                    diffSelect.selected = true;
                    playerSelect.TitleAndBottomAppear(true);
                }
                if(Input.GetKeyDown(KeyCode.X)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeGoBack);
                    diffSelect.selected = false;
                    diffSelect.selecting = true;
                }

                if (diffSelect.selected == false && Input.GetKeyDown(KeyCode.X)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeGoBack);
                    SetState(MenuState.MainMenu);
                    menuDecoManager.MoveCirclePos(-8f);
                    diffSelect.SetAllDiffInvisible();
                    diffSelect.TitleAndBottomAppear(false);
                }
                

                break;
            case MenuState.PlayerSelect:
                if (Input.GetKeyDown(KeyCode.X)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeGoBack);
                    SetState(MenuState.DifficultySelect);
                    diffSelect.selected = false;
                    playerSelect.SetAllPlayersInvisible();
                    playerSelect.TitleAndBottomAppear(false);
                }
                if (Input.GetKeyDown(KeyCode.Z)) {
                    AudioManager.Manager.PlaySound(AudioNames.SeSelect);
                    transition.gameObject.SetActive(true);
                    transition.sceneName = "GameScene";
                }
                break;

        }


    }



    private void Update() {
        ChangeMenu();
    }
    
}
