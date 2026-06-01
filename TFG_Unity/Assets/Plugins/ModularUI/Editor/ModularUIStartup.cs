using UnityEditor;
using UnityEngine;
using System.IO;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIStartup
    {
        private static int delayFrames = 0;

        public static string GetPrefsKey()
        {
            string projectPath = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
            return "ModularUI_WizardShown_" + projectPath;
        }

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

            delayFrames++;
            if (delayFrames < 30)
            {
                return;
            }

            EditorApplication.update -= RunOnce;

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