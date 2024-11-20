using UnityEngine;
using System.Collections;
using System;
using _Scripts.EnemyBullet;

[ExecuteAlways]
public class ShowBasicInfo : MonoBehaviour
{

    public float fpsMeasuringDelta = 1f;
    public int TargetFrame = 60;

    private float timePassed;
    private long timePassedSystem;
    long timeSystem;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;
    private float m_lowFPS = 0.0f;
    TimeSpan span = new TimeSpan(0,0,1); // 1小时的时间跨度
    long ticksMesure; // 获取这个时间跨度对应的百纳秒数量

    //private int _totBulletNum => GameObject.FindObjectsByType<State>(FindObjectsSortMode.None).Length;
    private int _totBulletNum => State.bulletSet.Count;

    private void Start()
    {
        timePassed = 0.0f;
        ticksMesure = span.Ticks;
        timeSystem = DateTime.UtcNow.Ticks;
        TargetFrame = Application.targetFrameRate;
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;
        long delta = DateTime.UtcNow.Ticks - timeSystem;
        timePassedSystem += delta;
        timeSystem = DateTime.UtcNow.Ticks;
        if (timePassedSystem >= ticksMesure)
        {
            double second = (double)timePassedSystem / ticksMesure;
            m_FPS = m_FrameCount / (float)second;
            timePassedSystem = 0;
            timePassed = 0.0f;
            m_FrameCount = 0;
            m_lowFPS = float.MaxValue;
        }
        m_lowFPS = Mathf.Min((float)(1.0 / ((double)delta/span.Ticks)),m_lowFPS);
    }

    private void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;
        bb.normal.textColor =
            (m_FPS > TargetFrame * 3 / 4 ? Color.white : (m_FPS > TargetFrame * 1 / 4 ? Color.cyan : Color.red)) -
            new Color(0, 0, 0, 0.3f);
        bb.fontSize = Math.Max(6,(int)((16f)* Screen.height/960f));
        bb.alignment = TextAnchor.LowerRight;
        //居中显示FPS
        string vsyncData = "";
        if (QualitySettings.vSyncCount > 0)
        {
            vsyncData = $" (vsync {QualitySettings.vSyncCount})";
        }

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height),
            "Total Bullet Number: " + _totBulletNum + "\nAverage Fps: " + m_FPS.ToString("f1") +
            vsyncData + "\nLowest Fps Per Second: " + m_lowFPS.ToString("f1"), bb);
    }
}
