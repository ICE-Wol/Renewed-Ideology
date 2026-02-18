using UnityEditor;

[CustomEditor(typeof(AutoGridLayout), true)]
[CanEditMultipleObjects]
public class AutoGridLayoutInspector : Editor
{
    SerializedProperty autoWidthProp;
    SerializedProperty autoHeightProp;
    SerializedProperty columnsProp;
    SerializedProperty controlAspectRatioProp;
    SerializedProperty aspectRatioProp;

    void OnEnable()
    {
        autoWidthProp = serializedObject.FindProperty("autoWidth");
        autoHeightProp = serializedObject.FindProperty("autoHeight");
        columnsProp = serializedObject.FindProperty("columns");
        controlAspectRatioProp = serializedObject.FindProperty("controlAspectRatio");
        aspectRatioProp = serializedObject.FindProperty("aspectRatio");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(autoWidthProp);
        EditorGUILayout.PropertyField(autoHeightProp);
        EditorGUILayout.PropertyField(columnsProp);
        EditorGUILayout.PropertyField(controlAspectRatioProp);
        if (controlAspectRatioProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(aspectRatioProp);
            EditorGUI.indentLevel--;
        }

        bool autoWidth = autoWidthProp.boolValue;
        bool autoHeight = autoHeightProp.boolValue;
        bool controlAspectRatio = controlAspectRatioProp.boolValue;
        int columns = columnsProp.intValue;
        float aspectRatio = aspectRatioProp.floatValue;

        if (!autoWidth && !autoHeight)
        {
            EditorGUILayout.HelpBox("autoWidth and autoHeight are both disabled. Auto sizing will not run.", MessageType.Info);
        }

        if (autoWidth && columns <= 0)
        {
            EditorGUILayout.HelpBox("columns must be greater than 0 when autoWidth is enabled.", MessageType.Error);
        }

        if (controlAspectRatio)
        {
            if (!(autoWidth ^ autoHeight))
            {
                EditorGUILayout.HelpBox("controlAspectRatio only works when exactly one of autoWidth/autoHeight is enabled.", MessageType.Warning);
            }

            if (aspectRatio <= 0f)
            {
                EditorGUILayout.HelpBox("aspectRatio must be greater than 0 when controlAspectRatio is enabled.", MessageType.Error);
            }
        }

        DrawPropertiesExcluding(serializedObject, "m_Script", "autoWidth", "autoHeight", "columns", "controlAspectRatio", "aspectRatio");

        serializedObject.ApplyModifiedProperties();
    }
}
