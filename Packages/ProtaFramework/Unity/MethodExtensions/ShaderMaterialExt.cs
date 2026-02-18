using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static IEnumerable<string> EnumeratePropertieNames(this Shader shader)
        {
            var count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                yield return shader.GetPropertyName(i);
            }
        }
        
        public static IEnumerable<string> EnumeratePropertieNames(this Material material)
        {
            return material.shader.EnumeratePropertieNames();
        }
        
        public static ShaderPropertyType GetPropertyType(this Shader shader)
        {
            var count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                shader.GetPropertyType(i);
            }
            return ShaderPropertyType.Float;
        }
        
        
        // texture: 贴图
        // float4 uvRect: sprite在贴图上的位置, 范围 [0, 1].
        // float2 pivot: sprite中心在 sprite 上的位置, 范围 [0, 1].
        public static void SetSprite(this Material material, int? texturePropId, int? uvRectPropId, int? pivotPropId, int? spriteSizeId, Sprite sprite)
        {
            if (sprite == null) return;
            GetSpriteInfoForMat(sprite, out var uv, out var pivotRatio, out var spriteSize);
            if (texturePropId.HasValue) material.SetTexture(texturePropId.Value, sprite.texture);
            if (uvRectPropId.HasValue) material.SetVector(uvRectPropId.Value, uv);
            if (pivotPropId.HasValue) material.SetVector(pivotPropId.Value, pivotRatio);
            if (spriteSizeId.HasValue) material.SetVector(spriteSizeId.Value, spriteSize);
        }

        public static void SetSprite(this MaterialPropertyBlock material, int? texturePropId, int? uvRectPropId, int? pivotPropId, int? spriteSizeId, Sprite sprite)
        {
            if (sprite == null) return;
            GetSpriteInfoForMat(sprite, out var uv, out var pivotRatio, out var spriteSize);
            if (texturePropId.HasValue) material.SetTexture(texturePropId.Value, sprite.texture);
            if (uvRectPropId.HasValue) material.SetVector(uvRectPropId.Value, uv);
            if (pivotPropId.HasValue) material.SetVector(pivotPropId.Value, pivotRatio);
            if (spriteSizeId.HasValue) material.SetVector(spriteSizeId.Value, spriteSize);
        }
        
        public static void GetSpriteInfoForMat(this Sprite sprite, out Vector4 uv, out Vector2 pivotRatio, out Vector2 spriteSize)
        {
            var texture = sprite.texture;
            var pixelSize = texture.texelSize;
            var texRect = sprite.textureRect;  // in pixel.
            var min = texRect.min * pixelSize;
            var max = texRect.max * pixelSize;
            uv = new Vector4(min.x, min.y, max.x, max.y);
            var pivot = sprite.pivot;       // position in the rect, in pixel.
            pivotRatio = pivot / texRect.size;
            spriteSize = texRect.size / sprite.pixelsPerUnit;
        }

        public static void SetSprite(this Material material, string texture, string uvRect, string pivot, string spriteSize, Sprite sprite)
        {
            int? texturePropId = texture == null ? null : Shader.PropertyToID(texture);
            int? uvRectPropId = uvRect == null ? null : Shader.PropertyToID(uvRect);
            int? pivotPropId = pivot == null ? null : Shader.PropertyToID(pivot);
            int? spriteSizeId = spriteSize == null ? null : Shader.PropertyToID(spriteSize);
            material.SetSprite(texturePropId, uvRectPropId, pivotPropId, spriteSizeId, sprite);
        }
        
        public static void SetSprite(this MaterialPropertyBlock material, string texture, string uvRect, string pivot, string spriteSize, Sprite sprite)
        {
            int? texturePropId = texture == null ? null : Shader.PropertyToID(texture);
            int? uvRectPropId = uvRect == null ? null : Shader.PropertyToID(uvRect);
            int? pivotPropId = pivot == null ? null : Shader.PropertyToID(pivot);
            int? spriteSizeId = spriteSize == null ? null : Shader.PropertyToID(spriteSize);
            material.SetSprite(texturePropId, uvRectPropId, pivotPropId, spriteSizeId, sprite);
        }
        
    }
}
