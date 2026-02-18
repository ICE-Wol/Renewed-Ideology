using System;
using _Scripts;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class SpellBannerCtrl : MonoBehaviour {
    private static readonly int OffsetMultiplier = Shader.PropertyToID("_OffsetMultiplier");
    public SpriteRenderer[] lineBanners;
    public SpellBannerCircle[] rollBanners;

    public float maxAlpha;
    private float _targetAlpha;
    private float _currentAlpha;

    private void Start() {
        for(int i = 0;i < lineBanners.Length;i++) {
            lineBanners[i].material.SetVector(OffsetMultiplier, new Vector2((i % 2 == 0) ? 1f : -1f, 0f));
            
        }
    }

    private void Update() {
        _currentAlpha.ApproachRef(_targetAlpha, 12f);
        foreach (var sr in lineBanners) {
            sr.color = sr.color.SetAlpha(_currentAlpha);
        }
    
        foreach (var banner in rollBanners) {
            foreach (var t in banner.spellBannerSet) {
                t.color = t.color.SetAlpha(_currentAlpha);
            }
        }
    }

    public void BannerAppear() {
        _currentAlpha = 0f;
        _targetAlpha = maxAlpha;
    }

    public void BannerDisappear() {
        _targetAlpha = 0f;
    }
}
