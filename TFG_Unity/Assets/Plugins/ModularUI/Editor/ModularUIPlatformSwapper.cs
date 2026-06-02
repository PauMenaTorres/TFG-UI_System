using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace ModularUIRuntime
{
    public static class ModularUIPlatformSwapper
    {
        public static void SwapPrefabs(UIConfiguration.TargetPlatform newPlatform)
        {
            ModularCanvasInitializer initializer = Object.FindFirstObjectByType<ModularCanvasInitializer>();

            if (initializer == null)
            {
                Debug.LogWarning("ModularCanvasInitializer not found in scene");
                return;
            }

            List<GameObject> currentScreens = new List<GameObject>();
            Dictionary<string, bool> screenStates = new Dictionary<string, bool>();

            foreach (Transform child in initializer.transform)
            {
                ModularScreenBase screenBase = child.GetComponent<ModularScreenBase>();

                if (screenBase != null)
                {
                    string childName = child.gameObject.name.Replace("(Clone)", "").Trim();
                    string baseName = GetBaseName(childName);

                    screenStates[baseName] = child.gameObject.activeSelf;
                    currentScreens.Add(child.gameObject);
                }
            }

            if (currentScreens.Count == 0)
            {
                Debug.LogWarning("No ModularScreenBase components found in children");
                return;
            }

            string newPath = GetPlatformPath(newPlatform);
            string newSuffix = GetPlatformSuffix(newPlatform);

            foreach (GameObject screen in currentScreens)
            {
                string baseName = GetBaseName(screen.name.Replace("(Clone)", "").Trim());
                bool wasActive = screenStates[baseName];

                Undo.DestroyObjectImmediate(screen);

                string prefabPath = $"{newPath}/{baseName}_{newSuffix}.prefab";
                GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (newPrefab != null)
                {
                    GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab, initializer.transform);
                    inst.name = newPrefab.name;
                    inst.SetActive(wasActive);
                    Undo.RegisterCreatedObjectUndo(inst, "Swap UI Prefab");
                }
                else
                {
                    Debug.LogWarning($"Prefab not found at: {prefabPath}");
                }
            }

            EditorUtility.SetDirty(initializer.gameObject);
        }

        private static string GetBaseName(string fullName)
        {
            if (fullName.Contains("_Desktop"))
            {
                return fullName.Replace("_Desktop", "");
            }

            if (fullName.Contains("_Landscape"))
            {
                return fullName.Replace("_Landscape", "");
            }

            if (fullName.Contains("_Portrait"))
            {
                return fullName.Replace("_Portrait", "");
            }

            if (fullName.Contains("_VR"))
            {
                return fullName.Replace("_VR", "");
            }

            if (fullName.Contains("_Base"))
            {
                return fullName.Replace("_Base", "");
            }

            return fullName;
        }

        private static string GetPlatformPath(UIConfiguration.TargetPlatform platform)
        {
            switch (platform)
            {
                case UIConfiguration.TargetPlatform.Desktop:
                    return "Assets/Plugins/ModularUI/Templates/Desktop";

                case UIConfiguration.TargetPlatform.MobileLandscape:
                    return "Assets/Plugins/ModularUI/Templates/Mobile/Landscape";

                case UIConfiguration.TargetPlatform.MobilePortrait:
                    return "Assets/Plugins/ModularUI/Templates/Mobile/Portrait";

                case UIConfiguration.TargetPlatform.VR:
                    return "Assets/Plugins/ModularUI/Templates/VR";

                default:
                    return "Assets/Plugins/ModularUI/Templates/Base";
            }
        }

        private static string GetPlatformSuffix(UIConfiguration.TargetPlatform platform)
        {
            switch (platform)
            {
                case UIConfiguration.TargetPlatform.Desktop:
                    return "Desktop";

                case UIConfiguration.TargetPlatform.MobileLandscape:
                    return "Landscape";

                case UIConfiguration.TargetPlatform.MobilePortrait:
                    return "Portrait";

                case UIConfiguration.TargetPlatform.VR:
                    return "VR";

                default:
                    return "Base";
            }
        }
    }
}
#endif
