using System;
using _Scripts;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;
using Unity.Profiling;

[Serializable]
public abstract class BulletSegment
{
    private static readonly ProfilerMarker s_ApplyMarker = new ProfilerMarker("BulletSystem.BulletSegment.Apply");

    public float startTime;

    public int StartTick => TimelineClock.GetTickCount(startTime);
    public float endTime;
    public int EndTick => TimelineClock.GetTickCount(endTime);
    public int DurationCount => (EndTick - StartTick + 1);

    public bool ResetRelativeStates;

    /// <summary>
    /// 是否为并行段落
    /// </summary>
    public bool isParallel;

    /// <summary>
    /// 用于存储非并行段落的游标时间
    /// </summary>
    public float cursorTime;

    // Unity事件
    public virtual void OnSegmentStart(ref BulletRuntimeState state) { }
    public virtual void OnSegmentEnd(ref BulletRuntimeState state) { }

    // 私有字段用于跟踪事件是否已触发
    private bool _startEventTriggered;
    private bool _endEventTriggered;

    public bool IsActiveTick(int tick) => tick >= StartTick && tick <= EndTick;

    /// <summary>段内进度 [0,1]，单 tick 时段返回 0 避免除零。</summary>
    public float GetPrecentage(int tick)
    {
        if (DurationCount <= 1) return 0f;
        return (tick - StartTick) / (DurationCount - 1f);
    }

    /// <summary>
    /// 重置事件触发状态，用于重新开始segment
    /// </summary>
    public virtual void ResetEvents()
    {
        _startEventTriggered = false;
        _endEventTriggered = false;
    }

    public virtual void Apply(ref BulletRuntimeState state, int tick)
    {
        if (!_startEventTriggered && tick >= StartTick)
        {
            OnSegmentStart(ref state);
            _startEventTriggered = true;
        }

        if (!_endEventTriggered && tick >= EndTick)
        {
            OnSegmentEnd(ref state);
            _endEventTriggered = true;
        }

        if (!IsActiveTick(tick))
            return;
    }
    public void Append(BulletTrack track)
    {
        track.bulletSegments.Add(this);
    }
}

public interface IBulletSegmentDSL
{
    void Append(BulletTrack track);
}

[Serializable]
public class FlowerSegment : BulletSegment
{
    public int petalCount;
    public int petalIndex;
    public Vector2 petalAngleChange;
    public float petalRadius;

    public BulletSegment fallBackSegment;

    public bool isFallBackInited = false;

    public float GetAngle(int tick) => Mathf.Lerp(petalAngleChange.x, petalAngleChange.y, GetPrecentage(tick));

    public Vector2 GetPosition(int tick)
    {
        return Calc.Deg2Dir(GetAngle(tick) + 360f / petalCount * petalIndex) * petalRadius;
    }

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        // if (TimelineRunner.bulletStateToIDMap[state.parentID].isAlive)
        // {
        //     base.Apply(ref state, tick);
        //     state.position = GetPosition(tick);
        //     state.rotation = 360f / petalCount * petalIndex + 90f + GetAngle(tick);
        // }
        // else
        // {
        //     if (!isFallBackInited)
        //     {
        //         isFallBackInited = true;
        //         float fallBackInterval = fallBackSegment.endTime - fallBackSegment.startTime;
        //         fallBackSegment.startTime = TimelineClock.GetTickTime(tick);
        //         fallBackSegment.endTime = TimelineClock.GetTickTime(tick) + fallBackInterval;
        //     }
        //     fallBackSegment.Apply(ref state, tick);
        // }
    }

}

[Serializable]
public class LinearAccelerateSegment : BulletSegment, IBulletSegmentDSL
{
    public BulletRuntimeState initialState;

    /// <summary>
    /// 速度变化量, 为开始到结束的两端值，注意不能小于0
    /// </summary>
    public Vector2 speedChange;
    public Vector2 angleChange;

    public float GetSpeed(int tick) => Mathf.Lerp(speedChange.x, speedChange.y, GetPrecentage(tick));
    public float GetAngle(int tick) => Mathf.Lerp(angleChange.x, angleChange.y, GetPrecentage(tick));

