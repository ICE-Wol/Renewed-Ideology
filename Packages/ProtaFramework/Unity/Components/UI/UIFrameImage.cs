using UnityEngine;
using UnityEngine.UI;
using Prota.Unity;
using System.Data;

namespace UI
{
	[ExecuteAlways]
	public class UIFrameImage : MonoBehaviour
	{
		[SerializeField] Sprite _backgroundSprite = null!;
		[SerializeField] Sprite _frameSprite = null!;

		[SerializeField] Color _backgroundColor = Color.white;
		[SerializeField] Color _frameColor = Color.white;

		[SerializeField] float _bgPaddingLeft = 0f;
		[SerializeField] float _bgPaddingRight = 0f;
		[SerializeField] float _bgPaddingTop = 0f;
		[SerializeField] float _bgPaddingBottom = 0f;

		[SerializeField] float _framePaddingLeft = 0f;
		[SerializeField] float _framePaddingRight = 0f;
		[SerializeField] float _framePaddingTop = 0f;
		[SerializeField] float _framePaddingBottom = 0f;

		[SerializeField] float _pixelsPerUnitMultiplier = 1f;
		[SerializeField] bool _preserveAspect = false;
		
		public Sprite backgroundSprite
		{
			get => _backgroundSprite;
			set
			{
				if(_backgroundSprite == value) return;
				_backgroundSprite = value;
				ApplyValues();
			}
		}
		
		public Sprite frameSprite
		{
			get => _frameSprite;
			set
			{
				if(_frameSprite == value) return;
				_frameSprite = value;
				ApplyValues();
			}
		}

		public Color backgroundColor
		{
			get => _backgroundColor;
			set
			{
				if (_backgroundColor == value) return;
				_backgroundColor = value;
				ApplyValues();
			}
		}

		public Color frameColor
		{
			get => _frameColor;
			set
			{
				if (_frameColor == value) return;
				_frameColor = value;
				ApplyValues();
			}
		}

		public float paddingLeft
		{
			get => _bgPaddingLeft;
			set
			{
				if (_bgPaddingLeft == value) return;
				_bgPaddingLeft = value;
				ApplyValues();
			}
		}

		public float paddingRight
		{
			get => _bgPaddingRight;
			set
			{
				if (_bgPaddingRight == value) return;
				_bgPaddingRight = value;
				ApplyValues();
			}
		}

		public float paddingTop
		{
			get => _bgPaddingTop;
			set
			{
				if (_bgPaddingTop == value) return;
				_bgPaddingTop = value;
				ApplyValues();
			}
		}

		public float paddingBottom
		{
			get => _bgPaddingBottom;
			set
			{
				if (_bgPaddingBottom == value) return;
				_bgPaddingBottom = value;
				ApplyValues();
			}
		}

		public float framePaddingLeft
		{
			get => _framePaddingLeft;
			set
			{
				if (_framePaddingLeft == value) return;
				_framePaddingLeft = value;
				ApplyValues();
			}
		}

		public float framePaddingRight
		{
			get => _framePaddingRight;
			set
			{
				if (_framePaddingRight == value) return;
				_framePaddingRight = value;
				ApplyValues();
			}
		}

		public float framePaddingTop
		{
			get => _framePaddingTop;
			set
			{
				if (_framePaddingTop == value) return;
				_framePaddingTop = value;
				ApplyValues();
			}
		}

		public float framePaddingBottom
		{
			get => _framePaddingBottom;
			set
			{
				if (_framePaddingBottom == value) return;
				_framePaddingBottom = value;
				ApplyValues();
			}
		}

		public float pixelsPerUnitMultiplier
		{
			get => _pixelsPerUnitMultiplier;
			set
			{
				if (_pixelsPerUnitMultiplier == value) return;
				_pixelsPerUnitMultiplier = value;
				ApplyValues();
			}
		}

		public bool preserveAspect
		{
			get => _preserveAspect;
			set
			{
				if (_preserveAspect == value) return;
				_preserveAspect = value;
				ApplyValues();
			}
		}
		
		[field: SerializeField]
		public Image bgImage { get; private set; }
		
