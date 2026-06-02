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

        [MenuItem("Tools/Modular UI/Clean and Fix Sample Scenes")]
        public static void CleanAndFixSampleScenes()
        {
            ModularUIWizard wizard = CreateInstance<ModularUIWizard>();
            wizard.AdaptSampleScenesToPlatform();
            DestroyImmediate(wizard);
            
        }

        private string GetRootPath()
        {
            if (AssetDatabase.IsValidFolder("Packages/com.pau.modularui"))
            {
                return "Packages/com.pau.modularui";
            }

            return "Assets/Plugins/ModularUI";
        }

        private string GetPhysicalSourcePath(string assetPath)
        {
            if (assetPath.StartsWith("Packages/com.pau.modularui"))
            {
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.pau.modularui");
                if (packageInfo != null)
                {
                    string relativePath = assetPath.Substring("Packages/com.pau.modularui".Length);
                    return (packageInfo.resolvedPath + relativePath).Replace('\\', '/');
                }
            }
            return assetPath;
        }

        private void OnEnable()
        {
            string key = ModularUIStartup.GetPrefsKey();
            EditorPrefs.SetBool(key, true);
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

            Color headerColor = new Color(0.12f, 0.16f, 0.22f); 
            Color boxColor = new Color(0.18f, 0.18f, 0.18f); 
            Color accentColor = new Color(0f, 0.67f, 0.71f); 
            Color accentHoverColor = new Color(0f, 0.8f, 0.85f); 

            headerTex = MakeTex(2, 2, headerColor);
            boxTex = MakeTex(2, 2, boxColor);
            buttonTex = MakeTex(2, 2, accentColor);
            buttonHoverTex = MakeTex(2, 2, accentHoverColor);

            headerStyle = new GUIStyle();
            headerStyle.normal.background = headerTex;
            headerStyle.padding = new RectOffset(10, 10, 15, 15);
            headerStyle.margin = new RectOffset(0, 0, 0, 0);

            sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            sectionHeaderStyle.normal.textColor = new Color(0f, 0.85f, 0.9f); 
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
            
            toggleStyle = new GUIStyle(EditorStyles.label);
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

            GUILayout.BeginHorizontal(headerStyle);
            GUILayout.Label("MODULAR UI SYSTEM", new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(15, 15, 15, 15) });

            EditorGUILayout.LabelField("1. INTEGRATION SETTINGS", sectionHeaderStyle);
            GUILayout.BeginVertical(boxStyle);
            
            currentStatus = (ProjectStatus)EditorGUILayout.EnumPopup("Project Status:", currentStatus);
            currentScope = (ImportScope)EditorGUILayout.EnumPopup("Import Scope:", currentScope);

            if (currentScope == ImportScope.SPECIFIC_MODULES)
            {
                EditorGUILayout.Space(10);

                Rect lineRect = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(lineRect, new Color(0.3f, 0.3f, 0.3f));
                EditorGUILayout.Space(10);

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

            EditorGUILayout.LabelField("2. ARCHITECTURE CONFIGURATION", sectionHeaderStyle);
            GUILayout.BeginVertical(boxStyle);
            
            selectedPlatform = (UIConfiguration.TargetPlatform)EditorGUILayout.EnumPopup("Target Platform:", selectedPlatform);
            selectedGenre = (UIConfiguration.GameGenre)EditorGUILayout.EnumPopup("Game Genre:", selectedGenre);
            
            GUILayout.EndVertical();

            DrawPlatformHelpBox();

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(15, 15, 15, 15) });
            if (GUILayout.Button("IMPORT & CONFIGURE SYSTEM", buttonStyle, GUILayout.Height(45)))
            {
                ExecuteImport();
                Close();
            }
            GUILayout.EndVertical();
        }

        private void CreateAndApplyConfiguration()
        {
            
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
            
        }

        private void ExecuteImport()
        {
            CleanupTargetDirectories();

            if (!AssetDatabase.IsValidFolder(targetPath))
            {
                EnsureFolderExists(targetPath + "/dummy.txt");
            }

            var guidReplacements = new System.Collections.Generic.Dictionary<string, string>();

            if (currentScope == ImportScope.FULL_SYSTEM)
            {
                CopyAssetItem("BaseUI", "BaseUI", guidReplacements);
                CopyAssetItem("Templates", "Templates", guidReplacements);
                CopyAssetItem("Dialogues", "Dialogues", guidReplacements);
                CopyAssetItem("Resources", "Resources", guidReplacements);
                CopyAssetItem("Minimap", "Minimap", guidReplacements);
                CopyAssetItem("Samples", "Samples", guidReplacements);
                
                if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
                {
                    CopyAssetItem("VR_Enviroment~", "VR_Enviroment", guidReplacements);
                    CopyAssetItem("Templates/VR~", "Templates/VR", guidReplacements);
                }
            }
            else
            {
                if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
                {
                    CopyAssetItem("VR_Enviroment~", "VR_Enviroment", guidReplacements);
                }

                if (importBaseUI)
                {
                    CopyAssetItem("BaseUI", "BaseUI", guidReplacements);
                }

                if (importMainMenu)
                {
                    ImportTemplateVariant("MainMenu", guidReplacements);
                }

                if (importHUD)
                {
                    ImportTemplateVariant("HUD", guidReplacements);
                }

                if (importInventory)
                {
                    ImportTemplateVariant("InventoryPanel", guidReplacements);
                }

                if (importOptions)
                {
                    ImportTemplateVariant("Options", guidReplacements);
                }

                if (importPauseMenu)
                {
                    ImportTemplateVariant("PauseMenu", guidReplacements);
                }

                if (importCredits)
                {
                    ImportTemplateVariant("Credits", guidReplacements);
                }

                if (importWinLose)
                {
                    ImportTemplateVariant("WinLoseMenu", guidReplacements);
                }

                if (importDialogues)
                {
                    CopyAssetItem("Dialogues", "Dialogues", guidReplacements);
                    ImportTemplateVariant("DialoguePanel", guidReplacements);
                }

                if (importSettings)
                {
                    CopyAssetItem("Resources", "Resources", guidReplacements);
                }

                if (importMinimap)
                {
                    CopyAssetItem("Minimap", "Minimap", guidReplacements);
                }

                if (importSamples)
                {
                    CopyAssetItem("Samples", "Samples", guidReplacements);
                }

                if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait || selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
                {
                    CopyAssetItem("Templates/Mobile/MobileControls.prefab", "Templates/Mobile/MobileControls.prefab", guidReplacements);
                }
            }

            EditorPrefs.SetBool("ModularUI_PendingSetup", true);
            EditorPrefs.SetInt("ModularUI_SelectedPlatform", (int)selectedPlatform);
            EditorPrefs.SetInt("ModularUI_SelectedGenre", (int)selectedGenre);
            EditorPrefs.SetString("ModularUI_GuidReplacements", SerializeGuidMap(guidReplacements));

            AssetDatabase.Refresh();
        }

        private void ImportTemplateVariant(string templateName, System.Collections.Generic.Dictionary<string, string> guidReplacements)
        {
            CopyAssetItem($"Templates/Base/{templateName}_Base.prefab", $"Templates/Base/{templateName}_Base.prefab", guidReplacements);

            if (selectedPlatform == UIConfiguration.TargetPlatform.Desktop)
            {
                CopyAssetItem($"Templates/Desktop/{templateName}_Desktop.prefab", $"Templates/Desktop/{templateName}_Desktop.prefab", guidReplacements);
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait)
            {
                CopyAssetItem($"Templates/Mobile/Portrait/{templateName}_Portrait.prefab", $"Templates/Mobile/Portrait/{templateName}_Portrait.prefab", guidReplacements);
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.MobileLandscape)
            {
                CopyAssetItem($"Templates/Mobile/Landscape/{templateName}_Landscape.prefab", $"Templates/Mobile/Landscape/{templateName}_Landscape.prefab", guidReplacements);
            }

            if (selectedPlatform == UIConfiguration.TargetPlatform.VR)
            {
                CopyAssetItem($"Templates/VR~/{templateName}_VR.prefab", $"Templates/VR/{templateName}_VR.prefab", guidReplacements);
            }
        }

        private void CopyAssetItem(string subPath, string targetSubPath, System.Collections.Generic.Dictionary<string, string> guidReplacements)
        {
            string source = GetRootPath() + "/" + subPath;
            string destination = targetPath + "/" + targetSubPath;

            string physicalSource = GetPhysicalSourcePath(source);
            string physicalDest = Path.GetFullPath(destination).Replace('\\', '/');
            string physicalSourceNorm = Path.GetFullPath(physicalSource).Replace('\\', '/');

            if (string.Equals(physicalSourceNorm, physicalDest, System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (Directory.Exists(physicalSourceNorm))
            {
                PhysicalCopyFolder(physicalSourceNorm, physicalDest, guidReplacements);
            }
            else if (File.Exists(physicalSourceNorm))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(physicalDest));
                try
                {
                    File.Copy(physicalSourceNorm, physicalDest, true);
                    
                    string sourceMeta = physicalSourceNorm + ".meta";
                    string destMeta = physicalDest + ".meta";
                    if (File.Exists(sourceMeta))
                    {
                        string metaContent = File.ReadAllText(sourceMeta);
                        string oldGuid = ExtractGuidFromMeta(metaContent);
                        if (!string.IsNullOrEmpty(oldGuid))
                        {
                            string newGuid = System.Guid.NewGuid().ToString("N");
                            guidReplacements[oldGuid] = newGuid;
                            string newMetaContent = ReplaceGuidInMeta(metaContent, oldGuid, newGuid);
                            File.WriteAllText(destMeta, newMetaContent);
                        }
                    }
                }
                catch (System.Exception) { }
            }
        }

        private void PhysicalCopyFolder(string sourceDir, string destDir, System.Collections.Generic.Dictionary<string, string> guidReplacements)
        {
            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                if (file.EndsWith(".meta")) continue;

                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);

                try
                {
                    File.Copy(file, destFile, true);

                    string sourceMeta = file + ".meta";
                    string destMeta = destFile + ".meta";
                    if (File.Exists(sourceMeta))
                    {
                        string metaContent = File.ReadAllText(sourceMeta);
                        string oldGuid = ExtractGuidFromMeta(metaContent);
                        if (!string.IsNullOrEmpty(oldGuid))
                        {
                            string newGuid = System.Guid.NewGuid().ToString("N");
                            guidReplacements[oldGuid] = newGuid;
                            string newMetaContent = ReplaceGuidInMeta(metaContent, oldGuid, newGuid);
                            File.WriteAllText(destMeta, newMetaContent);
                        }
                    }
                }
                catch (System.Exception) { }
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                if (dirName.Contains("~")) continue;

                string destSubDir = Path.Combine(destDir, dirName);
                
                PhysicalCopyFolder(subDir, destSubDir, guidReplacements);

                string sourceMeta = subDir + ".meta";
                string destMeta = destSubDir + ".meta";
                if (File.Exists(sourceMeta))
                {
                    try
                    {
                        string metaContent = File.ReadAllText(sourceMeta);
                        string oldGuid = ExtractGuidFromMeta(metaContent);
                        if (!string.IsNullOrEmpty(oldGuid))
                        {
                            string newGuid = System.Guid.NewGuid().ToString("N");
                            guidReplacements[oldGuid] = newGuid;
                            string newMetaContent = ReplaceGuidInMeta(metaContent, oldGuid, newGuid);
                            File.WriteAllText(destMeta, newMetaContent);
                        }
                    }
                    catch (System.Exception) { }
                }
            }
        }

        private void CleanupTargetDirectories()
        {
            if (!Directory.Exists(targetPath)) return;

            string[] directories = Directory.GetDirectories(targetPath);
            foreach (string dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                
                bool shouldDelete = dirName == "BaseUI" || dirName == "Templates" || 
                                   dirName == "Dialogues" || dirName == "Minimap" || 
                                   dirName == "Samples" || dirName == "VR_Enviroment" ||
                                   dirName.StartsWith("BaseUI ") || dirName.StartsWith("Templates ") ||
                                   dirName.StartsWith("Dialogues ") || dirName.StartsWith("Minimap ") ||
                                   dirName.StartsWith("Samples ") || dirName.StartsWith("VR_Enviroment ");

                if (shouldDelete)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        
                        string metaFile = dir + ".meta";
                        if (File.Exists(metaFile))
                        {
                            File.Delete(metaFile);
                        }
                    }
                    catch (System.Exception) { }
                }
            }
        }

        private string ExtractGuidFromMeta(string metaContent)
        {
            int index = metaContent.IndexOf("guid: ");
            if (index >= 0)
            {
                int start = index + "guid: ".Length;
                int end = metaContent.IndexOf('\n', start);
                if (end < 0) end = metaContent.Length;
                return metaContent.Substring(start, end - start).Trim().Replace("\r", "");
            }
            return null;
        }

        private string ReplaceGuidInMeta(string metaContent, string oldGuid, string newGuid)
        {
            return metaContent.Replace(oldGuid, newGuid);
        }

        private string SerializeGuidMap(System.Collections.Generic.Dictionary<string, string> map)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var kvp in map)
            {
                sb.Append(kvp.Key).Append(':').Append(kvp.Value).Append(';');
            }
            return sb.ToString();
        }

        public System.Collections.Generic.Dictionary<string, string> DeserializeGuidMap(string data)
        {
            var map = new System.Collections.Generic.Dictionary<string, string>();
            if (string.IsNullOrEmpty(data)) return map;

            string[] pairs = data.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string pair in pairs)
            {
                string[] kv = pair.Split(':');
                if (kv.Length == 2)
                {
                    map[kv[0]] = kv[1];
                }
            }
            return map;
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
            
            new TemplateGuids("e3afe0e0415f5ba458544930bee5e004", "dec0a1e36b707a642b31b52286f051f0", "df40bc6f858a13a4ba9ba5e891112264", "e1fdd4c627b80614a87403a184b63049", "9353380678cc3184986c119d051853e4"),
            
            new TemplateGuids("702398ab09e300f47ba568e6b4195742", "e823d7a97257a7f49a5f273cbde8a441", "200bb68c930bb7d49905e3ec89a580ad", "39a77a1c7213d024a990f70fe89fc545", "913ec37a587702148a97e4ca5039d799"),
            
            new TemplateGuids("1664fdb73fce08d438bff449f1af0220", "ad278d46559f3c6458f8e6ec2ad55b53", "4ac02dbf33a86e341b191ac32da197f4", "075ee29682b3688499750a61d5ffd0fb", "b3db1b8be2b62ee48b7757926c484af3"),
            
            new TemplateGuids("280d4a726717bd4479f6c315d0b930f8", "a84a063bee5ced548b5aa022a0c0b16a", "504e6f55592b25540bb0432954ff199c", "4d547bddf8f2fab47a3d349e2bd6e438", "4e6e570e0baba9a4b972aa1bec044e6c"),
            
            new TemplateGuids("2933797009db68b40add57646c807a0d", "e787e55ab78a0c84faa54ebac8634ea2", "0b00912e1035fed47bbcfde78d866ee4", "49df74fb8385f804fb8fe5b47f2832c5", "d4738b07df292344c87ac10bbcd9d79a"),
            
            new TemplateGuids("099141bdc54090749bd00100da4e795f", "a0a7d573907e6944e89e9a4035204a34", "ab13962dfc683b749908065c4ce4467a", "840983b5871f609469a4e2b5d5a14730", "6bd84b1247604014fb75dd5bd5071cb2"),
            
            new TemplateGuids("cb47e69543f031b4ab7794cf87273783", "97b13ae1613e1eb448eff8b2184263b5", "6dfc2e2f554f1c943b6f85c46e57d403", "0c0f43bacacf9d6498031ad9a4b7ecb4", "6cbc9461b960b6944a2a8b2c4dbd9c23"),
            
            new TemplateGuids("4d186dd1d003b07428131f324345ef8a", "f752d32f51b315c4ab46d658c614ca0b", "9f77067a6d9aff14082cc74762a8bef7", "6e60e00bd8d0c3a429ff2b44e85f934b", "b1a9daba7191b654a8871f51baee148f")
        };

        private void AdaptSampleScenesToPlatform(System.Collections.Generic.Dictionary<string, string> guidReplacements = null)
        {
            if (!Directory.Exists(targetPath))
            {
                return;
            }

            if (guidReplacements == null)
            {
                guidReplacements = new System.Collections.Generic.Dictionary<string, string>();
                string packageRoot = "Packages/com.pau.modularui";
                if (AssetDatabase.IsValidFolder(packageRoot))
                {
                    var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(packageRoot);
                    if (packageInfo != null)
                    {
                        string physicalPackageRoot = packageInfo.resolvedPath.Replace('\\', '/');
                        string[] allFiles = Directory.GetFiles(physicalPackageRoot, "*.*", SearchOption.AllDirectories);
                        foreach (string physicalFile in allFiles)
                        {
                            string normPath = physicalFile.Replace('\\', '/');
                            if (normPath.EndsWith(".meta") || normPath.Contains("~")) continue;

                            string relativePath = normPath.Substring(physicalPackageRoot.Length);
                            string packageAssetPath = packageRoot + relativePath;
                            string localAssetPath = targetPath + relativePath;

                            string oldGuid = AssetDatabase.AssetPathToGUID(packageAssetPath);
                            string newGuid = AssetDatabase.AssetPathToGUID(localAssetPath);

                            if (!string.IsNullOrEmpty(oldGuid) && !string.IsNullOrEmpty(newGuid) && oldGuid != newGuid)
                            {
                                guidReplacements[oldGuid] = newGuid;
                            }
                        }
                    }
                }
            }

            var filesToPatch = new System.Collections.Generic.List<string>();
            foreach (string file in Directory.GetFiles(targetPath, "*.*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".unity" || ext == ".prefab" || ext == ".asset")
                {
                    filesToPatch.Add(file.Replace('\\', '/'));
                }
            }

            foreach (string filePath in filesToPatch)
            {
                try
                {
                    string content = File.ReadAllText(filePath);
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

                    // Apply automatic package-to-local GUID replacements
                    foreach (var replacement in guidReplacements)
                    {
                        if (content.Contains(replacement.Key))
                        {
                            content = content.Replace(replacement.Key, replacement.Value);
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        File.WriteAllText(filePath, content);
                        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceSynchronousImport);
                    }
                }
                catch (System.Exception)
                {
                }
            }

            string originalScenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

            UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string samplesPath = targetPath + "/Samples";
            if (Directory.Exists(samplesPath))
            {
                string[] unityScenes = Directory.GetFiles(samplesPath, "*.unity", SearchOption.AllDirectories);
                foreach (string scenePath in unityScenes)
                {
                    CleanAndParentSceneObjects(scenePath.Replace('\\', '/'));
                }
            }

            if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(GetPhysicalSourcePath(originalScenePath)))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(originalScenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
            }
        }

        private void CleanAndParentSceneObjects(string scenePath)
        {
            try
            {
                
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
                if (!scene.IsValid()) return;

                bool isModified = false;
                GameObject[] rootObjects = scene.GetRootGameObjects();

                GameObject canvasObj = null;
                foreach (var obj in rootObjects)
                {
                    if (obj != null && (obj.name == "ModularCanvas" || obj.GetComponent<Canvas>() != null))
                    {
                        canvasObj = obj;
                        break;
                    }
                }

                string configPath = targetPath + "/Resources/UIConfiguration.asset";
                UIConfiguration configAsset = AssetDatabase.LoadAssetAtPath<UIConfiguration>(configPath);

                if (canvasObj != null && configAsset != null)
                {
                    var initializer = canvasObj.GetComponent<ModularCanvasInitializer>();
                    if (initializer != null)
                    {
                        if (initializer.config != configAsset)
                        {
                            initializer.config = configAsset;
                            isModified = true;
                            
                        }
                    }
                }

                if (canvasObj != null)
                {
                    
                    rootObjects = scene.GetRootGameObjects();
                    foreach (var obj in rootObjects)
                    {
                        if (obj != null && obj != canvasObj)
                        {
                            
                            if (obj.GetComponent<RectTransform>() != null && obj.GetComponent<Canvas>() == null)
                            {
                                
                                obj.transform.SetParent(canvasObj.transform, false);
                                isModified = true;
                                
                            }
                        }
                    }
                }

                rootObjects = scene.GetRootGameObjects();
                foreach (var obj in rootObjects)
                {
                    if (obj != null && (obj.name.Contains("VR_EventSystem") || obj.name.Contains("Modular_VR_EventSystem")))
                    {
                        Undo.DestroyObjectImmediate(obj);
                        isModified = true;
                        
                    }
                }

#if UNITY_2023_1_OR_NEWER
                var eventSystem = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
#else
                var eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
#endif
                if (eventSystem == null)
                {
                    GameObject eventSystemObj = new GameObject("EventSystem");
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();

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
                    
                }

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
                        var gmType = gameManager.GetType();
                        var bindFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;


                        var playerField = gmType.GetField("playerTransform", bindFlags);
                        if (playerField != null)
                        {
                            var currentPlayer = playerField.GetValue(gameManager) as UnityEngine.Object;
                            if (currentPlayer == null)
                            {
                                GameObject playerGo = GameObject.Find("Player");
                                if (playerGo != null)
                                {
                                    playerField.SetValue(gameManager, playerGo.transform);
                                    EditorUtility.SetDirty(gameManager);
                                    isModified = true;
                                }
                            }
                        }

                        var pickupsField = gmType.GetField("pickupsParent", bindFlags);
                        if (pickupsField != null)
                        {
                            var currentPickups = pickupsField.GetValue(gameManager) as UnityEngine.Object;
                            if (currentPickups == null)
                            {
                                GameObject pickupsGo = GameObject.Find("Pickups");
                                if (pickupsGo == null)
                                {
                                    pickupsGo = new GameObject("Pickups");
                                    Undo.RegisterCreatedObjectUndo(pickupsGo, "Create Pickups container");
                                }
                                pickupsField.SetValue(gameManager, pickupsGo.transform);
                                EditorUtility.SetDirty(gameManager);
                                isModified = true;
                            }
                        }

                        var slotsField = gmType.GetField("slotsContainer", bindFlags);
                        if (slotsField != null)
                        {
                            var currentSlots = slotsField.GetValue(gameManager) as UnityEngine.Object;
                            if (currentSlots == null)
                            {
                                var invManager = Object.FindAnyObjectByType<ModularInventoryManager>(FindObjectsInactive.Include);
                                if (invManager != null)
                                {
                                    var slot = invManager.GetComponentInChildren<ModularInventorySlot>(true);
                                    if (slot != null)
                                    {
                                        slotsField.SetValue(gameManager, slot.transform.parent);
                                        EditorUtility.SetDirty(gameManager);
                                        isModified = true;
                                    }
                                }
                            }
                        }

                        var itemPrefabField = gmType.GetField("itemPrefab", bindFlags);
                        if (itemPrefabField != null)
                        {
                            var currentPrefab = itemPrefabField.GetValue(gameManager) as UnityEngine.Object;
                            if (currentPrefab == null)
                            {
                                string[] prefabGuids = AssetDatabase.FindAssets("InventoryItem t:Prefab");
                                if (prefabGuids != null && prefabGuids.Length > 0)
                                {
                                    string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[0]);
                                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                                    if (prefab != null)
                                    {
                                        itemPrefabField.SetValue(gameManager, prefab);
                                        EditorUtility.SetDirty(gameManager);
                                        isModified = true;
                                    }
                                }
                            }
                        }

                        var fixMethod = gmType.GetMethod("FixRenderPipelineShaders", bindFlags);
                        if (fixMethod != null)
                        {
                            fixMethod.Invoke(gameManager, null);
                            isModified = true;
                        }
                    }
                }

                var modularComponents = Object.FindObjectsByType<ModularComponents>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var comp in modularComponents)
                {
                    if (comp != null)
                    {
                        comp.ApplyThemeInEditor();
                        isModified = true;
                    }
                }

                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                if (sceneName == "MainMenu_Scene")
                {
#if UNITY_2023_1_OR_NEWER
                    var mainMenus = Object.FindObjectsByType<ModularMainMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    var optionsMenus = Object.FindObjectsByType<ModularOptionsMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    var creditsMenus = Object.FindObjectsByType<ModularCredits>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                    var mainMenus = Object.FindObjectsOfType<ModularMainMenu>(true);
                    var optionsMenus = Object.FindObjectsOfType<ModularOptionsMenu>(true);
                    var creditsMenus = Object.FindObjectsOfType<ModularCredits>(true);
#endif

                    ModularMainMenu mainMenu = mainMenus.Length > 0 ? mainMenus[0] : null;
                    ModularOptionsMenu optionsMenu = optionsMenus.Length > 0 ? optionsMenus[0] : null;
                    ModularCredits creditsMenu = creditsMenus.Length > 0 ? creditsMenus[0] : null;

                    if (mainMenu != null)
                    {
                        var menuActions = mainMenu.GetComponent<ModularMenuActions>();
                        if (menuActions != null)
                        {
                            SerializedObject so = new SerializedObject(mainMenu);
                            SerializedProperty menuButtonsProp = so.FindProperty("menuButtons");
                            if (menuButtonsProp != null && menuButtonsProp.isArray)
                            {
                                for (int i = 0; i < menuButtonsProp.arraySize; i++)
                                {
                                    SerializedProperty buttonDataProp = menuButtonsProp.GetArrayElementAtIndex(i);
                                    SerializedProperty nameProp = buttonDataProp.FindPropertyRelative("buttonName");
                                    if (nameProp != null)
                                    {
                                        string btnName = nameProp.stringValue;
                                        if (btnName == "Options" && optionsMenu != null)
                                        {
                                            FixButtonOpenPanelArgument(buttonDataProp, menuActions, optionsMenu.gameObject, mainMenu.gameObject);
                                            isModified = true;
                                        }
                                        else if (btnName == "Credits" && creditsMenu != null)
                                        {
                                            FixButtonOpenPanelArgument(buttonDataProp, menuActions, creditsMenu.gameObject, mainMenu.gameObject);
                                            isModified = true;
                                        }
                                    }
                                }
                                so.ApplyModifiedProperties();
                            }
                        }

                        if (creditsMenu != null)
                        {
                            var creditsMenuActions = creditsMenu.GetComponent<ModularMenuActions>();
                            if (creditsMenuActions == null)
                            {
                                creditsMenuActions = creditsMenu.gameObject.AddComponent<ModularMenuActions>();
                            }

                            if (creditsMenuActions != null)
                            {
                                FixCreditsFinishedEvent(creditsMenu, creditsMenuActions, mainMenu.gameObject);
                                isModified = true;
                            }
                        }
                    }

                    foreach (var opt in optionsMenus)
                    {
                        if (opt != null && opt.gameObject.activeSelf)
                        {
                            opt.gameObject.SetActive(false);
                            EditorUtility.SetDirty(opt.gameObject);
                            isModified = true;
                            
                        }
                    }

                    foreach (var cred in creditsMenus)
                    {
                        if (cred != null && cred.gameObject.activeSelf)
                        {
                            cred.gameObject.SetActive(false);
                            EditorUtility.SetDirty(cred.gameObject);
                            isModified = true;
                            
                        }
                    }
                }
                else if (sceneName == "HUD_Scene")
                {
                    var pauseMenus = Object.FindObjectsByType<ModularPauseMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var pm in pauseMenus)
                    {
                        if (pm != null && pm.gameObject.activeSelf)
                        {
                            pm.gameObject.SetActive(false);
                            EditorUtility.SetDirty(pm.gameObject);
                            isModified = true;
                            
                        }
                    }
                }
                else if (sceneName == "Demo_Scene")
                {
                    var pauseMenus = Object.FindObjectsByType<ModularPauseMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var pm in pauseMenus)
                    {
                        if (pm != null && pm.gameObject.activeSelf)
                        {
                            pm.gameObject.SetActive(false);
                            EditorUtility.SetDirty(pm.gameObject);
                            isModified = true;
                            
                        }
                    }

                    var inventoryManagers = Object.FindObjectsByType<ModularInventoryManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var inv in inventoryManagers)
                    {
                        if (inv != null && inv.gameObject.activeSelf)
                        {
                            inv.gameObject.SetActive(false);
                            EditorUtility.SetDirty(inv.gameObject);
                            isModified = true;
                            
                        }
                    }
                }

                if (canvasObj != null)
                {
                    int childCount = canvasObj.transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        Transform child = canvasObj.transform.GetChild(i);
                        if (child == null) continue;

                        string childName = child.name.ToLower();
                        bool shouldDeactivate = false;

                        if (sceneName == "MainMenu_Scene")
                        {
                            if ((childName.Contains("options") || childName.Contains("credits")) && 
                                !childName.Contains("button") && !childName.Contains("btn"))
                            {
                                shouldDeactivate = true;
                            }
                        }
                        else if (sceneName == "HUD_Scene")
                        {
                            if (childName.Contains("pause") && 
                                !childName.Contains("button") && !childName.Contains("btn"))
                            {
                                shouldDeactivate = true;
                            }
                        }
                        else if (sceneName == "Demo_Scene")
                        {
                            if ((childName.Contains("pause") || childName.Contains("inventory")) && 
                                !childName.Contains("button") && !childName.Contains("btn"))
                            {
                                shouldDeactivate = true;
                            }
                        }

                        if (shouldDeactivate && child.gameObject.activeSelf)
                        {
                            child.gameObject.SetActive(false);
                            EditorUtility.SetDirty(child.gameObject);
                            isModified = true;
                            
                        }
                    }
                }

                if (isModified)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                }
            }
            catch (System.Exception)
            {
                
            }
        }

        private void FixButtonOpenPanelArgument(SerializedProperty buttonDataProp, ModularMenuActions menuActions, GameObject panelGameObject, GameObject mainMenuGameObject)
        {
            SerializedProperty onClickProp = buttonDataProp.FindPropertyRelative("OnClick");
            if (onClickProp == null) return;

            SerializedProperty persistentCallsProp = onClickProp.FindPropertyRelative("m_PersistentCalls");
            if (persistentCallsProp == null) return;

            SerializedProperty callsProp = persistentCallsProp.FindPropertyRelative("m_Calls");
            if (callsProp == null || !callsProp.isArray) return;

            for (int j = 0; j < callsProp.arraySize; j++)
            {
                SerializedProperty callProp = callsProp.GetArrayElementAtIndex(j);
                SerializedProperty methodNameProp = callProp.FindPropertyRelative("m_MethodName");
                if (methodNameProp != null)
                {
                    string methodName = methodNameProp.stringValue;
                    if (methodName == "OpenPanel")
                    {
                        SerializedProperty targetProp = callProp.FindPropertyRelative("m_Target");
                        if (targetProp != null)
                        {
                            targetProp.objectReferenceValue = menuActions;
                        }

                        SerializedProperty argumentsProp = callProp.FindPropertyRelative("m_Arguments");
                        if (argumentsProp != null)
                        {
                            SerializedProperty objectArgProp = argumentsProp.FindPropertyRelative("m_ObjectArgument");
                            if (objectArgProp != null)
                            {
                                objectArgProp.objectReferenceValue = panelGameObject;
                            }

                            SerializedProperty objectArgTypeProp = argumentsProp.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                            if (objectArgTypeProp != null)
                            {
                                objectArgTypeProp.stringValue = "UnityEngine.GameObject, UnityEngine";
                            }
                        }

                        SerializedProperty modeProp = callProp.FindPropertyRelative("m_Mode");
                        if (modeProp != null)
                        {
                            modeProp.intValue = 2; 
                        }
                    }
                    else if (methodName == "ClosePanel")
                    {
                        SerializedProperty targetProp = callProp.FindPropertyRelative("m_Target");
                        if (targetProp != null)
                        {
                            targetProp.objectReferenceValue = menuActions;
                        }

                        SerializedProperty argumentsProp = callProp.FindPropertyRelative("m_Arguments");
                        if (argumentsProp != null)
                        {
                            SerializedProperty objectArgProp = argumentsProp.FindPropertyRelative("m_ObjectArgument");
                            if (objectArgProp != null)
                            {
                                objectArgProp.objectReferenceValue = mainMenuGameObject;
                            }

                            SerializedProperty objectArgTypeProp = argumentsProp.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                            if (objectArgTypeProp != null)
                            {
                                objectArgTypeProp.stringValue = "UnityEngine.GameObject, UnityEngine";
                            }
                        }

                        SerializedProperty modeProp = callProp.FindPropertyRelative("m_Mode");
                        if (modeProp != null)
                        {
                            modeProp.intValue = 2; 
                        }
                    }
                }
            }
        }

        private void FixCreditsFinishedEvent(ModularCredits creditsMenu, ModularMenuActions menuActions, GameObject mainMenuGameObject)
        {
            SerializedObject so = new SerializedObject(creditsMenu);
            SerializedProperty onFinishedProp = so.FindProperty("OnCreditsFinished");
            if (onFinishedProp == null) return;

            SerializedProperty persistentCallsProp = onFinishedProp.FindPropertyRelative("m_PersistentCalls");
            if (persistentCallsProp == null) return;

            SerializedProperty callsProp = persistentCallsProp.FindPropertyRelative("m_Calls");
            if (callsProp == null || !callsProp.isArray) return;

            if (callsProp.arraySize < 2)
            {
                callsProp.arraySize = 2;
            }

            SerializedProperty call0 = callsProp.GetArrayElementAtIndex(0);
            call0.FindPropertyRelative("m_MethodName").stringValue = "OpenPanel";
            call0.FindPropertyRelative("m_Target").objectReferenceValue = menuActions;
            call0.FindPropertyRelative("m_Mode").intValue = 2;

            SerializedProperty args0 = call0.FindPropertyRelative("m_Arguments");
            if (args0 != null)
            {
                args0.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = mainMenuGameObject;
                args0.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = "UnityEngine.GameObject, UnityEngine";
            }

            SerializedProperty call1 = callsProp.GetArrayElementAtIndex(1);
            call1.FindPropertyRelative("m_MethodName").stringValue = "ClosePanel";
            call1.FindPropertyRelative("m_Target").objectReferenceValue = menuActions;
            call1.FindPropertyRelative("m_Mode").intValue = 2;

            SerializedProperty args1 = call1.FindPropertyRelative("m_Arguments");
            if (args1 != null)
            {
                args1.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = creditsMenu.gameObject;
                args1.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = "UnityEngine.GameObject, UnityEngine";
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(creditsMenu);
        }

        public void InitializeAfterCompilation(UIConfiguration.TargetPlatform platform, UIConfiguration.GameGenre genre, System.Collections.Generic.Dictionary<string, string> guidReplacements)
        {
            selectedPlatform = platform;
            selectedGenre = genre;

            CreateAndApplyConfiguration();
            AdaptSampleScenesToPlatform(guidReplacements);

            AssetDatabase.Refresh();
        }
    }

    [InitializeOnLoad]
    public static class ModularUIPostProcessor
    {
        static ModularUIPostProcessor()
        {
            EditorApplication.delayCall += CheckPendingSetup;
        }

        private static void CheckPendingSetup()
        {
            if (EditorPrefs.GetBool("ModularUI_PendingSetup", false))
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    EditorApplication.delayCall += CheckPendingSetup;
                    return;
                }

                EditorPrefs.SetBool("ModularUI_PendingSetup", false);
                RunPhase2Setup();
            }
        }

        private static void RunPhase2Setup()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var platform = (UIConfiguration.TargetPlatform)EditorPrefs.GetInt("ModularUI_SelectedPlatform", 0);
            var genre = (UIConfiguration.GameGenre)EditorPrefs.GetInt("ModularUI_SelectedGenre", 0);
            string guidData = EditorPrefs.GetString("ModularUI_GuidReplacements", "");

            ModularUIWizard wizard = ScriptableObject.CreateInstance<ModularUIWizard>();
            var guidReplacements = wizard.DeserializeGuidMap(guidData);
            wizard.InitializeAfterCompilation(platform, genre, guidReplacements);
            ScriptableObject.DestroyImmediate(wizard);

            EditorPrefs.DeleteKey("ModularUI_GuidReplacements");
            EditorPrefs.DeleteKey("ModularUI_SelectedPlatform");
            EditorPrefs.DeleteKey("ModularUI_SelectedGenre");

            Debug.Log("[Modular UI] Setup and scene adaptation completed successfully after script compilation!");
        }
    }
}