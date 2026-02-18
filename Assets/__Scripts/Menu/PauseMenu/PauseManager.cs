using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.Item;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public Canvas canvas;
    public PauseMenuManager pauseMenuManagerPrefab;
    public PauseMenuManager pauseMenuManager;
    public GameObject[] pauseList;

    public bool isPaused = false;

    public MeshRenderer meshRenderer;
    public MeshRenderer blurMeshRenderer;
    
    public RenderTexture texOri;
    public RenderTexture texBlur;
    
    public Material matBlur;
    
    public float curBlurAlpha = 0;
    public float tarBlurAlpha = 0;
    
    /// <summary>
    /// 进入暂停、终止暂停的协程过程中，无法再次触发暂停函数
    /// </summary>
    public bool isLock = false;


    /// <summary>
    /// 游戏结束触发的暂停无法被按键解除
    /// </summary>
    public bool isEndPause = false;

    /// <summary>
    /// 游戏是否在成功时结束
    /// </summary>
    public bool isSuccessEnd = false;
    
    //模糊进度达到1时，将图片应用到spriteRenderer上防止shader反复采样造成卡顿
    //旧方案
    

    public void Update() {
        curBlurAlpha.ApproachRef(tarBlurAlpha, 8f);
        blurMeshRenderer.material.color = Color.Lerp(Color.white, Color.gray, curBlurAlpha);
        blurMeshRenderer.material.color = blurMeshRenderer.material.color.SetAlpha(curBlurAlpha);
        
        
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !isLock) {
            if (isEndPause && isPaused) return;
            TriggerPause();
        }
    }
    

    public IEnumerator<float>  TriggerEndPause(bool isSuccess) {
        isSuccessEnd = isSuccess;
        isEndPause = true;
        yield return Calc.WaitForFrames(45);
        CaptureScreenshot();
    }
    
    public void TriggerPause() {
        if (isPaused == false) {
            if(!isEndPause) AudioManager.Manager.PlaySound(AudioNames.SePause);
            //StartCoroutine(CaptureScreenshot());
            CaptureScreenshot();
        }
        else {
            if (isEndPause) return;
            StartCoroutine(ResetPause());
        }
    }
    
    // void OnEnable()
    // {
    //     RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    // }
    //
    // void OnDisable()
    // {
    //     RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    // }
    //
    // bool doBlur = true;
    // private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    // {
    //     if (camera != camOri) return;
    //     // Debug.LogError("on end camera rendering" + camera.name + " " + doBlur);
    //     if(!doBlur) return;
    //     Debug.LogError("setup " + matBlur);
    //     var commandBuffer = new CommandBuffer() { name = "Blur" };
    //     //matBlur出了问题。注意充分控制变量
    //     //matBlur.SetTexture("_MainTex", texOri);
    //     commandBuffer.Blit(texOri, texBlur, matBlur);
    //     context.ExecuteCommandBuffer(commandBuffer);
    //     commandBuffer.Release();
    //     //doBlur = false;
    // }

    private void Start() {
        meshRenderer.sortingLayerName = "UI";
        blurMeshRenderer.sortingLayerName = "UI";
        blurMeshRenderer.sortingOrder = 1;
        meshRenderer.enabled = false;
        blurMeshRenderer.enabled = false;
    }

    private void CaptureScreenshot() {
        isLock = true;
        isPaused = true;
        //yield return new WaitForEndOfFrame();
        tarBlurAlpha = 1f;
        meshRenderer.enabled = true;
        blurMeshRenderer.enabled = true;
        Graphics.Blit(texOri, texBlur, matBlur);
        //Graphics.ExecuteCommandBuffer(commandBuffer);
        //commandBuffer.Release();
        
        //yield return null;
        meshRenderer.material.mainTexture = texOri;
        blurMeshRenderer.material.SetTexture("_MainTex", texBlur);
        
        
        // texture = new Texture2D(texOri.width, texOri.height, TextureFormat.RGBA32, false,true);
        // RenderTexture.active = texOri;
        // texture.ReadPixels(new Rect(0, 0, texOri.width, texOri.height), 0, 0);
        // texture.Apply();
        // RenderTexture.active = null; 
        
        //这个函数必须在协程的EndOfFrame中调用(也就是在绘制工作完成后调用)
        // Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
        // var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // texture.SetPixels(screenShot.GetPixels());
        // texture.Apply();
        
        //检查清楚你到底是赋值给哪个sprite
        // sprOri = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        // spriteRenderer.sprite = sprOri;
        // spriteRenderer.material = matBlur;
        // Destroy(screenShot);
        
        // transitionCanvas.gameObject.SetActive(true);
        // uiCanvas.gameObject.SetActive(true);

        // yield return null;
        // //等待下一帧的绘制工作完成
        // yield return new WaitForEndOfFrame();
        
        // texture = new Texture2D(texBlur.width, texBlur.height, TextureFormat.RGB24, false,true);
        // RenderTexture.active = texBlur;
        // texture.ReadPixels(new Rect(0, 0, texBlur.width, texBlur.height), 0, 0);
        // texture.Apply();
        // RenderTexture.active = null; 
        // sprBlur = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        // blurSpriteRenderer.sprite = sprBlur;
        // spriteRenderer.material = matDefault;
        // tarBlurAlpha = 1f;
        
        
        
        //if(transition != null) transition.gameObject.SetActive(true);
        Timing.PauseCoroutines();
        Timing.ResumeCoroutines("Transition");
        
        foreach (var obj in pauseList) {
            obj.SetActive(false);
        }

        foreach (var bullet in State.bulletSet) {
            if (bullet == null) continue;
            bullet.gameObject.SetActive(false);
        }

        foreach (var item in Item.items) {
            if (item == null) continue;
            item.gameObject.SetActive(false);
        }

        foreach (var bulletGenerator in BulletGenerator.bulletGenerators) {
            if(bulletGenerator == null) continue;
            bulletGenerator.gameObject.SetActive(false);
        }

        foreach (var damageable in Damageable.damageableSet) {
            if(damageable == null) continue;
            damageable.gameObject.SetActive(false);
        }
        
        foreach (var curveLaserHead in CurveLaserHead.curveLaserHeads) {
            if(curveLaserHead == null) continue;
            curveLaserHead.gameObject.SetActive(false);
        }
        //isPaused = !isPaused;
        
        pauseMenuManager = Instantiate(pauseMenuManagerPrefab,canvas.transform);
        
        //transition.transform.SetAsLastSibling();
        
        if (!isEndPause) {
            if (PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode) {
                pauseMenuManager.GenerateMenu(PauseMenuType.SpellPracticePause);
            }
            else {
                pauseMenuManager.GenerateMenu(PauseMenuType.NormalGamePause);
            }
        }
        else {
            if (isSuccessEnd) {
                if (PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode) {
                    pauseMenuManager.GenerateMenu(PauseMenuType.SpellPracticeSucceed);
                }
                else {
                    pauseMenuManager.GenerateMenu(PauseMenuType.NormalGameSucceed);
                }
            }
            else {
                if (PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode) {
                    pauseMenuManager.GenerateMenu(PauseMenuType.SpellPracticeFailed);
                }
                else {
                    pauseMenuManager.GenerateMenu(PauseMenuType.NormalGameFailed);
                }
            }
        }

        isLock = false;
        Time.timeScale = 0;
    }

    public IEnumerator ResetPause() {
        isLock = true;
        tarBlurAlpha = 0f;
        yield return new WaitForEndOfFrame();
        pauseMenuManager.DestroyMenu();
        yield return new WaitUntil(() => curBlurAlpha <= 0.01f); 
        // DestroyImmediate(sprOri,true);
        // DestroyImmediate(sprBlur,true);
        
        Timing.ResumeCoroutines();
        
        foreach (var obj in pauseList) {
            obj.SetActive(true);
        }

        foreach (var bullet in State.bulletSet) {
            if (bullet == null) continue;
            bullet.gameObject.SetActive(true);
        }
        
        foreach (var item in Item.items) {
            if (item == null) continue;
            item.gameObject.SetActive(true);
        }

        foreach (var bulletGenerator in BulletGenerator.bulletGenerators) {
            if(bulletGenerator == null) continue;
            bulletGenerator.gameObject.SetActive(true);
        }

        foreach (var damageable in Damageable.damageableSet) {
            if(damageable == null) continue;
            damageable.gameObject.SetActive(true);
        }
        
        foreach (var curveLaserHead in CurveLaserHead.curveLaserHeads) {
            if(curveLaserHead == null) continue;
            curveLaserHead.gameObject.SetActive(true);
        }
        //isPaused = !isPaused; 
        isPaused = false;
        isLock = false;
        Destroy(pauseMenuManager.gameObject);
        //canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //canvas.worldCamera = Camera.main;
        Time.timeScale = 1;
    }


    /*public class A : MonoBehaviour {
        static IEnumerator<float><object> F() {
            Console.WriteLine("123123");
            yield return new Wait(10f);
            Console.WriteLine("456456");
            yield return 1f;
        }

        private IEnumerator<float> e1;
        
        void StartE1()
        {
            e1 = F();
        }
        
        
        void Update() {
            var p = e1.Current;
            if (e1.Current is Wait wait) {
                if (Time.time - wait.startWaitTime >= wait.duration) {
                    e1.MoveNext();
                }
            }
            else if (e1.Current is float f) {
                
            }
            
        }
    }*/
}
