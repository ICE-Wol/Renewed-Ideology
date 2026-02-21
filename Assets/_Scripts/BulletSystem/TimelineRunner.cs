using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using _Scripts.Player;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimelineRunner : MonoBehaviour
{
    private static readonly ProfilerMarker s_UpdateMarker = new ProfilerMarker("BulletSystem.TimelineRunner.Update");
    private static readonly ProfilerMarker s_EvaluateMarker = new ProfilerMarker("BulletSystem.Pattern.Evaluate");
    private static readonly ProfilerMarker s_SyncMarker = new ProfilerMarker("BulletSystem.BulletManager.Sync");
    private static readonly ProfilerMarker s_CheckCollisionMarker = new ProfilerMarker("BulletSystem.Snapshot.CheckCollision");

    public static TimelineRunner instance;
    [SerializeField]
    public BulletManager bulletManager = new();
    [SerializeField]
    public BulletSnapshot snapshot = new();
    [SerializeField]
    public List<PatternBuilder> patternBuilders = new();

    public float timeStamp = 0f;
    public bool isStarted = false;
    void Awake()
    {
        timeStamp = Time.time;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        bulletManager.Awake();
    }

    /// <summary>
    /// 清空所有子弹并重新执行时间线初始化（与首次启动时 46-53 行逻辑一致）。
    /// 可在 Inspector 按钮或运行时 UI 中调用。
    /// </summary>
    [ContextMenu("Restart Timeline")]
    public void RestartTimeline()
    {
        snapshot.states.Clear();
        bulletManager.Sync(snapshot);
        TimelineClock.SetTime(0);
        //patternBuilders.Clear();
        //patternBuilders.Add(new PatternBuilder_Test_0_2());
        foreach (var patternBuilder in patternBuilders)
            patternBuilder.Build();
    }

    public void Update()
    {
        if (isStarted == false && Time.time - timeStamp > 2f)
        {
            isStarted = true;
            TimelineClock.SetTime(0);
            //patternBuilders.Add(new PatternBuilder_Test_MountainOfFaith());
            patternBuilders.Add(new PatternBuilder_Test_0_3_3());
            foreach (var patternBuilder in patternBuilders)
            {
                patternBuilder.Build();
                //snapshot = patternBuilders[0].pattern.Evaluate(0, snapshot);
            }
        }
        if (isStarted)
        {
            UpdateTick();
            
        }
    }
    public void UpdateTick()
    {
        int lastTick = TimelineClock.GetTickCount();
        TimelineClock.Tick(Time.deltaTime);
        int currentTick = TimelineClock.GetTickCount();

        bool hit = false;
        for (int i = 1; i <= currentTick - lastTick; i++)
        {
            using (s_UpdateMarker.Auto())
            {
                using (s_EvaluateMarker.Auto())
                {
                    foreach (var patternBuilder in patternBuilders)
                    {
                        patternBuilder.pattern.Evaluate(snapshot, lastTick + i);
                    }
                }
                using (s_CheckCollisionMarker.Auto())
                    hit |= snapshot.CheckAllBulletCollision(PlayerCtrl.instance.transform.position);
            }
        }
        using (s_SyncMarker.Auto())
            bulletManager.Sync(snapshot);
        if (hit)
        {
            PlayerCtrl.instance.GetHit();
        }
    }
}

public static class TimelineClock
{
    public static float Time { get; private set; }
    public static float Speed { get; set; } = 1f;
    public static int tickPerSecond = 60;
    public static float tickInterval => 1f / tickPerSecond;

    public static void Tick(float deltaTime)
    {
        Time += deltaTime * Speed;
    }
    public static void SetTime(float time)
    {
        Time = time;
    }

    //注意浮点误差，使用FloorToInt而非(int)(Time / tickInterval)
    public static int GetTickCount() => Mathf.FloorToInt(Time / tickInterval);
    public static int GetTickCount(float time) => Mathf.FloorToInt(time / tickInterval);
    public static float GetTickTime() => GetTickCount() * tickInterval;
    public static float GetTickTime(int tick) => tick * tickInterval;
    public static bool IsAtTick() => Mathf.Approximately(Time, GetTickTime());
    public static bool IsAtTick(float time) => Mathf.Approximately(time, GetTickTime(GetTickCount(time)));
}
