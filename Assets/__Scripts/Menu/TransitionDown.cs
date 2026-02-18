using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionDown : MonoBehaviour {
    public SpriteMask mask;
    public LoadingCtrl loadingCtrl;

    public Vector3[,] points;
    public SpriteMask[,] masks;
    public bool[,] isStarted;

    private void Start() {
        points = new Vector3[5, 12];
        masks = new SpriteMask[5, 12];
        isStarted = new bool[5, 12];
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 12; j++) {
                points[i, j] = new Vector3((j - 5) * 2 + i - 2f, (i - 2) * 2, 0);
                masks[i, j] = Instantiate(mask, points[i, j], Quaternion.identity);
                masks[i, j].transform.parent = transform;
            }
        }
        Timing.RunCoroutine(StartMasking());
    }
    
    public IEnumerator<float> StartMaskingLine(int line) {
        if (line == 2) {
            Instantiate(loadingCtrl);
        }

        if (line % 2 == 1) {
            for (int j = 0; j < 12; j++) {
                isStarted[line, j] = true;
                yield return Timing.WaitForSeconds(0.05f);
            }
        } else {
            for (int j = 11; j >= 0; j--) {
                isStarted[line, j] = true;
                yield return Timing.WaitForSeconds(0.05f);
            }
        }
    }
    public IEnumerator<float> StartMasking() {
        for (int i = 0; i < 5; i++) {
            Timing.RunCoroutine(StartMaskingLine(i));
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    private void Update() {
        bool isAllFinished = true;
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 12; j++) {
                if (isStarted[i, j]) {
                    masks[i, j].transform.position = masks[i, j].transform.position
                        .ApproachValue(i % 2 != 0 ? new Vector3(3f, -7f, 0) : new Vector3(-3f, -7f, 0), 16f);
                }

                if (!masks[i, j].transform.position.Equal(points[i, j], 0.01f)) {
                    isAllFinished = false;
                }
            }
        }

        if (isAllFinished) Destroy(gameObject);
    }
}
