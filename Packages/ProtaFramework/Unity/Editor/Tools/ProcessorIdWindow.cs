using System.Threading;
using Prota;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    public class ProcessorIdWindow : EditorWindow
    {
        int currentProcessorId;
		int currentCoreId;
		
        [MenuItem("ProtaFramework/Tools/Processor ID")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProcessorIdWindow>("Processor ID");
            window.titleContent = new GUIContent("Processor ID");
            window.Show();
        }
        
        void OnEnable()
        {
            RefreshData();
        }
        
        void RefreshData()
        {
            currentProcessorId = ProcessorUtils.GetCurrentThreadProcessorId();
			currentCoreId = ProcessorUtils.GetCurrentThreadCore();
        }
        
        void OnGUI()
        {
			RefreshData();
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("当前线程的处理器核心 ID:", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Processor ID:", GUIPreset.width[100]);
            EditorGUILayout.LabelField(currentProcessorId.ToString(), EditorStyles.textField);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Core ID:", GUIPreset.width[100]);
            EditorGUILayout.LabelField(currentCoreId.ToString(), EditorStyles.textField);
            EditorGUILayout.EndHorizontal();
			
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("此窗口显示当前 Unity 编辑器线程正在运行的处理器核心 ID。", MessageType.Info);
        }
    }
}

