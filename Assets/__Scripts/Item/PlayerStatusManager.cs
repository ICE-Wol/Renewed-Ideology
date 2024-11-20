using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace _Scripts {
    public class PlayerStatusManager : MonoBehaviour {
        public static PlayerStatusManager Manager;
        private void Awake() {
            if (Manager == null) {
                Manager = this;
            }
            else {
                Destroy(gameObject);
            }
            _timer = 0;
            InitSlot(2,3);
        }
        
        [Header("画面右上方转动魔法阵")]
        public Image squareInner;
        public Image squareOuter;
        public Image squareGlow;
        public Image squareBlur;
        
        [Header("玩家残机和雷槽显示")]
        public Transform slotParent;
        public Image slotPrefab;
        public Image[,] slotLife;
        public Image[,] slotBomb;
        public Sprite[] sprFrag;

        private int _timer;
        
        private const int MaxFullRes = 8;
        private const int MaxPieceRes = 5;
        private const int SingleRotation = 72;
        private const int GapLengthX = 45;
        private const int GapLengthY = 22;

        private void InitSlot(int numLife, int numBomb) {
            slotLife = new Image[MaxFullRes, MaxPieceRes];
            slotBomb = new Image[MaxFullRes, MaxPieceRes];
            for (int i = 0; i < 7; i++) {
                for (int j = 0; j < 5; j++) {
                    slotLife[i, j] = Instantiate(slotPrefab,slotParent);
                    slotLife[i, j].transform.localPosition = new Vector3(i * GapLengthX, GapLengthY, 0);
                    slotLife[i, j].transform.localRotation = Quaternion.Euler(0, 0, j * SingleRotation);
                    slotLife[i, j].sprite = i < numLife ? sprFrag[1] : sprFrag[0];
                    slotBomb[i, j] = Instantiate(slotPrefab,slotParent);
                    slotBomb[i, j].transform.localPosition = new Vector3(i * GapLengthX, -GapLengthY, 0);
                    slotBomb[i, j].transform.localRotation = Quaternion.Euler(0, 0, j * SingleRotation);
                    slotBomb[i, j].sprite = i < numBomb ? sprFrag[2] : sprFrag[0];
                }
            }
        }

        public void RefreshSlot() {
            var numLife = Player.PlayerCtrl.Player.state.life;//GameManager.Player.playerData.Life;
            var numBomb = Player.PlayerCtrl.Player.state.bomb;
            var numLifeFrag = Player.PlayerCtrl.Player.state.LifeFrag;
            var numBombFrag = Player.PlayerCtrl.Player.state.BombFrag;
            for (int i = 0; i < 7; i++) {
                for (int j = 0; j < 5; j++) {
                    slotLife[i, j].sprite = i < numLife ? sprFrag[1] : sprFrag[0];
                    if (i == numLife && j < numLifeFrag) slotLife[i, j].sprite = sprFrag[1]; 
                    slotBomb[i, j].sprite = i < numBomb ? sprFrag[2] : sprFrag[0];
                    if (i == numBomb && j < numBombFrag) slotBomb[i, j].sprite = sprFrag[2]; 
                }
            }

        }

        private void Update() {
            squareInner.transform.localRotation = Quaternion.Euler(0,0,_timer/4f);
            squareOuter.transform.localRotation = Quaternion.Euler(0,0,-_timer/6f);
            squareGlow.transform.localRotation = Quaternion.Euler(0,0,-_timer/3f);
            squareBlur.transform.localRotation = Quaternion.Euler(0,0,_timer/4f);
            _timer++;
        }
    }
}
