using MEC;
using UnityEngine;
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
    public TransitionUp transitionUp;

    private void Start() {
        SetState(MenuState.PressAnyKey);
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
                if (Input.anyKeyDown)
                    SetState(MenuState.MainMenu);
                break;
            case MenuState.MainMenu:
                if (Input.GetKeyDown(KeyCode.Z)) {
                    switch (menuCtrl.curMenuPointer) {
                        case 0:
                            Debug.Log("Start Game");
                            SetState(MenuState.DifficultySelect);

                            break;
                        case 7:
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
                    print("entered");
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.D)) {
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.W)) {
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.S)) {
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    curDiff = curDiff.Next();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    curDiff = curDiff.Prev();
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    curDiff = curDiff.Next();
                }

                if (Input.anyKeyDown) {
                    diffSelect.curDiff = curDiff;
                }
                
                if (Input.GetKeyDown(KeyCode.Z)) {
                    SetState(MenuState.PlayerSelect);
                    diffSelect.selected = true;
                    playerSelect.TitleAndBottomAppear(true);
                }
                if(Input.GetKeyDown(KeyCode.X)) {
                    diffSelect.selected = false;
                    diffSelect.selecting = true;
                }

                if (diffSelect.selected == false && Input.GetKeyDown(KeyCode.X)) {
                    SetState(MenuState.MainMenu);
                    menuDecoManager.MoveCirclePos(-8f);
                    diffSelect.SetAllDiffInvisible();
                    diffSelect.TitleAndBottomAppear(false);
                }
                

                break;
            case MenuState.PlayerSelect:
                if (Input.GetKeyDown(KeyCode.X)) {
                    SetState(MenuState.DifficultySelect);
                    diffSelect.selected = false;
                    playerSelect.SetAllPlayersInvisible();
                    playerSelect.TitleAndBottomAppear(false);
                }
                if (Input.GetKeyDown(KeyCode.Z)) {
                    transitionUp.gameObject.SetActive(true);
                }
                break;

        }


    }



    private void Update() {
        ChangeMenu();
    }
    
}
