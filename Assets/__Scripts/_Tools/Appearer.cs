
using _Scripts.Tools;
using UnityEngine;

public class Appearer : MonoBehaviour {
    
    public SpriteRenderer spriteRenderer;

    public float speed;
    public float targetAlpha;
    public float currentAlpha;
    public bool isFinished;
    
    public int startTimer;
    public int timer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentAlpha = 0;
        spriteRenderer.color = spriteRenderer.color.SetAlpha(currentAlpha);
        
    }
    
    void Active() {
        timer = startTimer;
    }
    private void Update() {
        if (timer < startTimer) {
            timer++;
            return;
        }

        if (!isFinished) {
            currentAlpha.ApproachRef(targetAlpha, speed);
            spriteRenderer.color = spriteRenderer.color.SetAlpha(currentAlpha);
        }

        if (currentAlpha.Equal(targetAlpha,0.01f)){
            isFinished = true;
        }
    }
}
