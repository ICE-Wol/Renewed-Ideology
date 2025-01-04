using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BulletGenerator : MonoBehaviour {
    public static HashSet<BulletGenerator> bulletGenerators = new();
    public static List<GameObject> subBulletGenerators = new();
    
    public bool isEnchanting;

    public abstract IEnumerator<float> ShootSingleWave();
    public abstract IEnumerator<float> AutoShoot();
    

    [Tooltip("time interval between two waves")]
    public int waveFrameInterval;
    
    /// <summary>
    /// breaking waves into smaller layers
    /// </summary>
    [Tooltip("time interval between two layers")]
    public int layerFrameInterval;

    /// <summary>
    /// number of layers which a wave of bullet contain 
    /// </summary>
    [Tooltip("number of layers which a wave of bullet contain")]

    public int layerCountInWave;

    [Tooltip("total number of waves in game")]
    public int waveCount;

    [Header("仅限演示时，发射器位置")]
    public Vector3 demonstratePos;
    private void Start() {
        Timing.RunCoroutine(AutoShoot().CancelWith(gameObject),"Shoot");
    }

    
    private void Awake() {
        bulletGenerators.Add(this);
        transform.localPosition = transform.parent == null ? demonstratePos : Vector3.zero;
    }
    private void OnDestroy() {
        bulletGenerators.Remove(this);
    }
}
