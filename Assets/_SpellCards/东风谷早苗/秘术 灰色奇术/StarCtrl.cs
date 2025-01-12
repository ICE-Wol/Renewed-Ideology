using System.Collections.Generic;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using static _Scripts.Tools.Calc;

public class StarCtrl : MonoBehaviour
{
    public PointLaserManager pointLaserManager;
    public DoubleSpeedApproach riceBulletPrefab;
    public DoubleSpeedApproach fasterRiceBulletPrefab;
    public DoubleSpeedApproach starBulletPrefab;
    public DoubleSpeedApproach tailBulletPrefab;

    public float[] curRadius;
    public bool[] isActivated;


    public float radius;
    
    public int pNum;
    
    public bool bulletSpawned;

    public float rotation;
    
    public float randInitDeg;

    public bool isSmashing;
    public int timer;


    private void Start() {
        pNum = pointLaserManager.laserPoints.Length;
        curRadius = new float[pNum];
        isActivated = new bool[pNum];
        rotation = 360f;
        randInitDeg = Random.Range(0, 360);
        isSmashing = false;

        for (int i = 0; i < pNum; i++) {
            curRadius[i] = 0;
        }

        Timing.RunCoroutine(ActivateLaserNode());
    }

    public IEnumerator<float> ActivateLaserNode() {
        for (int i = 0; i < pNum; i++) {
            isActivated[i] = true;
            yield return WaitForFrames(5);
        }
    }
    
    public IEnumerator<float> SmashStar(int frames) {
        isSmashing = true;
        yield return WaitForFrames(frames);
        
        for (int i = 0; i < pNum; i++) {
            isActivated[i] = false;
            //yield return WaitForFrames(5);
        }

        for (int i = 0; i < 5; i++) {
            var rand = Random.Range(0f, 360f);
            for (int j = 0; j < 20; j++) {
                var b = Instantiate(tailBulletPrefab, transform.position, Quaternion.Euler(0, 0, j * 18));
                b.direction = 360f / 20f * j + rand;
            }
            yield return WaitForFrames(10);
        }
        
        pointLaserManager.SetClose();
        yield return WaitForFrames(30);
        Destroy(gameObject);

    }

    public IEnumerator<float> SpawnLineBullet(int i) {
        var begin = pointLaserManager.laserPoints[i].transform.position;
        var end = pointLaserManager.laserPoints[i].nextPoint.transform.position;
        DoubleSpeedApproach[] bullets = new DoubleSpeedApproach[180];
        
        for (int j = 0; j < 180; j++) {
            //var pos = Vector3.Lerp(begin, end, j / 60f);
            var pos = Vector3.Lerp(begin, end, j / 180f);
            var dir = Vector2.SignedAngle(Vector2.right, pos - transform.position);
            //var dir = j * 12;
            
            DoubleSpeedApproach b;
            if (j % 2 == 0) b = Instantiate(riceBulletPrefab, pos, Quaternion.Euler(0, 0, randInitDeg + dir + j * 6));
            else {
                b = Instantiate(fasterRiceBulletPrefab, pos, Quaternion.Euler(0, 0, randInitDeg + dir + j * 6));
                Timing.RunCoroutine(AccelerateBullet(b));
            }
            bullets[j] = b;
            b.direction = randInitDeg + dir + j * 6; // Vector2.SignedAngle(pos, Vector2.right);
            yield return WaitForFrames(1);
        }

        yield return WaitForFrames(30);
        for (int j = 0; j < 120; j++) {
            bullets[j].trigger = true;
        }
        pointLaserManager.laserState = LaserState.Active;
    }
    
    public IEnumerator<float> AccelerateBullet(DoubleSpeedApproach b) {
        yield return WaitForFrames(200);
        b.endSpeed = 2f;
    }

   

    public IEnumerator<float> SpawnStarBullet() {
        var starList = new List<DoubleSpeedApproach>();
        for (int i = 0; i < pNum; i++) {
            var basicPos = pointLaserManager.laserPoints[i].transform.position;
            var basicDir = Vector2.SignedAngle(Vector2.right, basicPos - PlayerCtrl.instance.transform.position);
            for (int r = 1; r <= 4; r++) {
                for (int j = 0; j < 20; j++) {
                    var dir = 360f / 20f * j;
                    var subRadius = Mathf.Abs(Mathf.Sin((dir % 72f / 72f) * 180f * Mathf.Deg2Rad) * 0.5f * r/3f);

                    var pos = basicPos + (r/3f - subRadius) * (dir + 36f + basicDir).Deg2Dir3();

                    DoubleSpeedApproach b;
                    //if(r == 1)
                        b = Instantiate(starBulletPrefab, pos, Quaternion.Euler(0, 0, dir));
                    //else
                    //
                    // b = Instantiate(fasterRiceBulletPrefab, pos, Quaternion.Euler(0, 0, dir));
                    b.direction = dir + basicDir + 36f;
                    starList.Add(b);
                }

                yield return WaitForFrames(5);
            }
        }

        yield return WaitForFrames(60);
        
        foreach (var b in starList) {
            b.trigger = true;
        }
        
        starList.Clear();
        starList = null;
        
    }

    public bool CheckAllNodesInPlace() {
        for (int i = 0; i < pNum; i++) {
            if (!curRadius[i].Equal(radius, 0.1f)) {
                return false;
            }
        }
        
        if(rotation > 0.5f) {
            return false;
        }

        return true;
    }
    
    private void Update() {
        radius.ApproachRef(3f, 16f);
        
        rotation.ApproachRef(0f, 16f);

        if (!isSmashing) {
            for (int i = 0; i < pNum; i++) {
                if (isActivated[i]) {
                    curRadius[i].ApproachRef(radius, 16f);
                }

                var angle = i * 360f / pNum * 2f + 90f + rotation;
                var rad = angle * Mathf.Deg2Rad;
                var x = transform.position.x + curRadius[i] * Mathf.Cos(rad);
                var y = transform.position.y + curRadius[i] * Mathf.Sin(rad);
                pointLaserManager.laserPoints[i].transform.position = new Vector3(x, y, 0);
            }

            if (CheckAllNodesInPlace() && !bulletSpawned) {
                bulletSpawned = true;
                for (int i = 0; i < pNum; i++) {
                    Timing.RunCoroutine(SpawnLineBullet(i));
                    Timing.RunCoroutine(SpawnStarBullet());
                }
                Timing.RunCoroutine(SmashStar(720));
            }
        }
        else {
            for (int i = 0; i < pNum; i++) {
                if (!isActivated[i]) {
                    curRadius[i].ApproachRef(0f, 16f);
                    
                    var angle = i * 360f / pNum * 2f + 90f + rotation;
                    var rad = angle * Mathf.Deg2Rad;
                    var x = transform.position.x + curRadius[i] * Mathf.Cos(rad);
                    var y = transform.position.y + curRadius[i] * Mathf.Sin(rad);
                    pointLaserManager.laserPoints[i].transform.position = new Vector3(x, y, 0);
                }
            }
        }

        timer++;
    }
}
