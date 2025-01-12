using System;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class SCPMenuManager : MonoBehaviour
{
    public List<MenuElement> elements;
    public int centerElementIndex;
    public bool isMenuShown;
    public bool isBlinking;

    [Header("请填写以下参数")] public TMP_Text menuElementPrefab;
    public float centerElementDefaultOffsetInX = 200;
    public int elementWidthInY = 100;
    public float decayLengthBetweenElementsInX = 40;
    public int maxElementInOneSide = 4;
    public Color blinkColor = Color.red;

    [Serializable]
    public class MenuElement
    {
        public TMP_Text tmpComp;
        public float posX;
        public float posY;
        public float alpha;
        public float scale;

        public MenuElement(TMP_Text tmpComp, float posX, float posY, float alpha, float scale) {
            this.tmpComp = tmpComp;
            this.posX = posX;
            this.posY = posY;
            this.alpha = alpha;
            this.scale = scale;
        }
    }

    private void Awake() {
        centerElementIndex = 0;
    }

    /// <summary>
    /// 初始化元素列表，必须在Start之前调用一次
    /// </summary>
    /// <param name="elements">你想注入的元素的list</param>
    public void InitElements(List<TMP_Text> texts) {
        foreach (var e in texts) {
            var newElement = new MenuElement(e, 0, 0, 0, 0);
            elements.Add(newElement);
        }

        InitElementsCurrentAndTargetValues();
        AppearElementsFromInit();
    }
    
    public void InitElements(List<string> texts, Transform parent) {
        foreach (var t in texts) {
            var newElement = Instantiate(menuElementPrefab, parent);
            newElement.text = t;
            var newElementComp = newElement.GetComponent<TMP_Text>();
            var newElementMenu = new MenuElement(newElementComp, 0, 0, 0, 0);
            elements.Add(newElementMenu);
        }

        InitElementsCurrentAndTargetValues();
        AppearElementsFromInit();
    }

    /// <summary>
    /// 将元素的实际参数值逐渐变为目标值，逐帧执行
    /// 其他时刻只需要调整elements中的元素的目标值即可
    /// </summary>
    public void UpdateElementsCurrentValues() {
        for (int i = 0; i < elements.Count; i++) {
            //中心元素闪烁，写在前面防止透明度渐变被覆盖
            elements[i].tmpComp.color = elements[i].tmpComp.color
                .ApproachValue(
                    i == centerElementIndex
                        ? Color.Lerp(blinkColor, Color.white, (isBlinking ? (Mathf.Cos(timer * Mathf.Deg2Rad * 2f) + 1) / 2f : 1f))
                        : Color.white, 16f);

            elements[i].tmpComp.transform.localPosition = elements[i].tmpComp.transform.localPosition
                .ApproachValue(new Vector3(elements[i].posX, elements[i].posY, 0), 16f);
            elements[i].tmpComp.alpha = elements[i].tmpComp.alpha.ApproachValue(elements[i].alpha, 16f);
            elements[i].tmpComp.transform.localScale = elements[i].tmpComp.transform.localScale
                .ApproachValue(elements[i].scale * Vector3.one, 16f);
        }

        timer++;

    }
    
    public int timer = 0;


    /// <summary>
    /// 初始化目标值和实际值为初始状态
    /// </summary>
    public void InitElementsCurrentAndTargetValues() {
        for (int i = 0; i < elements.Count; i++) {
            elements[i].posX = 0;
            elements[i].posY = 0;
            elements[i].alpha = 0;
            elements[i].scale = 0;

            elements[i].tmpComp.transform.localPosition = new Vector3(elements[i].posX, elements[i].posY, 0);
            elements[i].tmpComp.alpha = elements[i].alpha;
            elements[i].tmpComp.transform.localScale = elements[i].scale * Vector3.one;
        }
    }

    /// <summary>
    /// 将所有元素从初始化状态显现成可见状态
    /// </summary>
    public void AppearElementsFromInit() {
        for (int i = 0; i < elements.Count; i++) {
            elements[i].posY = -i * elementWidthInY;

            var alpha = 1 - i * 0.2f;
            if (alpha < 0) alpha = 0;
            elements[i].alpha = alpha;
            elements[i].scale = alpha;
        }

        //posX比较复杂，单独算一下
        var curX = centerElementDefaultOffsetInX;
        for (int i = 0; i < elements.Count; i++) {
            elements[i].posX = curX;
            curX -= decayLengthBetweenElementsInX;
        }
    }

    public void DisappearElements() {
        for (int i = 0; i < elements.Count; i++) {
            elements[i].posX = 0;
            elements[i].posY = 0;
            elements[i].alpha = 0;
            elements[i].scale = 0;
        }
    }
    
    /// <summary>
    /// 先将所有元素逐渐消失，再销毁元素，协程函数！
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> DestroySelf() {
        isDestroyed = true;
        DisappearElements();
        yield return Timing.WaitForSeconds(1);
        foreach (var e in elements) {
            Destroy(e.tmpComp.gameObject);
        }
        Destroy(gameObject);
    }

    public void MoveUp() {
        //末尾元素为中心元素时上移切换到开头
        if (centerElementIndex == elements.Count - 1) {
            centerElementIndex = 0;
        }
        //上移时中心元素下标加一
        else centerElementIndex += 1;
        UpdateElementsTargetValues();
    }

    public void MoveDown() {
        //首个元素为中心元素时下移切换到末尾
        if (centerElementIndex == 0) {
            centerElementIndex = elements.Count - 1;
        }

        //下移时中心元素下标减一
        else centerElementIndex -= 1;
        UpdateElementsTargetValues();
    }

    /// <summary>
    /// 更新元素目标值，也可以用于显现元素
    /// </summary>
    public void UpdateElementsTargetValues() {
        var hideX = centerElementDefaultOffsetInX - decayLengthBetweenElementsInX * (maxElementInOneSide + 1);
        var hideY = elementWidthInY * (maxElementInOneSide + 1);

        //以中心元素为起点，向两侧更新元素的位置，透明度与缩放
        var curX = centerElementDefaultOffsetInX;
        var curY = 0;
        var curAlpha = 1f;
        
        for(int i = centerElementIndex; i >= 0; i--) {
            elements[i].posX = curX;
            elements[i].posY = curY;
            elements[i].alpha = curAlpha;
            elements[i].scale = curAlpha;
            curX -= decayLengthBetweenElementsInX;
            curY += elementWidthInY;
            curAlpha -= 0.2f;
            if (curAlpha < 0) {
                curAlpha = 0;
                //将不显示的元素位置堆叠于底部/顶部消失点位置
                elements[i].posX = hideX;
                elements[i].posY = hideY;
            }
        }
        curX = centerElementDefaultOffsetInX;
        curY = 0;
        curAlpha = 1f;
        for(int i = centerElementIndex; i < elements.Count; i++) {
            elements[i].posX = curX;
            elements[i].posY = curY;
            elements[i].alpha = curAlpha;
            elements[i].scale = curAlpha;
            curX -= decayLengthBetweenElementsInX;
            //注意y轴菜单分布是有下半部分的
            curY -= elementWidthInY;
            curAlpha -= 0.2f;
            if (curAlpha < 0) {
                curAlpha = 0;
                elements[i].posX = hideX;
                elements[i].posY = -hideY;
            }
        }
    }

    
    
    private bool isDestroyed = false;

    private void Update() {
        UpdateElementsCurrentValues();
        //这里要做销毁标记，不然上下键会触发目标值更新导致无法正常消失
        if (!isDestroyed && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))) {
            UpdateElementsTargetValues();
        }

    }
}
