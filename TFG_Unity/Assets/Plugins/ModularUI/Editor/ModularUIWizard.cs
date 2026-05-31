using UnityEngine;
using UnityEditor;
using System.IO;
using ModularUIRuntime;

namespace ModularUIEditor
{
    public class ModularUIWizard : EditorWindow
    {
        public enum ProjectStatus
        {
            NEW_PROJECT,
            EXISTING_PROJECT
        }

        public enum ImportScope
        {
            FULL_SYSTEM,
            SPECIFIC_MODULES
        }

        private UIConfiguration.TargetPlatform selectedPlatform = UIConfiguration.TargetPlatform.Desktop;
        private UIConfiguration.GameGenre selectedGenre = UIConfiguration.GameGenre.RPG;
        private ProjectStatus currentStatus = ProjectStatus.NEW_PROJECT;
        private ImportScope currentScope = ImportScope.FULL_SYSTEM;

        private bool importBaseUI = true;
        private bool importHUD = true;
        private bool importMainMenu = true;
        private bool importInventory = true;
        private bool importOptions = true;
        private bool importPauseMenu = true;
        private bool importCredits = true;
        private bool importWinLose = true;
        private bool importDialogues = true;
        private bool importSettings = true;
        private bool importMinimap = true;
        private bool importSamples = true;

        private string targetPath = "Assets/Plugins/ModularUI";
        private Vector2 scrollPosition;

        private GUIStyle headerStyle;
        private GUIStyle sectionHeaderStyle;
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle toggleStyle;

        private Texture2D headerTex;
        private Texture2D boxTex;
        private Texture2D buttonTex;
        private Texture2D buttonHoverTex;

        [MenuItem("Tools/Modular UI/Setup Wizard")]
        public static void ShowWindow()
        {
            ModularUIWizard window = GetWindow<ModularUIWizard>("UI Wizard");
            window.minSize = new Vector2(420, 680);
            window.Show();
        }

        private string GetRootPath()
        {
            if (Directory.Exists("Packages/com.pau.modularui"))
            {
                return "Packages/com.pau.modularui";
            }

            return "Assets/Plugins/ModularUI";
        }

        private void OnDestroy()
        {
            if (headerTex != null) DestroyImmediate(headerTex);
            if (boxTex != null) DestroyImmediate(boxTex);
            if (buttonTex != null) DestroyImmediate(buttonTex);
            if (buttonHoverTex != null) DestroyImmediate(buttonHoverTex);
        }

        private void InitializeStyles()
        {
            if (headerStyle != null) return;

            // Colors
            Color headerColor = new Color(0.12f, 0.16f, 0.22f); // Dark Slate Blue
            Color boxColor = new Color(0.18f, 0.18f, 0.18f); // Dark Charcoal
            Color accentColor = new Color(0f, 0.67f, 0.71f); // Vibrant Teal
            Color accentHoverColor = new Color(0f, 0.8f, 0.85f); // Lighter Teal for Hover

            headerTex = MakeTex(2, 2, headerColor);
            boxTex = MakeTex(2, 2, boxColor);
            buttonTex = MakeTex(2, 2, accentColor);
            buttonHoverTex = MakeTex(2, 2, accentHoverColor);

            headerStyle = new GUIStyle();
            headerStyle.normal.background = headerTex;
            headerStyle.padding = new RectOffset(10, 10, 15, 15);
            headerStyle.margin = new RectOffset(0, 0, 0, 0);

            sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            sectionHeaderStyle.normal.textColor = new Color(0f, 0.85f, 0.9f); // Cyan
            sectionHeaderStyle.fontSize = 13;
            sectionHeaderStyle.margin = new RectOffset(0, 0, 10, 5);

            boxStyle = new GUIStyle();
            boxStyle.normal.background = boxTex;
            boxStyle.padding = new RectOffset(15, 15, 15, 15);
            boxStyle.margin = new RectOffset(0, 0, 5, 15);

            buttonStyle = new GUIStyle();
            buttonStyle.normal.background = buttonTex;
            buttonStyle.hover.background = buttonHoverTex;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.fontSize = 13;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(10, 10, 12, 12);
            
            toggleStyle = new GUIStyle(EditorStyles.toggle);
            toggleStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            toggleStyle.fontSize = 11;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void DrawPlatformHelpBox()
        {
            string helpTitle = "";
            string helpText = "";

            switch (selectedPlatform)
            {
                case UIConfiguration.TargetPlatform.Desktop:
                    helpTitle = "Desktop UI Mode Activated";
                    helpText = "• Uses Keyboard & Mouse adapters.\n• Cursor auto-locks during gameplay.\n• Resolution standard 1920x1080 scales.";
                    break;
                case UIConfiguration.TargetPlatform.MobilePortrait:
                    helpTitle = "Mobile Portrait Mode Activated";
                    helpText = "• Aspect ratios auto-fitted vertically.\n• Safe Area padding enabled for notch displays.\n• Mobile controls overlay instantiated automatically.";
                    break;
                case UIConfiguration.TargetPlatform.MobileLandscape:
                    helpTitle = "Mobile Landscape Mode Activated";
                    helpText = "• Wide aspect fit.\n• Left/Right virtual joysticks layout.\n• Safe Area horizontal margins applied.";
                    break;
                case UIConfiguration.TargetPlatform.VR:
                    helpTitle = "Virtual Reality UI Mode Activated";
                    helpText = "• Canvas switched to World Space automatically.\n• Uses OVRRaycaster & laserpointer inputs.\n• Configures standard event system to support VR pointers.";
                    break;
            }

            EditorGUILayout.LabelField("ARCHITECTURE INFO", sectionHeaderStyle);
            GUILayout.BeginVertical(boxStyle);
            
            GUIStyle helpTitleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, normal = { textColor = new Color(0.9f, 0.9f, 0.9f) } };
            GUIStyle helpTextStyle = new GUIStyle(EditorStyles.miniLabel) { fontSize = 10, wordWrap = true, normal = { textColor = new Color(0.7f, 0.7f, 0.7f) } };

            EditorGUILayout.LabelField(helpTitle, helpTitleStyle);
            EditorGUILayout.LabelField(helpText, helpTextStyle);
            
            GUILayout.EndVertical();
        }

