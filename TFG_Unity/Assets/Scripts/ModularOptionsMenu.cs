using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    public class ModularOptionsMenu : MonoBehaviour
    {
        public enum MenuLayoutType
        {
            Vertical,
            Horizontal,
            Grid
        }

        [Header("Options Content")]
        [SerializeField] private string titleText = "OPTIONS";

        [Header("References")]
        [SerializeField] private GameObject tabButtonsContainer;
        [SerializeField] private TextMeshProUGUI titleComponent;

        [Header("Layout Settings")]
        [SerializeField] private MenuLayoutType layoutType = MenuLayoutType.Horizontal;
        [SerializeField] private int gridColumns = 3;
        [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
        [SerializeField] private Vector2 cellSize = new Vector2(150f, 50f);

        [SerializeField] private int paddingLeft = 0;
        [SerializeField] private int paddingRight = 0;
        [SerializeField] private int paddingTop = 0;
        [SerializeField] private int paddingBottom = 0;

        [Header("Tabs Setup")]
        [SerializeField] private List<OptionTabButtonData> optionTabs = new List<OptionTabButtonData>();

        private GridLayoutGroup gridLayout;
        private string defaultTitle;

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            defaultTitle = titleText;
            UpdateLayout();

            for (int i = 0; i < optionTabs.Count; i++)
            {
                OptionTabButtonData tabData = optionTabs[i];
                if (tabData.tabButton != null && tabData.sectionPanel != null)
                {
                    Button menuBtn = tabData.tabButton.GetComponent<Button>();
                    if (menuBtn != null)
                    {
                        menuBtn.onClick.RemoveAllListeners();
                        menuBtn.onClick.AddListener(() => OpenTab(tabData.sectionPanel));
                    }
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
                if (tab.sectionPanel != null)
                {
                    bool isOpen = tab.sectionPanel == panelToOpen;
                    tab.sectionPanel.SetActive(isOpen);

                    if (isOpen)
                    {
                        UpdateTitleUI(tab.tabName.ToUpper());
                    }
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

        private void UpdateTitleUI(string newTitle)
        {
            if (titleComponent != null)
            {
                titleComponent.text = newTitle;
                titleComponent.SetAllDirty();

                ModularText modText = titleComponent.GetComponent<ModularText>();
                if (modText != null)
                {
                    modText.UpdateTextFromExternal(newTitle);
                }
            }
        }

        protected void OnValidate()
        {
            if (titleComponent != null)
            {
                titleComponent.text = titleText;
                titleComponent.SetAllDirty();

                ModularText modText = titleComponent.GetComponent<ModularText>();
                if (modText != null)
                {
                    modText.UpdateTextFromExternal(titleText);
                }
            }

            if (optionTabs != null)
            {
                foreach (OptionTabButtonData data in optionTabs)
                {
                    if (data.tabButton != null)
                    {
                        TextMeshProUGUI textComp = data.tabButton.GetComponentInChildren<TextMeshProUGUI>();
                        string detectedName = textComp != null ? textComp.text : data.tabButton.name;

                        if (data.tabName != detectedName)
                        {
                            data.tabName = detectedName;
                        }
                    }
                }
            }

            UpdateLayout();
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

        public void UpdateLayout()
        {
            if (tabButtonsContainer == null)
            {
                return;
            }

            if (gridLayout == null)
            {
                gridLayout = tabButtonsContainer.GetComponent<GridLayoutGroup>();
            }

            if (gridLayout == null)
            {
                return;
            }

            if (gridLayout.spacing != spacing)
            {
                gridLayout.spacing = spacing;
            }

            if (gridLayout.cellSize != cellSize)
            {
                gridLayout.cellSize = cellSize;
            }

            if (gridLayout.padding == null ||
                gridLayout.padding.left != paddingLeft ||
                gridLayout.padding.right != paddingRight ||
                gridLayout.padding.top != paddingTop ||
                gridLayout.padding.bottom != paddingBottom)
            {
                gridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
            }

            GridLayoutGroup.Constraint newConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            int newCount = 1;

            switch (layoutType)
            {
                case MenuLayoutType.Vertical:
                    newConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    newCount = 1;
                    break;
                case MenuLayoutType.Horizontal:
                    newConstraint = GridLayoutGroup.Constraint.FixedRowCount;
                    newCount = 1;
                    break;
                case MenuLayoutType.Grid:
                    newConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    newCount = gridColumns;
                    break;
            }

            if (gridLayout.constraint != newConstraint)
            {
                gridLayout.constraint = newConstraint;
            }

            if (gridLayout.constraintCount != newCount)
            {
                gridLayout.constraintCount = newCount;
            }
        }
    }
}