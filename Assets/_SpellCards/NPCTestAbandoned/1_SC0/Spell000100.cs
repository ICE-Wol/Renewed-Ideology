using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spell000100 : BulletGenerator {
    public CircleSpawner spawner;
    public BulletMovement[][] bullets;
    
    public bool isMirror;
    public bool isAuto;

    public float initDegOffset;
    public float degreeOffset;
    public float degOffsetIncrement = 10;
    
    /// <summary>
    /// the angle at which a single bullet's direction deviates from the center of the circle
    /// </summary>
    public float directionOffset = 10f;
    
    /// <summary>
    /// * (1 + 0.1f * dirOffsetMultiplier * i)
    /// </summary>
    public float dirOffsetMultiplier = 1f;
    
    /// <summary>
    /// after x frame bullet change its behaviour
    /// </summary>
    public int changeFrame = 60;
    /// <summary>
    /// parts which the circle split into.
    /// </summary>
    public int splitParts = 9;
    IEnumerator<float> ChangeMovement(int i) {
        
        var d = Timing.RunCoroutine(GameManager.WaitForFrames(changeFrame));
        yield return Timing.WaitUntilDone(d);
        
        var num = bullets[i].Length;
        
        for (int j = 0; j < num; j++) {
            var b = bullets[i][j];
            if (b is DoubleSpeedApproach) {
                var b2 = b as DoubleSpeedApproach;
                b2.endSpeed += (isMirror ? (1f - j % splitParts / 4f) : (j % splitParts / 6f));
            }
            
        }
    }

    public override IEnumerator<float> ShootSingleWave() {
        isEnchanting = true;
        for (int i = 0; i < layerCountInWave; i++) {
            spawner.SetParameters(null,transform.position, 1, isMirror ? 24 : 48,
                initDegOffset + degreeOffset, 0, 360, 
                degreeOffset * (isMirror ? 1 : -1) * (1 + 0.1f * dirOffsetMultiplier * i),false);
            bullets[i] = spawner.Shoot();

            var newCoroutine = ChangeMovement(i);
            Timing.RunCoroutine(newCoroutine);
            
            degreeOffset += degOffsetIncrement;
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }

        isMirror = !isMirror;
        isEnchanting = false;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
            
            
            // style 1
            //int time2 = Time.frameCount;
             //yield return new WaitUntil(() => Time.frameCount - time2 >= waveFrameInterval);
            //waituntil实际上接收了一个函数对象,这个对象的返回值决定了是否 Movenext 
             
            // style 2
            //Func<bool> timesUp = () => Time.frameCount - time2 >= waveFrameInterval;
            //yield return new WaitUntil(timesUp);
            
            // style3
            //Func<bool> timesUp = () => {
            //    return Time.frameCount - time2 >= waveFrameInterval;
            //};
            //yield return new WaitUntil(timesUp);
            
            
            //() => {}
            //闭包 = 函数 + 数据.数据来源于外部变量
            
            //Func<type1,type2...,re> 是函数泛型,返回值是re,参数是type1,type2...
            //他是一个函数的对象
            //Func<type> fun = () => {return xxx;};
            //Func<type> fun = () => xxx; xxx类型与type相同
            
            
        }
    }
    
    // Start is called before the first frame update!!!
    // NOT THE REAL START POINT
    // Awake is the real start point
    public void Awake() {
        bullets = new BulletMovement[layerCountInWave][];
        initDegOffset = Random.Range(0, 360);
    }
    
    public void Start() {
        Timing.RunCoroutine(AutoShoot());
    }
}
