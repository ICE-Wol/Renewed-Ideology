using _Scripts.Tools;
using UnityEngine;


public class Fader : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    
    public float speed;
    public int startTimer;
    public bool fadeTag;
    
    public int timer;
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Active() {
        fadeTag = true;
    }
    
    void Update() {
        if (timer < startTimer || !fadeTag) {
            timer++;
            return;
        }

        spriteRenderer.color = spriteRenderer.color.Fade(speed);
    }
}
