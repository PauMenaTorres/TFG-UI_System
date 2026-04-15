using UnityEditor;
using UnityEngine;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularThemeData))]
    public class ModularThemeDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == "m_Script")
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(property);
                    GUI.enabled = true;
                    continue;
                }

                if (property.name == "sliderBackground")
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Sync Button Normal to All States", GUILayout.Height(20)))
                    {
                        ApplyNormalToAll("buttonNormal", new string[] { "buttonHovered", "buttonPressed", "buttonDisabled" });
                    }
                    EditorGUILayout.Space();
                }

                if (property.name == "toggleBackground")
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Sync Slider Background to Fill", GUILayout.Height(20)))
                    {
                        ApplyNormalToAll("sliderBackground", new string[] { "sliderFill" });
                    }
                    EditorGUILayout.Space();
                }

                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ApplyNormalToAll(string normalName, string[] targetNames)
        {
            SerializedProperty normalProp = serializedObject.FindProperty(normalName);

            if (normalProp == null)
            {
                return;
            }

            foreach (string targetName in targetNames)
            {
                SerializedProperty targetProp = serializedObject.FindProperty(targetName);

                if (targetProp != null)
                {
                    targetProp.FindPropertyRelative("backgroundType").enumValueIndex = normalProp.FindPropertyRelative("backgroundType").enumValueIndex;
                    targetProp.FindPropertyRelative("backgroundColor").colorValue = normalProp.FindPropertyRelative("backgroundColor").colorValue;
                    targetProp.FindPropertyRelative("backgroundSprite").objectReferenceValue = normalProp.FindPropertyRelative("backgroundSprite").objectReferenceValue;
                    targetProp.FindPropertyRelative("tintColor").colorValue = normalProp.FindPropertyRelative("tintColor").colorValue;
                }
            }
        }
    }
}