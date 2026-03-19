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

                DrawPropertySmart(property);
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

                    if (!property.name.StartsWith("override"))
                    {
                        continue;
                    }

                    DrawPropertySmart(property);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPropertySmart(SerializedProperty property)
        {
            if (property.type == "ModularStyleBox")
            {
                property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName, true);
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    SerializedProperty typeProp = property.FindPropertyRelative("backgroundType");
                    EditorGUILayout.PropertyField(typeProp);

                    int typeValue = typeProp.enumValueIndex;

                    if (typeValue == 0) // SolidColor
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("backgroundColor"));
                    }
                    else if (typeValue == 1) // Sprite
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("backgroundSprite"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("tintColor"));
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }
    }
}
