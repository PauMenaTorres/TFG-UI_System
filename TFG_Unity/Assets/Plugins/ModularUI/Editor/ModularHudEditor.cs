using UnityEngine;
using UnityEditor;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularUIRuntime.ModularHudController))]
    public class ModularHudEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ModularUIRuntime.ModularHudController hud = (ModularUIRuntime.ModularHudController)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Theme to HUD UI", GUILayout.Height(30)))
            {
                foreach (var targetObj in targets)
                {
                    if (targetObj is ModularUIRuntime.ModularHudController hudController)
                    {
                        var modularComponents = hudController.GetComponentsInChildren<ModularUIRuntime.ModularComponents>(true);
                        foreach (var comp in modularComponents)
                        {
                            Undo.RecordObject(comp, "Apply Theme to HUD UI");

                            var textComponent = comp.GetComponent<TMPro.TextMeshProUGUI>();
                            if (textComponent != null)
                            {
                                Undo.RecordObject(textComponent, "Apply Theme to HUD UI");
                            }

                            var imageComponents = comp.GetComponents<UnityEngine.UI.Image>();
                            foreach (var image in imageComponents)
                            {
                                Undo.RecordObject(image, "Apply Theme to HUD UI");
                            }

                            var sliderComponent = comp.GetComponent<UnityEngine.UI.Slider>();
                            if (sliderComponent != null)
                            {
                                Undo.RecordObject(sliderComponent, "Apply Theme to HUD UI");
                            }

                            var toggleComponent = comp.GetComponent<UnityEngine.UI.Toggle>();
                            if (toggleComponent != null)
                            {
                                Undo.RecordObject(toggleComponent, "Apply Theme to HUD UI");
                            }

                            var selectableComponent = comp.GetComponent<UnityEngine.UI.Selectable>();
                            if (selectableComponent != null)
                            {
                                Undo.RecordObject(selectableComponent, "Apply Theme to HUD UI");
                            }

                            comp.ApplyThemeInEditor();

                            PrefabUtility.RecordPrefabInstancePropertyModifications(comp);
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

                        var fitters = hudController.GetComponentsInChildren<ModularUIRuntime.ModularPanelFitter>(true);
                        foreach (var fitter in fitters)
                        {
                            var rectTransform = fitter.GetComponent<RectTransform>();
                            if (rectTransform != null)
                            {
                                Undo.RecordObject(rectTransform, "Apply Theme to HUD UI");
                            }
                            Undo.RecordObject(fitter, "Apply Theme to HUD UI");

                            fitter.FitToParent();

                            if (rectTransform != null)
                            {
                                PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
                            }
                            PrefabUtility.RecordPrefabInstancePropertyModifications(fitter);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}   