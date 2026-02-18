using System;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class EditorLagSimulator : EditorWindow
{
    private bool isEnabled = false;
    private int lagMilliseconds = 16; // 默认16ms，约60fps的帧时间
    private Stopwatch stopwatch = new Stopwatch();
    
    [MenuItem("Tools/Editor Lag Simulator")]
    public static void ShowWindow()
    {
        var window = GetWindow<EditorLagSimulator>();
        window.titleContent = new GUIContent("Editor Lag Simulator");
        window.Show();
    }
    
    void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }
    
    void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }
    
    void OnUpdate()
    {
        if (!isEnabled)
            return;
        
        stopwatch.Restart();
		while(stopwatch.ElapsedMilliseconds < lagMilliseconds)
		{
			Thread.Sleep(1); 
		}
        stopwatch.Stop();
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Editor Lag Simulator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        isEnabled = EditorGUILayout.Toggle("Enable Lag Simulation", isEnabled);
        
        EditorGUI.BeginDisabledGroup(!isEnabled);
        lagMilliseconds = EditorGUILayout.IntSlider("Lag (ms)", lagMilliseconds, 0, 100);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            $"Current lag: {lagMilliseconds}ms\n" +
            $"Simulated FPS: {(lagMilliseconds > 0 ? 1000.0 / lagMilliseconds : 0):F1}",
            MessageType.Info
        );
    }
}

