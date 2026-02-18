using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Prota.Unity;


namespace Prota.Editor
{

    [CustomEditor(typeof(RangeQueryManager))]
    public class RangeQueryManagerInspector : UnityEditor.Editor
    {
        void OnEnable()
        {
            EditorApplication.update += Repaint;
        }
        
        void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var manager = (RangeQueryManager)target;
            
            if(!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("运行时数据仅在 Play Mode 下可用", MessageType.Info);
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Layer 统计信息", EditorStyles.boldLabel);
            
            // 获取所有定义的 layer
            var allLayers = RangeLayer.nameToLayer.Values;
            
            EditorGUILayout.BeginVertical("box");
            
            // 表头
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layer", EditorStyles.boldLabel, GUIPreset.width[150]);
            EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel, GUIPreset.width[80]);
            EditorGUILayout.LabelField("Queries", EditorStyles.boldLabel, GUIPreset.width[80]);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(2);
            
            // 显示每个 layer 的统计信息
            foreach(var layer in allLayers)
            {
                var nodeCount = manager.GetNodeCount(layer);
                var queryCount = manager.GetQueryCount(layer);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(layer.ToString(), GUIPreset.width[150]);
                EditorGUILayout.LabelField(nodeCount.ToString(), GUIPreset.width[80]);
                EditorGUILayout.LabelField(queryCount.ToString(), GUIPreset.width[80]);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            // 总计
            var totalQueries = manager.GetTotalQueryCount();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("总 Query 数量:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(totalQueries.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }

}
