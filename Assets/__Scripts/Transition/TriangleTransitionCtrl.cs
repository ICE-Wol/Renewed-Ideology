using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class TriangleTransitionCtrl : MonoBehaviour
{
    public Image image;
    public Image frameImage;
    public TransformApproacher transformApproacher;

    public Vector3 initScale;

    public float initRandR;
    public float initRandG;
    
    public float amplitudeMultiplier;
    
    public bool isGrayMode;

    public bool isFading;
    public float t;

    public void Awake() {
        image = GetComponent<Image>();
        transformApproacher = GetComponent<TransformApproacher>();
    }
    
    public void Start() {
        
        initRandR = image.color.r;
        initRandG = image.color.g;
        
        amplitudeMultiplier = Random.value;
        
        transformApproacher.activatePosApproach = true;
        transformApproacher.posApproachRate =
            new Vector3(Random.Range(16f, 32f), Random.Range(16f, 32f), Random.Range(16f, 32f));
        
        transformApproacher.activateRotApproach = true;
        transformApproacher.rotApproachRate =
            new Vector3(Random.Range(16f, 32f), Random.Range(16f, 32f), Random.Range(16f, 32f));
        
        transformApproacher.activateScaleApproach = true;
        transformApproacher.scaleApproachRate =
            new Vector3(16f, 16f, 16f);

        image.color = image.color.SetAlpha(1f);

    }
    
    public void Update() {
        float randR = initRandR + (1-amplitudeMultiplier) * 0.2f * Mathf.Sin(Time.time);
        float randG = initRandG + amplitudeMultiplier * 0.2f * Mathf.Cos(Time.time);
        //image.color = new Color(randR, randG, 1, 0.1f);
        if(isGrayMode) {
            var rgb = (randR + randG) / 2f;
            var a = image.color.a;
            image.color = new Color(rgb, rgb, rgb, a);
        }
        
        if(isFading) {
            image.color = image.color.SetAlpha(Mathf.SmoothStep(1f,0f,t));
            t += 1 / 60f * 4f;
        }
    }
    
}
