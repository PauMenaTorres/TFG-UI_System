using UnityEngine;
using UnityEditor;

namespace ModularUIEditor
{
    public class ModularUIWizard : EditorWindow
    {
        public enum TargetPlatform
        {
            DESKTOP,
            MOBILE,
            VR
        }

        public enum GameGenre
        {
            SHOOTER,
            FPS,
            ACTION_ADVENTURE,
            RPG,
            MOBA,
            SANDBOX,
            STRATEGY,
            RACING,
            PUZZLE,
            SPORT,
            SIMULATOR,
            FIGHTING
        }

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

        private TargetPlatform selectedPlatform = TargetPlatform.DESKTOP;
        private GameGenre selectedGenre = GameGenre.RPG;
        private ProjectStatus currentStatus = ProjectStatus.NEW_PROJECT;
        private ImportScope currentScope = ImportScope.FULL_SYSTEM;

        private bool importBaseUI = true;
        private bool importHUD = true;
        private bool importMainMenu = true;
        private bool importDialogues = true;
        private bool importSettings = true;
        private bool importMinimap = true;

        private string targetPath = "Assets/ModularUI";

        [MenuItem("Tools/Modular UI/Setup Wizard")]
        public static void ShowWindow()
        {
            ModularUIWizard window = GetWindow<ModularUIWizard>("UI Wizard");
            window.minSize = new Vector2(400, 580);
            window.Show();
        }

        private string GetRootPath()
        {
            if (System.IO.Directory.Exists("Packages/com.pau.modularui"))
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
                importBaseUI = EditorGUILayout.Toggle("Base UI", importBaseUI);
                importMainMenu = EditorGUILayout.Toggle("Main Menu Template", importMainMenu);
                importHUD = EditorGUILayout.Toggle("HUD Template", importHUD);
                importDialogues = EditorGUILayout.Toggle("Dialogue System", importDialogues);
                importSettings = EditorGUILayout.Toggle("Themes (Resources)", importSettings);
                importMinimap = EditorGUILayout.Toggle("Minimap System", importMinimap);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("2. Architecture Configuration", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            selectedPlatform = (TargetPlatform)EditorGUILayout.EnumPopup("Target Platform:", selectedPlatform);
            selectedGenre = (GameGenre)EditorGUILayout.EnumPopup("Game Genre:", selectedGenre);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Import UI Files to Project", GUILayout.Height(50)))
            {
                ExecuteImport();
            }
        }

        private void ExecuteImport()
        {
            if (!AssetDatabase.IsValidFolder(targetPath))
            {
                AssetDatabase.CreateFolder("Assets", "ModularUI");
            }

            if (currentScope == ImportScope.FULL_SYSTEM)
            {
                CopyAssetItem("BaseUI", "BaseUI");
                CopyAssetItem("Templates", "Templates");
                CopyAssetItem("Dialogues", "Dialogues");
                CopyAssetItem("Resources", "Resources");
                CopyAssetItem("Minimap", "Minimap");
            }

            if (currentScope != ImportScope.FULL_SYSTEM)
            {
                if (importBaseUI)
                {
                    CopyAssetItem("BaseUI", "BaseUI");
                }

                if (importMainMenu)
                {
                    CopyAssetItem("Templates/MainMenu.prefab", "Templates/MainMenu.prefab");
                }

                if (importHUD)
                {
                    CopyAssetItem("Templates/HUD.prefab", "Templates/HUD.prefab");
                }

                if (importDialogues)
                {
                    CopyAssetItem("Dialogues", "Dialogues");
                }

                if (importSettings)
                {
                    CopyAssetItem("Resources", "Resources");
                }

                if (importMinimap)
                {
                    CopyAssetItem("Minimap", "Minimap");
                }
            }

            AssetDatabase.Refresh();
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