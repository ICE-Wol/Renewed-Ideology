using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UI;
using Prota.Editor;

[CustomEditor(typeof(UIFrameImage))]
public class UIFrameImageInspector : Editor
{
	public new UIFrameImage target => base.target as UIFrameImage;

	void OnEnable()
	{
		EditorApplication.update += Update;
	}

	void OnDisable()
	{
		EditorApplication.update -= Update;
	}
	
	void Update()
	{
		if (target == null) return;

		target.ApplyValues();
	}
	
	public override void OnInspectorGUI()
	{
		if (target == null) return;

		Undo.RecordObject(target, "UIFrameImage Change Sprites");
		
		var backgroundLabel = GUIPreset.content["Background"];
		var frameLabel = GUIPreset.content["Frame"];
		var backgroundColorLabel = GUIPreset.content["Background Color"];
		var frameColorLabel = GUIPreset.content["Frame Color"];
		var ppuMultiplierLabel = GUIPreset.content["Pixel Per Unit Multiplier"];
		var preserveAspectLabel = GUIPreset.content["Keep Aspect Ratio"];

		EditorGUILayout.LabelField("Sprites", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();
		target.backgroundSprite = (Sprite)EditorGUILayout.ObjectField(backgroundLabel, target.backgroundSprite, typeof(Sprite), false, GUIPreset.height[16]);
		target.frameSprite = (Sprite)EditorGUILayout.ObjectField(frameLabel, target.frameSprite, typeof(Sprite), false, GUIPreset.height[16]);
		target.backgroundColor = EditorGUILayout.ColorField(backgroundColorLabel, target.backgroundColor);
		target.frameColor = EditorGUILayout.ColorField(frameColorLabel, target.frameColor);

		using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("Padding Background");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Left", GUIPreset.width[50]);
			target.paddingLeft = EditorGUILayout.FloatField("", target.paddingLeft, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Right", GUIPreset.width[50]);
			target.paddingRight = EditorGUILayout.FloatField("", target.paddingRight, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Bottom", GUIPreset.width[50]);
			target.paddingBottom = EditorGUILayout.FloatField("", target.paddingBottom, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Top", GUIPreset.width[50]);
			target.paddingTop = EditorGUILayout.FloatField("", target.paddingTop, GUIPreset.width[50]);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Padding Frame");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Left", GUIPreset.width[50]);
			target.framePaddingLeft = EditorGUILayout.FloatField("", target.framePaddingLeft, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Right", GUIPreset.width[50]);
			target.framePaddingRight = EditorGUILayout.FloatField("", target.framePaddingRight, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Bottom", GUIPreset.width[50]);
			target.framePaddingBottom = EditorGUILayout.FloatField("", target.framePaddingBottom, GUIPreset.width[50]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Top", GUIPreset.width[50]);
			target.framePaddingTop = EditorGUILayout.FloatField("", target.framePaddingTop, GUIPreset.width[50]);
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Image", EditorStyles.boldLabel);
		target.pixelsPerUnitMultiplier = EditorGUILayout.FloatField(ppuMultiplierLabel, target.pixelsPerUnitMultiplier);
		target.preserveAspect = EditorGUILayout.Toggle(preserveAspectLabel, target.preserveAspect);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Generated Images", EditorStyles.boldLabel);

		EditorGUI.BeginDisabledGroup(true);
		
		EditorGUILayout.ObjectField("Bg Image", target.bgImage, typeof(Image), true);
		EditorGUILayout.ObjectField("Frame Image", target.frameImage, typeof(Image), true);
		
		EditorGUI.EndDisabledGroup();
		
		EditorGUILayout.Space();

		if (GUILayout.Button("同步到子节点", GUIPreset.height[16]))
		{
			target.RebuildImages();
		}
		
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}
