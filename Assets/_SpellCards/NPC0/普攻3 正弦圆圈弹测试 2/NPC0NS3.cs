using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;

public class NPC0NS3 : BulletGenerator
{
    public NPC0NS3BulletRing bulletRingPrefab;

    public bool isClockwise;
    public bool isJade;
    private void Start()
    {
        Timing.RunCoroutine(AutoShoot());
    }
    
    public override IEnumerator<float> ShootSingleWave()
    {
        var bulletRing = Instantiate(bulletRingPrefab, transform.position, Quaternion.identity);
        //bulletRing.isClockwise = isClockwise;
        //isClockwise = !isClockwise;
        //bulletRing.isJade = isJade;
        //isJade = !isJade;
        yield return Calc.WaitForFrames(20);
        
    }

    public override IEnumerator<float> AutoShoot() {
        while (true) {
            Timing.RunCoroutine(ShootSingleWave());
            yield return Calc.WaitForFrames(waveFrameInterval);
        }
    }
    

}
