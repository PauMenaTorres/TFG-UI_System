using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIConfigurationListener
    {
        private static UIConfiguration lastConfig;
        private static UIConfiguration.TargetPlatform lastPlatform = UIConfiguration.TargetPlatform.Desktop;
        private static bool isReadapting = false;

        static ModularUIConfigurationListener()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            RefreshConfigReference();
        }

        private static void OnEditorUpdate()
        {
            RefreshConfigReference();
        }

        private static void RefreshConfigReference()
        {
            if (isReadapting) return;

            UIConfiguration config = AssetDatabase.LoadAssetAtPath<UIConfiguration>("Assets/Plugins/ModularUI/Resources/UIConfiguration.asset");
            
            if (config == null)
            {
                config = AssetDatabase.LoadAssetAtPath<UIConfiguration>("Packages/com.pau.modularui/Resources/UIConfiguration.asset");
            }

            if (config != null && config.selectedPlatform != lastPlatform)
            {
                lastPlatform = config.selectedPlatform;
                TriggerSceneReadaptation(config);
            }

            lastConfig = config;
        }

        private static void TriggerSceneReadaptation(UIConfiguration config)
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            
            if (string.IsNullOrEmpty(currentScenePath))
            {
                return;
            }

            isReadapting = true;
            EditorApplication.delayCall += () =>
            {
                try
                {
                    if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                    {
                        EditorApplication.delayCall += () => TriggerSceneReadaptation(config);
                        isReadapting = false;
                        return;
                    }

                    bool hasUnsavedScenes = false;
                    for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                    {
                        var scene = EditorSceneManager.GetSceneAt(i);
                        if (scene.isDirty)
                        {
                            hasUnsavedScenes = true;
                            break;
                        }
                    }

                    if (hasUnsavedScenes)
                    {
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    }

                    ModularUIWizard.ReadaptScenesForPlatform(config.selectedPlatform);
                    Debug.Log($"[Modular UI] Scenes auto-adapted to platform: {config.selectedPlatform}");
                }
                finally
                {
                    isReadapting = false;
                }
            };
        }
    }
}
