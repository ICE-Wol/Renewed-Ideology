 
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;
using UnityEngine.Scripting;


namespace Prota.Editor
{
    [InitializeOnLoad]
    public class AutoNormalBind : AssetPostprocessor
    {
        static EditorPrefEntryBool enabled = new("Prota:AutoNormalBindEnabled", true);
        
        static EditorPrefEntryBool swapRG = new("Prota:AutoNormalBindSwapRG", true);
        
        const string normalBindMenu = "ProtaFramework/Functionality/Toggle Normal Bind";
        const string swapRGMenu = "ProtaFramework/Functionality/Toggle Normal Swap RG";
        
        static AutoNormalBind()
        {
            Menu.SetChecked(normalBindMenu, enabled.value);
            Menu.SetChecked(swapRGMenu, swapRG.value);
        }
        
        [MenuItem(normalBindMenu, priority = 2400)]
        static void ToggleNormalBind()
        {
            enabled.value = !enabled.value;
            Menu.SetChecked(normalBindMenu, enabled.value);
        }
        
        [MenuItem(swapRGMenu, priority = 2401)]
        static void ToggleSwapRGB()
        {
            swapRG.value = !swapRG.value;
            Menu.SetChecked(swapRGMenu, swapRG.value);
        }
        
        static readonly List<string> normalExt = new() {
            "_n", "_normal", "_norm", "_normalmap", "_nm", "_法线"
        };
        
        void OnPostprocessTexture(Texture2D tex)
        {
            var importer = assetImporter as TextureImporter;
            if(importer == null) return;
            
            var f = importer.assetPath.AsFileInfo();
            var fname = f.NameWithoutExt().ToLower();
            
            var vext = normalExt.Where(x => fname.EndsWith(x)).FirstOrDefault();
            if(vext != null)        // 判断自己是法线贴图.
            {
                // 把法线贴图的后缀 vext 去掉, 找 atlas 贴图.
                var fatlas = f.WithNameOfSameExt(fname[..^vext.Length]);
                if(!fatlas.Exists)
                {
                    Debug.Log($"Found normal map [{f.FullName}] but no corresponding atlas [{fatlas.FullName}].");
                    return;
                }
                var fatlasPath = fatlas.FullName.FullPathToAssetPath();
                var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(fatlasPath); 
                BindTextureWithNormal(atlas, fatlasPath, tex, this.assetPath);
            }
            else        // 自己不是法线贴图, 那么有没有对应的法线贴图?
            {
                var fnormal = normalExt.Where(ext => f.WithNameOfSameExt(fname + ext).Exists).FirstOrDefault();
                if(fnormal != null)        // 有对应的法线贴图.
                {
                    var fnormalPath = f.WithNameOfSameExt(fname + fnormal).FullName.FullPathToAssetPath();
                    var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(fnormalPath);
                    BindTextureWithNormal(tex, this.assetPath, normal, fnormalPath);
                }
            }
        }
        
        static void CorrectNormalImportSettings(TextureImporter importer)
        {
            if(importer == null)
            {
                Debug.LogWarning($"TextureImporter is null.");
                return;
            }
            
            bool needSave = false;
            if(importer.textureType != TextureImporterType.NormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                needSave = true;
            }
            
            if(importer.swizzleR != TextureImporterSwizzle.G
            || importer.swizzleG != TextureImporterSwizzle.R)
            {
                importer.swizzleR = TextureImporterSwizzle.G;
                importer.swizzleG = TextureImporterSwizzle.R;
                needSave = true;
            }
            
            if(needSave) importer.SaveAndReimport();
        }
        
        static void BindTextureWithNormal(Texture2D atlas, string atlasPath, Texture2D normal, string normalPath)
        {
            var atlasImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
            var normalImporter = AssetImporter.GetAtPath(normalPath) as TextureImporter;
            
            CorrectNormalImportSettings(normalImporter);
            
            var secondaryTextures = atlasImporter.secondarySpriteTextures;
            int normalMapIndex = secondaryTextures.FindIndex(st => st.name == "_NormalMap");
            if(normalMapIndex == -1)
            {
                secondaryTextures = secondaryTextures.Resize(secondaryTextures.Length + 1);
                secondaryTextures.Last(new SecondarySpriteTexture() {
                    name = "_NormalMap",
                    texture = normal
                });
                atlasImporter.secondarySpriteTextures = secondaryTextures;
                atlasImporter.SaveAndReimport();
            }
            else if(secondaryTextures[normalMapIndex].texture != normal)
            {
                secondaryTextures[normalMapIndex].texture = normal;
                atlasImporter.secondarySpriteTextures = secondaryTextures;
                atlasImporter.SaveAndReimport();
            }
            else
            {
                Debug.Log($"Normal map [{normalPath}] already binded to [{atlasPath}].");
            }
            
            
        }
        
    }
}
