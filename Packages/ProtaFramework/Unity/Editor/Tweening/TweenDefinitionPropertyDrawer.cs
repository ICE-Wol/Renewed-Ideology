using UnityEditor;
using UnityEngine;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(TweenDefinition))]
    public class TweenDefinitionPropertyDrawer : PropertyDrawer
    {
        // bool showHDR = false;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty GetProp(string name)
            {
                return property.FindPropertyRelative(name);
            }
			
			var duration = GetProp("duration");

            var move = GetProp("move");
            var moveFrom = GetProp("moveFrom");
            var moveTo = GetProp("moveTo");
            var moveEase = GetProp("moveEase");
            var moveCurve = GetProp("moveCurve");

            var rotate = GetProp("rotate");
            var rotateFrom = GetProp("rotateFrom");
            var rotateTo = GetProp("rotateTo");
            var rotateEase = GetProp("rotateEase");
            var rotateCurve = GetProp("rotateCurve");
            var useClosestRotate = GetProp("useClosestRotate");

            var scale = GetProp("scale");
            var scaleFrom = GetProp("scaleFrom");
            var scaleTo = GetProp("scaleTo");
            var scaleEase = GetProp("scaleEase");
            var scaleCurve = GetProp("scaleCurve");

            var color = GetProp("color");
            var colorFrom = GetProp("colorFrom");
            var colorTo = GetProp("colorTo");
            var colorEase = GetProp("colorEase");
            var colorGradient = GetProp("colorGradient");

            var alpha = GetProp("alpha");
            var alphaFrom = GetProp("alphaFrom");
            var alphaTo = GetProp("alphaTo");
            var alphaEase = GetProp("alphaEase");
            var alphaCurve = GetProp("alphaCurve");

            var size = GetProp("size");
            var sizeFrom = GetProp("sizeFrom");
            var sizeTo = GetProp("sizeTo");
            var sizeEase = GetProp("sizeEase");
            var sizeCurve = GetProp("sizeCurve");

            var sizeX = GetProp("sizeX");
            var sizeXFrom = GetProp("sizeXFrom");
            var sizeXTo = GetProp("sizeXTo");
            var sizeXEase = GetProp("sizeXEase");
            var sizeXCurve = GetProp("sizeXCurve");

            var sizeY = GetProp("sizeY");
            var sizeYFrom = GetProp("sizeYFrom");
            var sizeYTo = GetProp("sizeYTo");
            var sizeYEase = GetProp("sizeYEase");
            var sizeYCurve = GetProp("sizeYCurve");
            
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);
            ProtaEditorUtils.SeperateLine(2);
			
			EditorGUI.PropertyField(fieldRect, duration, new GUIContent("时长 duration"));
            fieldRect.y += lineHeight + spacing;
			
			EditorGUI.PropertyField(fieldRect, move, new GUIContent("移动 move"));
            fieldRect.y += lineHeight + spacing;
            if (move.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, moveFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, moveTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, moveEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (moveEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, moveCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.PropertyField(fieldRect, rotate, new GUIContent("旋转 rotate"));
            fieldRect.y += lineHeight + spacing;
            if (rotate.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, rotateFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, rotateTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, rotateEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (rotateEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, rotateCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
                EditorGUI.PropertyField(fieldRect, useClosestRotate, new GUIContent("    use Closest rotate"));
                fieldRect.y += lineHeight + spacing;
            }

            EditorGUI.PropertyField(fieldRect, scale, new GUIContent("缩放 scale"));
            fieldRect.y += lineHeight + spacing;
            if (scale.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, scaleFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, scaleTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, scaleEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (scaleEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, scaleCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }
            
            EditorGUI.PropertyField(fieldRect, color, new GUIContent("颜色 color"));
            fieldRect.y += lineHeight + spacing;
            if (color.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, colorFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, colorTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, colorEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (colorEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, colorGradient, new GUIContent("    gradient"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.PropertyField(fieldRect, alpha, new GUIContent("透明度 alpha"));
            fieldRect.y += lineHeight + spacing;
            if (alpha.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, alphaFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, alphaTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, alphaEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (alphaEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, alphaCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.PropertyField(fieldRect, size, new GUIContent("尺寸 size"));
            fieldRect.y += lineHeight + spacing;
            if (size.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, sizeFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (sizeEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, sizeCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.PropertyField(fieldRect, sizeX, new GUIContent("尺寸X sizeX"));
            fieldRect.y += lineHeight + spacing;
            if (sizeX.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, sizeXFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeXTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeXEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (sizeXEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, sizeXCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.PropertyField(fieldRect, sizeY, new GUIContent("尺寸Y sizeY"));
            fieldRect.y += lineHeight + spacing;
            if (sizeY.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, sizeYFrom, new GUIContent("    from"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeYTo, new GUIContent("    to"));
                fieldRect.y += lineHeight + spacing;
                EditorGUI.PropertyField(fieldRect, sizeYEase, new GUIContent("    ease"));
                fieldRect.y += lineHeight + spacing;
                if (sizeYEase.intValue < 0)
                {
                    EditorGUI.PropertyField(fieldRect, sizeYCurve, new GUIContent("    curve"));
                    fieldRect.y += lineHeight + spacing;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float step = lineHeight + spacing;

            float totalHeight = 0;
            
            SerializedProperty GetProp(string name)
            {
                return property.FindPropertyRelative(name);
            }
			
			totalHeight += step;
            
            var move = GetProp("move");
            totalHeight += step;
            totalHeight += move.boolValue ? 3 * step : 0;
            var moveEase = GetProp("moveEase");
            totalHeight += move.boolValue && moveEase.intValue < 0 ? step : 0;
            
            var rotate = GetProp("rotate");
            totalHeight += step;
            totalHeight += rotate.boolValue ? 4 * step : 0;
            var rotateEase = GetProp("rotateEase");
            totalHeight += rotate.boolValue && rotateEase.intValue < 0 ? step : 0;
            
            var scale = GetProp("scale");
            totalHeight += step;
            totalHeight += scale.boolValue ? 3 * step : 0;
            var scaleEase = GetProp("scaleEase");
            totalHeight += scale.boolValue && scaleEase.intValue < 0 ? step : 0;
            
            var color = GetProp("color");
            totalHeight += step;
            totalHeight += color.boolValue ? 3 * step : 0;
            var colorEase = GetProp("colorEase");
            totalHeight += color.boolValue && colorEase.intValue < 0 ? step : 0;
            
            var alpha = GetProp("alpha");
            totalHeight += step;
            totalHeight += alpha.boolValue ? 3 * step : 0;
            var alphaEase = GetProp("alphaEase");
            totalHeight += alpha.boolValue && alphaEase.intValue < 0 ? step : 0;
            
            var size = GetProp("size");
            totalHeight += step;
            totalHeight += size.boolValue ? 3 * step : 0;
            var sizeEase = GetProp("sizeEase");
            totalHeight += size.boolValue && sizeEase.intValue < 0 ? step : 0;
            
            var sizeX = GetProp("sizeX");
            totalHeight += step;
            totalHeight += sizeX.boolValue ? 3 * step : 0;
            var sizeXEase = GetProp("sizeXEase");
            totalHeight += sizeX.boolValue && sizeXEase.intValue < 0 ? step : 0;
            
            var sizeY = GetProp("sizeY");
            totalHeight += step;
            totalHeight += sizeY.boolValue ? 3 * step : 0;
            var sizeYEase = GetProp("sizeYEase");
            totalHeight += sizeY.boolValue && sizeYEase.intValue < 0 ? step : 0;

            return totalHeight;
        }
    }
}
