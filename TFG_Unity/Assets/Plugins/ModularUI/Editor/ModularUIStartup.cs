using UnityEditor;
using UnityEngine;

namespace ModularUIEditor
{
    [InitializeOnLoad]
    public class ModularUIStartup
    {
        private static string GetPrefsKey()
        {
            return "ModularUI_" + Application.dataPath.GetHashCode();
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

            if (!EditorPrefs.GetBool(GetPrefsKey(), false))
            {
                EditorApplication.delayCall += () =>
                {
                    ModularUIWizard.ShowWindow();
                    EditorPrefs.SetBool(GetPrefsKey(), true);
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