using _Scripts;
using _Scripts.Player;
using TMPro;
using UnityEngine;

public class PlayerStateDisplay : MonoBehaviour {
    public TMP_Text tmpDifficulty;
    public TMP_Text tmpHiScoreText;
    public TMP_Text tmpHiScoreDisplay;
    public TMP_Text tmpScoreText;
    public TMP_Text tmpScoreDisplay;
    public TMP_Text tmpLifeText;
    public TMP_Text tmpBombText;
    public TMP_Text tmpLifeDisplay;
    public TMP_Text tmpBombDisplay;
    public TMP_Text tmpLifeFragDisplay;
    public TMP_Text tmpBombFragDisplay;
    public TMP_Text tmpPowerText;
    public TMP_Text tmpPowerDisplay;

    public int preLifeNum;
    public int preBombNum;
    public int preLifeFrag;
    public int preBombFrag;
    public int prePower;
    public int preMaxPoint;
    public int preScore;

    public void Start() {
        tmpDifficulty.text = "<align=\"center\"><size=8>L</size>UCID <size=8>D</size>REAM";
        tmpDifficulty.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpHiScoreText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpHiScoreDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpScoreText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpScoreDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpBombText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpLifeText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpBombDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpLifeDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpBombFragDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpLifeFragDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpPowerText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        tmpPowerDisplay.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        preBombNum = 0;
        preLifeNum = 0;
        preBombFrag = 0;
        preLifeFrag = 0;
        prePower = 0;
        preMaxPoint = 0;
        preScore = 0;
    }

    public void Update() {
        if (preBombNum != PlayerCtrl.instance.state.bomb) {
            var curBombNum = PlayerCtrl.instance.state.bomb;
            string s = "";
            for (int i = 0; i < curBombNum; i++) {
                s += "● ";
            }

            for (int i = curBombNum; i < 8; i++) {
                s += "○ ";
            }

            tmpBombDisplay.text = "<align=\"right\">" + s;
            preBombNum = curBombNum;
        }
        
        if (preLifeNum != PlayerCtrl.instance.state.life) {
            var curLifeNum = PlayerCtrl.instance.state.life;
            string s = "";
            for (int i = 0; i < curLifeNum; i++) {
                s += "● ";
            }

            for (int i = curLifeNum; i < 8; i++) {
                s += "○ ";
            }

            tmpLifeDisplay.text = "<align=\"right\">" + s;
            preLifeNum = curLifeNum;
        }
        
        if (preBombFrag != PlayerCtrl.instance.state.BombFrag) {
            var curBombNum = PlayerCtrl.instance.state.BombFrag;
            string s = "";
            for (int i = 0; i < curBombNum; i++) {
                s += "●";
            }

            for (int i = curBombNum; i < 5; i++) {
                s += "○";
            }

            tmpBombFragDisplay.text = "<align=\"right\">" + s;
            preBombFrag = curBombNum;
        }
        
        if (preLifeFrag != PlayerCtrl.instance.state.LifeFrag) {
            var curLifeNum = PlayerCtrl.instance.state.LifeFrag;
            string s = "";
            for (int i = 0; i < curLifeNum; i++) {
                s += "●";
            }

            for (int i = curLifeNum; i < 5; i++) {
                s += "○";
            }

            tmpLifeFragDisplay.text = "<align=\"right\">" + s;
            preLifeFrag = curLifeNum;
        }

        //Debug.Log(prePower + " | " + Ctrl.Player.state.Power);
        if (prePower != PlayerCtrl.instance.state.Power) {
            string s;
            var curPower = PlayerCtrl.instance.state.Power;
            s = "<align=\"right\"><size=6>" + curPower / 100 + 
                "</size>" + "." + ((curPower % 100 < 10) ? "0" : "")
                + curPower % 100 
                + " <size=2.5>/ 4.00";
            tmpPowerDisplay.text = s;
            prePower = curPower;
        }

        /*if (preScore != Ctrl.Player.state.score) {
            var curScore = Ctrl.Player.state.score;
            tmpScoreDisplay.text = "<align=\"right\">999,999,999";//curScore.ToString();
            preScore = curScore;
        }*/
    }
}
