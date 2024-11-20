using System.Net.Mime;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;


public class UIFader : MonoBehaviour {
    public Image image;
    
    public float speed;
    public int startTimer;
    public bool fadeTag;
    
    public int timer;
    void Start() {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }
    
    void Active() {
        fadeTag = true;
    }
    
    void Update() {
        if (timer < startTimer || !fadeTag) {
            timer++;
            return;
        }
        image.color = image.color.Fade(speed);
        //spriteRenderer.color = spriteRenderer.color.Fade(speed);
    }
}