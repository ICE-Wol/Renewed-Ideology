using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota;
using Prota.Unity;
using UnityEngine.Rendering;

using HierarchyUtilities = SandolkakosDigital.EditorUtils.SceneHierarchyUtility;
using UnityEditor.ShaderGraph.Internal;
using Unity.Hierarchy;

namespace Prota.Editor
{
    [InitializeOnLoad]
    public class EnhancedHierarchy : UnityEditor.Editor
    {
        static EnhancedHierarchy()
        {
            UpdateSettings();
        }
        
        static EditorPrefEntryBool registered = new("Prota:EnhancedHierarchyEnabled");
        
        [MenuItem("ProtaFramework/Functionality/Toggle Enhanced Hierarchy", priority = 2250)]
        static void SwitchEnhancedHierarchy()
        {
            registered.value = !registered.value;
            Menu.SetChecked("ProtaFramework/Functionality/Toggle Enhanced Hierarchy", registered.value);
            UpdateSettings();
        }
        
        [MenuItem("ProtaFramework/Functionality/Toggle Enhanced Hierarchy", true)]
        static bool SwitchEnhancedHierarchyValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Toggle Enhanced Hierarchy", registered.value);
            return true;
        }
        
        static EditorPrefEntryBool showZ = new("Prota:ShowZCoordinates");
        [MenuItem("ProtaFramework/Functionality/Show Z Coordinates", priority = 2251)]
        static void SwitchShowZ()
        {
            showZ.value = !showZ.value;
            Menu.SetChecked("ProtaFramework/Functionality/Show Z Coordinates", showZ.value);
            UpdateSettings();
        }
        
