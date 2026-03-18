using UnityEditor;

[CustomEditor(typeof(ModularButton))]
public class ModularButtonEditor : Editor
{
    private SerializedProperty currentTheme;
    private SerializedProperty useOverride;
    private SerializedProperty overrideNormal;
    private SerializedProperty overrideHovered;
    private SerializedProperty overridePressed;
    private SerializedProperty overrideDisabled;

    private void OnEnable()
    {
        currentTheme = serializedObject.FindProperty("currentTheme");
        useOverride = serializedObject.FindProperty("useOverride");
        overrideNormal = serializedObject.FindProperty("overrideNormal");
        overrideHovered = serializedObject.FindProperty("overrideHovered");
        overridePressed = serializedObject.FindProperty("overridePressed");
        overrideDisabled = serializedObject.FindProperty("overrideDisabled");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(currentTheme);
        EditorGUILayout.PropertyField(useOverride);

        if (useOverride.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(overrideHovered);
            EditorGUILayout.PropertyField(overrideNormal);
            EditorGUILayout.PropertyField(overridePressed);
            EditorGUILayout.PropertyField(overrideDisabled);
        }

        serializedObject.ApplyModifiedProperties();
    }
}