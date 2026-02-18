using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

[Serializable]
public class BulletTrack
{
    private static readonly ProfilerMarker s_EvaluateMarker = new ProfilerMarker("BulletSystem.BulletTrack.Evaluate");

    public PatternTimeline parentTimeline;

    public BulletTrack()
    {
        startPosition = Vector2.zero;
        startRotation = 0f;
        parentID = -1;
        layer = 0;
    }
    public int id;
    /// <summary>所属 emitter 编号，0..n-1；-1 表示无 emitter 或本 track 即为 emitter。</summary>
    public int emitterId = -1;
    public int parentID;
    public int layer = 0;
    public float spawnTime;
    public int SpawnTick => TimelineClock.GetTickCount(spawnTime);
    public float despawnTime;
    public int DespawnTick => TimelineClock.GetTickCount(despawnTime);

    public Vector2 startPosition;
    public float startRotation;

    public Func<Vector2> PositionGetter;
    public Func<float> RotationGetter;

    public float cursorTime;

    public bool isCleared = false;


    [SerializeReference]
    public List<BulletSegment> bulletSegments = new();

    /// <summary>按 StartTick 排序的 segment 列表，懒构建。</summary>
    [NonSerialized] private List<BulletSegment> _segmentsSortedByStart;
    /// <summary>下一个待加入活跃队列的 segment 在 _segmentsSortedByStart 中的下标。</summary>
    [NonSerialized] private int _nextToActivate;
    /// <summary>当前 tick 下活跃的 segment 队列（StartTick &lt;= tick 且 EndTick+1 &gt;= tick）。</summary>
    [NonSerialized] private List<BulletSegment> _activeSegments;
    [NonSerialized] private int _lastProcessedTick = int.MinValue;

    public bool IsActiveTick(int tick) => tick >= SpawnTick && tick <= DespawnTick;

    private void EnsureSegmentsSorted()
    {
        if (_segmentsSortedByStart != null && _segmentsSortedByStart.Count == bulletSegments.Count)
            return;
        _segmentsSortedByStart = new List<BulletSegment>(bulletSegments);
        _segmentsSortedByStart.Sort((a, b) => a.StartTick.CompareTo(b.StartTick));
        _nextToActivate = 0;
        _activeSegments ??= new List<BulletSegment>();
        _activeSegments.Clear();
        _lastProcessedTick = int.MinValue;
    }

    /// <summary>
    /// 基于子弹前一刻的状态，计算当前 tick 的状态（内部用 tick 计算）。
    /// 仅遍历当前 tick 活跃的 segment（按左端点排序维护的队列）。
    /// </summary>
    public BulletRuntimeState Evaluate(BulletRuntimeState state, int tick)
    {
        if (state.isAlive == false)
            return state;
        if (!IsActiveTick(tick))
            return null;
        using (s_EvaluateMarker.Auto())
        {
            EnsureSegmentsSorted();

            if (tick < _lastProcessedTick)
            {
                _nextToActivate = 0;
                _activeSegments.Clear();
            }
            _lastProcessedTick = tick;

            while (_nextToActivate < _segmentsSortedByStart.Count)
            {
                var seg = _segmentsSortedByStart[_nextToActivate];
                if (seg.StartTick > tick)
                    break;
                _activeSegments.Add(seg);
                _nextToActivate++;
            }

            for (int i = _activeSegments.Count - 1; i >= 0; i--)
            {
                var seg = _activeSegments[i];
                if (seg.EndTick + 1 < tick)
                {
                    _activeSegments.RemoveAt(i);
                    continue;
                }
                if (tick <= seg.EndTick || tick == seg.EndTick + 1)
                    seg.Apply(ref state, tick);
            }
        }
        return state;
    }

    /// <summary>
    /// 入口：读入 time，转为 tick 后调用 Evaluate(state, tick)
    /// </summary>
    public BulletRuntimeState Evaluate(BulletRuntimeState state, float time)
    {
        int tick = TimelineClock.GetTickCount(time);
        return Evaluate(state, tick);
    }

    public BulletRuntimeState Evaluate(float time)
    {
        int tick = TimelineClock.GetTickCount(time);
        return Evaluate(tick);
    }

    public BulletRuntimeState Evaluate(int tick)
    {
        if (tick < SpawnTick || tick > DespawnTick) return null;
        else
        {
            BulletRuntimeState state = new()
            {
                id = id,
                position = startPosition,
                rotation = startRotation,
                color = Color.white,
                isErased = false
            };
            for (int i = SpawnTick; i <= tick; i++)
            {
                state = Evaluate(state, i);
            }
            return state;
        }
    }
}