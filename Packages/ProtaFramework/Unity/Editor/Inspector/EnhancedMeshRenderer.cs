using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Prota.Unity;
using System.Linq;
using UnityEngine.UIElements;


namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(MeshRenderer), true)]
    [CanEditMultipleObjects]
    public class CustomMeshRendererInspector : Editor
    {
        Editor originalEditor;
        
        void OnEnable()
        {
            originalEditor = CreateEditor(targets, Type.GetType("UnityEditor.MeshRendererEditor, UnityEditor"));
        }
    
        void OnDisable()
        {
            DestroyImmediate(originalEditor);
        }

        public override void OnInspectorGUI()
        {
            if(use2DMeshRendererInspector.value)
            {
                var materialProp = serializedObject.FindProperty("m_Materials.Array.data[0]");
                var sortingLayerIDProp = serializedObject.FindProperty("m_SortingLayerID");
                var sortingOrderProp = serializedObject.FindProperty("m_SortingOrder");
                var renderingLayerMaskProp = serializedObject.FindProperty("m_RenderingLayerMask");
                var rendererPriority = serializedObject.FindProperty("m_RendererPriority");
                
                Undo.RecordObjects(targets, "MeshRenderer Inspector");
                
                // Sorting.
                {
                    var id = sortingLayerIDProp.intValue;
                    var index = SortingLayer.layers.FindIndex(l => l.id == id);
                    var newIndex = EditorGUILayout.Popup("Sorting Layer", index, SortingLayer.layers.Select(l => l.name).ToArray());
                    if(newIndex != index) sortingLayerIDProp.intValue = SortingLayer.layers[newIndex].id;
                }
                
                // Sorting Order.
                EditorGUILayout.PropertyField(sortingOrderProp, true);
                
                // Rendering Layer Mask.
                if(showRenderingLayerMask.value) EditorGUILayout.PropertyField(renderingLayerMaskProp, true);
                
                // Material.
                EditorGUILayout.PropertyField(materialProp, true);
                
                // Renderer Priority.
                EditorGUILayout.PropertyField(rendererPriority, true);
                
                var t = target as MeshRenderer;
                
                serializedObject.FindProperty("m_Materials.Array.size").intValue = 1;
                serializedObject.FindProperty("m_ReceiveShadows").boolValue = false;
                serializedObject.FindProperty("m_CastShadows").boolValue = false;
                serializedObject.FindProperty("m_DynamicOccludee").boolValue = false;
                serializedObject.FindProperty("m_StaticShadowCaster").boolValue = false;
                serializedObject.FindProperty("m_ReceiveGI").boolValue = false;
                serializedObject.FindProperty("m_LightProbeUsage").intValue = 0;
                serializedObject.FindProperty("m_ReflectionProbeUsage").intValue = 0;
                serializedObject.FindProperty("m_MotionVectors").intValue = 2;
                serializedObject.FindProperty("m_RayTracingMode").intValue = 0;
                serializedObject.FindProperty("m_RayTraceProcedural").intValue = 0;
                serializedObject.FindProperty("m_ProbeAnchor").objectReferenceValue = null;
                serializedObject.FindProperty("m_LightProbeVolumeOverride").objectReferenceValue = null;
                serializedObject.FindProperty("m_LightmapParameters").objectReferenceValue = null;
                
                
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                serializedObject.Update();
                if(originalEditor != null) originalEditor.OnInspectorGUI();
            }
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        static EditorPrefEntryBool use2DMeshRendererInspector = new EditorPrefEntryBool("ProtaEditor.Use2DMeshRendererInspector", true);
        static EditorPrefEntryBool showSortingLayer = new EditorPrefEntryBool("ProtaEditor.2DMeshInspectorShowSortingLayer", true);
        static EditorPrefEntryBool showOrderInLayer = new EditorPrefEntryBool("ProtaEditor.2DMeshInspectorShowOrderInLayer", true);
        static EditorPrefEntryBool showRenderingLayerMask = new EditorPrefEntryBool("ProtaEditor.2DMeshInspectorShowRenderingLayerMask", false);
        static EditorPrefEntryBool showRendererPriority = new EditorPrefEntryBool("ProtaEditor.2DMeshInspectorShowRendererPriority", false);
        
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Use 2D Inspector", false, 2800)]
        static void ToggleSpriteRendererHDRColor()
        {
            use2DMeshRendererInspector.value = !use2DMeshRendererInspector.value;
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Use 2D Inspector", use2DMeshRendererInspector.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Use 2D Inspector", true)]
        static bool ToggleSpriteRendererHDRColorValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Use 2D Inspector", use2DMeshRendererInspector.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Sorting Layer", false, 2801)]
        static void ToggleSpriteRendererShowOrderInLayer()
        {
            showOrderInLayer.value = !showOrderInLayer.value;
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Layer", showOrderInLayer.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Sorting Layer", true)]
        static bool ToggleSpriteRendererShowOrderInLayerValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Layer", showOrderInLayer.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Sorting Order in Layer", false, 2802)]
        static void ToggleSpriteRendererShowSortingLayer()
        {
            showSortingLayer.value = !showSortingLayer.value;
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Order in Layer", showSortingLayer.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Sorting Order in Layer", true)]
        static bool ToggleSpriteRendererShowSortingLayerValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Order in Layer", showSortingLayer.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Rendering Layer Mask", false, 2803)]
        static void ToggleSpriteRendererShowRenderingLayerMask()
        {
            showRenderingLayerMask.value = !showRenderingLayerMask.value;
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Rendering Layer Mask", showRenderingLayerMask.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Rendering Layer Mask", true)]
        static bool ToggleSpriteRendererShowRenderingLayerMaskValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Rendering Layer Mask", showRenderingLayerMask.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Renderer Priority", false, 2804)]
        static void ToggleSpriteRendererShowSortingOrder()
        {
            showRendererPriority.value = !showRendererPriority.value;
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Renderer Priority", showRendererPriority.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/MeshRenderer Show Renderer Priority", true)]
        static bool ToggleSpriteRendererShowSortingOrderValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Renderer Priority", showRendererPriority.value);
            return true;
        }
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Use 2D Inspector", use2DMeshRendererInspector.value);
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Layer", showSortingLayer.value);
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Sorting Order in Layer", showOrderInLayer.value);
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Rendering Layer Mask", showRenderingLayerMask.value);
            Menu.SetChecked("ProtaFramework/Functionality/MeshRenderer Show Renderer Priority", showRendererPriority.value);
        }
        
        
        
    
    }
}
