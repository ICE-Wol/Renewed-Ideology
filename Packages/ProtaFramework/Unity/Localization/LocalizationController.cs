using System;
using System.Collections.Generic;
using System.Linq;
using Prota;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class LocalizationController : MonoBehaviour
{
	public static HashSet<LocalizationController> all = new();

	Component _componentCache;

	[SerializeField]
	string _sourceText = "Hello, World!";

	[SerializeField]
	public string[] elements = Array.Empty<string>();

	public string sourceText
	{
		get => _sourceText;
		set
		{
			if (value == sourceText) return;
			_sourceText = value;
			UpdateText();
		}
	}

	public Color color
	{
		get
		{
			EnsureComponentCache();
			return _componentCache switch
			{
				Text unityText => unityText.color,
				TextMeshPro tmpTextScene => tmpTextScene.color,
				TextMeshProUGUI tmpTextUI => tmpTextUI.color,
				_ => Color.white,
			};
		}
		set
		{
			EnsureComponentCache();
			switch (_componentCache)
			{
				case Text unityText: unityText.color = value; break;
				case TextMeshPro tmpTextScene: tmpTextScene.color = value; break;
				case TextMeshProUGUI tmpTextUI: tmpTextUI.color = value; break;
			}
		}
	}

	public IReadOnlyList<string> GetElements() => elements;
	
	void Awake()
	{
		all.Add(this);
		UpdateText();
	}
	
	void OnDestroy()
	{
		all.Remove(this);
	}
	
	public void UpdateText()
	{
		EnsureComponentCache();
		if (_componentCache == null) return;

		LocalizationDatabase.Translate(sourceText, out var translatedText);
		translatedText = translatedText.PatternReplace(elements);
		SetComponentText(translatedText);
	}
	
	void EnsureComponentCache()
	{
		if(_componentCache != null) return;

		var allComponents = GetComponents<Component>();
		for(int i = 0; i < allComponents.Length; i++)
		{
			var component = allComponents[i];
			if(component is Text || component is TextMeshPro || component is TextMeshProUGUI)
			{
				_componentCache = component;
				return;
			}
		}
		
		if(Application.isPlaying)
			Debug.LogError("LocalizationController: no supported text component found.");
	}

	void SetComponentText(string value)
	{
		switch (_componentCache)
		{
			case Text unityText:
				unityText.text = value;
				return;
			case TextMeshPro tmpTextScene:
				tmpTextScene.text = value;
				return;
			case TextMeshProUGUI tmpTextUI:
				tmpTextUI.text = value;
				return;
			default:
				return;
		}
	}

	public void SetSourceTextAndElements(string sourceText, string[] elements)
	{
		this._sourceText = sourceText;
		this.elements = elements;
		UpdateText();
	}

	// ============================================================================
	// element management
	// ============================================================================

	public void SetElements(string[] elements)
	{
		this.elements = (string[])(elements.Clone());
		UpdateText();
	}

	public void SetElementsCount(int count)
	{
		if (elements.Length != count)
		{
			Array.Resize(ref elements, count);
		}
	}
	
	public void SetElement(int index, string element)
	{
		elements[index] = element;
		UpdateText();
	}
	
}
