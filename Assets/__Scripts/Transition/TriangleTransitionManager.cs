using System;
using System.Collections.Generic;
using _Scripts.Tools;
using DG.Tweening;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TriangleTransitionManager : MonoBehaviour
{
    public TriangleTransitionCtrl triangleBgPrefab;
    public LoadingCtrl loadingPrefab;
    
    private readonly Vector2 triSize = new (0.5f,0.5f * Mathf.Sqrt(3));
    public Vector2Int triCount = new (50,12);
    public TriangleTransitionCtrl[,] triangles;
    public Transform center;

    public string sceneName;
    
    public bool isGrayMode;
    public bool isExpandFinished;

    public bool isExpanding;

    public void Init(int x, int y) {
        triangles = new TriangleTransitionCtrl[x, y];
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                TriangleTransitionCtrl triangleBg = Instantiate(triangleBgPrefab, transform);
                triangleBg.transform.localPosition =
                    new Vector3((j % 2 == 0 ? 0 : triSize.x) + i * triSize.x, j * triSize.y, 0) * 100;
                triangleBg.transformApproacher.targetPos = triangleBg.transform.localPosition;
                triangleBg.transform.localScale = (i % 2 == 0 ? 1 : -1) * triSize.y * Vector3.up;
                triangleBg.transformApproacher.targetScale = triangleBg.transform.localScale;
                triangleBg.initScale = 1.05f * new Vector3(1, (i % 2 == 0 ? 1 : -1) * triSize.y, 1);

                if (!isExpanding) {
                    triangleBg.transform.localScale = triangleBg.initScale;
                    triangleBg.transformApproacher.targetScale = triangleBg.initScale;
                }
                //triangleBg.transformApproacher.targetScale = triangleBg.initScale;
                
                float randR = i/(float)x + Random.Range(-0.1f, 0.1f);
                float randG = j/(float)y + Random.Range(-0.1f, 0.1f);
                triangleBg.image.color = new Color(randR, randG, 1, 0.1f);
                if(isGrayMode) triangleBg.isGrayMode = true;
                triangles[i, j] = triangleBg;
            }
        }
        //canvas内后创建的物体会渲染在前面
        loadingPrefab = Instantiate(loadingPrefab, transform);
    }

    public IEnumerator<float> Expand(bool isOpening) {
        //Debug.Log("Expand");
        if(!isOpening) loadingPrefab.isDestroyed = true;
        bool[,] isChanged = new bool[triCount.x, triCount.y];
        for (int i = 0; i < triCount.x; i++) {
            for (int j = 0; j < triCount.y; j++) {
                isChanged[i, j] = false;
            }
        }
        //if(!isOpening) print("Closing"); 
        var radius = 0f;
        //canvas的overlay模式下物体位置单位是像素
        //而camera模式下是世界单位
        //同时只有Canvas shader graph模板可以应用在overlay模式
        while(radius < 1100) {
            //print("loop");
            radius += 30f;
            for (int i = 0; i < triCount.x; i++) {
                for (int j = 0; j < triCount.y; j++) {
                    TriangleTransitionCtrl triangleBg = triangles[i, j];
                    var distance = Vector3.Distance(triangleBg.transform.position, center.transform.position);
                    //print(distance + " " + radius);
                    if (isOpening) {
                        if (distance < radius) {
                            triangleBg.transformApproacher.targetScale = triangleBg.initScale;
                            //print(triangleBg.transformApproacher.targetScale);
                        }
                        
                        if(isChanged[i, j]) continue;
                        if(distance < 1.2f * radius) {
                            triangleBg.image.color = triangleBg.image.color.SetAlpha(0f);
                            triangleBg.image.DOFade(1f, 1f).SetEase(Ease.InOutQuad);
                            isChanged[i, j] = true;
                        }
                    }
                    else {
                        if (distance < radius) {
                            triangleBg.transformApproacher.targetScale = (i % 2 == 0 ? 1 : -1) * triSize.y * Vector3.up;
                        }

                        if(isChanged[i, j]) continue;
                        if (distance < 1.2f * radius) {
                            //triangleBg.image.DOFade(0, 0.5f).SetEase(Ease.InOutQuad);
                            triangleBg.isFading = true;
                            isChanged[i, j] = true;
                        }
                    }
                }
            }
            yield return Timing.WaitForOneFrame;
        }
        //print("out");
        
        yield return Timing.WaitForSeconds(1.2f);
        if(!isOpening) {
            //transform.parent.gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Destroy(gameObject);
            Destroy(center.gameObject);
        }
        isExpandFinished = true;
    }
    
    //LoadSceneAsync如果在未加载完成时就打开了场景，就可能造成画面的闪烁
//因此需要在Transition内使用协程完成场景的加载。
    private IEnumerator<float> LoadSceneAsyncCoroutine(string sceneName) {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 禁用场景激活，加载完资源后手动激活
        asyncOperation.allowSceneActivation = false;

        while (true/*!asyncOperation.isDone*/) {
            // 输出加载进度（值范围：0.0 - 0.9）
            Debug.Log($"Loading progress: {asyncOperation.progress}");

            // 判断是否加载完成
            if (asyncOperation.progress >= 0.9f && isExpandFinished) {
                Debug.Log("Scene is ready. Activating it now.");
                break;
            }

            yield return Timing.WaitForOneFrame; // 等待下一帧
        }
        
        // 获取目标场景并移动物体
        // Scene targetScene = SceneManager.GetSceneByName(sceneName);
        // if (targetScene.IsValid())
        // {
        //     SceneManager.MoveGameObjectToScene(transform.parent.gameObject, targetScene);
        //     Debug.Log($"Moved {gameObject.name} to scene {sceneName}.");
        // }
        // else
        // {
        //     Debug.LogError($"Failed to load target scene '{sceneName}'.");
        // }
        
        //物体移动完成后再激活场景
        asyncOperation.allowSceneActivation = true;
        
        //只有上一个场景激活后才能卸载
        while (!asyncOperation.isDone)
        {
            yield return Timing.WaitForOneFrame;
        }
        
        //等待一帧是不够的，因为场景激活需要时间（0.9 - 1）
        //yield return Timing.WaitForOneFrame;
        
        //必须有至少一个激活场景
        asyncOperation = SceneManager.UnloadSceneAsync(0);
        
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                Debug.Log("Unloading...");
                break;
            }

            yield return Timing.WaitForOneFrame;
        }
        
        //overlay的canvas shader透明度通道会丢失
        //ScreenSpace - Camera的camara会在场景切换后丢失
        //因此需要重新设置
        //transform.parent.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        //isSceneChangeFinished = true;
    }
    
    //放弃移动物体的方案
    //事实证明图案偏移是Time.time决定的，因此跨场景用同一个obj不会有跳变问题
    //下次选择解决方案时要事先想清楚

    public void Start() {
        Init(triCount.x, triCount.y);
        Timing.RunCoroutine(Expand(isExpanding).CancelWith(gameObject),"Transition");
        if(isExpanding)
            Timing.RunCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }
}
