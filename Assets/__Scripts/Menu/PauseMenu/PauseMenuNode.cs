using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using PauseMenuStruct = PauseMenuCtrl.PauseMenuStruct;

public class PauseMenuNode : MonoBehaviour
{
    public PauseMenuStruct node;
    
    [Header("需要填写")]
    public Color tarColor;
    public float maxRowOffset = 100f;
    public float flickSpeed = 5f;

    [Header("不需要填写")] 
    public PauseMenuCtrl parentMenu;
    public TMP_Text engTMP;
    public PauseMenuNode engMenuNode;
    
    
    public float tarColorLerp;
    public float curColorLerp;
    
    public float initRowOffset;
    public float tarRowOffset;
    public float curRowOffset;

    /// <summary>
    /// 该选项卡是否被选中,由父菜单控制
    /// </summary>
    public bool isSelected;
    
    /// <summary>
    /// 锁定移动与颜色特效
    /// </summary>
    public bool isLocked = false;
    
    public int timer = 0;

    private void Start() {
        node.tmpComp = GetComponent<TMP_Text>();
        Timing.RunCoroutine(StartNode().CancelWith(gameObject));
        isLocked = true;
    }

    public IEnumerator<float> StartNode() {
        int t = 0,maxT = 30;
        float alpha;
        float maxInitRowOffset = initRowOffset;
        while (t <= maxT) {
            alpha = Mathf.SmoothStep(0f, 1f, (float)t / maxT);
            node.tmpComp.color = node.tmpComp.color.SetAlpha(alpha);
            
            initRowOffset = Mathf.SmoothStep(-100, maxInitRowOffset, (float)t / maxT);
            transform.localPosition = new Vector3(initRowOffset, transform.localPosition.y, 0);
            
            t++;
            yield return Timing.WaitForOneFrame;
        } 
        isLocked = false;
    }
    
    public bool isDestroying = false;

    public IEnumerator<float> DestroyNode() {
        isDestroying = true;
        engMenuNode.isDestroying = true;
        int t = 0,maxT = 15;
        float alpha;
        while (t <= maxT) {
            alpha = Mathf.SmoothStep(1f, 0f, (float)t / maxT);
            node.tmpComp.color = node.tmpComp.color.SetAlpha(alpha);
            engTMP.color = engTMP.color.SetAlpha(alpha);
            t++;
            yield return Timing.WaitForOneFrame;
        } 
        Destroy(gameObject);
        //if(parentMenu != null) Destroy(parentMenu.gameObject);
    }
    
    public IEnumerator<float> Select() {
        int t = 0;
        while (t <= 60) {
            node.tmpComp.color = Color.Lerp(Color.red, Color.white,Mathf.Sin(t));
            t++;
            yield return Timing.WaitForOneFrame;
        } 
        //if(hasparentMenu.GenerateSubMenu();
        //parentMenu.isLocked = false;
        //isLocked = false;
        timer = 0;
        curColorLerp = 0;
    }

    public void Update()
    {
        // if (Input.GetKey(KeyCode.Z) && isSelected && !isLocked) {
        //     isLocked = true;
        //     engMenuNode.isLocked = true;
        //     Timing.RunCoroutine(Select());
        // }
        //
        // if(Input.GetKey(KeyCode.X) && isSelected && isLocked) {
        //     isLocked = false;
        //     engMenuNode.isLocked = false;
        // }
        
        //销毁时有单独的颜色变化，因此要加入判断
        if (!isDestroying) {
            if (!isLocked) {
                //alpha会被LerpColor覆盖，所以要先记录值，最后再赋值
                var alpha = node.tmpComp.color.a;
                tarColorLerp = isSelected ? (Mathf.Sin(timer * flickSpeed * Mathf.Deg2Rad) + 1f) / 4f + 0.5f : 0;
                curColorLerp.ApproachRef(tarColorLerp, 4f);
                node.tmpComp.color = Color.Lerp(Color.white, tarColor, curColorLerp);

                tarRowOffset = isSelected ? maxRowOffset : 0;
                curRowOffset.ApproachRef(tarRowOffset, 4f);
                transform.localPosition = new Vector3(initRowOffset + curRowOffset, transform.localPosition.y, 0);
                
                alpha.ApproachRef(1f, 16f);
                node.tmpComp.color = node.tmpComp.color.SetAlpha(alpha);
                
                timer++;
            }

            if (isLocked) {
                var alpha = node.tmpComp.color.a;
                alpha.ApproachRef(0.1f, 16f);
                node.tmpComp.color = node.tmpComp.color.SetAlpha(alpha);
            }
        }
    }
}
