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

        [MenuItem("Tools/Modular UI/Setup Wizard")]
        public static void ShowWindow()
        {
            ModularUIWizard window = GetWindow<ModularUIWizard>("UI Wizard");
            window.minSize = new Vector2(400, 700);
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

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("MODULAR UI SYSTEM SETUP", titleStyle);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("1. Integration Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            currentStatus = (ProjectStatus)EditorGUILayout.EnumPopup("Project Status:", currentStatus);
            currentScope = (ImportScope)EditorGUILayout.EnumPopup("Import Scope:", currentScope);

            if (currentScope == ImportScope.SPECIFIC_MODULES)
            {
                EditorGUILayout.Space(5);
                EditorGUI.indentLevel++;
                importBaseUI = EditorGUILayout.Toggle("Base UI (Required)", importBaseUI);
                importMainMenu = EditorGUILayout.Toggle("Main Menu Template", importMainMenu);
                importHUD = EditorGUILayout.Toggle("HUD Template", importHUD);
                importInventory = EditorGUILayout.Toggle("Inventory System", importInventory);
                importOptions = EditorGUILayout.Toggle("Options Menu", importOptions);
                importPauseMenu = EditorGUILayout.Toggle("Pause Menu", importPauseMenu);
                importCredits = EditorGUILayout.Toggle("Credits Screen", importCredits);
                importWinLose = EditorGUILayout.Toggle("Win/Lose Screens", importWinLose);
                importDialogues = EditorGUILayout.Toggle("Dialogue System", importDialogues);
                importSettings = EditorGUILayout.Toggle("Themes (Resources)", importSettings);
                importMinimap = EditorGUILayout.Toggle("Minimap System", importMinimap);
                importSamples = EditorGUILayout.Toggle("Demo Samples", importSamples);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("2. Architecture Configuration", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            selectedPlatform = (UIConfiguration.TargetPlatform)EditorGUILayout.EnumPopup("Target Platform:", selectedPlatform);
            selectedGenre = (UIConfiguration.GameGenre)EditorGUILayout.EnumPopup("Game Genre:", selectedGenre);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Import & Configure UI System", GUILayout.Height(50)))
            {
                ExecuteImport();
                CreateAndApplyConfiguration();
            }
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