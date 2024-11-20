using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using static _Scripts.Tools.Calc;

public class StarToCircle : MonoBehaviour
{
    public DoubleSpeedApproach jadeBulletPrefab;
    public int bulletNum;
    public float radius;
    public float randDir;
    public bool isCreationFinished;
    public bool isClockWise;
    public bool allArrived = true;
    public bool isOddWave;
    public bool isRandom;
    public Color evenColor;
    public Vector3 centerPos;
    public Vector3[] starPos;
    public List<Vector3> bulletPos;
    public List<DoubleSpeedApproach> bullets;
    

    private void Start() {
        bullets = new List<DoubleSpeedApproach>();
        bulletPos = new List<Vector3>();
        transform.position = centerPos;
        Timing.RunCoroutine(CreateCircle());

        if (!isOddWave) {
            isClockWise = !isClockWise;
        }
    }

    private void Update() {
        if (isRandom && randDir == 0) {
            randDir = UnityEngine.Random.Range(0, 360);
        }
        if (isCreationFinished) {
            //Timing.RunCoroutine(ToCircle());
            //isCreationFinished = false;
            for (int i = 0; i < bullets.Count; i++) {
                var tarPos = bulletPos[i]; //radius * (360f / bullets.Count * i).Deg2Dir3();
                if (bullets[i] != null)
                    bullets[i].transform.position =
                        bullets[i].transform.position.ApproachValue(tarPos, 16f);
            }

            for (int i = 0; i < bullets.Count; i++) {
                var tarPos = bulletPos[i]; //radius * (360f / bullets.Count * i).Deg2Dir3();
                if (bullets[i] != null) {
                    allArrived = bullets[i].transform.position.Equal(tarPos, 0.01f);
                    if (!allArrived) break;
                }
            }

            if (allArrived) {
                for (int i = 0; i < bullets.Count; i++) {
                    if (bullets[i] != null) {
                        bullets[i].trigger = true;
                        bullets[i].direction = randDir + i * 360f / bullets.Count * 3f;
                        bullets[i].GetComponent<Highlight>().Fade();
                        if (isClockWise) bullets[i].direction += 180f;
                        //Vector2.SignedAngle(Vector2.right, bulletPos[i] - Vector3.zero);
                    }
                    // if (bullets[i] != null) {
                    //     starBullets[i].trigger = true;
                    //     starBullets[i].direction = i * 360f / bulletNum * 3f;
                    //     if (isClockWise) starBullets[i].direction += 180f;
                    //     //Vector2.SignedAngle(Vector2.right, bulletPos[i] - Vector3.zero);
                    // }
                }
                isCreationFinished = false;
                Destroy(gameObject);
            }
        }

        
    }

    public IEnumerator<float> CreateCircle() {
        //for (int j = 0; j < 5; j++) {
            //for (int i = j * bulletNum / 5; i < (j + 1) * (bulletNum / 5); i++) {
            // for (int i = 0; i < bulletNum; i++) {
            //     var pos = radius * (360f / bulletNum * i - 90f).Deg2Dir3();
            //     var b = Instantiate(jadeBulletPrefab, pos, Quaternion.identity);
            //     bullets.Add(b);
            //     yield return WaitForFrames(3);
            // }
            for (int j = 0; j < bulletNum / 5; j++) {
                for (int i = 0; i < 5; i++) {
                    var pos = centerPos + radius * (360f / bulletNum * (i * bulletNum / 5f + j) - 90f).Deg2Dir3();
                    if (isClockWise) pos = centerPos + radius * (360f / bulletNum * (-i * bulletNum / 5f - j) - 90f).Deg2Dir3();
                    var b = Instantiate(jadeBulletPrefab, pos, Quaternion.identity);
                    if (!isOddWave) {
                        b.GetComponent<Config>().color = evenColor;
                        b.GetComponent<State>().SetColor(evenColor);
                    }
                    bullets.Add(b);
                    //starBullets[i * bulletNum / 5 + j] = b;
                }

                yield return WaitForFrames(3);
            }

            GetStarPos();
            isCreationFinished = true;
    }

    public void GetStarPos() {
        starPos = new Vector3[5];

        for (int i = 0; i < 5; i++) {
            starPos[i] = centerPos + new Vector3(
                Mathf.Sin(Mathf.PI * 2 / 5 * i) * radius,
                Mathf.Cos(Mathf.PI * 2 / 5 * i) * radius,
                0
            );
        }

        var edgeNum = bulletNum / 5;
        for (int i = 0, cnt = 0; cnt < 5; i += 2, cnt++) {
            for (int j = 0; j < edgeNum; j++) {
                var pos = Vector3.Lerp(starPos[i % 5], starPos[(i + 2) % 5], (float)j / edgeNum);
                //pos = pos.SetY(-pos.y);
                bulletPos.Add(pos);
            }
        }
    }

    public IEnumerator<float> CreateStar() {
        starPos = new Vector3[5];

        for (int i = 0; i < 5; i++) {
            starPos[i] = new Vector3(
                Mathf.Sin(Mathf.PI * 2 / 5 * i) * radius,
                Mathf.Cos(Mathf.PI * 2 / 5 * i) * radius,
                0
            );
        }

        var edgeNum = bulletNum / 5;
        for (int i = 0, cnt = 0; cnt < 5; i += 2, cnt++) {
            for (int j = 0; j < edgeNum; j++) {
                var pos = Vector3.Lerp(starPos[i % 5], starPos[(i + 2) % 5], (float)j / edgeNum);
                var b = Instantiate(jadeBulletPrefab, pos, Quaternion.identity);
                bullets.Add(b);
                yield return WaitForFrames(5);
            }
        }
        
        isCreationFinished = true;
    }

    public IEnumerator<float> ToCircle() {
        for (int i = 0; i < bullets.Count; i++) {
            var tarPos = radius * (360f / bullets.Count * i).Deg2Dir3();
            bullets[i].transform.position = bullets[i].transform.position.ApproachValue(tarPos, 16f);
            yield return WaitForFrames(1);
        }
    }
}