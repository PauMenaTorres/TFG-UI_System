using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ModularUI.Editor
{
    public static class ModularUIMenuItems
    {
        private const string BASE_PATH = "Assets/Plugins/ModularUI/";
        private const string BASE_UI_PATH = BASE_PATH + "BaseUI/";
        private const string TEMPLATES_PATH = BASE_PATH + "Templates/";
        private const int MENU_PRIORITY = 10;

        #region Base UI
        
        [MenuItem("GameObject/Modular UI/Base UI/Button", false, MENU_PRIORITY)]
        public static void CreateButton() => InstantiatePrefab(BASE_UI_PATH + "Button.prefab", "Modular Button");

        [MenuItem("GameObject/Modular UI/Base UI/Text", false, MENU_PRIORITY + 1)]
        public static void CreateText() => InstantiatePrefab(BASE_UI_PATH + "Text.prefab", "Modular Text");

        [MenuItem("GameObject/Modular UI/Base UI/Image", false, MENU_PRIORITY + 2)]
        public static void CreateImage() => InstantiatePrefab(BASE_UI_PATH + "Image.prefab", "Modular Image");

        [MenuItem("GameObject/Modular UI/Base UI/Slider", false, MENU_PRIORITY + 3)]
        public static void CreateSlider() => InstantiatePrefab(BASE_UI_PATH + "Slider.prefab", "Modular Slider");

        [MenuItem("GameObject/Modular UI/Base UI/Toggle", false, MENU_PRIORITY + 4)]
        public static void CreateToggle() => InstantiatePrefab(BASE_UI_PATH + "Toggle.prefab", "Modular Toggle");

        [MenuItem("GameObject/Modular UI/Base UI/Dropdown", false, MENU_PRIORITY + 5)]
        public static void CreateDropdown() => InstantiatePrefab(BASE_UI_PATH + "Dropdown.prefab", "Modular Dropdown");

        [MenuItem("GameObject/Modular UI/Base UI/Background", false, MENU_PRIORITY + 6)]
        public static void CreateBackground() => InstantiatePrefab(BASE_UI_PATH + "Background.prefab", "Modular Background");

        [MenuItem("GameObject/Modular UI/Base UI/Modular Canvas", false, MENU_PRIORITY + 20)]
        public static void CreateModularCanvas() => InstantiatePrefab(BASE_UI_PATH + "ModularCanvas.prefab", "Modular Canvas");

        #endregion

        #region Templates VR

        [MenuItem("GameObject/Modular UI/Templates/VR/Main Menu", false, MENU_PRIORITY + 40)]
        public static void CreateVRMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "VR/MainMenu_VR.prefab", "MainMenu_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Pause Menu", false, MENU_PRIORITY + 41)]
        public static void CreateVRPauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "VR/PauseMenu_VR.prefab", "PauseMenu_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/HUD", false, MENU_PRIORITY + 42)]
        public static void CreateVRHUD() => InstantiatePrefab(TEMPLATES_PATH + "VR/HUD_VR.prefab", "HUD_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Dialogue Panel", false, MENU_PRIORITY + 43)]
        public static void CreateVRDialogue() => InstantiatePrefab(TEMPLATES_PATH + "VR/DialoguePanel_VR.prefab", "DialoguePanel_VR");

        #endregion

        #region Templates Desktop

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Main Menu", false, MENU_PRIORITY + 60)]
        public static void CreateDesktopMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/MainMenu_Desktop.prefab", "MainMenu_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Pause Menu", false, MENU_PRIORITY + 61)]
        public static void CreateDesktopPauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/PauseMenu_Desktop.prefab", "PauseMenu_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/HUD", false, MENU_PRIORITY + 62)]
        public static void CreateDesktopHUD() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/HUD_Desktop.prefab", "HUD_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Dialogue Panel", false, MENU_PRIORITY + 63)]
        public static void CreateDesktopDialogue() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/DialoguePanel_Desktop.prefab", "DialoguePanel_Desktop");

        #endregion

        #region Templates Mobile

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Main Menu", false, MENU_PRIORITY + 80)]
        public static void CreateMobileLandscapeMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/MainMenu_Landscape.prefab", "MainMenu_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/HUD", false, MENU_PRIORITY + 81)]
        public static void CreateMobileLandscapeHUD() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/HUD_Landscape.prefab", "HUD_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Main Menu", false, MENU_PRIORITY + 100)]
        public static void CreateMobilePortraitMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/MainMenu_Portrait.prefab", "MainMenu_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/HUD", false, MENU_PRIORITY + 101)]
        public static void CreateMobilePortraitHUD() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/HUD_Portrait.prefab", "HUD_Portrait");

        #endregion

        private static void InstantiatePrefab(string path, string defaultName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[ModularUI] Could not find prefab at path: {path}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance == null) return;

            instance.name = defaultName;

            GameObject selected = Selection.activeGameObject;
            Canvas canvas = null;

            if (selected != null)
            {
                canvas = selected.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    GameObjectUtility.SetParentAndAlign(instance, selected);
                }
            }

            if (canvas == null)
            {
                canvas = Object.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    canvas = CreateNewCanvas();
                }
                GameObjectUtility.SetParentAndAlign(instance, canvas.gameObject);
            }

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }

        private static Canvas CreateNewCanvas()
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
            }

            return canvas;
        }
    }
}
