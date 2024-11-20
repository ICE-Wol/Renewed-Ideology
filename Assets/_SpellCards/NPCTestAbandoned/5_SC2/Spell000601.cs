using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Player;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class Spell000601 : BulletGenerator
{
    public Spell000600 bulletTemplate;
    
    public bool isClockwise;

    public int isFinalWave;
    
    public override IEnumerator<float> ShootSingleWave() {
        for (int i = 0; i < layerCountInWave; i++) {
            var b = Instantiate(bulletTemplate,transform.position, Quaternion.identity);
            
            var dir = -Vector2.SignedAngle((PlayerCtrl.Player.transform.position - transform.position), Vector2.right);
            b.direction = (isClockwise ? -1 : 1) * (float)i / layerCountInWave * 360f + dir;

            if (isFinalWave % 3 != 2) {
                if (i > 0 && i <= layerCountInWave / 2) {
                    b.dirIncrease = -0.3f;
                }
                else if (i > layerCountInWave / 2) {
                    b.dirIncrease = 0.3f;
                }
            }
            else {
                if (i > 0 && i <= layerCountInWave / 2) {
                    b.dirIncrease = -0.3f * (isClockwise ? -1 : 1);
                }
                else if (i > layerCountInWave / 2) {
                    b.dirIncrease = 0.3f * (isClockwise ? -1 : 1);
                }
            }

            b.speed = 6f - 0.05f * i;
            b.spdIncrease = -0.01f;

            if (isFinalWave % 3 == 2) 
                isClockwise = !isClockwise;
            
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(layerFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }   
        
        isClockwise = !isClockwise;
        isFinalWave++;
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            var d = Timing.RunCoroutine(GameManager.WaitForFrames(waveFrameInterval));
            yield return Timing.WaitUntilDone(d);
        }
    }

    private void Start() {
        Timing.RunCoroutine(AutoShoot());
    }

    private void Update() {
        
    }
}
