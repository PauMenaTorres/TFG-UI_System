using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ModularUIRuntime;

namespace ModularUI.Editor
{
    public static class ModularUIMenuItems
    {
        private const string ASSETS_ROOT = "Assets/Plugins/ModularUI";
        private const string PACKAGE_ROOT = "Packages/com.pau.modularui";

        private static string RootPath =>
            AssetDatabase.IsValidFolder(ASSETS_ROOT) ? ASSETS_ROOT : PACKAGE_ROOT;

        private static string BASE_PATH => RootPath + "/";
        private static string BASE_UI_PATH => RootPath + "/BaseUI/";
        private static string TEMPLATES_PATH => RootPath + "/Templates/";
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

        [MenuItem("GameObject/Modular UI/Base UI/Back Button", false, MENU_PRIORITY + 7)]
        public static void CreateBackButton() => InstantiatePrefab(BASE_UI_PATH + "BackButton.prefab", "Modular Back Button");

        [MenuItem("GameObject/Modular UI/Base UI/Dialogue Choice", false, MENU_PRIORITY + 8)]
        public static void CreateDialogueChoice() => InstantiatePrefab(BASE_UI_PATH + "DialogueChoice.prefab", "Modular Dialogue Choice");

        [MenuItem("GameObject/Modular UI/Base UI/Inventory Item", false, MENU_PRIORITY + 9)]
        public static void CreateInventoryItem() => InstantiatePrefab(BASE_UI_PATH + "InventoryItem.prefab", "Modular Inventory Item");

        [MenuItem("GameObject/Modular UI/Base UI/Inventory Slot", false, MENU_PRIORITY + 10)]
        public static void CreateInventorySlot() => InstantiatePrefab(BASE_UI_PATH + "InventorySlot.prefab", "Modular Inventory Slot");

        [MenuItem("GameObject/Modular UI/Base UI/Item Slot", false, MENU_PRIORITY + 11)]
        public static void CreateItemSlot() => InstantiatePrefab(BASE_UI_PATH + "ItemSlot.prefab", "Modular Item Slot");

        [MenuItem("GameObject/Modular UI/Base UI/Selection Frame", false, MENU_PRIORITY + 12)]
        public static void CreateSelectionFrame() => InstantiatePrefab(BASE_UI_PATH + "SelectionFrame.prefab", "Modular Selection Frame");

        [MenuItem("GameObject/Modular UI/Base UI/Modular Canvas", false, MENU_PRIORITY + 20)]
        public static void CreateModularCanvas() => InstantiatePrefab(BASE_UI_PATH + "ModularCanvas.prefab", "Modular Canvas");

        #endregion

        #region Minimap

        [MenuItem("GameObject/Modular UI/Minimap/Minimap Camera", false, MENU_PRIORITY + 30)]
        public static void CreateMinimapCamera() => InstantiatePrefab(BASE_PATH + "Minimap/MinimapCamera.prefab", "Minimap Camera");

        #endregion

        #region Templates Base

