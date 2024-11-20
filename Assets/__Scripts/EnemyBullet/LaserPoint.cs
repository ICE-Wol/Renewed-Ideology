using System;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum LaserState {
    WarnLine,
    Active,
    Inactive
}

public class LaserPoint : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float flickerRotation;
    public float flickerScale;
    
    public LineRenderer lineRenderer;
    public LaserPoint nextPoint;

    public Color color;
    public LaserState state;
    public bool isNodeActive;
    public bool isLaserThin;

    public float curScale;
    public float tarWidth;
    public float curWidth;
    public float tarAlpha;
    public float curAlpha;

    public float hitRadius;
    
    public int timer;
    

    float DistancePointToLineSegment(Vector3 A, Vector3 B, Vector3 P) {
        Vector3 AP = P - A;
        Vector3 AB = B - A;

        float magnitudeAB = AB.sqrMagnitude;
        float ABAPproduct = Vector3.Dot(AP, AB);// AP · AB
        float distance = ABAPproduct / magnitudeAB;
        // AP · AB / |AB|^2 = |AP| * |AB| * cosθ / |AB|^2 = |AP| * |AB| * cosθ / |AB| = |AP| * cosθ = |AP'| * cosθ

        if (distance < 0)
        {
            return Vector3.Distance(P, A); // 点P在端点A的外侧,计算到A点的距离
        }
        else if (distance > 1)
        {
            return Vector3.Distance(P, B); // 点P在端点B的外侧,计算到B点的距离
        }
        else
        {
            Vector3 projection = A + distance * AB; // 计算投影点
            return Vector3.Distance(P, projection); // 点P到投影点的距离
        }
    }

    public void SetWidth(float width) {
        //tarWidth = width;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        //lineRenderer.material.SetFloat("_Width", width);
    }
    
    public void SetAlpha(float alpha) {
        lineRenderer.material.SetColor("_Color", color.SetAlpha(alpha));
    }
    
    public void SetState(LaserState state) {
        this.state = state;
        switch (state) {
            case LaserState.WarnLine:
                tarWidth = 0.01f;
                tarAlpha = 0.5f;
                break;
            case LaserState.Active:
                tarWidth = 0.5f;
                if(isLaserThin) tarWidth = 0.25f;
                tarAlpha = 1f;
                break;
            case LaserState.Inactive:
                tarWidth = 0f;
                tarAlpha = 0f;
                break;
        }
    }
    
    public void SetColor(Color color) {
        this.color = color;
        lineRenderer.material.SetColor("_Color", color.SetAlpha(curAlpha));
        
        Color.RGBToHSV(color, out var h, out var s, out var v);
        spriteRenderer.material.SetFloat("_Hue", h);
        spriteRenderer.material.SetFloat("_Saturation", s);
        
        lineRenderer.material.SetFloat("_Hue", h);
        lineRenderer.material.SetFloat("_Saturation", s);
        
    }
    
    private void Start() {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position.SetZ(10f));
        if(nextPoint != null)
            lineRenderer.SetPosition(1, nextPoint.transform.position.SetZ(10f));
        
        lineRenderer.sortingLayerName = "EnemyBullet";
        
    }
    
    private void Update() {
        if (nextPoint != null && state == LaserState.Active && curWidth.Equal(tarWidth, 0.01f)) {
            var distance = DistancePointToLineSegment
            (transform.position,
                nextPoint.transform.position,
                PlayerCtrl.Player.transform.position);
            var playerHitRadius = PlayerCtrl.Player.state.hitRadius;
            if (distance < playerHitRadius + hitRadius) {
                if (!PlayerCtrl.Player.CheckInvincibility()) {
                    PlayerCtrl.Player.GetHit();
                }
            }
        }


        if(timer % 2 == 0)
            flickerRotation = Random.Range(0f, 360f);
        timer++;
        
        flickerScale = Mathf.Abs(Mathf.Sin(Time.time * 20) * 0.3f) + 1.8f;
        if (!isNodeActive) {
            curScale.ApproachRef(0, 16f);
        }else {
            curScale.ApproachRef(1, 16f);
        }
        transform.localScale = new Vector3(flickerScale * curScale, flickerScale * curScale, 1);
        transform.Rotate(0, 0, flickerRotation);
        
        curAlpha.ApproachRef(tarAlpha, 8f,0.1f);
        curWidth.ApproachRef(tarWidth, 16f,0.1f);
        SetWidth(curWidth);
        SetAlpha(curAlpha);
        
        lineRenderer.SetPosition(0, transform.position.SetZ(10f));
        if(nextPoint != null)
            lineRenderer.SetPosition(1, nextPoint.transform.position.SetZ(10f));
    }
}
