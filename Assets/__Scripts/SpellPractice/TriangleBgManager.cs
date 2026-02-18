using System;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TriangleBgManager : MonoBehaviour
{
    public TriangleBgCtrl triangleBgPrefab;
    
    private readonly Vector2 triSize = new (0.5f,0.5f * Mathf.Sqrt(3));
    public Vector2Int triCount = new (50,12);
    public TriangleBgCtrl[,] triangles;

    public bool isGrayMode;

    public void Init(int x, int y) {
        triangles = new TriangleBgCtrl[x, y];
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                TriangleBgCtrl triangleBg = Instantiate(triangleBgPrefab, transform);
                triangleBg.transform.localPosition =
                    new Vector3((j % 2 == 0 ? 0 : triSize.x) + i * triSize.x, j * triSize.y, 0);
                triangleBg.transformApproacher.targetPos = triangleBg.transform.localPosition;
                
                triangleBg.transform.localScale = Vector3.zero;
                triangleBg.initScale = new Vector3(1, (i % 2 == 0 ? 1 : -1) * triSize.y, 1);
                triangleBg.transformApproacher.targetScale = triangleBg.initScale;
                
                float randR = i/(float)x + UnityEngine.Random.Range(-0.1f, 0.1f);
                float randG = j/(float)y + UnityEngine.Random.Range(-0.1f, 0.1f);
                triangleBg.spriteRenderer.color = new Color(randR, randG, 1, 0.1f);
                triangleBg.initAngle = (i * x + j)/(float)(x * y) * 360f;
                if(isGrayMode) triangleBg.isGrayMode = true;
                triangles[i, j] = triangleBg;
            }
        }
        
    }
    
    public void Start() {
        Init(triCount.x, triCount.y);
    }

    public bool isCircleMode;
    public void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            isCircleMode = !isCircleMode;

            if (isCircleMode) {
                for (int i = 0; i < triCount.x; i++) {
                    for (int j = 0; j < triCount.y; j++) {
                        
                        triangles[i, j].isCircleMode = true;

                    }
                }
            }
            else {
                for (int i = 0; i < triCount.x; i++) {
                    for (int j = 0; j < triCount.y; j++) {
                        triangles[i, j].isCircleMode = false;
                        TriangleBgCtrl triangleBg = triangles[i, j];
                        triangleBg.transformApproacher.targetPos =
                            new Vector3((j % 2 == 0 ? 0 : triSize.x) + i * triSize.x, j * triSize.y, 0);
                        triangleBg.transformApproacher.targetScale = new Vector3(1, (i % 2 == 0 ? 1 : -1) * triSize.y, 1);
                    }
                }
            }
        }
    }
    
    
}
