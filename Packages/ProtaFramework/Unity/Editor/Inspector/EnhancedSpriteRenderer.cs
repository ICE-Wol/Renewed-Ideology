using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Prota.Unity;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Prota.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(SpriteRenderer), true)]
    [CanEditMultipleObjects]
    public class CustomSpriteRendererInspector : Editor
    {
        Editor originalEditor = null;
        
        void OnEnable()
        {
            
        }
    
        void OnDisable()
        {
            DestroyImmediate(originalEditor);
            originalEditor = null;
        }

        bool showOriginalInspector = false;
    
        public override void OnInspectorGUI()
        {
            var n = targets.Length;
            
            if(!useEnhancedInspector.value || n > 1)
            {
                ShowDefaultInspector();
                return;
            }
            
            var spriteProp = serializedObject.FindProperty("m_Sprite");
            var colorProp = serializedObject.FindProperty("m_Color");
            var flipXProp = serializedObject.FindProperty("m_FlipX");
            var flipYProp = serializedObject.FindProperty("m_FlipY");
            var drawModeProp = serializedObject.FindProperty("m_DrawMode");
            var maskInteractionProp = serializedObject.FindProperty("m_MaskInteraction");
            var spriteSortPointProp = serializedObject.FindProperty("m_SpriteSortPoint");
            var materialProp = serializedObject.FindProperty("m_Materials.Array.data[0]");
            var sortingLayerIDProp = serializedObject.FindProperty("m_SortingLayerID");
            var sortingOrderProp = serializedObject.FindProperty("m_SortingOrder");
            var renderingLayerMaskProp = serializedObject.FindProperty("m_RenderingLayerMask");
            
            Undo.RecordObjects(targets, "SpriteRenderer Inspector");
            
            // Sorting.
            if(showSortingLayer.value)
            {
                var id = sortingLayerIDProp.intValue;
                var index = SortingLayer.layers.FindIndex(l => l.id == id);
                var newIndex = EditorGUILayout.Popup("Sorting Layer", index, SortingLayer.layers.Select(l => l.name).ToArray());
                if(newIndex != index) sortingLayerIDProp.intValue = SortingLayer.layers[newIndex].id;
            }
            
            // Sorting Order.
            if(showSortingOrderInLayer.value)
            {
                EditorGUILayout.PropertyField(sortingOrderProp, new GUIContent("Order in Layer"));
            }
            
            
            var color = colorProp.colorValue;
            EditorGUILayout.PropertyField(colorProp, new GUIContent("Color"));
            color = EditorGUILayout.ColorField(new GUIContent("Color"), colorProp.colorValue, true, true, true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("RGBA");
            color.r = EditorGUILayout.FloatField(color.r);
            color.g = EditorGUILayout.FloatField(color.g);
            color.b = EditorGUILayout.FloatField(color.b);
            color.a = EditorGUILayout.FloatField(color.a);
            colorProp.colorValue = color;
            EditorGUILayout.EndHorizontal();
            
            // Material.
            EditorGUILayout.PropertyField(materialProp, new GUIContent("Material"));
            
            // Flip.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Flip", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("X", GUILayout.MaxWidth(10));
            flipXProp.boolValue = EditorGUILayout.Toggle("", flipXProp.boolValue, GUILayout.MaxWidth(20));
            EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(10));
            flipYProp.boolValue = EditorGUILayout.Toggle("", flipYProp.boolValue, GUILayout.MaxWidth(20));
            EditorGUILayout.EndHorizontal();
            
            // Mask Interaction.
            if(showMaskInteraction.value)
            {
                EditorGUILayout.PropertyField(maskInteractionProp, new GUIContent("Mask Interaction"));
            }
            
            // Sprite pick.
            spriteProp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Sprite"), spriteProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if(showSpritePick.value)
            {
                var sprite = (spriteProp.objectReferenceValue as Sprite).CheckNull();
                SpriteSelectorInspector.DrawInspector(sprite, colorProp.colorValue, out var newSprite);
                if(newSprite != sprite)
                {
                    spriteProp.objectReferenceValue = newSprite;
                    EditorUtility.SetDirty(target);
                }
            }
            
            
            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if(showOriginalInspector = EditorGUILayout.Foldout(showOriginalInspector, "Show Original Inspector"))
            {
                ShowDefaultInspector();
            }
        }
        
        void ShowDefaultInspector()
        {
            serializedObject.Update();
            if(originalEditor == null || originalEditor.target != this.target)
            {
                if(originalEditor != null) DestroyImmediate(originalEditor);
                originalEditor = CreateEditor(targets, Type.GetType("UnityEditor.SpriteRendererEditor, UnityEditor"));
            }
            originalEditor.OnInspectorGUI();
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        static EditorPrefEntryBool useEnhancedInspector = new EditorPrefEntryBool("ProtaEditor.UseEnhancedSpriteRendererInspector", true);
        static EditorPrefEntryBool showSpritePick = new EditorPrefEntryBool("ProtaEditor.EnhancedSpritePick", true);
        static EditorPrefEntryBool showSortingLayer = new EditorPrefEntryBool("ProtaEditor.EnhancedSpriteSortingLayer", true);
        static EditorPrefEntryBool showSortingOrderInLayer = new EditorPrefEntryBool("ProtaEditor.EnhancedSpriteSortingOrderInLayer", true);
        static EditorPrefEntryBool showRenderingLayerMask = new EditorPrefEntryBool("ProtaEditor.EnhancedSpriteRenderingLayerMask", false);
        static EditorPrefEntryBool showMaskInteraction = new EditorPrefEntryBool("ProtaEditor.EnhancedSpriteMaskInteraction", false);
        static EditorPrefEntryBool showRendererPriority = new EditorPrefEntryBool("ProtaEditor.EnhancedSpriteRendererPriority", false);
        
        [MenuItem("ProtaFramework/Functionality/SpriteRenderer Use Enhanced Inspector", false, 2700)]
        static void ToggleUseEnhancedInspector()
        {
            useEnhancedInspector.value = !useEnhancedInspector.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRenderer Use Enhanced Inspector", useEnhancedInspector.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRenderer Use Enhanced Inspector", true)]
        static bool ToggleUseEnhancedInspectorValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRenderer Use Enhanced Inspector", useEnhancedInspector.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sprite Pick", false, 2702)]
        static void ToggleSpriteRendererSpritePick()
        {
            showSpritePick.value = !showSpritePick.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sprite Pick", showSpritePick.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sprite Pick", true)]
        static bool ToggleSpriteRendererSpritePickValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sprite Pick", showSpritePick.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sorting Layer", false, 2703)]
        static void ToggleSortingLayerAndOrder()
        {
            showSortingLayer.value = !showSortingLayer.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sorting Layer", showSortingLayer.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sorting Layer", true)]
        static bool ToggleSortingLayerAndOrderValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sorting Layer", showSortingLayer.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sorting Order in Layer", false, 2704)]
        static void ToggleSortingOrderInLayer()
        {
            showSortingOrderInLayer.value = !showSortingOrderInLayer.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sorting Order in Layer", showSortingOrderInLayer.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Sorting Order in Layer", true)]
        static bool ToggleSortingOrderInLayerValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Sorting Order in Layer", showSortingOrderInLayer.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Rendering Layer Mask", false, 2705)]
        static void ToggleRenderingLayerMask()
        {
            showRenderingLayerMask.value = !showRenderingLayerMask.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Rendering Layer Mask", showRenderingLayerMask.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Rendering Layer Mask", true)]
        static bool ToggleRenderingLayerMaskValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Rendering Layer Mask", showRenderingLayerMask.value);
            return true;
        }
                
        [MenuItem("ProtaFramework/Functionality/SpriteRender Mask Interaction", false, 2706)]
        static void ToggleMaskInteraction()
        {
            showMaskInteraction.value = !showMaskInteraction.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Mask Interaction", showMaskInteraction.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Mask Interaction", true)]
        static bool ToggleMaskInteractionValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Mask Interaction", showMaskInteraction.value);
            return true;
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Renderer Priority", false, 2707)]
        static void ToggleRendererPriority()
        {
            showRendererPriority.value = !showRendererPriority.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Renderer Priority", showRendererPriority.value);
        }
        
        [MenuItem("ProtaFramework/Functionality/SpriteRender Renderer Priority", true)]
        static bool ToggleRendererPriorityValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteRender Renderer Priority", showRendererPriority.value);
            return true;
        }
        
    
    }
    
    
    public static class SpriteSelectorInspector
    {
        public static EditorPrefEntryBool showConfig = new("Prota.SpriteSelectorInspector.ShowConfigInInspector", false);
        public static EditorPrefEntryFloat gridSize = new("Prota.SpriteSelectorInspector.gridSize", 60);
        public static EditorPrefEntryColor backColor = new("Prota.SpriteSelectorInspector.backColor", new Color(0.1f, 0.1f, 0.1f, 1));
        public static EditorPrefEntryColor selectColor = new("Prota.SpriteSelectorInspector.selectColor", new Color(0.4f, 0.4f, 0.4f, 1));

        static readonly ConditionalWeakTable<Texture, Sprite[]> cache = new();
        
        public static void DrawInspector(Sprite oldSprite, Color color, out Sprite newSprite)
        {
            oldSprite = oldSprite.CheckNull();
            newSprite = oldSprite;
            
            var texture = oldSprite?.texture;
            texture = EditorGUILayout.ObjectField("Atlas", texture, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Texture2D;
            if(texture == null) return;
            
            
            var sprites = GetSpritesFromTexture(texture, out var s) ? s : null;
            
            const float rightPadding = 20;
            var columns = ((EditorGUIUtility.currentViewWidth - rightPadding) / gridSize.value).FloorToInt().Max(1);
            var lines = (sprites.Length / (float)columns).CeilToInt();
            var actualControlSize = (EditorGUIUtility.currentViewWidth - rightPadding) / columns;
            var cellSize = new Vector2(actualControlSize, actualControlSize);
            var baseRect = EditorGUILayout.GetControlRect(false, cellSize.y * lines);
            var moveRect = baseRect.WithWidth(cellSize.x * columns);
            
            var selectedColor = selectColor.value;
            var nonSelectedColor = backColor.value;
            var oldSpriteInAtlas = false;
            
            for (int i = 0; i < sprites.Length; i++)
            {
                var curColumn = i % columns;
                var curLine = i / columns;

                var sprite = sprites[i];
                if (sprite == null) continue;
                var imageRect = moveRect.Move(curColumn * cellSize.x, curLine * cellSize.y);
                imageRect.size = cellSize;
                var bgRect = imageRect.Shrink(2, 2, 2, 2);
                var selected = oldSprite == sprite;
                oldSpriteInAtlas |= selected;
                var bgColor = selected ? selectedColor : nonSelectedColor;
                EditorGUI.DrawRect(bgRect, bgColor);
                // using(new GUIColorScope(color))
                GUI.DrawTextureWithTexCoords(bgRect, sprite.texture, sprite.GetNormalizedRect());
                if(Event.current.type == EventType.MouseDown && imageRect.Contains(Event.current.mousePosition))
                {
                    newSprite = sprite;
                    GUI.changed = true;
                }
            }
            
            if(!oldSpriteInAtlas && sprites.Length > 0)
            {
                newSprite = sprites[0];
            }
            
            if(showConfig.value)
            {
                gridSize.value = EditorGUILayout.Slider("grid size", gridSize.value, 20, 200);
                backColor.value = EditorGUILayout.ColorField("back color", backColor.value);
                selectColor.value = EditorGUILayout.ColorField("select color", selectColor.value);
            }
        }

        public static bool GetSpritesFromTexture(Texture2D texture, out Sprite[] sprites)
        {
            sprites = null;
            var texturePath = AssetDatabase.GetAssetPath(texture);
            if (texture != null && texturePath == null)
            {
                EditorGUILayout.HelpBox("Texture is not an asset", MessageType.Warning);
                return false;
            }

            if (texture != null && !cache.TryGetValue(texture, out sprites))
            {
                sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath).CastNotNull<Sprite>().ToArray();
                cache.Add(texture, sprites);
            }
            
            return true;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        [MenuItem("ProtaFramework/Functionality/SpriteSelect Show Config", false, 2699)]
        static void ToggleSpriteRendererSpritePick()
        {
            showConfig.value = !showConfig.value;
            Menu.SetChecked("ProtaFramework/Functionality/SpriteSelect Show Config", showConfig.value);
        }
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            Menu.SetChecked("ProtaFramework/Functionality/SpriteSelect Show Config", showConfig.value);
        }
        
        
    }

}