    public Vector2 GetSpeedVector(int tick) => new Vector2(Mathf.Cos(GetAngle(tick) * Mathf.Deg2Rad), Mathf.Sin(GetAngle(tick) * Mathf.Deg2Rad)) * GetSpeed(tick);

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        state.position += GetSpeedVector(tick) * TimelineClock.tickInterval;
        state.speed = GetSpeed(tick);
        state.direction = GetAngle(tick);
    }
}

public static class LinearAccelerateSegmentDSL
{
    public static BulletTrack LinearAccelerate(
        this BulletTrack track,
        float duration,
        Vector2 speedChange,
        Vector2 angleChange)
    {
        track.bulletSegments.Add(new LinearAccelerateSegment()
        {
            startTime = track.cursorTime,
            endTime = track.cursorTime + duration,
            speedChange = speedChange,
            angleChange = angleChange,
        });
        track.cursorTime += duration;
        return track;
    }

    public static BulletTrack LinearAccelerate(
        this BulletTrack track,
        Vector2 speedChange,
        Vector2 angleChange,
        Vector2 timePeriod)
    {
        track.bulletSegments.Add(new LinearAccelerateSegment()
        {
            startTime = timePeriod.x,
            endTime = timePeriod.y,
            speedChange = speedChange,
            angleChange = angleChange,
        });
        return track;
    }
}
[Serializable]
public class RgbColorChangeSegment : BulletSegment
{
    public Color colorStart;
    public Color colorEnd;

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        state.color = Color.Lerp(colorStart, colorEnd, GetPrecentage(tick));
    }
}

public static class RgbColorChangeSegmentDSL
{
    public static BulletTrack RgbColorChange(
        this BulletTrack track,
        float startTime,
        float endTime,
        Color colorStart,
        Color colorEnd)
    {
        track.bulletSegments.Add(new RgbColorChangeSegment()
        {
            startTime = startTime,
            endTime = endTime,
            colorStart = colorStart,
            colorEnd = colorEnd,
        });
        return track;
    }
}

[Serializable]
public class HSVColorChangeSegment : BulletSegment
{
    public Color colorStart;
    public Color colorEnd;

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        float t = GetPrecentage(tick);
        state.color = Calc.LerpColorInHSV(colorStart, colorEnd, t);
    }
}

public static class HSVColorChangeSegmentDSL
{
    public static BulletTrack HSVColorChange(
        this BulletTrack track,
        float startTime,
        float endTime,
        Color colorStart, Color colorEnd)
    {
        track.bulletSegments.Add(new HSVColorChangeSegment()
        {
            startTime = startTime,
            endTime = endTime,
            colorStart = colorStart,
            colorEnd = colorEnd,
        });
        return track;
    }
    
    public static BulletTrack HSVColorChangeDuration(
        this BulletTrack track,
        float startTime,
        float duration,
        Color colorStart, Color colorEnd)
    {
        track.bulletSegments.Add(new HSVColorChangeSegment()
        {
            startTime = startTime,
            endTime = startTime + duration,
            colorStart = colorStart,
            colorEnd = colorEnd,
        });
        return track;
    }
}

/// <summary>
/// 到点只触发一次：在 triggerTime 对应的那一 tick 触发 OnTrigger，不参与每帧状态计算。
/// 用法：继承本类并重写 OnTrigger(ref state)；只需设置 triggerTime，无需设 startTime/endTime。
/// </summary>
[Serializable]
public abstract class TriggerSegment : BulletSegment
{
    /// <summary>触发时刻（秒），内部会同步为 startTime=endTime，使该 tick 只生效一次。</summary>
    public float triggerTime
    {
        get => startTime;
        set => startTime = endTime = value;
    }
}

public class MarkForDestroySegment : TriggerSegment
{
    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        state.isAlive = false;
    }
}

public class CirclePositionFromRefBullet : TriggerSegment
{
    public BulletTrack refBulletTrack;
    public float angle;
    public float radius;

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        var timeline = refBulletTrack.parentTimeline;
        state.position = timeline.bulletStateToIDMap[refBulletTrack.id].position + Calc.Deg2Dir(angle) * radius;
        state.rotation = angle;
    }
}

