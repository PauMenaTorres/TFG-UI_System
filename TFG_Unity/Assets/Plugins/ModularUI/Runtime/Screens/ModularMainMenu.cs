using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ModularUIRuntime
{
    public class ModularMainMenu : ModularScreenBase
    {
        [Header("Menu Content")]
        [SerializeField] private string titleText = "MAIN MENU";
        [SerializeField] private string versionText = "v.1.0.0";

        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI titleComponent;
        [SerializeField] private TextMeshProUGUI versionComponent;

        [Header("Menu Buttons Setup")]
        [SerializeField] private List<ModularButtonData> menuButtons = new List<ModularButtonData>();

        protected override Transform GetButtonsContainer()
        {
            return buttonsContainer;
        }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ApplyLayout();
            WireButtonActions(menuButtons);
        }

        protected void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                SyncTitleText();
                SyncVersionText();
                EditorDetectButtonNames(menuButtons);
                ApplyLayout();
            };
#endif
        }

        public void UpdateTextFromChild(ModularText child, string newText)
        {
            if (titleComponent != null && child.gameObject == titleComponent.gameObject)
            {
                if (titleText != newText)
                {
                    titleText = newText;
                }
            }

            if (versionComponent != null && child.gameObject == versionComponent.gameObject)
            {
                if (versionText != newText)
                {
                    versionText = newText;
                }
            }
        }

        private void SyncTitleText()
        {
            if (titleComponent == null || titleComponent.text == titleText)
            {
                return;
            }

            titleComponent.text = titleText;
            titleComponent.SetAllDirty();

            ModularText modText = titleComponent.GetComponent<ModularText>();

            if (modText != null)
            {
                modText.UpdateTextFromExternal(titleText);
            }
        }

        private void SyncVersionText()
        {
            if (versionComponent == null || versionComponent.text == versionText)
            {
                return;
            }

            versionComponent.text = versionText;
            versionComponent.SetAllDirty();

            ModularText modVersion = versionComponent.GetComponent<ModularText>();

            if (modVersion != null)
            {
                modVersion.UpdateTextFromExternal(versionText);
            }
        }
    }
}