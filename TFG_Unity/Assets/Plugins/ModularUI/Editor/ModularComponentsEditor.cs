using UnityEditor;
using UnityEngine;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularUIRuntime.ModularComponents), true)]
    public class ModularComponentsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (target is ModularUIRuntime.ModularText)
            {
                SerializedProperty overrideFontProp = serializedObject.FindProperty("overrideFont");
                SerializedProperty overrideFontAssetProp = serializedObject.FindProperty("overrideFontAsset");
                if (overrideFontProp != null && overrideFontProp.objectReferenceValue != null && 
                    (overrideFontAssetProp == null || overrideFontAssetProp.objectReferenceValue == null))
                {
                    EditorGUILayout.HelpBox("Warning: 'overrideFont' (standard Font) is assigned but deprecated. In Edit Mode it won't apply to avoid auto-dirtying prefabs.\n\nPlease assign a TextMeshPro font asset to 'overrideFontAsset' instead.", MessageType.Warning);
                }
            }

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
                DrawOverridesSection();
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                foreach (var t in targets)
                {
                    if (t is ModularUIRuntime.ModularComponents modularComponent)
                    {
                        modularComponent.ApplyThemeInEditor();
                    }
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Active Theme Preview", GUILayout.Height(30)))
            {
                foreach (var t in targets)
                {
                    if (t is ModularUIRuntime.ModularComponents modularComponent)
                    {
                        Undo.RecordObject(modularComponent, "Apply Active Theme Preview");

                        var textComponent = modularComponent.GetComponent<TMPro.TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            Undo.RecordObject(textComponent, "Apply Active Theme Preview");
                        }

                        var imageComponents = modularComponent.GetComponents<UnityEngine.UI.Image>();
                        foreach (var image in imageComponents)
                        {
                            Undo.RecordObject(image, "Apply Active Theme Preview");
                        }

                        var sliderComponent = modularComponent.GetComponent<UnityEngine.UI.Slider>();
                        if (sliderComponent != null)
                        {
                            Undo.RecordObject(sliderComponent, "Apply Active Theme Preview");
                        }

                        var toggleComponent = modularComponent.GetComponent<UnityEngine.UI.Toggle>();
                        if (toggleComponent != null)
                        {
                            Undo.RecordObject(toggleComponent, "Apply Active Theme Preview");
                        }

                        var selectableComponent = modularComponent.GetComponent<UnityEngine.UI.Selectable>();
                        if (selectableComponent != null)
                        {
                            Undo.RecordObject(selectableComponent, "Apply Active Theme Preview");
                        }

                        modularComponent.ApplyThemeInEditor();

                        PrefabUtility.RecordPrefabInstancePropertyModifications(modularComponent);
                        if (textComponent != null)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(textComponent);
                        }
                        foreach (var image in imageComponents)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
                        }
                        if (sliderComponent != null)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(sliderComponent);
                        }
                        if (toggleComponent != null)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(toggleComponent);
                        }
                        if (selectableComponent != null)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(selectableComponent);
                        }
                    }
                }
            }
        }

        private void DrawOverridesSection()
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

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

            EditorGUILayout.Space(5);

            if (target is ModularButton)
            {
                if (GUILayout.Button("Sync All Button States", GUILayout.Height(20)))
                {
                    ApplyNormalToAll("overrideNormalBG", new string[] { "overrideHoveredBG", "overridePressedBG", "overrideDisabledBG" });
                }
            }

            if (target is ModularSlider)
            {
                if (GUILayout.Button("Sync Background to Fill", GUILayout.Height(20)))
                {
                    ApplyNormalToAll("overrideBackground", new string[] { "overrideFill" });
                }
            }

            if (target is ModularToggle)
            {
                if (GUILayout.Button("Sync Toggle States", GUILayout.Height(20)))
                {
                    ApplyNormalToAll("overrideBackground", new string[] { "overrideCheckmark" });
                }
            }
        }

        private void DrawStyleBox(SerializedProperty property)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                SerializedProperty typeProp = property.FindPropertyRelative("backgroundType");
                EditorGUILayout.PropertyField(typeProp);

                if (typeProp.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("backgroundColor"));
                }

                if (typeProp.enumValueIndex == 1)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("backgroundSprite"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("tintColor"));
                }

                EditorGUI.indentLevel--;
            }
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