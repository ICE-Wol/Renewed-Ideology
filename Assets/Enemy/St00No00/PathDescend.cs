using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PathDescend : MonoBehaviour {
    public Vector3 startPoint;
    public float descendDistance;
    public int waitTime;

    public bool isBonded;
    public GameObject spawner;
    
    
    private float _tarDirection;
    private float _tarSpeed;

    public IEnumerator<float> Descend() {
        var tarPos = transform.position + Vector3.down * descendDistance;
        for (; startPoint.y - transform.position.y - descendDistance< -0.1f;) {
            transform.position = transform.position.ApproachValue(tarPos,32f);
            yield return Timing.WaitForOneFrame;
        }

        if (isBonded) spawner.SetActive(true);
        else {
            for (int i = 0; i < 3; i++) {
                Instantiate(spawner, transform.position, Quaternion.identity);
                
                var d = Timing.RunCoroutine(GameManager.WaitForFrames(120).CancelWith(gameObject));
                yield return Timing.WaitUntilDone(d);
            }
        }
        
        var dd = Timing.RunCoroutine(GameManager.WaitForFrames((int)waitTime).CancelWith(gameObject));
        yield return Timing.WaitUntilDone(dd);

        Timing.RunCoroutine(Leave().CancelWith(gameObject));
    }

    public IEnumerator<float> Leave() {
        //Debug.Log("isLeaving");
        while (true) {
            transform.position += _tarSpeed * _tarDirection.Deg2Dir3();
            yield return Timing.WaitForOneFrame;
        }
    }


    public void Start() {
        _tarDirection = Random.Range(200f, 340f);
        _tarSpeed = Random.Range(0.01f, 0.03f);
        transform.position = startPoint;
        Timing.RunCoroutine(Descend().CancelWith(gameObject));
        
    }
}
