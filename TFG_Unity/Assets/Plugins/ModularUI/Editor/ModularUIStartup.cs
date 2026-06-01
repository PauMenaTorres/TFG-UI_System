using UnityEditor;
using UnityEngine;
using System.IO;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIStartup
    {
        public static string GetPrefsKey()
        {
            string projectPath = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
            return "ModularUI_WizardShown_" + projectPath;
        }

        static ModularUIStartup()
        {
            EditorApplication.delayCall += TryShowWizard;
        }

        private static void TryShowWizard()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryShowWizard;
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:UIConfiguration");
            bool configured = guids != null && guids.Length > 0;

            string key = GetPrefsKey();
            if (!EditorPrefs.GetBool(key, false) || !configured)
            {
                ModularUIWizard.ShowWindow();
            }
        }

        [MenuItem("Tools/Modular UI/Debug/Reset First Load Popup")]
        public static void ResetFirstLoad()
        {
            EditorPrefs.DeleteKey(GetPrefsKey());
        }
    }
}