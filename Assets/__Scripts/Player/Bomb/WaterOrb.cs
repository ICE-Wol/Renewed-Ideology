using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;

public class WaterOrb : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer squarePrefab;
    public WaterOrb waterOrbPair;
    public GameObject waterOrbParent;
    public WaterOrbParticle waterOrbParticlePrefab;
    public SpriteRenderer[,] rotateSquares;
    public float[,] radius;
    public float[,] degree;
    public float[] rotateDegree;
    public float alpha;
    public bool isClockwise;

    public bool isDamageOrb;
    public float damageRad;
    public int damageLimit;
    public int totDamage;
    public int frameDamage;
    public int burstDamage;
    
    public bool isExploded;
    public float radAddRate;

    public int sqrCntOnCircle;
    public float basicRad;

    public int timer;
    public int explodeTimer;
    void Start() {
        rotateSquares = new SpriteRenderer[3, sqrCntOnCircle];
        radius = new float[3, sqrCntOnCircle];
        degree = new float[3, sqrCntOnCircle];
        rotateDegree = new float[3];
        
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < sqrCntOnCircle; j++) {
                rotateSquares[i, j] = Instantiate(squarePrefab, transform);
                radius[i, j] = basicRad * (i + 2);
                degree[i, j] = 360f / sqrCntOnCircle * j;
                rotateSquares[i, j].transform.localPosition = radius[i, j] * degree[i, j].Deg2Dir3();
                rotateSquares[i, j].transform.localRotation = Quaternion.Euler(0, 0, 45f + 360f / sqrCntOnCircle * j);
                rotateSquares[i, j].transform.localScale = (0.1f * (i + 1)) * new Vector3(1, 1, 1);
                rotateSquares[i, j].color = Color.HSVToRGB(Random.Range(0.4f, 0.6f), Random.Range(0.8f, 1f),
                    Random.Range(0.8f, 1f));
                rotateSquares[i, j].color = rotateSquares[i, j].color.SetAlpha(alpha);
            }
        }
    }

    public void GiveDamage() {
        foreach (var target in Damageable.damageableSet) {
            if (target == null) return;
            if(target.hitRadius * target.hitRadius + damageRad * damageRad > (target.transform.position - transform.position).sqrMagnitude) {
                if (target.isInvincible) {
                    totDamage += frameDamage;
                } else {
                    target.TakeDamage(frameDamage);
                    totDamage += frameDamage;
                }

                if (totDamage >= damageLimit) {
                    if (!target.isInvincible) target.TakeDamage(burstDamage);
                    if (target == null) return;
                    isExploded = true;
                    waterOrbPair.isExploded = true;
                }
            }
        }
    }
    
    void Update() {
        if(!isExploded && isDamageOrb) GiveDamage();
        
        
        if (!isExploded) {
            spriteRenderer.color = Color.HSVToRGB(0.5f + 0.1f * Mathf.Sin(Time.time),
                0.9f + 0.1f * Mathf.Sin(Time.time),
                0.9f + 0.1f * Mathf.Sin(Time.time));
        }
        else {
            spriteRenderer.color = spriteRenderer.color.SetAlpha(spriteRenderer.color.a - 0.05f);
            explodeTimer++;
            if (explodeTimer >= 20) {
                Destroy(waterOrbParent);
            }
        }

        transform.localScale = (1f + 0.2f * Mathf.Sin(Time.time * 10)) * new Vector3(1, 1, 1);
        
        if (timer % 2 == 0) {
            var particle = Instantiate(waterOrbParticlePrefab, transform.position, Quaternion.identity);
            particle.parentPos = transform.position;
        }
        timer++;

        for (int i = 0; i < 3; i++) {
            if(isClockwise) rotateDegree[i] += 0.5f * (i + 1);
            else rotateDegree[i] -= 0.5f * (i + 1);
        }

        
        if(Input.GetKeyDown(KeyCode.Space)) {
            isExploded = true;
        }
        if (!isExploded) {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < sqrCntOnCircle; j++) {
                    //radius[i, j] = (basicRad + 0.05f * Mathf.Sin(Time.time * 2)) * (i + 2);
                    degree[i, j] = 360f / sqrCntOnCircle * j + rotateDegree[i];
                    rotateSquares[i, j].transform.localPosition = radius[i, j] * degree[i, j].Deg2Dir3();
                    rotateSquares[i, j].transform.localRotation =
                        Quaternion.Euler(0, 0, 45f + rotateDegree[i] + 360f / sqrCntOnCircle * j);
                    rotateSquares[i, j].transform.localScale = (0.1f * (i + 1)) * new Vector3(1, 1, 1);
                }
            }
        }
        else {
            radAddRate += 0.01f;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < sqrCntOnCircle; j++) {
                    radius[i, j] += radAddRate * (i + 1);
                    degree[i, j] = 360f / sqrCntOnCircle * j + rotateDegree[i];
                    rotateSquares[i, j].transform.localPosition = radius[i, j] * degree[i, j].Deg2Dir3();
                    rotateSquares[i, j].transform.localRotation =
                        Quaternion.Euler(0, 0, 45f + rotateDegree[i] + 360f / sqrCntOnCircle * j);
                    rotateSquares[i, j].transform.localScale = (0.1f * (i + 1)) * new Vector3(1, 1, 1);
                    rotateSquares[i, j].color = rotateSquares[i, j].color.SetAlpha(rotateSquares[i, j].color.a - 0.05f);
                }
            }
        }
    }
}
