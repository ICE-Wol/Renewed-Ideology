using System.Collections;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC0NS5 : MonoBehaviour
{
    public DoubleSpeedApproach scalePrefab;
    public DoubleSpeedApproach jadeRPrefab;
    public DoubleSpeedApproach jadeSPrefab;
    public DoubleSpeedApproach jadeGPrefab;
    
    public float circleRad;
    public int subCircleWays;
    public int genInterval;
    
    public DoubleSpeedApproach[,] Scales;
    public DoubleSpeedApproach[] jadeGs;
    public bool[] jadeGTriggered;
    
    public IEnumerator<float> CircleGen(int order,Vector3 centerPos, int ways,float radius) {
        for (int i = 0; i < ways/5; i++) {
            for (int j = 0; j < 5; j++) {
                DoubleSpeedApproach scale = Instantiate(scalePrefab, transform.position, Quaternion.identity);
                Scales[order, i + j * ways / 5] = scale;

                float deg = (i + j * ways / 5f) * 360f / ways;
                scale.direction = deg;
                scale.transform.position = centerPos + radius * deg.Deg2Dir3();

                Color c = Color.HSVToRGB(Mathf.Repeat(Vector2.SignedAngle((scale.transform.position - centerPos),
                    centerPos - transform.position) / 360f, 1f), 1f, 1f);
                scale.GetComponent<Config>().color = c;
                scale.GetComponent<State>().SetColor(c);
            }

            yield return Calc.WaitForFrames(1);
        }
    }
    
    public void JadeRGen(int type, Vector3 centerPos, int ways,float initDeg,float radius,float speed) {
        for (int i = 0; i < ways; i++) {
            DoubleSpeedApproach jadeR;
            if (type == 1) jadeR = Instantiate(jadeRPrefab, transform.position, Quaternion.identity);
            else jadeR = Instantiate(jadeSPrefab, transform.position, Quaternion.identity);

            float deg = initDeg + i * 360f / ways;
            jadeR.direction = deg;
            jadeR.transform.position = centerPos + radius * deg.Deg2Dir3();
            jadeR.endSpeed = speed;

            Color c = Color.HSVToRGB(Mathf.Repeat(Vector2.SignedAngle((jadeR.transform.position - centerPos),
                centerPos - transform.position) / 360f, 1f), 1f, 1f);
            //Color c = Color.HSVToRGB(
            //    Vector2.SignedAngle(jadeR.transform.position - centerPos, transform.position) / 360f, 1f, 1f);
            jadeR.GetComponent<Config>().color = c;
            jadeR.GetComponent<State>().SetColor(c);
        }
    }

    public IEnumerator<float> ShootJadeG(int order) {
        var jadeG = Instantiate(jadeGPrefab, transform.position, Quaternion.identity);
        jadeG.direction = 360f / 5 * order + 90f;
        jadeGs[order] = jadeG;
        jadeG.GetComponent<Config>().color = Color.HSVToRGB(order / 5f, 1f, 1f);
        jadeG.GetComponent<State>().SetColor(Color.HSVToRGB(order / 5f, 1f, 1f));
        
        yield return Calc.WaitForFrames(1);
        //留出一帧时间给jadeG初始化
        
        while (true) {
            if (jadeG == null) break;
            if (jadeG.IsSpeedChangeFinished(0.3f) && !jadeGTriggered[order]) {
                jadeGTriggered[order] = true;
                Timing.RunCoroutine(CircleGen(order, jadeG.transform.position, subCircleWays,circleRad/2f));
                break;
            }
            yield return Calc.WaitForFrames(1);
        }
    }
    public IEnumerator<float> Shoot() {
        
        for (int i = 0; i < 5; i++) {
            Timing.RunCoroutine(ShootJadeG(i));
            yield return Calc.WaitForFrames(genInterval);
        }
        
        yield return Calc.WaitForFrames(60);

        //最终位置不动的抖动
        for (int i = 0; i < 5; i++) {
            if (jadeGs[i] == null) continue;
            Timing.RunCoroutine(Shake(i, 10, 5, 1.5f));
        }
        
        yield return Calc.WaitForFrames(60 + 10);
        
        //trigger执行时，CircleGen还未执行完毕，会导致Scales[i, j]为空
        Trigger();
        for (int i = 0; i < 60; i++) {
            ChangeDirection();
            yield return Calc.WaitForFrames(5);
        }
    }

    private IEnumerator<float> Shake(int order, int times,int interval,float maxSpeed) {
        // if (jadeGs[order] == null) yield break;
        // Vector3 ori = jadeGs[order].transform.position;
        // for (int k = 0; k < times; k++) {
        //     float dir = Random.Range(0, 360f);
        //     float dist = 0.1f * (times - k) / times;
        //     Vector3 dest = jadeGs[order].transform.position + dist * dir.Deg2Dir3();
        //     while (!jadeGs[order].transform.position.Equal(dest)) {
        //         jadeGs[order].transform.position = jadeGs[order].transform.position.ApproachValue(dest, 16f);
        //         yield return Calc.WaitForFrames(order);
        //     }
        // }
        // while (!jadeGs[order].transform.position.Equal(ori)) {
        //     jadeGs[order].transform.position = jadeGs[order].transform.position.ApproachValue(ori, 16f);
        //     yield return Calc.WaitForFrames(1);
        // }
        
        Vector3 ori = jadeGs[order].transform.position;
        for (int k = 0; k < times; k++) {
            jadeGs[order].SetSpeed(maxSpeed * (times - k) / times);
            jadeGs[order].direction += Random.Range(0, 360f);
            yield return Calc.WaitForFrames(interval);
            jadeGs[order].SetSpeed(maxSpeed * (times - k) / times);
            jadeGs[order].direction -= 180f;
            yield return Calc.WaitForFrames(interval);
        }
        


        jadeGs[order].GetComponent<State>().SetState(EBulletStates.Destroying);
        JadeRGen(2,jadeGs[order].transform.position, 30, jadeGs[order].direction, 1f, 1f);
        JadeRGen(1,jadeGs[order].transform.position, 30, jadeGs[order].direction + 6f, 1f, 1.5f);
        
        //最终位置移动的抖动
        // for (int k = 0; k < times; k++) {
        //     for (int i = 0; i < 5; i++) {
        //         if (jadeGs[i] == null) continue;
        //         print("shake");
        //         jadeGs[i].SetSpeed(Random.Range(-2f, 2f));
        //         jadeGs[i].direction += Random.Range(0, 360f);
        //         
        //     }
        //     yield return Calc.WaitForFrames(interval);
        // }
    }

    private void ChangeDirection() {
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < subCircleWays; j++) {
                if (Scales[i, j] == null) continue;
                Scales[i, j].direction += (3 - j % 7) * 0.5f;
            }
        }
    }

    private void Trigger() {
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < subCircleWays; j++) {
                if (Scales[i, j] == null) continue;
                Scales[i, j].trigger = true;
                Scales[i, j].endSpeed = 2 + (3 - Mathf.Abs(3 - j % 7)) / 6f;
            }
        }
    }
    
    
    private void Start() {
        Scales = new DoubleSpeedApproach[5,subCircleWays];
        jadeGs = new DoubleSpeedApproach[5];
        jadeGTriggered = new bool[5];
        for (int i = 0; i < 5; i++) {
            jadeGTriggered[i] = false;
        }
        Timing.RunCoroutine(Shoot());
    }  
    
}
