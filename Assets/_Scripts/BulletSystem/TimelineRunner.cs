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
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        bulletManager.Awake();
        
        patternBuilders.Add(new PatternBuilder_Test_MountainOfFaith());
        //patternBuilders.Add(new PatternBuilder_Test_0_2());
        foreach (var patternBuilder in patternBuilders)
        {
            patternBuilder.Build();
            snapshot = patternBuilders[0].pattern.Evaluate(0, snapshot);
        }
    }
    void Update()
    {
        // // 检测数字键输入并输出对应数字
        // if (Keyboard.current != null)
        // {
        //     // 检查数字键 0-9
        //     if (Keyboard.current.digit0Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 0"); snapshot = pattern.Evaluate(0); TimelineClock.SetTime(0); }
        //     else if (Keyboard.current.digit1Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 1"); snapshot = pattern.Evaluate(1); TimelineClock.SetTime(1); }
        //     else if (Keyboard.current.digit2Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 2"); snapshot = pattern.Evaluate(2); TimelineClock.SetTime(2); }
        //     else if (Keyboard.current.digit3Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 3"); snapshot = pattern.Evaluate(3); TimelineClock.SetTime(3); }
        //     else if (Keyboard.current.digit4Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 4"); snapshot = pattern.Evaluate(4); TimelineClock.SetTime(4); }
        //     else if (Keyboard.current.digit5Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 5"); snapshot = pattern.Evaluate(5); TimelineClock.SetTime(5); }
        //     else if (Keyboard.current.digit6Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 6"); snapshot = pattern.Evaluate(6); TimelineClock.SetTime(6); }
        //     else if (Keyboard.current.digit7Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 7"); snapshot = pattern.Evaluate(7); TimelineClock.SetTime(7); }
        //     else if (Keyboard.current.digit8Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 8"); snapshot = pattern.Evaluate(8); TimelineClock.SetTime(8); }
        //     else if (Keyboard.current.digit9Key.wasPressedThisFrame) { Debug.Log("按下了数字键: 9"); snapshot = pattern.Evaluate(9); TimelineClock.SetTime(9); }
        //
        // }



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
    public static int tickPerSecond = 90;
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
