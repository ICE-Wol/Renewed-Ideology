using System;
using _Scripts.Tools;
using UnityEngine;

public class SpellBannerCircle : MonoBehaviour
{
    public SpriteRenderer spellBannerSingle;
    public SpriteRenderer[] spellBannerSet;

    public float radius;
    public int num;

    private void Start() {
        spellBannerSet = new SpriteRenderer[(int)num];
        for (int i = 0; i < num; i++) {
            spellBannerSet[i] = Instantiate(spellBannerSingle, transform);
        }
    }
    
    private void Update() {
        for (int i = 0; i < num; i++) {
            var deg = 360f / num * i + Time.time * 50f;
            spellBannerSet[i].transform.localPosition = radius * deg.Deg2Dir3();
            spellBannerSet[i].transform.localRotation = Quaternion.Euler(0, 0, deg + 90f);
            
        }
    }
}
