using System;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using MEC;

public class TimeLine : MonoBehaviour {
    [Serializable]
    public class TimeNode {
        [Tooltip("简短的敌人描述")] public string str;
        public float timeInSecond;
        public GameObject enemy;
    }

    public float FrameToSecond(int f) {
        return f / 60f;
    }
    
    public int SecondToFrame(float s) {
        return (int) s * 60;
    }
    
    public TimeNode[] timeNodes;
    
    public float StartTimerInSecond;
    public int timer;
    
    public PathDescend openingFairy;
    public PathDescend moonLightFairy;
    public FairyCtrl ellipseFairy;
    IEnumerator<float> Opening(bool isReverse) {
        for(int i = 0; i < 10; i++) {
            var f = Instantiate(openingFairy,new Vector3(0,-5f,0),Quaternion.identity);
            var pos = new Vector3((isReverse ? -1 : 1) * (i - 5) * 0.5f, 5f, 0);
            f.startPoint = pos;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(30));
            yield return Timing.WaitUntilDone(d);
        }
    }
    
    IEnumerator<float> MoonLight(bool isReverse) {
        for(int i = 0; i < 10; i++) {
            var f = Instantiate(timeNodes[2].enemy,new Vector3(0,-5f,0),Quaternion.identity);
            if (isReverse) f.GetComponent<FairyCtrl>().path.reversePath = true;
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(15));
            yield return Timing.WaitUntilDone(d);
        }
    }
    
    IEnumerator<float> StarRing() {
        for(int i = 0; i < 10; i++) {
            var f = Instantiate(timeNodes[5].enemy,new Vector3(0,-5f,0),Quaternion.identity);
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(60));
            yield return Timing.WaitUntilDone(d);
        }
    }

    void Start() {
        timer = (int)(StartTimerInSecond * 60f);
    }
    void Update() {
        if (timer == SecondToFrame(6)) Timing.RunCoroutine(Opening(false));
        if (timer == SecondToFrame(10)) Timing.RunCoroutine(Opening(true));
        //叠起来做两边到中间(两个timer 都360)
        
        if (timer == SecondToFrame(15)) {
            var f = Instantiate(moonLightFairy,new Vector3(0,-5f,0),Quaternion.identity);
            f.startPoint = new Vector3(0, 5, 0);
        }
        
        if (timer == SecondToFrame(20)) {
            var f = Instantiate(moonLightFairy,new Vector3(0,-5f,0),Quaternion.identity);
            f.startPoint = new Vector3(2, 5, 0);
            f = Instantiate(moonLightFairy,new Vector3(0,-5f,0),Quaternion.identity);
            f.startPoint = new Vector3(-2, 5, 0);
        }
        
        if (timer == SecondToFrame(30)) {
            var f = Instantiate(moonLightFairy,new Vector3(0,-5f,0),Quaternion.identity);
            f.startPoint = new Vector3(0, 5, 0);
        }
        
        if (timer == SecondToFrame(35)) Timing.RunCoroutine(MoonLight(false));
        if (timer == SecondToFrame(40)) Timing.RunCoroutine(MoonLight(true));
        
        if(timer == SecondToFrame(50)) {
            var f = Instantiate(timeNodes[3].enemy,new Vector3(0,-5f,0),Quaternion.identity);
        }
        
        if(timer == SecondToFrame(60)) {
            var f = Instantiate(timeNodes[4].enemy,new Vector3(0,-5f,0),Quaternion.identity);
            //Timing.RunCoroutine(StarRing());
        }
        
        if(timer == SecondToFrame(70)) {
            var f = Instantiate(timeNodes[6].enemy,new Vector3(0,-5f,0),Quaternion.identity);
        }
        
        timer++;
    }
}
