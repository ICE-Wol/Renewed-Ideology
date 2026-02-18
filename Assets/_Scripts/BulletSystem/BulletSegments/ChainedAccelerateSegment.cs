using System;
using System.Collections.Generic;
using _Scripts.Tools;
using UnityEngine;

/// <summary>
/// 速度/角度分段拼接的加速段落，spd 与 angle 各自维护时间游标，可链式 .Spd(5f,2,3,linear).Spd(3f,3,2,easeInOut).Angle(...)
/// </summary>
[Serializable]
public class ChainedAccelerateSegment : BulletSegment, IBulletSegmentDSL
{
    [Serializable]
    public struct SpeedPhase
    {
        public float startTime;
        public float endTime;
        public float fromSpeed;
        public float toSpeed;
        public LerpType ease;
    }

    [Serializable]
    public struct AnglePhase
    {
        public float startTime;
        public float endTime;
        public float fromAngle;
        public float toAngle;
        public LerpType ease;
    }

    public List<SpeedPhase> speedPhases = new List<SpeedPhase>();
    public List<AnglePhase> anglePhases = new List<AnglePhase>();

    static float EvalPhase(float time, float phaseStart, float phaseEnd, float from, float to, LerpType ease)
    {
        if (phaseEnd <= phaseStart) return from;
        float t = Mathf.Clamp01((time - phaseStart) / (phaseEnd - phaseStart));
        return Calc.Lerp(from, to, t, ease);
    }

    public float GetSpeed(int tick)
    {
        float time = TimelineClock.GetTickTime(tick);
        if (speedPhases.Count == 0) return 0f;
        var first = speedPhases[0];
        var last = speedPhases[speedPhases.Count - 1];
        if (time <= first.startTime) return first.fromSpeed;
        if (time >= last.endTime) return last.toSpeed;
        foreach (var p in speedPhases)
        {
            if (time >= p.startTime && time <= p.endTime)
                return EvalPhase(time, p.startTime, p.endTime, p.fromSpeed, p.toSpeed, p.ease);
        }
        return last.toSpeed;
    }

    public float GetAngle(int tick)
    {
        float time = TimelineClock.GetTickTime(tick);
        if (anglePhases.Count == 0) return 0f;
        var first = anglePhases[0];
        var last = anglePhases[anglePhases.Count - 1];
        if (time <= first.startTime) return first.fromAngle;
        if (time >= last.endTime) return last.toAngle;
        foreach (var p in anglePhases)
        {
            if (time >= p.startTime && time <= p.endTime)
                return EvalPhase(time, p.startTime, p.endTime, p.fromAngle, p.toAngle, p.ease);
        }
        return last.toAngle;
    }

    public Vector2 GetSpeedVector(int tick) => new Vector2(Mathf.Cos(GetAngle(tick) * Mathf.Deg2Rad), Mathf.Sin(GetAngle(tick) * Mathf.Deg2Rad)) * GetSpeed(tick);

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        state.position += GetSpeedVector(tick) * TimelineClock.tickInterval;
        state.speed = GetSpeed(tick);
        state.direction = GetAngle(tick);
    }
}

/// <summary>
/// 链式加速段落构建器：Spd/Angle 各自追加时间，.End() 时提交 segment 并推进 tb.cursorTime。
/// </summary>
public class ChainedAccelerateBuilder
{
    public BulletTrack track;
    public ChainedAccelerateSegment segment;
    public float segmentStartTime;
    public float speedCursor;
    public float angleCursor;

    public ChainedAccelerateBuilder(BulletTrack track)
    {
        this.track = track;
        segmentStartTime = track.spawnTime;
        speedCursor = track.spawnTime;
        angleCursor = track.spawnTime;
        segment = new ChainedAccelerateSegment();
    }

    public ChainedAccelerateBuilder Speed(float duration, float fromSpeed, float toSpeed, LerpType ease)
    {
        segment.speedPhases.Add(new ChainedAccelerateSegment.SpeedPhase()
        {
            startTime = speedCursor,
            endTime = speedCursor + duration,
            fromSpeed = fromSpeed,
            toSpeed = toSpeed,
            ease = ease,
        });
        speedCursor += duration;
        return this;
    }

    public ChainedAccelerateBuilder Angle(float duration, float fromAngle, float toAngle, LerpType ease)
    {
        segment.anglePhases.Add(new ChainedAccelerateSegment.AnglePhase()
        {
            startTime = angleCursor,
            endTime = angleCursor + duration,
            fromAngle = fromAngle,
            toAngle = toAngle,
            ease = ease,
        });
        angleCursor += duration;
        return this;
    }

    public BulletTrack End()
    {
        float totalSpeed = speedCursor - segmentStartTime;
        float totalAngle = angleCursor - segmentStartTime;
        segment.startTime = segmentStartTime;
        segment.endTime = segmentStartTime + Mathf.Max(totalSpeed, totalAngle);
        track.bulletSegments.Add(segment);
        return track;
    }
}

public static class ChainedAccelerateSegmentDSL
{
    public static ChainedAccelerateBuilder ChainedAccelerate(this BulletTrack track)
    {
        return new ChainedAccelerateBuilder(track);
    }
}