using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimelineRunner))]
public class TimelineRunnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var runner = (TimelineRunner)target;
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Restart Timeline（清空子弹并重新执行）"))
            runner.RestartTimeline();
        EditorGUI.EndDisabledGroup();
        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("运行时按钮可用。", MessageType.None);
    }
}
