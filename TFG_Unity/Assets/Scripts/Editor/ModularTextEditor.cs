using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModularText))]
public class ModularTextEditor : Editor
{
    private SerializedProperty currentTheme;
    private SerializedProperty textRole;
    private SerializedProperty useOverride;
    private SerializedProperty overrideTextFont;
    private SerializedProperty overrideTitleFont;
    private SerializedProperty overridePrimaryColor;
    private SerializedProperty overrideSecondaryColor;
    private SerializedProperty overrideTextFontSize;
    private SerializedProperty overrideTitleFontSize;

    private void OnEnable()
    {
        currentTheme = serializedObject.FindProperty("currentTheme");
        textRole = serializedObject.FindProperty("textRole");
        useOverride = serializedObject.FindProperty("useOverride");
        overrideTextFont = serializedObject.FindProperty("overrideTextFont");
        overrideTitleFont = serializedObject.FindProperty("overrideTitleFont");
        overridePrimaryColor = serializedObject.FindProperty("overridePrimaryColor");
        overrideSecondaryColor = serializedObject.FindProperty("overrideSecondaryColor");
        overrideTextFontSize = serializedObject.FindProperty("overrideTextFontSize");
        overrideTitleFontSize = serializedObject.FindProperty("overrideTitleFontSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(currentTheme);
        EditorGUILayout.PropertyField(textRole);
        EditorGUILayout.PropertyField(useOverride);

        if (useOverride != null && useOverride.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(overrideTextFont);
            EditorGUILayout.PropertyField(overrideTitleFont);
            EditorGUILayout.PropertyField(overridePrimaryColor);
            EditorGUILayout.PropertyField(overrideSecondaryColor);
            EditorGUILayout.PropertyField(overrideTextFontSize);
            EditorGUILayout.PropertyField(overrideTitleFontSize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}