		[field: SerializeField]
		public Image frameImage { get; private set; }
		
		void Awake()
		{
		
		}
		
		void OnDestroy()
		{
			ClearImages();
		}
		
		public void ClearImages()
		{
			if (bgImage != null)
			{
				DestroyManagedImage(bgImage);
				bgImage = null;
			}
			
			if (frameImage != null)
			{
				DestroyManagedImage(frameImage);
				frameImage = null;
			}
		}
		
		public void RebuildImages()
		{
			ClearImages();
			if (!IsCacheValid())
			{
				EnsureStructure();
			}
			ApplyValues();
		}
		
		bool IsCacheValid()
		{
			if (bgImage == null) return false;
			if (frameImage == null) return false;
			if (bgImage.rectTransform.parent != transform) return false;
			if (frameImage.rectTransform.parent != transform) return false;
			return true;
		}
		
		void EnsureStructure()
		{
			bgImage = EnsureChildImage(bgImage, "$BgImage");
			frameImage = EnsureChildImage(frameImage, "$FrameImage");
		}
		
		Image EnsureChildImage(Image cached, string name)
		{
			if (this.gameObject.IsPrefab()) return null!;
			
			if (cached == null)
			{
				var go = new GameObject(name);
				go.AddComponent<RectTransform>();
				go.AddComponent<CanvasRenderer>();
				var image = go.AddComponent<Image>();
				image.transform.SetParent(this.transform, worldPositionStays: false);
				SetupRectTransform(image.rectTransform);
				image.type = Image.Type.Sliced;
				go.name = name;
				cached = image;
			}
			
			return cached;
		}
		
		void SetupRectTransform(RectTransform rectTransform)
		{
			if (rectTransform == null) return;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.localRotation = Quaternion.identity;
		}

		void ApplyPadding(RectTransform rectTransform, float left, float right, float top, float bottom)
		{
			rectTransform.offsetMin = new Vector2(left, bottom);
			rectTransform.offsetMax = new Vector2(-right, -top);
		}
		
		void DestroyManagedImage(Image image)
		{
			if (image == null) return;
			DestroyImmediate(image.gameObject);
		}

		public void ApplyValues()
		{
			if (this.gameObject.IsPrefab()) return;
			
			if(this.bgImage == null)
			{
				Debug.LogError($"bg image not assigned {this.GetNamePath()}");
				return;
			}

			if (this.frameImage == null)
			{
				Debug.LogError($"frame image not assigned {this.GetNamePath()}");
				return;
			}
			
			var bgImage = this.bgImage;
			var frameImage = this.frameImage;

			bgImage.sprite = backgroundSprite;
			bgImage.color = _backgroundColor;
			bgImage.pixelsPerUnitMultiplier = _pixelsPerUnitMultiplier;
			bgImage.preserveAspect = _preserveAspect;
			bgImage.raycastTarget = true;
			SetImageType(bgImage, backgroundSprite);

			frameImage.sprite = frameSprite;
			frameImage.color = _frameColor;
			frameImage.pixelsPerUnitMultiplier = _pixelsPerUnitMultiplier;
			frameImage.preserveAspect = _preserveAspect;
			frameImage.raycastTarget = false;
			SetImageType(frameImage, frameSprite);

			var bgTransform = bgImage.rectTransform;
			var frameTransform = frameImage.rectTransform;

			ApplyPadding(bgTransform, _bgPaddingLeft, _bgPaddingRight, _bgPaddingTop, _bgPaddingBottom);
			ApplyPadding(frameTransform, _framePaddingLeft, _framePaddingRight, _framePaddingTop, _framePaddingBottom);

			bgTransform.SetSiblingIndex(0);
			frameTransform.SetSiblingIndex(1);
		}
		
		void SetImageType(Image image, Sprite sprite)
		{
			if (sprite == null)
			{
				image.type = Image.Type.Simple;
				return;
			}
			
			if (backgroundSprite.border.IsZero())
			{
				bgImage.type = Image.Type.Simple;
			}
			else
			{
				bgImage.type = Image.Type.Sliced;
			}
		}
	}
}
