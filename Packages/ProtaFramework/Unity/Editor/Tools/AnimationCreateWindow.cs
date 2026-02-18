using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using System.Collections.Generic;
using System.IO;

namespace Prota.Editor
{
    public class AnimationCreateWindow : EditorWindow
    {
        [MenuItem("ProtaFramework/Animation/Animation Create Window")]
        public static void ShowWindow()
        {
            GetWindow<AnimationCreateWindow>("Animation Create Window");
        }
        
        public bool fix;
        public List<Sprite> sprites = new List<Sprite>();
        
        public EditorPrefEntryInt frameStep = new EditorPrefEntryInt("AnimationCreateWindow_frameStep", 4);
        public EditorPrefEntryInt totalFrame = new EditorPrefEntryInt("AnimationCreateWindow_totalFrame", 4);
        public EditorPrefEntryFloat fps = new EditorPrefEntryFloat("AnimationCreateWindow_fps", 30);
        public EditorPrefEntryString animName = new EditorPrefEntryString("AnimationCreateWindow_animName", "");
        public EditorPrefEntryBool autoCreate = new EditorPrefEntryBool("AnimationCreateWindow_autoCreate", false);

        void OnEnable()
        {
            Selection.selectionChanged += Repaint;
        }
        
        void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
        }
        
        
        static string[] directions = { "down", "left", "right", "up", };
        void OnGUI()
        {
            using var __ = Selection.objects.Select(x => {
                if(x is Sprite) return (Sprite)x;
                if(x is Texture2D)
                {
                    var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(x));
                    return assets.OfType<Sprite>().FirstOrDefault();
                }
                return null;
            })
                .Where(x => x != null)
                .ToTempList(out var list);
            
            if(!fix)
            {
                sprites.Clear();
                sprites.AddRange(list.Cast<Sprite>());
            }
            
            if(sprites.Count == 0)
            {
                using(new EditorGUI.DisabledScope(true))
                {
                    foreach(var sprite in list)
                    {
                        EditorGUILayout.ObjectField(sprite, typeof(Sprite), false);
                    }
                }
            }
            
            if(sprites.Count != 0)
            {
                frameStep.value = EditorGUILayout.IntField("Frame Step", frameStep.value);
                totalFrame.value = EditorGUILayout.IntField("Total Frame", totalFrame.value);
                fps.value = EditorGUILayout.FloatField("FPS", fps.value);
                
                using(new EditorGUI.DisabledScope(true))
                    EditorGUILayout.FloatField("duration", totalFrame.value * frameStep.value / fps.value);
                    
                animName.value = EditorGUILayout.TextField("Animation Name", animName.value);
                
                var selectionCommonPrefix = list
                    .Select(x => x.name.Split('_'))
                    .CommonPrefix<string, IEnumerable<string>>()
                    .ToList();
                
                while(selectionCommonPrefix.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    foreach(var dir in directions)
                    {
                        var val = selectionCommonPrefix.Join("_") + "_" + dir;
                        if(GUILayout.Button(val))
                        {
                            animName.value = val;
                            if(autoCreate.value) CreateAnimation();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    selectionCommonPrefix.RemoveLast();
                }
                
                if(!autoCreate.value && GUILayout.Button("Create"))
                {
                    CreateAnimation();
                }
                
                autoCreate.value = EditorGUILayout.Toggle("Auto Create", autoCreate.value);
                
                for(int i = 0; i < sprites.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("↑", GUIPreset.width[20]))
                    {
                        if(i != 0) sprites.Swap(i, i - 1);
                    }
                    if(GUILayout.Button("↓", GUIPreset.width[20]))
                    {
                        if(i != sprites.Count - 1) sprites.Swap(i, i + 1);
                    }
                    EditorGUILayout.ObjectField(sprites[i], typeof(Sprite), false);
                    using(new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.TextField((i * frameStep.value).ToString(), GUIPreset.width[40]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
            }
            
            
        }

        private void CreateAnimation()
        {
            var path = Path.Combine("Assets", animName.value + ".anim");
            
            var clip = new AnimationClip();
            clip.frameRate = fps.value;
            clip.wrapMode = WrapMode.Loop;
            
            bool needLastFrame = frameStep.value != 1;
            var keys = new ObjectReferenceKeyframe[sprites.Count + (needLastFrame ? 1 : 0)];
            for(int i = 0; i < sprites.Count; i++) SetKey(keys, i, sprites[i]);
            if (needLastFrame) SetKey(keys, sprites.Count, sprites.Last());
            
            var binding = new EditorCurveBinding();
            binding.type = typeof(SpriteRenderer);
            binding.path = "";
            binding.propertyName = "m_Sprite";
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
            
            AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings {
                loopTime = true
            });
            
            AssetDatabase.CreateAsset(clip, path);
        }

        private void SetKey(ObjectReferenceKeyframe[] keys, int i, Sprite sprite)
        {
            keys[i] = new ObjectReferenceKeyframe();
            keys[i].time = i * frameStep.value / fps.value;
            keys[i].value = sprite;
        }
    }
    
}
