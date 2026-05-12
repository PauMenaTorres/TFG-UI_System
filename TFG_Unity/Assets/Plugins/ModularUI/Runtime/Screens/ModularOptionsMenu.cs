using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [Serializable]
    public class OptionTabButtonData
    {
        public string tabName = "New Tab";
        public ModularButton tabButton;
        public GameObject sectionPanel;
    }

    public class ModularOptionsMenu : ModularScreenBase
    {
        [Header("Options Content")]
        [SerializeField] private string titleText = "OPTIONS";

        [Header("Tab References")]
        [SerializeField] private GameObject tabButtonsContainer;
        [SerializeField] private TextMeshProUGUI titleComponent;

        [Header("Tabs Setup")]
        [SerializeField] private List<OptionTabButtonData> optionTabs = new List<OptionTabButtonData>();

        private string defaultTitle;

        protected override Transform GetButtonsContainer()
        {
            if (tabButtonsContainer != null)
            {
                return tabButtonsContainer.transform;
            }

            return null;
        }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            defaultTitle = titleText;
            ApplyLayout();

            for (int i = 0; i < optionTabs.Count; i++)
            {
                OptionTabButtonData tabData = optionTabs[i];

                if (tabData.tabButton == null || tabData.sectionPanel == null)
                {
                    continue;
                }

                Button menuBtn = tabData.tabButton.GetComponent<Button>();

                if (menuBtn != null)
                {
                    menuBtn.onClick.RemoveAllListeners();
                    menuBtn.onClick.AddListener(() => OpenTab(tabData.sectionPanel));
                }
            }

            ShowTabButtons();
        }

        private void OnEnable()
        {
            if (Application.isPlaying && !string.IsNullOrEmpty(defaultTitle))
            {
                ShowTabButtons();
            }
        }

        public void OpenTab(GameObject panelToOpen)
        {
            if (tabButtonsContainer != null)
            {
                tabButtonsContainer.SetActive(false);
            }

            foreach (OptionTabButtonData tab in optionTabs)
            {
                if (tab.sectionPanel == null)
                {
                    continue;
                }

                bool isOpen = tab.sectionPanel == panelToOpen;
                tab.sectionPanel.SetActive(isOpen);

                if (isOpen)
                {
                    UpdateTitleUI(tab.tabName.ToUpper());
                }
            }
        }

        public void ShowTabButtons()
        {
            foreach (OptionTabButtonData tab in optionTabs)
            {
                if (tab.sectionPanel != null)
                {
                    tab.sectionPanel.SetActive(false);
                }
            }

            if (tabButtonsContainer != null)
            {
                tabButtonsContainer.SetActive(true);
            }

            UpdateTitleUI(defaultTitle);
        }

        public void SetTitleText(string newTitle)
        {
            UpdateTitleUI(newTitle);
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
        }

        private void UpdateTitleUI(string newTitle)
        {
            if (titleComponent == null)
            {
                return;
            }

            titleComponent.text = newTitle;
            titleComponent.SetAllDirty();

            ModularText modText = titleComponent.GetComponent<ModularText>();

            if (modText != null)
            {
                modText.UpdateTextFromExternal(newTitle);
            }
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
                DetectTabNames();
                ApplyLayout();
            };
#endif
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

        private void DetectTabNames()
        {
            if (optionTabs == null)
            {
                return;
            }

            foreach (OptionTabButtonData data in optionTabs)
            {
                if (data.tabButton == null)
                {
                    continue;
                }

                TextMeshProUGUI textComp = data.tabButton.GetComponentInChildren<TextMeshProUGUI>();
                string detectedName = data.tabButton.name;

                if (textComp != null)
                {
                    detectedName = textComp.text;
                }

                if (data.tabName != detectedName)
                {
                    data.tabName = detectedName;
                }
            }
        }
    }
}