using UnityEngine;


public abstract class EffectSegment
{
    public float start;
    public float end;

    public bool IsActive(float t) => t >= start && t <= end;

    public abstract void Apply(ref BulletRuntimeState state, float localTime);
}