using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionUp : MonoBehaviour
{
    public SpriteMask mask;
    public LoadingCtrl loadingCtrl;

    public Vector3[,] points;
    public SpriteMask[,] masks;
    public bool[,] isStarted;

    private void Start() {
        points = new Vector3[5, 12];
        masks = new SpriteMask[5, 12];
        isStarted = new bool[5, 12];
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 12; j++) {
                points[i, j] = new Vector3((j - 5) * 2 + i - 2f, (i - 2) * 2, 0);
                if (i % 2 == 0) masks[i, j] = Instantiate(mask, new Vector3(-3f, -7f, 0), Quaternion.identity);
                else masks[i, j] = Instantiate(mask, new Vector3(3f, -7f, 0), Quaternion.identity);
                masks[i, j].transform.parent = transform;
            }
        }

        Timing.RunCoroutine(StartMasking());
        Timing.RunCoroutine(LoadSceneAsyncCoroutine("GameScene"));
    }

    public IEnumerator<float> StartMaskingLine(int line) {
        if (line == 2) {
            Instantiate(loadingCtrl,transform);
        }

        if (line % 2 == 1) {
            for (int j = 0; j < 12; j++) {
                isStarted[line, j] = true;
                yield return Timing.WaitForOneFrame;
            }
        }
        else {
            for (int j = 11; j >= 0; j--) {
                isStarted[line, j] = true;
                yield return Timing.WaitForOneFrame;
            }
        }
    }

    public IEnumerator<float> StartMasking() {
        for (int i = 0; i < 5; i++) {
            Timing.RunCoroutine(StartMaskingLine(i));
            yield return Timing.WaitForOneFrame;
        }
    }
    
    public IEnumerator<float> EndMasking() {
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 12; j++) {
                Timing.RunCoroutine(ShrinkMask(masks[i, j].transform).CancelWith(masks[i, j].gameObject));
                yield return Timing.WaitForOneFrame;
            }
        }

        yield return Timing.WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public IEnumerator<float> ShrinkMask(Transform mask) {
        while (mask.localScale.x > 0.01f) {
            //注意这个地方会自动填成0.1f，记得自己改
            mask.localScale = mask.localScale.ApproachValue(Vector3.zero, 16f);
            yield return Timing.WaitForOneFrame;
        }
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
            if (asyncOperation.progress >= 0.9f && isAllFinished) {
                Debug.Log("Scene is ready. Activating it now.");
                break;
            }

            yield return Timing.WaitForOneFrame; // 等待下一帧
        }
        
        // 获取目标场景并移动物体
        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        if (targetScene.IsValid())
        {
            SceneManager.MoveGameObjectToScene(transform.parent.gameObject, targetScene);
            Debug.Log($"Moved {gameObject.name} to scene {sceneName}.");
        }
        else
        {
            Debug.LogError($"Failed to load target scene '{sceneName}'.");
        }
        
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
        
        Timing.RunCoroutine(EndMasking());
    }

    public bool isAllFinished = true;

    private void Update() {
        isAllFinished = true;
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 12; j++) {
                if (isStarted[i, j]) {
                    masks[i, j].transform.position
                        = masks[i, j].transform.position.ApproachValue(points[i, j], 16f);
                }

                if (!masks[i, j].transform.position.Equal(points[i, j], 0.01f)) {
                    isAllFinished = false;
                }
            }
        }
    }
}
