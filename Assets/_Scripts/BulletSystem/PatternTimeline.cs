using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PatternTimeline
{
    private int idCounter = 0;
    public int GetNewID()
    {
        var id = idCounter;
        idCounter++;
        return id;
    }
    public int layerCount = 5;
    /// <summary>Emitter 轨道，先于所有 bullet 计算。与 bullet 共享编号，按生成顺序自然递增。</summary>
    public List<BulletTrack> emitterTracks;
    public List<BulletTrack> bulletTracks;
    public Dictionary<int, BulletRuntimeState> bulletStateToIDMap = new();
    public Dictionary<int, BulletTrack> bulletTrackToIDMap = new();

    public PatternTimeline()
    {
        emitterTracks = new List<BulletTrack>();
        bulletTracks = new List<BulletTrack>();
    }

    /// <summary>创建轨道，用于随后添加到 bulletTracks。</summary>
    public BulletTrack CreateTrack() {
        var track = new BulletTrack();
        track.id = GetNewID();
        bulletTracks.Add(track);
        track.parentTimeline = this;
        bulletTrackToIDMap[track.id] = track;
        return track;
    }

    /// <summary>创建 emitter 轨道，用于随后添加到 emitterTracks。</summary>
    public BulletTrack CreateEmitterTrack() {
        var track = new BulletTrack();
        track.emitterId = -1;
        track.id = GetNewID();
        track.parentTimeline = this;
        emitterTracks.Add(track);
        bulletTrackToIDMap[track.id] = track;
        return track;
    }

    public BulletSnapshot currentSnapshot = new();
    /// <summary>入口：只读入 time，内部统一转为 tick 计算。</summary>
    public BulletSnapshot Evaluate(BulletSnapshot snapshot, float time)
    {
        int tick = TimelineClock.GetTickCount(time);
        Evaluate(snapshot, tick);
        return snapshot;
    }

    private const int BatchSize = 100;

    public void Evaluate(BulletSnapshot snapshot, int tick)
    {
        // 1. 先计算 emitter，与 bullet 按生成顺序共享编号
        for (int i = 0; i < emitterTracks.Count; i++)
        {
            var track = emitterTracks[i];
            if (track.IsActiveTick(tick))
            {
                if (!bulletStateToIDMap.TryGetValue(track.id, out var state))
                {
                    var newState = new BulletRuntimeState()
                    {
                        id = track.id,
                        layer = track.layer,
                        parentID = track.parentID,
                        position = track.startPosition,
                        rotation = track.startRotation,
                        direction = track.startRotation,
                        color = Color.white,
                    };
                    snapshot.states.Add(newState);
                    bulletStateToIDMap[track.id] = newState;
                }
                else
                {
                    bulletTrackToIDMap[state.id].Evaluate(state, tick);
                }
            }
            else if (track.DespawnTick < tick && !track.isCleared)
            {
                if (bulletStateToIDMap.TryGetValue(track.id, out var st))
                {
                    snapshot.states.Remove(st);
                    bulletStateToIDMap.Remove(track.id);
                }
                track.isCleared = true;
            }
        }

        if (bulletTracks.Count == 0)
        {
            ApplyParentOffsetByLayer(snapshot);
            return;
        }

            int batchCount = (bulletTracks.Count + BatchSize - 1) / BatchSize;
            var batchNewStates = new List<BulletRuntimeState>[batchCount];
            var batchRemovals = new List<(int id, BulletTrack track)>[batchCount];
            for (int b = 0; b < batchCount; b++)
            {
                batchNewStates[b] = new List<BulletRuntimeState>();
                batchRemovals[b] = new List<(int, BulletTrack)>();
            }

            Parallel.For(0, batchCount, batchIdx =>
            {
                int start = batchIdx * BatchSize;
                int end = Math.Min(start + BatchSize, bulletTracks.Count);
                var newStates = batchNewStates[batchIdx];
                var removals = batchRemovals[batchIdx];

                for (int i = start; i < end; i++)
                {
                    var track = bulletTracks[i];
                    if (track.IsActiveTick(tick))
                    {
                        if (!bulletStateToIDMap.TryGetValue(track.id, out var state))
                        {
                            Vector2 pos = track.startPosition;
                            float rot = track.startRotation;
                            float dir = track.startRotation;
                            // if (track.emitterId >= 0 && bulletStateToIDMap.TryGetValue(track.emitterId, out var emitterState))
                            // {
                            //     pos += emitterState.position;
                            //     rot += emitterState.rotation;
                            //     dir += emitterState.direction;
                            // }
                            if (track.parentID != -1)
                            {
                                pos += bulletTrackToIDMap[track.parentID].startPosition;
                                rot += bulletTrackToIDMap[track.parentID].startRotation;
                                dir += bulletTrackToIDMap[track.parentID].startRotation;
                            }
                            newStates.Add(new BulletRuntimeState()
                            {
                                id = track.id,
                                layer = track.layer,
                                parentID = track.parentID,
                                position = pos,
                                rotation = rot,
                                color = Color.white,
                            });
                        }
                        else
                        {
                            bulletTrackToIDMap[state.id].Evaluate(state, tick);
                        }
                    }
                    else if (track.DespawnTick < tick && !track.isCleared)
                    {
                        removals.Add((track.id, track));
                    }
                }
            });

            // 合并：先添加新状态，再处理移除
            for (int b = 0; b < batchCount; b++)
            {
                foreach (var bulletState in batchNewStates[b])
                {
                    snapshot.states.Add(bulletState);
                    bulletStateToIDMap[bulletState.id] = bulletState;
                }
            }
            for (int b = 0; b < batchCount; b++)
            {
                foreach (var (id, track) in batchRemovals[b])
                {
                    if (bulletStateToIDMap.TryGetValue(id, out var st))
                    {
                        snapshot.states.Remove(st);
                        bulletStateToIDMap.Remove(id);
                    }
                    track.isCleared = true;
                }
            }

            ApplyParentOffsetByLayer(snapshot);
    }

    /// <summary>按 layer 缓存 state，再按层顺序应用父节点偏移（保证父先于子）。</summary>
    private void ApplyParentOffsetByLayer(BulletSnapshot snapshot)
    {
        var statesByLayer = new List<BulletRuntimeState>[layerCount];
        for (int i = 0; i < layerCount; i++)
            statesByLayer[i] = new List<BulletRuntimeState>();
        foreach (var state in snapshot.states)
        {
            if (state.layer >= 0 && state.layer < layerCount)
                statesByLayer[state.layer].Add(state);
        }
        for (int layer = 0; layer < layerCount; layer++)
        {
            foreach (var state in statesByLayer[layer])
            {
                if (state.parentID == -1) continue;
                if (bulletStateToIDMap.TryGetValue(state.parentID, out var parent))
                {
                    state.position += parent.position;
                    state.rotation += parent.rotation;
                }
            }
        }
    }

    /// <summary>入口：只读入 time，内部统一转为 tick 计算。</summary>
    public BulletSnapshot Evaluate(float time, BulletSnapshot snapshot)
    {
        int tick = TimelineClock.GetTickCount(time);
        for (int i = 0; i < emitterTracks.Count; i++)
        {
            var track = emitterTracks[i];
            if (track.IsActiveTick(tick))
            {
                var state = track.Evaluate(tick);
                if (state != null) snapshot.states.Add(state);
            }
        }
        foreach (var track in bulletTracks)
        {
            if (track.IsActiveTick(tick))
            {
                var state = track.Evaluate(tick);
                if (state != null) snapshot.states.Add(state);
            }
        }
        return snapshot;
    }
}
