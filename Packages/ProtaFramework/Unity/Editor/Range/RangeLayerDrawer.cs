using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota.Editor
{

	[CustomPropertyDrawer(typeof(RangeLayer))]
	public class RangeLayerDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var idProp = property.FindPropertyRelative("id");
			if (idProp == null)
			{
				EditorGUI.LabelField(position, label.text, "Error: id property not found");
				return;
			}

			var nameToLayer = RangeLayer.nameToLayer;
			if (nameToLayer == null || nameToLayer.Count == 0)
			{
				EditorGUI.PropertyField(position, idProp, label);
				return;
			}

			var names = nameToLayer.Keys.ToArray();
			var layers = nameToLayer.Values.ToArray();
			var currentId = idProp.intValue;

			int currentIndex = -1;
			for (int i = 0; i < layers.Length; i++)
			{
				if (layers[i].id == currentId)
				{
					currentIndex = i;
					break;
				}
			}

			// 如果当前 id 不在 dictionary 中, 增加一个显示项
			if (currentIndex == -1)
			{
				var namesList = names.ToList();
				namesList.Add($"Unknown ({currentId})");
				names = namesList.ToArray();
				currentIndex = names.Length - 1;
			}

			EditorGUI.BeginProperty(position, label, property);
			
			int nextIndex = EditorGUI.Popup(position, label.text, currentIndex, names);
			if (nextIndex != currentIndex && nextIndex >= 0 && nextIndex < layers.Length)
			{
				idProp.intValue = layers[nextIndex].id;
			}
			else if (nextIndex != currentIndex && nextIndex == layers.Length)
			{
				// Keep the unknown value or handle it. 
				// In this case, the popup won't let you change it to something else unless you select a valid one.
			}

			EditorGUI.EndProperty();
		}
	}
}
