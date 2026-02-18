using System;
using System.Collections.Generic;
using System.IO;
using Prota;
using Prota.Editor;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class FastSpriteGeneratorWindow : EditorWindow
{
    EditorPrefEntryInt cellCountX = new("Prota:FastSpriteGeneratorCellCountX", 1);
    EditorPrefEntryInt cellCountY = new("Prota:FastSpriteGeneratorCellCountY", 1);
    EditorPrefEntryInt padding = new("Prota:FastSpriteGeneratorPadding", 0);
    EditorPrefEntryVec2 pivotPoint = new("Prota:FastSpriteGeneratorPivotPoint", new Vector2(0.5f, 0.5f));
    EditorPrefEntryInt ppu = new("Prota:FastSpriteGeneratorPpu", 100);
    
    Texture2D selectedTexture => Selection.activeObject as Texture2D;
    string textureName => selectedTexture != null ? selectedTexture.name : "None";
    
    [MenuItem("ProtaFramework/Tools/Fast Sprite Generator")]
    public static void ShowWindow()
    {
        GetWindow<FastSpriteGeneratorWindow>("Fast Sprite Generator");
    }
    
    void OnEnable()
    {
        Selection.selectionChanged += Repaint;
    }
    
    void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
    }
    
    static bool IsDefinedByFile(string path, out FileInfo defFile)
    {
        defFile = null;
        var file = path.AsFileInfo();
        if(!file.Exists) return false;
        defFile = file.WithExt(".json");
        if(defFile.Exists) return true;
        return false;
    }
    
    private void OnGUI()
    {
        if (selectedTexture == null)
        {
            EditorGUILayout.HelpBox("Please select a texture asset to generate a sprite sheet.", MessageType.Info);
            return;
        }
        
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if(IsDefinedByFile(path, out var defFile))
        {
            GenerateByFileOnGUI(defFile);
            return;
        }
        
        GenerateManuallyOnGUI();
    }

    
    [Serializable]
    class ConfigDef
    {
        public Dictionary<string, SpriteDef> frames;
    }
    
    [Serializable]
    class SpriteDef
    {
        public FrameDef frame;
        public Vector2 pivot;
    }
    
    [Serializable]
    class FrameDef
    {
        public int x, y, w, h;
    }
    

    void GenerateManuallyOnGUI()
    {
        var res = EditorGUILayout.Vector2IntField("Grid Count", new Vector2Int(cellCountX.value, cellCountY.value));
        cellCountX.value = res.x;
        cellCountY.value = res.y;
        
        padding.value = EditorGUILayout.IntField("Padding", padding.value);
        pivotPoint.value = EditorGUILayout.Vector2Field("Pivot Point", pivotPoint.value);
        ppu.value = EditorGUILayout.IntField("Pixels Per Unit", ppu.value);
        
        // Button to apply changes
        if (GUILayout.Button("Regenerate Sprite"))
        {
            RegenerateSprite();
        }
    }
    
    private void GenerateByFileOnGUI(FileInfo defFile)
    {
        EditorGUILayout.HelpBox("Sprite is defined by a JSON file.", MessageType.Info);
        if(GUILayout.Button($"Generate by config file {defFile.Name}"))
        {
            GenerateByFile(defFile);
        }
    }
    
    void GenerateByFile(FileInfo defFile)
    {
        var fileContent = File.ReadAllText(defFile.FullName);
        var config = JsonUtility.FromJson<ConfigDef>(fileContent);
        if(!GetTextureImporter(out var textureImporter, out var dataProvider)) return;
        SetTextureConfig(textureImporter);
        SpriteRect[] rects = GenerateSpriteSheetFromConfig(config.frames);
        dataProvider.SetSpriteRects(rects);
        
        // Apply changes and save
        dataProvider.Apply();
        textureImporter.SaveAndReimport();
        Debug.Log("Sprite regenerated with new settings!");
    }

    private SpriteRect[] GenerateSpriteSheetFromConfig(Dictionary<string, SpriteDef> frames)
    {
        SpriteRect[] rects = new SpriteRect[frames.Count];
        int i = 0;
        var texHeight = (Selection.activeObject as Texture2D).height;
        foreach((var name, var def) in frames)
        {
            rects[i++] = new SpriteRect
            {
                name = name,
                rect = new Rect(def.frame.x, texHeight - def.frame.y - def.frame.h, def.frame.w, def.frame.h),
                pivot = def.pivot,
                alignment = SpriteAlignment.Custom
            };
        }

        return rects;
    }


    private void RegenerateSprite()
    {
        if (!GetTextureImporter(out var textureImporter, out var dataProvider)) return;
        SetTextureConfig(textureImporter);

        SpriteRect[] rects = GenerateSpriteSheet(cellCountX.value, cellCountY.value, padding.value);
        dataProvider.SetSpriteRects(rects);

        // Apply changes and save
        dataProvider.Apply();
        textureImporter.SaveAndReimport();
        Debug.Log("Sprite regenerated with new settings!");

    }

    private void SetTextureConfig(TextureImporter textureImporter)
    {
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePixelsPerUnit = ppu.value;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        var pts = textureImporter.GetPlatformTextureSettings("Default");
        pts.maxTextureSize = 8192;
        pts.compressionQuality = 100;
        textureImporter.SetPlatformTextureSettings(pts);
    }

    static bool GetTextureImporter(out TextureImporter textureImporter, out ISpriteEditorDataProvider dataProvider)
    {
        dataProvider = null;
        
        // Fetch the selected sprite asset path
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogWarning("Selected asset is not a valid texture.");
            return false;
        }
        
        dataProvider = new SpriteDataProviderFactories().GetSpriteEditorDataProviderFromObject(textureImporter);
        dataProvider.InitSpriteEditorDataProvider();
        
        return true;
    }

    private SpriteRect[] GenerateSpriteSheet(int cellCountX, int cellCountY, int padding)
    {
        int width = selectedTexture.width;
        int height = selectedTexture.height;
        SpriteRect[] metaData = new SpriteRect[cellCountX * cellCountY];

        int cellWidth = (width - (cellCountX + 1) * padding) / cellCountX;
        int cellHeight = (height - (cellCountY + 1) * padding) / cellCountY;

        for (int y = 0; y < cellCountY; y++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                int index = y * cellCountX + x;
                int cx = x * cellWidth + (x + 1) * padding;
                int cy = height - (y + 1) * (cellHeight + padding);
                metaData[index] = new SpriteRect {
                    name = textureName + "_" + index.ToString("00"),
                    rect = new Rect(cx, cy, cellWidth, cellHeight),
                    pivot = pivotPoint.value,
                    alignment = SpriteAlignment.Custom
                };
            }
        }

        return metaData;
    }
}
