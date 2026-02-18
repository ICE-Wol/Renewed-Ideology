using System;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using Unity.Profiling;

[Serializable]
public class BulletSnapshot
{
    private static readonly ProfilerMarker s_CheckCollisionMarker = new ProfilerMarker("BulletSystem.BulletSnapshot.CheckAllBulletCollision");

    [SerializeField]
    public List<BulletRuntimeState> states;

    public BulletSnapshot()
    {
        states = new();
    }

    public bool CheckAllBulletCollision(Vector2 playerPos)
    {
        using (s_CheckCollisionMarker.Auto())
        {
            foreach (var state in states)
            {
                if (state.isAlive == false)
                    continue;
                var distVector = state.position - playerPos;
                if (distVector.x > -1.5f && distVector.x < 1.5f && distVector.y > -1.5f && distVector.y < 1.5f)
                {
                    if (distVector.sqrMagnitude <= state.checkRadius * state.checkRadius)
                    {
                        state.isAlive = false;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}