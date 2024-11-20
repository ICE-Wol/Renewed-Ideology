using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TitleLineManager : MonoBehaviour {
    public Transform center;
    public SpriteRenderer lineTemplate;
    public SpriteRenderer[] lines;
    public DiffSelectManager difficultySelectManager;
    
    public int lineCount;
    [FormerlySerializedAs("lineRadius")] public float tarRadius;
    [FormerlySerializedAs("lineLength")] public float tarLength;
    public float[] curRadius;
    public float[] curLength;

    public int tarLineColorOffset;
    public float curLineColorOffset;

    public float tarLenDiffMultiplier;
    public float curLenDiffMultiplier;
    
    public bool isSecond;
    public bool isThird;
    
    private void Start() {
        lines = new SpriteRenderer[lineCount];
        curRadius = new float[lineCount];
        curLength = new float[lineCount];
        Timing.RunCoroutine(GenerateLine());
    }
    
    /*private IEnumerator<float> GenerateLine() {
        for (int i = 0,j = 0; i < lineCount/2; i++) {
            var line = Instantiate(lineTemplate, transform);
            line.gameObject.SetActive(true);
            lines[i] = line;
            
            j =  lineCount - i - 1;
            line = Instantiate(lineTemplate, transform);
            line.gameObject.SetActive(true);
            lines[j] = line;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(10));
            yield return Timing.WaitUntilDone(d);
        }
        
        
    }*/
    
    private IEnumerator<float> GenerateLine() {
        int segmentCount = 6; // 将360度分成60度一组，总共6组
        int linesPerSegment = lineCount / segmentCount; // 每组包含的线条数
    
        // 遍历每个分组
        for (int i = 0; i < linesPerSegment / 2; i++) {
            for (int segment = 0; segment < segmentCount; segment++) {
                int startIdx = segment * linesPerSegment + i;
                int endIdx = (segment + 1) * linesPerSegment - i - 1;
    
                // 生成头部线条
                var lineHead = Instantiate(lineTemplate, transform);
                lineHead.gameObject.SetActive(true);
                lines[startIdx] = lineHead;
                //Debug.Log("Section" +segment +"Head"+ startIdx);
                
                // 生成尾部线条
                var lineTail = Instantiate(lineTemplate, transform);
                lineTail.gameObject.SetActive(true);
                lines[endIdx] = lineTail;
                //Debug.Log("Section" +segment +"Tail"+ endIdx);
            }
    
            // 等待一段时间再生成下一对线条
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(10));
            yield return Timing.WaitUntilDone(d);
        }
    }




    private Color ColorPicker(int num,Color from,Color to) {
        var tag = (Time.time / 10  + 2f * num / lineCount) % 2;
        if (tag <= 1) {
            return from + tag * (to - from);
        }
        else {
            return from + (2 - tag) * (to - from);
            //return from + (tag) * (to - from);
        }
    }
    
    private void Update() {
        var diff = difficultySelectManager.GetCurDifficulty();                             
        tarLineColorOffset = diff == Difficulty.Reverie ? 10 :(int)diff * 2;
        curLineColorOffset.ApproachRef(tarLineColorOffset / 10f, 16f);
        
        tarLenDiffMultiplier = ((int)difficultySelectManager.GetCurDifficulty() + 1) / 6f;
        curLenDiffMultiplier = curLenDiffMultiplier.ApproachRef(tarLenDiffMultiplier, 32f);
        if (isThird) {
            if (diff == Difficulty.Reverie)
                curLenDiffMultiplier = curLenDiffMultiplier.ApproachRef(tarLenDiffMultiplier * 0.01f, 32f);
            else
                curLenDiffMultiplier = 0f;
        }


        for (int i = 0; i < lineCount; i++) {
            if (lines[i] == null) continue;
            curRadius[i].ApproachRef(tarRadius, 32f);
            curLength[i].ApproachRef(tarLength, 32f);
            //var line = transform.GetChild(i);
            var angle = 360f / lineCount * i; // + 180f;
            if (isSecond) angle = 360f / lineCount * (i + 0.5f); // + 180f;
            var pos = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 10f * Mathf.Sin(Mathf.Deg2Rad * Time.time))), Mathf.Sin(Mathf.Deg2Rad * (angle + 10f * Mathf.Sin(Mathf.Deg2Rad * Time.time)))) * curRadius[i];
            if(isSecond)pos = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle - 10f * Mathf.Sin(Mathf.Deg2Rad * Time.time))), Mathf.Sin(Mathf.Deg2Rad * (angle - 10f * Mathf.Sin(Mathf.Deg2Rad * Time.time)))) * curRadius[i];
            if (isThird)
                pos = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle)), Mathf.Sin(Mathf.Deg2Rad * (angle))) *
                      curRadius[i];
            lines[i].transform.position = center.transform.position + (Vector3)pos;
            lines[i].transform.position = new Vector3(lines[i].transform.position.x, lines[i].transform.position.y, 1f);
            lines[i].transform.rotation = Quaternion.Euler(0, 0, angle + 90);

            lines[i].transform.localScale = new Vector3(1,
                curLenDiffMultiplier * curLength[i] * (1 + Mathf.Sin(Time.time + 8f * angle * Mathf.Deg2Rad) * 0.5f),
                1f);
            if (isSecond)
                lines[i].transform.localScale = new Vector3(1,
                    curLenDiffMultiplier * curLength[i] *
                    (1 + Mathf.Sin(Time.time + 8f * angle * Mathf.Deg2Rad) * 0.5f), 1f);
            else if (isThird) {
                lines[i].transform.localScale = new Vector3(1,
                    curLenDiffMultiplier * curLength[i] *
                    (1 + Mathf.Sin(Time.time + 8f * angle * Mathf.Deg2Rad) * 0.3f), 1f);
                //if (i % 2 == 0) lines[i].transform.localScale = new Vector3(1f, 0f, 1f);
            }

            lines[i].color = ColorPicker(i,
                (1f - curLineColorOffset) * Color.red + curLineColorOffset * Color.cyan,
                (1f - curLineColorOffset) * Color.yellow + curLineColorOffset * new Color(0, 1, 0, 1));
            if (isSecond)
                lines[i].color = ColorPicker(i,
                    (1f - curLineColorOffset) * Color.green + curLineColorOffset * new Color(1, 0.5f, 0, 1),
                    (1f - curLineColorOffset) * Color.blue + curLineColorOffset * Color.yellow);
            else if (isThird)
                lines[i].color = Color.white;
            //lines[i].color = lines[i].color.SetAlpha(((int)diff + 8) / 10f);
        }

    }
}
