using UnityEditor;
using UnityEngine;
using System.IO;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIStartup
    {
        private static string GetPrefsKey()
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

            EditorApplication.update -= RunOnce;

            string key = GetPrefsKey();
            if (!EditorPrefs.GetBool(key, false))
            {
                
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        ModularUIWizard.ShowWindow();
                        EditorPrefs.SetBool(key, true);
                    };
                };
            }
        }

        [MenuItem("Tools/Modular UI/Debug/Reset First Load Popup")]
        public static void ResetFirstLoad()
        {
            EditorPrefs.DeleteKey(GetPrefsKey());
            
        }
    }
}