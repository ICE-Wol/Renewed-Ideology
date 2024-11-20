using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{   
    public MenuNode[] curMenuNodes;
    public int curMenuPointer;
    public int curMenuMaxPointer;
    public Vector3[] fullPos;
    public Vector3[] initPos;
    public int centerPosNumber;

    public Vector2 circlePos;
    public float circleRad;
    private void GenerateMenuNodePosition() {
        fullPos = new Vector3[2 * curMenuNodes.Length];
        initPos = new Vector3[2 * curMenuNodes.Length];
        Vector3 pos = new(0f, 5f, 0f); //initial position
        for (int i = -curMenuNodes.Length; i < curMenuNodes.Length; i++) {

            if (i < 0) pos += 0.5f * Vector3.down + 0.5f * Vector3.right;

            if (i == 0 || i == -1) {
                pos += 1f * Vector3.down;
            } //make top right node further from up and down nodes

            if (i >= 0) {
                pos += 0.5f * Vector3.down + 0.5f * Vector3.left;
            }

            fullPos[i + curMenuNodes.Length] = pos;
        }

        for (int i = -curMenuNodes.Length; i < curMenuNodes.Length; i++) {
            var p = fullPos[i + curMenuNodes.Length];
            p.x = Mathf.Abs(Mathf.Sqrt(circleRad * circleRad - p.y * p.y)) + circlePos.x;
            //calc x position using y based on circle equation
            fullPos[i + curMenuNodes.Length] = p;
            initPos[i + curMenuNodes.Length] = p + 12 * Vector3.right;
        }

        centerPosNumber = curMenuNodes.Length - 1;
    }

    public IEnumerator<float> SetMenuAppearOrVanish(bool isAppear) {
        for (int i = 0; i < curMenuNodes.Length; i++) {
            curMenuNodes[i].isMenuOnScreen = isAppear;

            var d = Timing.RunCoroutine(GameManager.WaitForFrames(5));
            yield return Timing.WaitUntilDone(d);
        }
    }


    public IEnumerator<float> AppearNodes() {
        for (int i = 0; i <= 100; i++) {
            foreach (var node in curMenuNodes) {
                node.SetAlpha(i / 100f);
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    public IEnumerator<float> FadeNodes() {
        while (!curMenuNodes[1].GetAlpha().Equal(0f, 0.0001f)) {
            foreach (var node in curMenuNodes) {
                var a = node.GetAlpha();
                a.ApproachRef(0f, 8f);
                node.SetAlpha(a);
            }

            yield return Timing.WaitForOneFrame;
        }
    }
    // Update is called once per frame
    
    private void ChangeScale() {
        for(int i = 0;i < curMenuNodes.Length; i++) {
            if (i == curMenuPointer) {
                curMenuNodes[i].tarScale = 1.5f;
            }
            else if (Mathf.Abs(i -curMenuPointer) <= 1){
                curMenuNodes[i].tarScale = 0.8f;
            }
            else if (Mathf.Abs(i -curMenuPointer) <= 2){
                curMenuNodes[i].tarScale = 0.6f;
            }
            else {
                curMenuNodes[i].tarScale = 0.5f;
            }
        }
    }

    public void ApproachMenu() {
        for (int i = 0; i < curMenuNodes.Length; i++) { 
            if (i == curMenuPointer) {
                curMenuNodes[i].transform.localPosition
                    = curMenuNodes[i].transform.localPosition.ApproachValue(fullPos[centerPosNumber], 8f);
            }
            else {
                curMenuNodes[i].transform.localPosition
                    = curMenuNodes[i].transform.localPosition.ApproachValue(fullPos[i - curMenuPointer + centerPosNumber], 8f);
            }
            
            if(!curMenuNodes[i].isMenuOnScreen) {
                curMenuNodes[i].transform.localPosition
                    = curMenuNodes[i].transform.localPosition.ApproachValue(initPos[i - curMenuPointer + centerPosNumber], 6f);
            }
        }
    }

    private void ChangeMenu() {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            curMenuPointer++;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            curMenuPointer--;
        }

        if (curMenuPointer > curMenuMaxPointer) {
            curMenuPointer = 0;
        }

        if (curMenuPointer < 0) {
            curMenuPointer = curMenuMaxPointer;
        }
    }
    
    private void Start() {
        GenerateMenuNodePosition();
        curMenuPointer = 0;
        curMenuMaxPointer = curMenuNodes.Length - 1;
        for (int i = 0; i < curMenuNodes.Length; i++) {
            curMenuNodes[i].SetAlpha(0f);
        }

        //Timing.RunCoroutine(SetMenuAppearOrVanish(true));
    }
    
    private void Update() {
        ChangeMenu();
        ChangeScale();
        ApproachMenu();
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < fullPos.Length; i++) {
            Gizmos.DrawSphere(fullPos[i], 0.1f);
        }
    }
}
