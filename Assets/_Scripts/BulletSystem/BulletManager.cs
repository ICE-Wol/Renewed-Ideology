using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.Profiling;

[Serializable]
public class BulletManager
{
    private static readonly ProfilerMarker s_SyncApplyMarker = new ProfilerMarker("BulletSystem.BulletManager.Sync.Apply");
    private static readonly ProfilerMarker s_SyncRecycleMarker = new ProfilerMarker("BulletSystem.BulletManager.Sync.Recycle");

    public Dictionary<BulletRuntimeState, BulletInstance> instances = new();
    public BulletInstance bulletPrefab;
    public BulletBreakCtrl bulletBreakPrefab;
    public ObjectPool<BulletInstance> bulletPool;
    public ObjectPool<BulletBreakCtrl> bulletBreakParticlePool;
    public void Awake()
    {
        bulletPool = new ObjectPool<BulletInstance>(
            () => GameObject.Instantiate(bulletPrefab),
            (obj) => obj.gameObject.SetActive(true),
            (obj) => obj.gameObject.SetActive(false),
            (obj) => GameObject.Destroy(obj.gameObject));
        bulletBreakParticlePool = new ObjectPool<BulletBreakCtrl>(
            () => GameObject.Instantiate(bulletBreakPrefab),
            (obj) => obj.gameObject.SetActive(true),
            (obj) => obj.gameObject.SetActive(false),
            (obj) => GameObject.Destroy(obj.gameObject));
    }



    public void Sync(BulletSnapshot snapshot)
    {
        HashSet<BulletRuntimeState> alive = new();

        using (s_SyncApplyMarker.Auto())
        {
            foreach (var state in snapshot.states)
            {
                if (state.isAlive == false)
                    continue;
                alive.Add(state);

                if (!instances.TryGetValue(state, out var go))
                {
                    go = bulletPool.Get();
                    instances[state] = go;
                }
                go.ApplyState(state);
            }
        }

        var toRemove = new List<BulletRuntimeState>();
        foreach (var kv in instances)
            if (!alive.Contains(kv.Key))
                toRemove.Add(kv.Key);

        using (s_SyncRecycleMarker.Auto())
        {
            foreach (var state in toRemove)
            {
                var particle = bulletBreakParticlePool.Get();
                particle.transform.position = instances[state].transform.position;
                particle.SetColor(state.color);
                bulletPool.Release(instances[state]);
                instances.Remove(state);
            }
        }
    }
}
