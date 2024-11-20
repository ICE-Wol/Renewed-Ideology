using _Scripts;
using _Scripts.Tools;
using UnityEngine;

public class EnemyArrowCtrl : MonoBehaviour
{
    public RectTransform arrow;
    public float tarScale = 1f;
    public float curScale = 0f;
    
    public bool isFunctioning = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Manager.curBoss == null || isFunctioning == false) {
            tarScale = 0f;
        }
        else {
            tarScale = 1f;
            arrow.anchoredPosition =
                new Vector2(
                    //Mathf.Clamp(Camera.main.WorldToScreenPoint(GameManager.Manager.curBoss.transform.position).x, -430,
                    Camera.main.WorldToScreenPoint(GameManager.Manager.curBoss.transform.position).x - 1920 / 2f,
                    arrow.anchoredPosition.y);
        }

        curScale.ApproachRef(tarScale, 16f);
        arrow.localScale = new Vector3(curScale, curScale, 1f);
    }
}
