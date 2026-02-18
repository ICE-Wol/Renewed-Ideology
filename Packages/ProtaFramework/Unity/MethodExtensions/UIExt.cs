using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Unity
{

	public static partial class UnityMethodExtensions
	{
		public static bool IsInteractable(this GameObject gameObject)
		{
			if(gameObject == null)
				throw new Exception($"{gameObject.GetNamePath()}: GameObject is null.");
			
			var tr = gameObject.transform;
			while(tr != null)
			{
				if(tr.TryGetComponent<CanvasGroup>(out var canvasGroup))
					if(!canvasGroup.interactable)
						return false;
				tr = tr.parent;
			}
			
			return true;
		}
	}
	
}