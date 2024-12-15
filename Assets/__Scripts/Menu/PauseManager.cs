using System.Collections;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class PauseManager : MonoBehaviour
{
    public Transform canvas;
    public PauseMenuCtrl pauseMenuPrefab;
    public PauseMenuCtrl pauseMenuInstance;
    public GameObject[] pauseList;

    public bool isPaused = false;

    public SpriteRenderer spriteRenderer;

    public Texture2D tex;
    public Sprite spr;

    public Material matBlur;
    public float curBlurAmount = 0;
    public float tarBlurAmount = 0;

    public float curBlurColor = 0;
    public float tarBlurColor = 0;
    
    private static readonly int Appear = Shader.PropertyToID("_Appear");

    public void Update() {
        curBlurAmount.ApproachRef(tarBlurAmount, 8f);
        if(!curBlurAmount.Equal(tarBlurAmount,0.1f))
            matBlur.SetFloat(Appear, curBlurAmount);
        curBlurColor.ApproachRef(tarBlurColor, 8f);
        spriteRenderer.color = Color.Lerp(Color.white, Color.gray, curBlurColor);
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {

            if (isPaused == false) {
                StartCoroutine(CaptureScreenshot());
            }
            else StartCoroutine(ResetPause());
            
            
        }
    }

    private IEnumerator CaptureScreenshot() {
        yield return new WaitForEndOfFrame();
        
        Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
        tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.SetPixels(screenShot.GetPixels());
        tex.Apply();
        spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
        spriteRenderer.sprite = spr;
        tarBlurColor = 1f;
        Destroy(screenShot);
        tarBlurAmount = 1f;

        Timing.PauseCoroutines();
        
        foreach (var obj in pauseList) {
            obj.SetActive(false);
        }

        foreach (var bullet in State.bulletSet) {
            if (bullet == null) continue;
            bullet.gameObject.SetActive(false);
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
        isPaused = !isPaused;
        
        pauseMenuInstance = Instantiate(pauseMenuPrefab,canvas);
    }

    private IEnumerator ResetPause() {
        yield return new WaitForEndOfFrame();
        pauseMenuInstance.DestroyMenu();
        tarBlurColor = 0f;
        tarBlurAmount = 0f;
        yield return new WaitUntil(() => curBlurAmount <= 0.01f); 
        Destroy(tex);
        Destroy(spr);
        
        Timing.ResumeCoroutines();
        
        foreach (var obj in pauseList) {
            obj.SetActive(true);
        }

        foreach (var bullet in State.bulletSet) {
            if (bullet == null) continue;

            bullet.gameObject.SetActive(true);
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
        isPaused = !isPaused; 
        
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