        [MenuItem("ProtaFramework/Functionality/Show Z Coordinates", true)]
        static bool SwitchShowZValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Show Z Coordinates", showZ.value);
            return true;
        }
        
        static EditorPrefEntryBool showLayer = new("Prota:ShowLayer");
        [MenuItem("ProtaFramework/Functionality/Show Layer", priority = 2252)]
        static void SwitchShowLayer()
        {
            showLayer.value = !showLayer.value;
            Menu.SetChecked("ProtaFramework/Functionality/Show Layer", showLayer.value);
            UpdateSettings();
        }
        
        [MenuItem("ProtaFramework/Functionality/Show Layer", true)]
        static bool SwitchShowLayerValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Show Layer", showLayer.value);
            return true;
        }
        
        static EditorPrefEntryBool clickFoldout = new("Prota:ClickFoldout");
        [MenuItem("ProtaFramework/Functionality/Click Foldout", priority = 2253)]
        static void SwitchClickFoldout()
        {   
            clickFoldout.value = !clickFoldout.value;
            Menu.SetChecked("ProtaFramework/Functionality/Click Foldout", clickFoldout.value);
            UpdateSettings();
        }
        
        [MenuItem("ProtaFramework/Functionality/Click Foldout", true)]
        static bool SwitchClickFoldoutValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Click Foldout", clickFoldout.value);
            return true;
        }
        
        
        [MenuItem("ProtaFramework/GC Unused Assets", priority = 10000)]
        static void GCUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
        
        
        static void UpdateSettings()
        {
            if(!registered.value)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            }
            else
            {
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            }
            
            Menu.SetChecked("ProtaFramework/Functionality/Toggle Enhanced Hierarchy", registered.value);
        }
        
        private const int pixelPerDepth = 14;
        static readonly List<Component> comps = new List<Component>();
        
        static FloatStringCache depthStringCache = new FloatStringCache(x => $"({x})");
        static IntStringCache sortingOrderStringCache = new IntStringCache(x => $"[{x}]");

        static Texture2D barTexture = null;
        static Texture2D backTexture = null;
        static Texture2D ecompTexture = null;
        static Texture2D ecompTextureActivated = null;
        static Texture2D erootTexture = null;
        static Texture2D erootTextureActivated = null;
        
        static GUIStyle layerStyle = null;
        static GUIStyle coordEqualStyle = null;
        static GUIStyle coordLowerStyle = null;
        static GUIStyle coordLargerStyle = null;
        
        const int maxIconCount = 10;
        static GUIStyle labelMidCenter = null;
        
        static bool inited = false;
        static void InitializeResources()
        {
            if(inited) return;
            barTexture ??= Resources.Load<Texture2D>("ProtaFramework/line_vertical_16_2");
            backTexture ??= Resources.Load<Texture2D>("ProtaFramework/rect_16");
            erootTexture ??= Resources.Load<Texture2D>("ProtaFramework/icon_eroot_1");
            erootTextureActivated ??= Resources.Load<Texture2D>("ProtaFramework/icon_eroot_2");
            ecompTexture ??= Resources.Load<Texture2D>("ProtaFramework/icon_ecomponent_1");
            ecompTextureActivated ??= Resources.Load<Texture2D>("ProtaFramework/icon_ecomponent_2");
            if(labelMidCenter == null)
            {
                labelMidCenter = new GUIStyle(GUI.skin.label);
                labelMidCenter.alignment = TextAnchor.MiddleCenter;
                labelMidCenter.fontStyle = FontStyle.Bold;
            }
            if(layerStyle == null)
            {
                layerStyle = new GUIStyle();
                layerStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                layerStyle.alignment = TextAnchor.MiddleLeft;
            }
            if(coordEqualStyle == null)
            {
                coordEqualStyle = new GUIStyle();
                coordEqualStyle.normal.textColor = new Color(.8f, .8f, .9f, 1f);
                coordEqualStyle.alignment = TextAnchor.MiddleRight;
            }
            if(coordLowerStyle == null)
            {
                coordLowerStyle = new GUIStyle();
                coordLowerStyle.normal.textColor = new Color(0.8f, 0.4f, 1f, 1f);
                coordLowerStyle.alignment = TextAnchor.MiddleRight;
            }
            if(coordLargerStyle == null)
            {
                coordLargerStyle = new GUIStyle();
                coordLargerStyle.normal.textColor = new Color(0.4f, 0.8f, 1f, 1f);
                coordLargerStyle.alignment = TextAnchor.MiddleRight;
            }
            inited = true;
        }
        
        static void OnHierarchyGUI(int instanceId, Rect area)
        {
            using (new ProfilerScope("EnhancedHierarchy.InitializeResources")) InitializeResources();
            
            var target = EditorUtility.InstanceIDToObject(instanceId);
            if(!(target is GameObject g)) return;
            
            var gtr = g.transform;
            var gName = g.name;
            var gParent = gtr.parent;
            
            var originalGUIColor = GUI.color;
            var rightMargin = area.xMax - 40;
            var space = area.height - 4;                // 每个图标向左偏移多少像素.
            
            var depth = gtr.GetDepth();
            
            var select = Selection.activeGameObject.CheckNull();
            
            if(showLayer.value)
            {
                using (new ProfilerScope("EnhancedHierarchy.ShowLayer"))
                {
                    var layerName = LayerMask.LayerToName(g.layer);
                    if(layerName != "")
                    {
                        var loc = Rect.MinMaxRect(rightMargin - 70, area.yMin, rightMargin - 10, area.yMax);
                        GUI.Label(loc, layerName, layerStyle);
                    }
                    rightMargin -= 70;
                }
            }
            
            if(showZ.value)
            {
                using (new ProfilerScope("EnhancedHierarchy.ShowZ"))
                {
                    if((gParent == null || gParent.GetComponentInParent<SortingGroup>() == null))
                    {
                        var selectTr = select?.transform;
                        // var gtr = g.transform;
                        var op = selectTr == null ? ""
                            : gtr.position.z > selectTr.position.z ? ">"
                            : gtr.position.z < selectTr.position.z ? "<"
                            : "=";
                        var style = op == "" ? coordEqualStyle
                            : op == "<" ? coordLargerStyle
                            : op == ">" ? coordLowerStyle
                            : coordEqualStyle;
                        
                        var loc = Rect.MinMaxRect(rightMargin - 70, area.yMin, rightMargin - 10, area.yMax);
                        GUI.Label(loc, $"{gtr.position.z:F2}{op}", style);
                    }
                    rightMargin -= 80;
                }
            }
            
            // SetActive 部分.
            using (new ProfilerScope("EnhancedHierarchy.SetActive"))
            {
                var active = EditorGUI.Toggle(new Rect(rightMargin, area.yMax - area.height, 16, 16), g.activeSelf);
                rightMargin -= space + 2;
                if(active != g.activeSelf)
                {
                    Undo.RecordObject(g, "Activation");
                    g.SetActive(active);
                    Selection.activeObject = g;
                    Event.current.Use();
                }
            }
            
            // 这个 gameobject 下属的 gameobject.
            using (new ProfilerScope("EnhancedHierarchy.GetComponents"))
            {
                g.GetComponents<Component>(comps);
                comps.RemoveAll(x => x == null || x is Transform);
                comps.Sort(Compare);
                // comps.Reverse();
            }
            
            // Component 图标部分.
            using (new ProfilerScope("EnhancedHierarchy.DrawIcons"))
            {
                int n = 0;
                var oriColor = GUI.color;
                foreach(var c in comps)
                {
                    GUIContent r = null;
                    n++;
                    if(n > maxIconCount) break;
                    if(r == null) r = c.FindEditorIconGUIContent();
                    rightMargin -= 1;
                    GUI.Label(new Rect(rightMargin, area.yMax - area.height, 16, 16), r);
                    GUI.color = oriColor;
                    rightMargin -= space;
                    
                    if(c is Camera cc)
                    {
                        rightMargin -= space;
                        GUI.Label(new Rect(rightMargin, area.yMax - area.height, space * 2 + 2, 16), depthStringCache[cc.depth]);
                        rightMargin -= space + 2;
                    }
                    else if(c is Renderer rd)
                    {
                        rightMargin -= space;
                        GUI.Label(new Rect(rightMargin, area.yMax - area.height, space * 2 + 2, 16), sortingOrderStringCache[rd.sortingOrder]);
                        rightMargin -= space + 2;
                    }
                }
            }
            
            
            // Canvas 标记部分. 如果一个物体被挂在 canvas 下方则有一个蓝色竖线标记.
            using (new ProfilerScope("EnhancedHierarchy.CanvasMarker"))
            {
                if(g.GetComponentInParent<Canvas>() != null)
                {
                    var r = new Rect(area);
                    r.xMin -= 20 + depth * pixelPerDepth;
                    r.xMax = r.xMin + r.height;
                    GUI.color = new Color(0.6f, 0.65f, 1, 1f);
                    GUI.DrawTexture(r, barTexture);
                }
            }
            
            // 全局单例标记部分, 命名带 # 号, 或者组件带有 Singleton.
            using (new ProfilerScope("EnhancedHierarchy.SingletonMarker"))
            {
                bool hasSingleton = false;
                foreach(var c in comps)
                {
                    var t = c.GetType();
                    if(!t.IsGenericType) continue;
                    t = t.GetGenericTypeDefinition();
                    if(t != typeof(SingletonComponent<>)) continue;
                    hasSingleton = true;
                    break;
                }
                if(hasSingleton || gName.StartsWith("#"))
                {
                    var r = new Rect(area);
                    r.xMin -= 22 + depth * pixelPerDepth;
                    r.xMax = r.xMin + r.height;
                    GUI.color = new Color(0.6f, 0.0f, 0.0f, 1f);
                    GUI.DrawTexture(r, barTexture);
                }
            }
            
            
            // 标记名称前缀为 >>> 或 ===, 后缀为 <<< 或 === 的物体.
            using (new ProfilerScope("EnhancedHierarchy.SpecialNameMarker"))
            {
                if(depth == 1 && gName.StartsWith(">>>"))
                {
                    var name = gName;
                    name = name[(name.LastIndexOf('>') + 1)..];
                    // 盖住原来的名字.
                    var r = new Rect(area);
                    r.xMin -= 80;
                    r.xMax += 40;
                    GUI.color = new Color(0.22f, 0.22f, 0.22f, 1f);
                    GUI.DrawTexture(r, backTexture);
                    
                    // 新的底板
                    var rc = new Rect(r);
                    rc.yMax += 1;
                    if(Selection.objects.Contains(g)) GUI.color = new Color(0.22f, 0.22f, 0.4f, 1f);
                    else GUI.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                    GUI.DrawTexture(rc, backTexture);
                    
                    // 文字
                    GUI.color = new Color(1f, 1f, 1f, 1f);
                    GUI.Label(r, name, labelMidCenter);
                }
            }
            
            // 标记当前选择的 GameObject 的 DataBinding.
            using (new ProfilerScope("EnhancedHierarchy.DataBindingMarker"))
            {
                if(Selection.activeGameObject != null
                    && Selection.activeGameObject.TryGetComponent<DataBinding>(out var dataBinding))
                {
                    var list = (SerializableHashMap<string, GameObject>)dataBinding.ProtaReflection().Get("data");
                    if(list.Any(x => x.Value == g))
                    {
                        var r = new Rect(area);
                        r.xMin = r.xMax;
                        r.xMax = r.xMin + 2;
                        GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
                        GUI.DrawTexture(r, backTexture);
                    }
                }
            }
            
            if(clickFoldout.value)
            {
                using (new ProfilerScope("EnhancedHierarchy.ClickFoldout"))
                {
                    // Debug.LogError(Event.current.type + " " + Event.current.button + " " + Event.current.mousePosition + " << " + area);
                    if (IsClick(EventType.MouseDown, ref area, select == g)) clickStart = Event.current.mousePosition;
                    if(clickStart.HasValue && IsClick(EventType.MouseUp, ref area, select == g) && Event.current.mousePosition == clickStart)
                    {
                        // bool isExpended = HierarchyUtilities.IsExpanded(g);
                        bool isCtrl = Event.current.control;
                        bool isAlt = Event.current.alt;
                        if(isCtrl) HierarchyUtilities.SetExpandedRecursive(g, true);
                        else if(isAlt) HierarchyUtilities.SetExpandedRecursive(g, false);
                        else HierarchyUtilities.SetExpanded(g, !HierarchyUtilities.IsExpanded(g));
                        Event.current.Use();
                        
                    }
                }
            }
            
            GUI.color = originalGUIColor;
        }
        
        
        static Vector2? clickStart = null;
        static bool IsClick(EventType eventType, ref Rect area, bool isSelected)
        {
            return Event.current.type == eventType
                && Event.current.button == 0
                && area.Contains(Event.current.mousePosition)
                && isSelected;
        }
        
        static int GetWeight(Component x)
        {
            // if(typeof(EComponent).IsAssignableFrom(x.GetType())) return 99;
            switch(x)
            {
                case Rigidbody _: return 90;
                case PhysicsContactRecorder3D _: return 89;
                case Collider _: return 88;
                
                case Rigidbody2D _: return 90;
                case PhysicsContactRecorder2D _: return 89;
                case Collider2D _: return 88;
                
                case DataBinding _: return 10;
                
            }
            
            return 0;
        }
        
        static int Compare(Component a, Component b)
        {
            // reversed.
            return GetWeight(b).CompareTo(GetWeight(a));
            // return GetWeight(a).CompareTo(GetWeight(b));
        }
    
    
        [MenuItem("Tools/Prefab Helper/Apply Changes to Prefab #p")]
        static void ApplyChangesToPrefab()
        {
            // Get the selected object in the scene
            GameObject selectedObject = Selection.activeGameObject;
            
            // Check if the selected object is valid and is part of a prefab
            if (selectedObject != null)
            {
                // Get the prefab asset that the selected object is part of
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(selectedObject);

                if (prefab != null)
                {
                    // Apply the changes to the prefab
                    PrefabUtility.ApplyPrefabInstance(selectedObject, InteractionMode.UserAction);
                    Debug.Log("Changes applied to prefab: " + prefab.name);
                }
                else Debug.LogWarning("Selected object is not part of a prefab.");
            }
            else Debug.LogWarning("No GameObject selected.");
        }
    }
}