public static class CirclePositionFromRefBulletDSL
{
    public static BulletTrack CirclePositionFromRefBullet(
        this BulletTrack track,
        BulletTrack emmitorTrack,
        float triggerTime,
        float angle,
        float radius)
    {
        track.emitterId = emmitorTrack.id;
        track.bulletSegments.Add(new CirclePositionFromRefBullet()
        {
            triggerTime = triggerTime,
            refBulletTrack = emmitorTrack,
            angle = angle,
            radius = radius,
        });
        return track;
    }
}

public class ExtendFromRefBullet : TriggerSegment
{
    public BulletTrack refBulletTrack;

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        state.position = refBulletTrack.parentTimeline.bulletStateToIDMap[refBulletTrack.id].position;
        state.rotation = refBulletTrack.parentTimeline.bulletStateToIDMap[refBulletTrack.id].rotation;
    }
}

public static class ExtendFromRefBulletDSL
{
    public static BulletTrack ExtendFromRefBullet(
        this BulletTrack track,
        BulletTrack refBulletTrack,
        float triggerTime)
    {
        track.bulletSegments.Add(new ExtendFromRefBullet()
        {
            triggerTime = triggerTime,
            refBulletTrack = refBulletTrack,
        });
        return track;
    }
}

[Serializable]
public class RotationFollowSegment : BulletSegment
{
    public float rate = 4f;
    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        state.rotation.ApproachRef(state.direction, 4f);
    }
}

public static class RotationFollowSegmentDSL
{
    public static BulletTrack RotationFollow(
        this BulletTrack track,
        float startTime,
        float endTime,
        float rate)
    {
        track.bulletSegments.Add(new RotationFollowSegment()
        {
            startTime = startTime,
            endTime = endTime,
            rate = rate
        });
        return track;
    }

    public static BulletTrack RotationFollow(
        this BulletTrack track,
        float rate)
    {
        track.bulletSegments.Add(new RotationFollowSegment()
        {
            startTime = track.spawnTime,
            endTime = track.despawnTime,
            rate = rate,
        });
        return track;
    }
}
[Serializable]
public class BeginFogSegment : BulletSegment
{
    public float startScale = 4f;
    public float startAlpha = 0f;
    public BulletType bulletType;
    public Color color;
    public override void OnSegmentStart(ref BulletRuntimeState state)
    {
        //state.sprite = BulletSpriteCollection.instance.bulletFog;
        if (GameManager.Manager.enemyBulletBasics.bulletBasics[(int)bulletType].size == BulletSize.Small)
        {
            state.sprite = GameManager.Manager.enemyBulletBasics.GetBulletSprite(BulletType.Point);
        }
        else
        {
            state.sprite = GameManager.Manager.enemyBulletBasics.GetBulletSprite(bulletType);
        }
        state.type = bulletType;
        //在这里设置颜色会导致初始出现黑圈
        state.color = color;
        state.alpha = startAlpha;
    }

    public override void OnSegmentEnd(ref BulletRuntimeState state)
    {
        //state.sprite = BulletSpriteCollection.instance.bulletSprites[(int)bulletType];
        state.sprite = GameManager.Manager.enemyBulletBasics.GetBulletSprite(bulletType);
        //弹雾阶段没有判定
        state.checkRadius = GameManager.Manager.enemyBulletBasics.bulletBasics[(int)bulletType].checkRadius;
    }

    public override void Apply(ref BulletRuntimeState state, int tick)
    {
        base.Apply(ref state, tick);
        float curScale = Mathf.Lerp(startScale, 1f, GetPrecentage(tick));
        float curAlpha = Mathf.Lerp(startAlpha, 1f, GetPrecentage(tick));
        state.scale = new Vector2(curScale, curScale);
        state.alpha = curAlpha;
    }
}

public static class BeginFogSegmentDSL
{
    public static BulletTrack BeginFog(
        this BulletTrack track,
        float startTime,
        float duration,
        BulletType bulletType, Color color)
    {
        track.bulletSegments.Add(new BeginFogSegment()
        {
            startTime = startTime,
            endTime = startTime + duration,
            bulletType = bulletType,
            color = color,
        });
        return track;
    }
}
public struct BulletRuntimeStateResetTag
{
    public bool resetPosition;
    public Vector2 position;
    public bool resetColor;
    public Color color;
}