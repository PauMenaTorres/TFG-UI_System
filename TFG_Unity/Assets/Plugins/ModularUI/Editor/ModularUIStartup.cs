using UnityEditor;
using UnityEngine;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIStartup
    {
        private const string PREFS_KEY = "ModularUI_Wizard_Shown_v1";

        static ModularUIStartup()
        {
            EditorApplication.update += RunOnce;
        }

        private static void RunOnce()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            EditorApplication.update -= RunOnce;

            if (!EditorPrefs.GetBool(PREFS_KEY, false))
            {
                // Un pequeño delay extra para asegurar que la UI de Unity está lista
                EditorApplication.delayCall += () =>
                {
                    ModularUIWizard.ShowWindow();
                    EditorPrefs.SetBool(PREFS_KEY, true);
                };
            }
        }

        [MenuItem("Tools/Modular UI/Debug/Reset First Load Popup")]
        public static void ResetFirstLoad()
        {
            EditorPrefs.DeleteKey(PREFS_KEY);
            Debug.Log("[Modular UI] Startup reset. The Wizard will show on next reload.");
        }
    }
}