        private void OnGUI()
        {
            InitializeStyles();

            // Header Banner
            GUILayout.BeginHorizontal(headerStyle);
            GUILayout.Label("MODULAR UI SYSTEM", new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
            GUILayout.EndHorizontal();

            // Main scrollable area
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(15, 15, 15, 15) });

            // Section 1: Integration
            EditorGUILayout.LabelField("1. INTEGRATION SETTINGS", sectionHeaderStyle);
            GUILayout.BeginVertical(boxStyle);
            
            currentStatus = (ProjectStatus)EditorGUILayout.EnumPopup("Project Status:", currentStatus);
            currentScope = (ImportScope)EditorGUILayout.EnumPopup("Import Scope:", currentScope);

            if (currentScope == ImportScope.SPECIFIC_MODULES)
            {
                EditorGUILayout.Space(10);
                
                // Draw a nice separator line
                Rect lineRect = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(lineRect, new Color(0.3f, 0.3f, 0.3f));
                EditorGUILayout.Space(10);

                // Grid layout (2 columns)
                EditorGUILayout.BeginHorizontal();
                importBaseUI = EditorGUILayout.ToggleLeft(" Base UI (Required)", importBaseUI, toggleStyle, GUILayout.Width(180));
                importMainMenu = EditorGUILayout.ToggleLeft(" Main Menu Template", importMainMenu, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                importHUD = EditorGUILayout.ToggleLeft(" HUD Template", importHUD, toggleStyle, GUILayout.Width(180));
                importInventory = EditorGUILayout.ToggleLeft(" Inventory System", importInventory, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                importOptions = EditorGUILayout.ToggleLeft(" Options Menu", importOptions, toggleStyle, GUILayout.Width(180));
                importPauseMenu = EditorGUILayout.ToggleLeft(" Pause Menu", importPauseMenu, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                importCredits = EditorGUILayout.ToggleLeft(" Credits Screen", importCredits, toggleStyle, GUILayout.Width(180));
                importWinLose = EditorGUILayout.ToggleLeft(" Win/Lose Screens", importWinLose, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                importDialogues = EditorGUILayout.ToggleLeft(" Dialogue System", importDialogues, toggleStyle, GUILayout.Width(180));
                importSettings = EditorGUILayout.ToggleLeft(" Themes (Resources)", importSettings, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                importMinimap = EditorGUILayout.ToggleLeft(" Minimap System", importMinimap, toggleStyle, GUILayout.Width(180));
                importSamples = EditorGUILayout.ToggleLeft(" Demo Samples", importSamples, toggleStyle, GUILayout.Width(180));
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            // Section 2: Architecture
            EditorGUILayout.LabelField("2. ARCHITECTURE CONFIGURATION", sectionHeaderStyle);
            GUILayout.BeginVertical(boxStyle);
            
            selectedPlatform = (UIConfiguration.TargetPlatform)EditorGUILayout.EnumPopup("Target Platform:", selectedPlatform);
            selectedGenre = (UIConfiguration.GameGenre)EditorGUILayout.EnumPopup("Game Genre:", selectedGenre);
            
            GUILayout.EndVertical();

            // Help Panel depending on platform
            DrawPlatformHelpBox();

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            // Bottom CTA Button Area
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(15, 15, 15, 15) });
            if (GUILayout.Button("IMPORT & CONFIGURE SYSTEM", buttonStyle, GUILayout.Height(45)))
            {
                ExecuteImport();
                CreateAndApplyConfiguration();
            }
            GUILayout.EndVertical();
        }

        private void CreateAndApplyConfiguration()
        {
            string configFolder = targetPath + "/Settings";

            if (!AssetDatabase.IsValidFolder(configFolder))
            {
                AssetDatabase.CreateFolder(targetPath, "Settings");
            }

            string configPath = configFolder + "/UIConfiguration.asset";
            UIConfiguration config = AssetDatabase.LoadAssetAtPath<UIConfiguration>(configPath);

            if (config == null)
            {
                config = CreateInstance<UIConfiguration>();
                AssetDatabase.CreateAsset(config, configPath);
            }

            config.selectedPlatform = selectedPlatform;
            config.selectedGenre = selectedGenre;

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait || selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
            {
                string mobileControlsPath = targetPath + "/Templates/Mobile/MobileControls.prefab";
                GameObject mobileControlsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mobileControlsPath);
                if (mobileControlsPrefab != null)
                {
                    config.mobileControlsPrefab = mobileControlsPrefab;
                }
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            string canvasPath = targetPath + "/BaseUI/ModularCanvas.prefab";
            GameObject canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(canvasPath);

            if (canvasPrefab != null)
            {
                using (var editingScope = new PrefabUtility.EditPrefabContentsScope(canvasPath))
                {
                    GameObject root = editingScope.prefabContentsRoot;

                    var initializer = root.GetComponent<ModularCanvasInitializer>();

                    if (initializer != null)
                    {
                        initializer.config = config;
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[ModularUI] Configuration applied: {selectedPlatform} for {selectedGenre}");
        }

        private void ExecuteImport()
        {
            if (!AssetDatabase.IsValidFolder(targetPath))
            {
                EnsureFolderExists(targetPath + "/dummy.txt");
            }

            if (currentScope == ImportScope.FULL_SYSTEM)
            {
                CopyAssetItem("BaseUI", "BaseUI");
                CopyAssetItem("Templates", "Templates");
                CopyAssetItem("Dialogues", "Dialogues");
                CopyAssetItem("Resources", "Resources");
                CopyAssetItem("Minimap", "Minimap");
                CopyAssetItem("Samples", "Samples");
            }
            else
            {
                if (importBaseUI)
                {
                    CopyAssetItem("BaseUI", "BaseUI");
                }

                if (importMainMenu)
                {
                    ImportTemplateVariant("MainMenu");
                }

                if (importHUD)
                {
                    ImportTemplateVariant("HUD");
                }

                if (importInventory)
                {
                    ImportTemplateVariant("InventoryPanel");
                }

                if (importOptions)
                {
                    ImportTemplateVariant("Options");
                }

                if (importPauseMenu)
                {
                    ImportTemplateVariant("PauseMenu");
                }

                if (importCredits)
                {
                    ImportTemplateVariant("Credits");
                }

                if (importWinLose)
                {
                    ImportTemplateVariant("WinLoseMenu");
                }

                if (importDialogues)
                {
                    CopyAssetItem("Dialogues", "Dialogues");
                    ImportTemplateVariant("DialoguePanel");
                }

                if (importSettings)
                {
                    CopyAssetItem("Resources", "Resources");
                }

                if (importMinimap)
                {
                    CopyAssetItem("Minimap", "Minimap");
                }

                if (importSamples)
                {
                    CopyAssetItem("Samples", "Samples");
                }

                if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait || selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
                {
                    CopyAssetItem("Templates/Mobile/MobileControls.prefab", "Templates/Mobile/MobileControls.prefab");
                }
            }

            AssetDatabase.Refresh();
        }

        private void ImportTemplateVariant(string templateName)
        {
            CopyAssetItem($"Templates/Base/{templateName}_Base.prefab", $"Templates/Base/{templateName}_Base.prefab");

            if (selectedPlatform == UIConfiguration.TargetPlatform.Desktop)
            {
                CopyAssetItem($"Templates/Desktop/{templateName}_Desktop.prefab", $"Templates/Desktop/{templateName}_Desktop.prefab");
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait)
            {
                CopyAssetItem($"Templates/Mobile/Portrait/{templateName}_Portrait.prefab", $"Templates/Mobile/Portrait/{templateName}_Portrait.prefab");
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
            {
                CopyAssetItem($"Templates/Mobile/Landscape/{templateName}_Landscape.prefab", $"Templates/Mobile/Landscape/{templateName}_Landscape.prefab");
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
            {
                CopyAssetItem($"Templates/VR/{templateName}_VR.prefab", $"Templates/VR/{templateName}_VR.prefab");
            }
        }

        private void CopyAssetItem(string subPath, string targetSubPath)
        {
            string source = GetRootPath() + "/" + subPath;
            string destination = targetPath + "/" + targetSubPath;

            EnsureFolderExists(destination);

            if (AssetDatabase.LoadAssetAtPath<Object>(destination) == null)
            {
                AssetDatabase.CopyAsset(source, destination);
            }
        }

        private void EnsureFolderExists(string path)
        {
            int lastSlash = path.LastIndexOf('/');

            if (lastSlash > 0)
            {
                string folderPath = path.Substring(0, lastSlash);

                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    string[] folders = folderPath.Split('/');
                    string current = folders[0];

                    for (int i = 1; i < folders.Length; i++)
                    {
                        if (!AssetDatabase.IsValidFolder(current + "/" + folders[i]))
                        {
                            AssetDatabase.CreateFolder(current, folders[i]);
                        }

                        current += "/" + folders[i];
                    }
                }
            }
        }
    }
}