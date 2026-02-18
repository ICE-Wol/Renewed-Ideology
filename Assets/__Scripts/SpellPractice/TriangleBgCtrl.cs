using System;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class TriangleBgCtrl : MonoBehaviour
{
    private static readonly int MousePos = Shader.PropertyToID("_MousePos");
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer frameSpriteRenderer;
    public TransformApproacher transformApproacher;

    public Vector3 initScale;

    public float initRandR;
    public float initRandG;
    
    public float amplitudeMultiplier;

    public bool isCircleMode;
    public float radius;
    public float initAngle;
    public float initRotation;
    public bool isGrayMode;

    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transformApproacher = GetComponent<TransformApproacher>();
    }
    
    public void Start() {
        initRandR = spriteRenderer.color.r;
        initRandG = spriteRenderer.color.g;
        
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
        
        
        radius = Random.Range(3f, 10f);
        initAngle = Random.Range(0f, 360f);
        initRotation = Random.Range(0f, 360f);

        if(isGrayMode) spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        else spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        
    }
    
    public void Update() {
        var x = Input.mousePosition.x;
        var y = Screen.height - Input.mousePosition.y;
        frameSpriteRenderer.material.SetVector(MousePos, new Vector2(x,y));
        
        float randR = initRandR + (1-amplitudeMultiplier) * 0.2f * Mathf.Sin(Time.time);
        float randG = initRandG + amplitudeMultiplier * 0.2f * Mathf.Cos(Time.time);
        spriteRenderer.color = new Color(randR, randG, 1, 0.1f);
        if(isGrayMode) {
            // float h, s, v;
            // Color color;
            // Color.RGBToHSV(spriteRenderer.color, out h, out s, out v);
            // color = Color.HSVToRGB(h, s / 2f, v / 5f);
            //spriteRenderer.color = color.SetAlpha(0.5f);
            var rgba = (randR + randG) / 2f;
            spriteRenderer.color = new Color(rgba, rgba, rgba,0.1f);
        }
        
        if (isCircleMode) {
            var speed = (11f - radius) / 2f;
            var angle = initAngle + Time.time * 10f * speed;
            var pos = new Vector3(15, 5, 0) + radius * angle.Deg2Dir3();
            transformApproacher.targetScale = radius / 10f * initScale;
            transformApproacher.targetPos = pos;
            transformApproacher.targetRot = new Vector3(0, 0, initRotation + angle);
        }
        else {
            transformApproacher.targetRot = new Vector3(0, 0, 0);
        }
    }
    
    
}