        [MenuItem("GameObject/Modular UI/Templates/Base/Main Menu", false, MENU_PRIORITY + 40)]
        public static void CreateBaseMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Base/MainMenu_Base.prefab", "MainMenu_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Pause Menu", false, MENU_PRIORITY + 41)]
        public static void CreateBasePauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "Base/PauseMenu_Base.prefab", "PauseMenu_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/HUD", false, MENU_PRIORITY + 42)]
        public static void CreateBaseHUD() => InstantiatePrefab(TEMPLATES_PATH + "Base/HUD_Base.prefab", "HUD_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Dialogue Panel", false, MENU_PRIORITY + 43)]
        public static void CreateBaseDialogue() => InstantiatePrefab(TEMPLATES_PATH + "Base/DialoguePanel_Base.prefab", "DialoguePanel_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Inventory Panel", false, MENU_PRIORITY + 44)]
        public static void CreateBaseInventory() => InstantiatePrefab(TEMPLATES_PATH + "Base/InventoryPanel_Base.prefab", "InventoryPanel_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Options", false, MENU_PRIORITY + 45)]
        public static void CreateBaseOptions() => InstantiatePrefab(TEMPLATES_PATH + "Base/Options_Base.prefab", "Options_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Credits", false, MENU_PRIORITY + 46)]
        public static void CreateBaseCredits() => InstantiatePrefab(TEMPLATES_PATH + "Base/Credits_Base.prefab", "Credits_Base");

        [MenuItem("GameObject/Modular UI/Templates/Base/Win Lose Menu", false, MENU_PRIORITY + 47)]
        public static void CreateBaseWinLose() => InstantiatePrefab(TEMPLATES_PATH + "Base/WinLoseMenu_Base.prefab", "WinLoseMenu_Base");

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

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Inventory Panel", false, MENU_PRIORITY + 64)]
        public static void CreateDesktopInventory() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/InventoryPanel_Desktop.prefab", "InventoryPanel_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Options", false, MENU_PRIORITY + 65)]
        public static void CreateDesktopOptions() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/Options_Desktop.prefab", "Options_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Credits", false, MENU_PRIORITY + 66)]
        public static void CreateDesktopCredits() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/Credits_Desktop.prefab", "Credits_Desktop");

        [MenuItem("GameObject/Modular UI/Templates/Desktop/Win Lose Menu", false, MENU_PRIORITY + 67)]
        public static void CreateDesktopWinLose() => InstantiatePrefab(TEMPLATES_PATH + "Desktop/WinLoseMenu_Desktop.prefab", "WinLoseMenu_Desktop");

        #endregion

        #region Templates Mobile

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Mobile Controls", false, MENU_PRIORITY + 80)]
        public static void CreateMobileControls() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/MobileControls.prefab", "MobileControls");

        #region Mobile Landscape

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Main Menu", false, MENU_PRIORITY + 81)]
        public static void CreateMobileLandscapeMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/MainMenu_Landscape.prefab", "MainMenu_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Pause Menu", false, MENU_PRIORITY + 82)]
        public static void CreateMobileLandscapePauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/PauseMenu_Landscape.prefab", "PauseMenu_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/HUD", false, MENU_PRIORITY + 83)]
        public static void CreateMobileLandscapeHUD() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/HUD_Landscape.prefab", "HUD_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Dialogue Panel", false, MENU_PRIORITY + 84)]
        public static void CreateMobileLandscapeDialogue() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/DialoguePanel_Landscape.prefab", "DialoguePanel_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Inventory Panel", false, MENU_PRIORITY + 85)]
        public static void CreateMobileLandscapeInventory() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/InventoryPanel_Landscape.prefab", "InventoryPanel_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Options", false, MENU_PRIORITY + 86)]
        public static void CreateMobileLandscapeOptions() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/Options_Landscape.prefab", "Options_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Credits", false, MENU_PRIORITY + 87)]
        public static void CreateMobileLandscapeCredits() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/Credits_Landscape.prefab", "Credits_Landscape");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Landscape/Win Lose Menu", false, MENU_PRIORITY + 88)]
        public static void CreateMobileLandscapeWinLose() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Landscape/WinLoseMenu_Landscape.prefab", "WinLoseMenu_Landscape");

        #endregion

        #region Mobile Portrait

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Main Menu", false, MENU_PRIORITY + 100)]
        public static void CreateMobilePortraitMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/MainMenu_Portrait.prefab", "MainMenu_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Pause Menu", false, MENU_PRIORITY + 101)]
        public static void CreateMobilePortraitPauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/PauseMenu_Portrait.prefab", "PauseMenu_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/HUD", false, MENU_PRIORITY + 102)]
        public static void CreateMobilePortraitHUD() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/HUD_Portrait.prefab", "HUD_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Dialogue Panel", false, MENU_PRIORITY + 103)]
        public static void CreateMobilePortraitDialogue() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/DialoguePanel_Portrait.prefab", "DialoguePanel_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Inventory Panel", false, MENU_PRIORITY + 104)]
        public static void CreateMobilePortraitInventory() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/InventoryPanel_Portrait.prefab", "InventoryPanel_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Options", false, MENU_PRIORITY + 105)]
        public static void CreateMobilePortraitOptions() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/Options_Portrait.prefab", "Options_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Credits", false, MENU_PRIORITY + 106)]
        public static void CreateMobilePortraitCredits() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/Credits_Portrait.prefab", "Credits_Portrait");

        [MenuItem("GameObject/Modular UI/Templates/Mobile/Portrait/Win Lose Menu", false, MENU_PRIORITY + 107)]
        public static void CreateMobilePortraitWinLose() => InstantiatePrefab(TEMPLATES_PATH + "Mobile/Portrait/WinLoseMenu_Portrait.prefab", "WinLoseMenu_Portrait");

        #endregion

        #endregion

        #region Templates VR

        [MenuItem("GameObject/Modular UI/Templates/VR/Main Menu", false, MENU_PRIORITY + 120)]
        public static void CreateVRMainMenu() => InstantiatePrefab(TEMPLATES_PATH + "VR/MainMenu_VR.prefab", "MainMenu_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Pause Menu", false, MENU_PRIORITY + 121)]
        public static void CreateVRPauseMenu() => InstantiatePrefab(TEMPLATES_PATH + "VR/PauseMenu_VR.prefab", "PauseMenu_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/HUD", false, MENU_PRIORITY + 122)]
        public static void CreateVRHUD() => InstantiatePrefab(TEMPLATES_PATH + "VR/HUD_VR.prefab", "HUD_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Dialogue Panel", false, MENU_PRIORITY + 123)]
        public static void CreateVRDialogue() => InstantiatePrefab(TEMPLATES_PATH + "VR/DialoguePanel_VR.prefab", "DialoguePanel_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Inventory Panel", false, MENU_PRIORITY + 124)]
        public static void CreateVRInventory() => InstantiatePrefab(TEMPLATES_PATH + "VR/InventoryPanel_VR.prefab", "InventoryPanel_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Options", false, MENU_PRIORITY + 125)]
        public static void CreateVROptions() => InstantiatePrefab(TEMPLATES_PATH + "VR/Options_VR.prefab", "Options_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Credits", false, MENU_PRIORITY + 126)]
        public static void CreateVRCredits() => InstantiatePrefab(TEMPLATES_PATH + "VR/Credits_VR.prefab", "Credits_VR");

        [MenuItem("GameObject/Modular UI/Templates/VR/Win Lose Menu", false, MENU_PRIORITY + 127)]
        public static void CreateVRWinLose() => InstantiatePrefab(TEMPLATES_PATH + "VR/WinLoseMenu_VR.prefab", "WinLoseMenu_VR");

        #endregion

        private static GameObject LoadPrefabWithFallback(string assetPath, params string[] fallbackAssetPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null) return prefab;

            if (fallbackAssetPaths != null)
            {
                foreach (var fallback in fallbackAssetPaths)
                {
                    if (string.IsNullOrWhiteSpace(fallback)) continue;
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fallback);
                    if (prefab != null) return prefab;
                }
            }

            return null;
        }

        private static void InstantiatePrefab(string path, string defaultName)
        {
            GameObject prefab = LoadPrefabWithFallback(
                path,
                // VR templates can live under VR~ when copied from a package
                path.Replace("/Templates/VR/", "/Templates/VR~/")
            );
            if (prefab == null)
            {
                
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
                var modularInitializer = Object.FindFirstObjectByType<ModularCanvasInitializer>();
                if (modularInitializer != null)
                {
                    canvas = modularInitializer.GetComponent<Canvas>();
                }

                if (canvas == null)
                {
                    canvas = CreateNewCanvas();
                }

                if (canvas != null)
                {
                    GameObjectUtility.SetParentAndAlign(instance, canvas.gameObject);
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }

        private static Canvas CreateNewCanvas()
        {
            // Create the system's ModularCanvas prefab so it configures itself via ModularCanvasInitializer.
            var modularCanvasPrefab = LoadPrefabWithFallback(BASE_UI_PATH + "ModularCanvas.prefab");
            if (modularCanvasPrefab != null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(modularCanvasPrefab);
                if (instance != null)
                {
                    instance.name = "ModularCanvas";
                    Undo.RegisterCreatedObjectUndo(instance, "Create ModularCanvas");

                    var canvas = instance.GetComponent<Canvas>();
                    if (canvas != null) return canvas;
                }
            }

            // Hard fallback: plain Unity Canvas.
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.layer = LayerMask.NameToLayer("UI");
            Canvas fallbackCanvas = canvasObj.AddComponent<Canvas>();
            fallbackCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
            }

            return fallbackCanvas;
        }
    }
}
