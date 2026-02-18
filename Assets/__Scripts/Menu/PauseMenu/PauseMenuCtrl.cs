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
        public TMP_Text tmpComp;
        public PauseMenuNode node;
        public bool hasConfirmMenu;
        public PauseFunction function;
        public string chiContent;
        public string engContent;
    }

    public PauseMenuStruct menuHead;
    public PauseMenuStruct[] menuNodes;
    
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

    void GenerateMenuNode(ref PauseMenuStruct @struct, float localX, float localY) {
        //生成中文节点
        @struct.tmpComp = Instantiate(menuNodeChiPrefab, transform);
        @struct.tmpComp.transform.localPosition = new Vector3(0, localY, 0);
        @struct.tmpComp.text = @struct.chiContent;

        //设置组件回调路径
        @struct.node = @struct.tmpComp.GetComponent<PauseMenuNode>();
        @struct.node.parentMenu = this;

        //设置组件初始X偏移
        @struct.node.initRowOffset = localX;

        //生成英文节点
        @struct.node.engTMP = Instantiate(menuNodeEngPrefab, @struct.tmpComp.transform);
        @struct.node.engTMP.transform.localPosition = new Vector3(0, -0.6f * lineSpacing, 0);
        @struct.node.engTMP.text = @struct.engContent;
        @struct.node.engMenuNode = @struct.node.engTMP.GetComponent<PauseMenuNode>();
    }

    public void DestroyMenu() {
        Timing.RunCoroutine(menuHead.node.DestroyNode().CancelWith(menuHead.node.gameObject));
        foreach (var node in menuNodes) {
            Timing.RunCoroutine(node.node.DestroyNode().CancelWith(node.node.gameObject));
        }
    }

    public void LockMenu() {
        menuHead.node.isLocked = true;
        menuHead.node.engMenuNode.isLocked = true;
        foreach (var node in menuNodes) {
            node.node.isLocked = true;
            node.node.engMenuNode.isLocked = true;
        }
    }
    
    public void UnlockMenu() {
        menuHead.node.isLocked = false;
        menuHead.node.engMenuNode.isLocked = false;
        foreach (var node in menuNodes) {
            node.node.isLocked = false;
            node.node.engMenuNode.isLocked = false;
        }
    }

    void Start() {
        //生成菜单头
        GenerateMenuNode(ref menuHead, initLocalX - 100, lineSpacing);

        //生成菜单组
        for (int i = 0; i < menuNodes.Length; i++) {
            GenerateMenuNode(ref menuNodes[i], initLocalX, -lineSpacing / 2 - i * lineSpacing);
        }
    }
    
    // 提供MoveDown和MoveUp两个方法，让管理器移动指针
    // 这样可以回避主菜单和确认菜单之间的锁定问题
    public void MoveUp() {
        curPointer = (curPointer - 1 + menuNodes.Length) % menuNodes.Length;
        menuNodes[curPointer].node.timer = 0;
    }

    public void MoveDown() {
        curPointer = (curPointer + 1) % menuNodes.Length;
        menuNodes[curPointer].node.timer = 0;
    }

    private void Update() {
        for (int i = 0; i < menuNodes.Length; i++)
            menuNodes[i].node.isSelected = i == curPointer;
        
    }
}
