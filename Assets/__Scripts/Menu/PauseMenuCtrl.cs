

using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseMenuCtrl : MonoBehaviour
{
    [Serializable]
    public struct PauseMenuStruct
    {
        public TMP_Text menu;
        public PauseMenuNode node;
        public string chiContent;
        public string engContent;
        
        public TMP_Text parent;
        public TMP_Text[] children;
    }

    public Image menuBackground; 
    public PauseMenuStruct menuHead;
    public PauseMenuStruct[] menuNodes;
    public PauseMenuCtrl subMenuPrefab;
    public PauseMenuCtrl subMenu;

    public Color menuBgColor1;
    public Color menuBgColor2;

    public float bgBegXPos = -150;
    public float bgEndXPos = 425;
    public float bgLerpRate = 0f;
    public float bgLerpTarRate = 0f;
    
    //颜色插值的实际值与趋近值
    public float[] tarColorLerp;
    public float[] curColorLerp;

    public TMP_Text menuNodeChiPrefab;
    public TMP_Text menuNodeEngPrefab;
    
    /// <summary>
    /// 行间距
    /// </summary>
    public float lineSpacing = 80;
    
    public float initLocalX;

    /// <summary>
    /// 当前菜单选项卡指针
    /// </summary>
    public int curPointer = 0;
    
    public bool isLocked = false;
        
    public int timer = 0;

    void GenerateMenuNode(ref PauseMenuStruct @struct,float localX, float localY)
    {
        //生成中文节点
        @struct.menu = Instantiate(menuNodeChiPrefab, transform);
        @struct.menu.transform.localPosition = new Vector3(0, localY, 0);
        @struct.menu.text = @struct.chiContent;
        
        //设置组件回调路径
        @struct.node = @struct.menu.GetComponent<PauseMenuNode>();
        @struct.node.parentMenu = this;
        
        //设置组件初始X偏移
        @struct.node.initRowOffset = localX;
            
        //生成英文节点
        @struct.node.engTMP = Instantiate(menuNodeEngPrefab, @struct.menu.transform);
        @struct.node.engTMP.transform.localPosition = new Vector3(0, - 0.6f * lineSpacing, 0);
        @struct.node.engTMP.text = @struct.engContent;
        @struct.node.engMenuNode = @struct.node.engTMP.GetComponent<PauseMenuNode>();
    }

    public void GenerateSubMenu() {
        if (subMenuPrefab != null && subMenu == null) {
            subMenu = Instantiate(subMenuPrefab, transform);
        }
    }

    public void DestroySubMenu() {
        if(subMenu != null) 
            subMenu.DestroyMenu();
    }
    
    public void DestroyMenu() {
        Timing.RunCoroutine(menuHead.node.DestroyNode());
        foreach (var node in menuNodes) {
            Timing.RunCoroutine(node.node.DestroyNode());
        }
    }

    IEnumerator<float> AppearBackground() {
        int t = 0;
        while (t <= 60) {
            menuBackground.color = menuBackground.color.SetAlpha(Mathf.SmoothStep(0, 1f, (float)t / 60));
            t++;
            yield return Timing.WaitForOneFrame;
        }
    }

    private void Awake() {
        if(menuBackground != null) menuBackground.color = menuBackground.color.SetAlpha(0);
    }

    void Start()
    {
        tarColorLerp = new float[menuNodes.Length];
        curColorLerp = new float[menuNodes.Length];
        
        if(menuBackground != null) Timing.RunCoroutine(AppearBackground());
        
        //生成菜单头
        GenerateMenuNode(ref menuHead, initLocalX - 100, lineSpacing);
        
        //生成菜单组
        for(int i = 0; i < menuNodes.Length; i++) {
            GenerateMenuNode(ref menuNodes[i], initLocalX, -lineSpacing / 2 - i * lineSpacing);
        }
    }

    private int _upTimer;
    private int _downTimer;
    private readonly int _activatePeriod = 15;
    private void Update() {
        //被选中的子节点锁定自身，传递锁定信号给父节点，父节点再锁定所有子节点
        //英文子节点也应当被锁定，以自动应用锁定视效
        if (isLocked) {
            foreach (var node in menuNodes) {
                node.node.isLocked = true;
                node.node.engMenuNode.isLocked = true;
            }
        } else {
            foreach (var node in menuNodes) {
                node.node.isLocked = false;
                node.node.engMenuNode.isLocked = true;
            }
        }

        
        if (menuBackground != null) {
            menuBackground.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 2) * 10);
            menuBackground.rectTransform.localPosition =
                new Vector3(Mathf.Lerp(bgBegXPos, bgEndXPos, bgLerpRate),
                    menuBackground.rectTransform.localPosition.y,
                    0);
            menuBackground.color = Color.Lerp(menuBgColor1, menuBgColor2, Mathf.SmoothStep(0, 1f, bgLerpRate));
            bgLerpRate.ApproachRef(bgLerpTarRate, 32f);
        }
        
        if (!isLocked) {
            //背景装饰移动、变色逻辑
            if (menuBackground != null) {
                if(Input.GetKey(KeyCode.Z)) {
                    bgLerpTarRate = 1f;
                }else if (Input.GetKey(KeyCode.X)) {
                    bgLerpTarRate = 0f;
                }
            }
            
            //如果按下了上下键，那么指针移动
            //持续按下上下键时，每隔ActivatePeriod移动一次指针
            if (Input.GetKey(KeyCode.UpArrow)) {
                if (_upTimer % _activatePeriod == 0) {
                    curPointer = (curPointer - 1 + menuNodes.Length) % menuNodes.Length;
                    menuNodes[curPointer].node.timer = 0;
                }
                _upTimer++;
            }
            else _upTimer = 0;

            if (Input.GetKey(KeyCode.DownArrow)) {
                if (_downTimer % _activatePeriod == 0) {
                    curPointer = (curPointer + 1) % menuNodes.Length;
                    menuNodes[curPointer].node.timer = 0;
                }
                _downTimer++;
            }
            else _downTimer = 0;
            

            for (int i = 0; i < menuNodes.Length; i++)
                menuNodes[i].node.isSelected = i == curPointer;

            timer++;
        }
    }
}
