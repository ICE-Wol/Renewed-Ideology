using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProtaFramework.Unity
{
    /// <summary>
    /// Synchronizes a SpriteRenderer's size and position with a RectTransform.
    /// Ensures the SpriteRenderer uses the sliced draw mode.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SyncSpriteSizeToRectTransform : UIBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public RectTransform rectTransform;

        protected override void Awake()
        {
            if(Application.isPlaying)
            {
                SyncSize();
            }
            else
            {
                #if UNITY_EDITOR
                EditorApplication.delayCall += SyncSize;
                #endif
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SyncSize();
        }
		
		void Update()
		{
			if(spriteRenderer == null) spriteRenderer = this.GetComponent<SpriteRenderer>();
			if(rectTransform == null) rectTransform = this.transform.parent?.GetComponent<RectTransform>();
			SyncSize();
		}

        /// <summary>
        /// Synchronizes the SpriteRenderer size and position with the RectTransform.
        /// </summary>
        [ContextMenu("Sync Size")]
        public void SyncSize()
        {
            if (spriteRenderer == null)
                return;

            if (spriteRenderer.sprite == null)
                return;

            if (rectTransform == null)
                return;
            
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;

            Vector2 size = rectTransform.rect.size;

            spriteRenderer.size = size;

            Sprite sprite = spriteRenderer.sprite;
            Vector2 spritePivotNormalized = sprite.pivot / sprite.rect.size;
            //  new Vector2(
            //     sprite.pivot.x / sprite.rect.width,
            //     sprite.pivot.y / sprite.rect.height
            // );

			var pivot = rectTransform.pivot;

            Vector3 pivotOffset = new Vector3(
                ((1 - pivot.x) - spritePivotNormalized.x) * size.x,
                ((1 - pivot.y) - spritePivotNormalized.y) * size.y,
                0f
            );

            spriteRenderer.transform.localPosition = pivotOffset;
        }
	}
}
