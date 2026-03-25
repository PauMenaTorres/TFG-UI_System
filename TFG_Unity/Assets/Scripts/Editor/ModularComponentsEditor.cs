using UnityEditor;
using UnityEngine;

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

            if (currentTheme != null)
            {
                EditorGUILayout.PropertyField(currentTheme);
            }

            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == "m_Script" || property.name == "currentTheme" || property.name == "useOverride" || property.name.StartsWith("override"))
                {
                    continue;
                }

                if (property.name == "textContent" || property.name == "textRole" ||property.name == "colorRole" || property.name == "customColor" || property.name == "alignment" || property.name == "fontStyle")
                {
                    Component comp = target as Component;
                    if (comp != null && comp.transform.parent != null && comp.transform.parent.GetComponent<UnityEngine.UI.Button>() != null)
                    {
                        continue;
                    }
                }

                if (property.type == "ModularStyleBox")
                {
                    DrawStyleBox(property);
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            if (useOverride != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useOverride);
            }

            if (useOverride != null && useOverride.boolValue)
            {
                property = serializedObject.GetIterator();
                enterChildren = true;
                while (property.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if (!property.name.StartsWith("override"))
                    {
                        continue;
                    }

                    if (property.type == "ModularStyleBox")
                    {
                        DrawStyleBox(property);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStyleBox(SerializedProperty property)
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
    }
}