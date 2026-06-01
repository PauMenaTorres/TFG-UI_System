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

        [SerializeField] private UIConfiguration.TargetPlatform selectedPlatform = UIConfiguration.TargetPlatform.Desktop;
        [SerializeField] private UIConfiguration.GameGenre selectedGenre = UIConfiguration.GameGenre.RPG;
        [SerializeField] private ProjectStatus currentStatus = ProjectStatus.NEW_PROJECT;
        [SerializeField] private ImportScope currentScope = ImportScope.FULL_SYSTEM;

        [SerializeField] private bool importBaseUI = true;
        [SerializeField] private bool importHUD = true;
        [SerializeField] private bool importMainMenu = true;
        [SerializeField] private bool importInventory = true;
        [SerializeField] private bool importOptions = true;
        [SerializeField] private bool importPauseMenu = true;
        [SerializeField] private bool importCredits = true;
        [SerializeField] private bool importWinLose = true;
        [SerializeField] private bool importDialogues = true;
        [SerializeField] private bool importSettings = true;
        [SerializeField] private bool importMinimap = true;
        [SerializeField] private bool importSamples = true;

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
                AdaptSampleScenesToPlatform();
                Close();
            }
            GUILayout.EndVertical();
        }

        private void CreateAndApplyConfiguration()
        {
            // Delete old configuration file if it exists
            string oldConfigPath = targetPath + "/Settings/UIConfiguration.asset";
            if (AssetDatabase.LoadAssetAtPath<UIConfiguration>(oldConfigPath) != null)
            {
                AssetDatabase.DeleteAsset(oldConfigPath);
            }
            string oldConfigFolder = targetPath + "/Settings";
            if (AssetDatabase.IsValidFolder(oldConfigFolder))
            {
                string[] files = Directory.GetFiles(oldConfigFolder);
                if (files.Length == 0)
                {
                    AssetDatabase.DeleteAsset(oldConfigFolder);
                }
            }

            string configFolder = targetPath + "/Resources";

            if (!AssetDatabase.IsValidFolder(configFolder))
            {
                AssetDatabase.CreateFolder(targetPath, "Resources");
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

            // Clear and bind genre themes by code
            config.genreThemes.Clear();
            foreach (UIConfiguration.GameGenre genre in System.Enum.GetValues(typeof(UIConfiguration.GameGenre)))
            {
                string themePath = $"{targetPath}/Resources/Theme_{genre}.asset";
                ModularThemeData themeAsset = AssetDatabase.LoadAssetAtPath<ModularThemeData>(themePath);
                if (themeAsset != null)
                {
                    config.genreThemes.Add(new UIConfiguration.GenreThemeMap
                    {
                        genre = genre,
                        theme = themeAsset
                    });
                }
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait || selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
            {
                string mobileControlsPath = targetPath + "/Templates/Mobile/MobileControls.prefab";
                GameObject mobileControlsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mobileControlsPath);
                if (mobileControlsPrefab != null)
                {
                    config.mobileControlsPrefab = mobileControlsPrefab;
                }
            }
            else if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
            {
                string vrCameraRigPath = targetPath + "/Templates/VR/Modular_VR_CameraRig.prefab";
                string vrEventSystemPath = targetPath + "/Templates/VR/Modular_VR_EventSystem.prefab";
                GameObject vrCameraRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(vrCameraRigPath);
                GameObject vrEventSystemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(vrEventSystemPath);
                config.vrSettings = new UIConfiguration.VRPlatformSettings
                {
                    vrCameraRigPrefab = vrCameraRigPrefab,
                    vrEventSystemPrefab = vrEventSystemPrefab
                };
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

            AssetDatabase.StartAssetEditing();
            try
            {
                if (currentScope == ImportScope.FULL_SYSTEM)
                {
                    CopyAssetItem("BaseUI", "BaseUI");
                    CopyAssetItem("Templates", "Templates");
                    CopyAssetItem("Dialogues", "Dialogues");
                    CopyAssetItem("Resources", "Resources");
                    CopyAssetItem("Minimap", "Minimap");
                    CopyAssetItem("Samples", "Samples");
                    
                    if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
                    {
                        CopyAssetItem("VR_Enviroment~", "VR_Enviroment");
                        CopyAssetItem("Templates/VR~", "Templates/VR");
                    }
                }
                else
                {
                    if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
                    {
                        CopyAssetItem("VR_Enviroment~", "VR_Enviroment");
                    }

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
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
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
                CopyAssetItem($"Templates/VR~/{templateName}_VR.prefab", $"Templates/VR/{templateName}_VR.prefab");
            }
        }

        private void CopyAssetItem(string subPath, string targetSubPath)
        {
            string source = GetRootPath() + "/" + subPath;
            string destination = targetPath + "/" + targetSubPath;

            if (AssetDatabase.LoadAssetAtPath<Object>(destination) == null)
            {
                if (AssetDatabase.LoadAssetAtPath<Object>(source) != null)
                {
                    EnsureFolderExists(destination);
                    AssetDatabase.CopyAsset(source, destination);
                }
                else
                {
                    // Fallback to System.IO for hidden folders (ending in ~)
                    if (Directory.Exists(source))
                    {
                        CopyDirectoryIO(source, destination);
                    }
                    else if (File.Exists(source))
                    {
                        EnsureFolderExists(destination);
                        File.Copy(source, destination, true);
                    }
                }
            }
        }

        private void CopyDirectoryIO(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectoryIO(subDir, destSubDir);
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

        private struct TemplateGuids
        {
            public string baseGuid;
            public string desktopGuid;
            public string mobilePortraitGuid;
            public string mobileLandscapeGuid;
            public string vrGuid;

            public TemplateGuids(string b, string d, string mp, string ml, string vr)
            {
                baseGuid = b;
                desktopGuid = d;
                mobilePortraitGuid = mp;
                mobileLandscapeGuid = ml;
                vrGuid = vr;
            }

            public string GetGuidForPlatform(UIConfiguration.TargetPlatform platform)
            {
                return platform switch
                {
                    UIConfiguration.TargetPlatform.Desktop => desktopGuid,
                    UIConfiguration.TargetPlatform.MobilePortrait => mobilePortraitGuid,
                    UIConfiguration.TargetPlatform.MobileLandscape => mobileLandscapeGuid,
                    UIConfiguration.TargetPlatform.VR => vrGuid,
                    _ => desktopGuid
                };
            }

            public string[] GetAllPlatformGuids()
            {
                return new string[] { baseGuid, desktopGuid, mobilePortraitGuid, mobileLandscapeGuid, vrGuid };
            }
        }

        private static readonly TemplateGuids[] AllTemplates = new TemplateGuids[]
        {
            // WinLoseMenu
            new TemplateGuids("e3afe0e0415f5ba458544930bee5e004", "dec0a1e36b707a642b31b52286f051f0", "df40bc6f858a13a4ba9ba5e891112264", "e1fdd4c627b80614a87403a184b63049", "9353380678cc3184986c119d051853e4"),
            // PauseMenu
            new TemplateGuids("702398ab09e300f47ba568e6b4195742", "e823d7a97257a7f49a5f273cbde8a441", "200bb68c930bb7d49905e3ec89a580ad", "39a77a1c7213d024a990f70fe89fc545", "913ec37a587702148a97e4ca5039d799"),
            // Options
            new TemplateGuids("1664fdb73fce08d438bff449f1af0220", "ad278d46559f3c6458f8e6ec2ad55b53", "4ac02dbf33a86e341b191ac32da197f4", "075ee29682b3688499750a61d5ffd0fb", "b3db1b8be2b62ee48b7757926c484af3"),
            // MainMenu
            new TemplateGuids("280d4a726717bd4479f6c315d0b930f8", "a84a063bee5ced548b5aa022a0c0b16a", "504e6f55592b25540bb0432954ff199c", "4d547bddf8f2fab47a3d349e2bd6e438", "4e6e570e0baba9a4b972aa1bec044e6c"),
            // InventoryPanel
            new TemplateGuids("2933797009db68b40add57646c807a0d", "e787e55ab78a0c84faa54ebac8634ea2", "0b00912e1035fed47bbcfde78d866ee4", "49df74fb8385f804fb8fe5b47f2832c5", "d4738b07df292344c87ac10bbcd9d79a"),
            // HUD
            new TemplateGuids("099141bdc54090749bd00100da4e795f", "a0a7d573907e6944e89e9a4035204a34", "ab13962dfc683b749908065c4ce4467a", "840983b5871f609469a4e2b5d5a14730", "6bd84b1247604014fb75dd5bd5071cb2"),
            // DialoguePanel
            new TemplateGuids("cb47e69543f031b4ab7794cf87273783", "97b13ae1613e1eb448eff8b2184263b5", "6dfc2e2f554f1c943b6f85c46e57d403", "0c0f43bacacf9d6498031ad9a4b7ecb4", "6cbc9461b960b6944a2a8b2c4dbd9c23"),
            // Credits
            new TemplateGuids("4d186dd1d003b07428131f324345ef8a", "f752d32f51b315c4ab46d658c614ca0b", "9f77067a6d9aff14082cc74762a8bef7", "6e60e00bd8d0c3a429ff2b44e85f934b", "b1a9daba7191b654a8871f51baee148f")
        };

        private void AdaptSampleScenesToPlatform()
        {
            string samplesPath = targetPath + "/Samples";
            if (!Directory.Exists(samplesPath)) return;

            string[] unityScenes = Directory.GetFiles(samplesPath, "*.unity", SearchOption.AllDirectories);
            
            // 1. First adapt GUIDs of prefabs in all scenes via fast text replacement
            foreach (string scenePath in unityScenes)
            {
                try
                {
                    string content = File.ReadAllText(scenePath);
                    bool modified = false;

                    foreach (var template in AllTemplates)
                    {
                        string targetGuid = template.GetGuidForPlatform(selectedPlatform);
                        foreach (string oldGuid in template.GetAllPlatformGuids())
                        {
                            if (oldGuid != targetGuid && content.Contains(oldGuid))
                            {
                                content = content.Replace(oldGuid, targetGuid);
                                modified = true;
                            }
                        }
                    }

                    if (modified)
                    {
                        File.WriteAllText(scenePath, content);
                        Debug.Log($"[ModularUI] Adapted scene prefabs to {selectedPlatform}: {Path.GetFileName(scenePath)}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[ModularUI] Failed to adapt scene {scenePath}: {e.Message}");
                }
            }

            // 2. Open each scene, fix hierarchy (parenting), configuration reference, EventSystem, and pre-render themes
            string originalScenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            
            // Ask user to save modified scenes first, if they want
            UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            foreach (string scenePath in unityScenes)
            {
                CleanAndParentSceneObjects(scenePath);
            }

            // Restore the original active scene if it was valid
            if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(originalScenePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(originalScenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
            }
        }

        private void CleanAndParentSceneObjects(string scenePath)
        {
            try
            {
                // Open the scene
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
                if (!scene.IsValid()) return;

                bool isModified = false;
                GameObject[] rootObjects = scene.GetRootGameObjects();

                // 1. Find the ModularCanvas
                GameObject canvasObj = null;
                foreach (var obj in rootObjects)
                {
                    if (obj != null && (obj.name == "ModularCanvas" || obj.GetComponent<Canvas>() != null))
                    {
                        canvasObj = obj;
                        break;
                    }
                }

                // Load the configuration asset to link it
                string configPath = targetPath + "/Resources/UIConfiguration.asset";
                UIConfiguration configAsset = AssetDatabase.LoadAssetAtPath<UIConfiguration>(configPath);

                // 2. Link configuration to Canvas Initializer
                if (canvasObj != null && configAsset != null)
                {
                    var initializer = canvasObj.GetComponent<ModularCanvasInitializer>();
                    if (initializer != null)
                    {
                        if (initializer.config != configAsset)
                        {
                            initializer.config = configAsset;
                            isModified = true;
                            Debug.Log($"[ModularUI] Set UIConfiguration on ModularCanvasInitializer in {Path.GetFileName(scenePath)}");
                        }
                    }
                }

                // 3. Reparent any root UI templates (containing RectTransform) under the Canvas
                if (canvasObj != null)
                {
                    // Refresh rootObjects because it may have changed
                    rootObjects = scene.GetRootGameObjects();
                    foreach (var obj in rootObjects)
                    {
                        if (obj != null && obj != canvasObj)
                        {
                            // If it has a RectTransform and is not a Canvas itself, reparent it
                            if (obj.GetComponent<RectTransform>() != null && obj.GetComponent<Canvas>() == null)
                            {
                                // Set its parent to the ModularCanvas
                                obj.transform.SetParent(canvasObj.transform, false);
                                isModified = true;
                                Debug.Log($"[ModularUI] Reparented root UI element {obj.name} under ModularCanvas in {Path.GetFileName(scenePath)}");
                            }
                        }
                    }
                }

                // 4. Destroy the stale VR EventSystem
                rootObjects = scene.GetRootGameObjects();
                foreach (var obj in rootObjects)
                {
                    if (obj != null && (obj.name.Contains("VR_EventSystem") || obj.name.Contains("Modular_VR_EventSystem")))
                    {
                        Undo.DestroyObjectImmediate(obj);
                        isModified = true;
                        Debug.Log($"[ModularUI] Removed VR EventSystem from {Path.GetFileName(scenePath)}");
                    }
                }

                // 5. Ensure a standard EventSystem is present
#if UNITY_2023_1_OR_NEWER
                var eventSystem = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
#else
                var eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
#endif
                if (eventSystem == null)
                {
                    GameObject eventSystemObj = new GameObject("EventSystem");
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();

                    // Use reflection to check for InputSystemUIInputModule to avoid compile-time dependency
                    System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                    if (inputSystemModuleType != null)
                    {
                        eventSystemObj.AddComponent(inputSystemModuleType);
                    }
                    else
                    {
                        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    }

                    Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create standard EventSystem");
                    isModified = true;
                    Debug.Log($"[ModularUI] Created standard EventSystem in {Path.GetFileName(scenePath)}");
                }

                // 6. Link Input Actions to DemoGameManager in Demo_Scene using reflection to avoid direct type dependencies
                if (Path.GetFileNameWithoutExtension(scenePath) == "Demo_Scene")
                {
                    MonoBehaviour gameManager = null;
                    var allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var behaviour in allBehaviours)
                    {
                        if (behaviour != null && behaviour.GetType().Name == "DemoGameManager")
                        {
                            gameManager = behaviour;
                            break;
                        }
                    }

                    if (gameManager != null)
                    {
                        var fieldInfo = gameManager.GetType().GetField("inputActions", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (fieldInfo != null && fieldInfo.GetValue(gameManager) == null)
                        {
                            string actionsPath = targetPath + "/Samples/InputSystem_Actions.inputactions";
                            var actionsAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(actionsPath);
                            if (actionsAsset != null)
                            {
                                fieldInfo.SetValue(gameManager, actionsAsset);
                                EditorUtility.SetDirty(gameManager);
                                isModified = true;
                                Debug.Log($"[ModularUI] Assigned InputSystem_Actions to DemoGameManager in {Path.GetFileName(scenePath)}");
                            }
                        }
                    }
                }

                // 7. Force all ModularComponents in the scene to pre-render the active theme and mark them dirty
                var modularComponents = Object.FindObjectsByType<ModularComponents>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var comp in modularComponents)
                {
                    if (comp != null)
                    {
                        comp.ApplyThemeInEditor();
                        isModified = true;
                    }
                }

                if (isModified)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ModularUI] Error cleaning and parenting scene {scenePath}: {e.Message}");
            }
        }
    }
}