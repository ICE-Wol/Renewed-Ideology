using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.Tools;
using UnityEngine;
public class BulletTrackBuilder
{
    public float cursorTime;

    public PatternTimeline pattern;

    public BulletTrack Track(
        float startTime,
        float endTime,
        BulletType bulletType,
        float fogDuration)
    {
        var track = pattern.CreateTrack();
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        track.bulletSegments.Add(new BeginFogSegment()
        {
            startTime = startTime,
            endTime = startTime + fogDuration,
            bulletType = bulletType,
        });
        cursorTime = track.spawnTime;
        return track;
    }

    public BulletTrack Emmitor(
        float startTime,
        float endTime)
    {
        var track = pattern.CreateEmitterTrack();
        track.spawnTime = startTime;
        track.despawnTime = endTime;
        cursorTime = track.spawnTime;
        return track;
    }
}

public static class BulletTrackBuilderDSL
{
    public static BulletTrack CirclePos(
        this BulletTrack track,
        Vector2 oriPos,
        float radius,
        float angle)
    {
        track.startPosition = oriPos + Calc.Deg2Dir(angle) * radius;
        track.startRotation = angle;
        return track;
    }

    public static BulletTrack CirclePos(
        this BulletTrack track,
        Vector2 oriPos,
        float radius,
        float angle,
        float angleOffset)
    {
        track.startPosition = oriPos + Calc.Deg2Dir(angle + angleOffset) * radius;
        track.startRotation = angle + angleOffset;
        return track;
    }

    // ========== PositionOnCircle 系列（BulletTrack 扩展，设置 startPosition + startRotation 并返回 track） ==========

    /// <summary>圆上按半径和角度取点</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, float radius, float angle)
    {
        track.startPosition = Calc.PositionOnCircleV2(radius, angle);
        track.startRotation = angle;
        return track;
    }

    /// <summary>以 origin 为圆心的圆上按半径和角度取点</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, Vector2 origin, float radius, float angle)
    {
        track.startPosition = Calc.PositionOnCircleV2(origin, radius, angle);
        track.startRotation = angle;
        return track;
    }

    /// <summary>圆上取点，angle = startAngle + i * step，设置 track 并返回</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, float radius, float startAngle, float step, int i, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(radius, startAngle, step, i, centered);
        track.startRotation = startAngle + i * step;
        return track;
    }

    /// <summary>以 origin 为圆心的圆上取点，angle = startAngle + i * step</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, Vector2 origin, float radius, float startAngle, float step, int i, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(origin, radius, startAngle, step, i, centered);
        track.startRotation = startAngle + i * step;
        return track;
    }

    /// <summary>圆上取点，共 count 个点，step = 360f/count</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, float radius, float startAngle, int i, int count, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(radius, startAngle, i, count, centered);
        float step = 360f / count;
        track.startRotation = centered ? startAngle + (i - (count - 1) * 0.5f) * step : startAngle + i * step;
        return track;
    }

    /// <summary>以 origin 为圆心的圆上取点，共 count 个点，step = 360f/count</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, Vector2 origin, float radius, float startAngle, int i, int count, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(origin, radius, startAngle, i, count, centered);
        float step = 360f / count;
        track.startRotation = centered ? startAngle + (i - (count - 1) * 0.5f) * step : startAngle + i * step;
        return track;
    }

    /// <summary>圆上 [startAngle, endAngle] 均匀 count 个点，第 i 个点</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, float radius, float startAngle, float endAngle, int i, int count)
    {
        track.startPosition = Calc.PositionOnCircleV2(radius, startAngle, endAngle, i, count);
        float step = count <= 1 ? 0f : (endAngle - startAngle) / (count - 1);
        track.startRotation = startAngle + i * step;
        return track;
    }

    /// <summary>以 origin 为圆心的圆上 [startAngle, endAngle] 均匀 count 个点，第 i 个点</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, Vector2 origin, float radius, float startAngle, float endAngle, int i, int count)
    {
        track.startPosition = Calc.PositionOnCircleV2(origin, radius, startAngle, endAngle, i, count);
        float step = count <= 1 ? 0f : (endAngle - startAngle) / (count - 1);
        track.startRotation = startAngle + i * step;
        return track;
    }

    /// <summary>圆上均匀 count 个点，起始角 0°</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, float radius, int i, int count, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(radius, 0f, i, count, centered);
        float step = 360f / count;
        track.startRotation = centered ? (i - (count - 1) * 0.5f) * step : i * step;
        return track;
    }

    /// <summary>以 origin 为圆心的圆上均匀 count 个点，起始角 0°</summary>
    public static BulletTrack PositionOnCircle(this BulletTrack track, Vector2 origin, float radius, int i, int count, bool centered = false)
    {
        track.startPosition = Calc.PositionOnCircleV2(origin, radius, i, count, centered);
        float step = 360f / count;
        track.startRotation = centered ? (i - (count - 1) * 0.5f) * step : i * step;
        return track;
    }
}
