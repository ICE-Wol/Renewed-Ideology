using _Scripts;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

public class SpellBannerCtrl : MonoBehaviour {
    public Transform[] rollBanners;
    public Transform[] lineBanners;
    public Image[] bannerImages;
    public Vector3[] initPos;

    public float maxAlpha;
    private float _targetAlpha;
    private float _currentAlpha;

    public void ResetBanners() {
        rollBanners[0].localRotation = Quaternion.Euler(0f, 0f, 0f);
        rollBanners[1].localRotation = Quaternion.Euler(0f, 0f, 0f);

        
        for (int i = 0; i < 9; i++) {
            lineBanners[i].localPosition = initPos[i];
        }
        
        foreach (var image in bannerImages) {
            image.color = image.color.SetAlpha(0f);
        }
    }

    private void Update() {
        rollBanners[0].localRotation = Quaternion.Euler(0f, 0f, 100f * Time.time);
        rollBanners[1].localRotation = Quaternion.Euler(0f, 0f, -100f * Time.time);

        for (int i = 0; i < 9; i++) {
            lineBanners[i].localPosition +=
                ((i % 2 == 0) ? 1 : -1) * 150f * Time.deltaTime * Calc.Deg2Dir3(15f);
        }

        _currentAlpha.ApproachRef(_targetAlpha, 12f);
        foreach (var image in bannerImages) {
            image.color = image.color.SetAlpha(_currentAlpha);
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
