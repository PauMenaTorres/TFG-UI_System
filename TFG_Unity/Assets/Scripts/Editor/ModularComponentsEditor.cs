using UnityEditor;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularUIRuntime.ModularComponents), true)]
    public class ModularComponentsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty currentTheme = serializedObject.FindProperty("currentTheme");
            SerializedProperty useOverride = serializedObject.FindProperty("useOverride");
            SerializedProperty property = serializedObject.GetIterator();

            if (currentTheme != null)
            {
                EditorGUILayout.PropertyField(currentTheme);
            }

            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == "m_Script" || property.name == "currentTheme" || property.name == "useOverride" || property.name.StartsWith("override"))
                {
                    continue;
                }

                EditorGUILayout.PropertyField(property, true);
            }

            if (useOverride != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useOverride);
            }

            if (useOverride != null && useOverride.boolValue)
            {
                property.Reset();
                enterChildren = true;

                while (property.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if (property.name.StartsWith("override"))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
