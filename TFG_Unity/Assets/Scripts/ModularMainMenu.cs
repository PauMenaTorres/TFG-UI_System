using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [Serializable]
    public class MenuButtonData
    {
        public string buttonName = "New Button";
        public ModularButton targetButton;
        public UnityEvent OnClickAction;
    }

    public class ModularMainMenu : MonoBehaviour
    {
        public enum MenuLayoutType
        {
            Vertical,
            Horizontal,
            Grid
        }

        [Header("Menu Content")]
        [SerializeField] private string titleText = "MAIN MENU";
        [SerializeField] private string versionText = "v.1.0.0";

        [Header("References")]
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private TextMeshProUGUI titleComponent;
        [SerializeField] private TextMeshProUGUI versionComponent;

        [Header("Layout Settings")]
        [SerializeField] private MenuLayoutType layoutType = MenuLayoutType.Vertical;
        [SerializeField] private int gridColumns = 2;
        [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
        [SerializeField] private Vector2 cellSize = new Vector2(200f, 50f);

        [SerializeField] private int paddingLeft = 0;
        [SerializeField] private int paddingRight = 0;
        [SerializeField] private int paddingTop = 0;
        [SerializeField] private int paddingBottom = 0;

        [Header("Menu Buttons Setup")]
        [SerializeField] private List<MenuButtonData> menuButtons = new List<MenuButtonData>();

        private GridLayoutGroup gridLayout;

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            UpdateLayout();

            foreach (MenuButtonData data in menuButtons)
            {
                if (data.targetButton != null)
                {
                    Button menuBtn = data.targetButton.GetComponent<Button>();
                    if (menuBtn != null)
                    {
                        menuBtn.onClick.RemoveAllListeners();
                        menuBtn.onClick.AddListener(() => data.OnClickAction?.Invoke());
                    }
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

            if (versionComponent != null)
            {
                versionComponent.text = versionText;
                versionComponent.SetAllDirty();

                ModularText modVersion = versionComponent.GetComponent<ModularText>();
                if (modVersion != null)
                {
                    modVersion.UpdateTextFromExternal(versionText);
                }
            }

            if (menuButtons != null)
            {
                foreach (MenuButtonData data in menuButtons)
                {
                    if (data.targetButton != null)
                    {
                        TextMeshProUGUI textComp = data.targetButton.GetComponentInChildren<TextMeshProUGUI>();
                        string detectedName = textComp != null ? textComp.text : data.targetButton.name;

                        if (data.buttonName != detectedName)
                        {
                            data.buttonName = detectedName;
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

            if (versionComponent != null && child.gameObject == versionComponent.gameObject)
            {
                if (versionText != newText)
                {
                    versionText = newText;
                }
            }
        }

        public void UpdateLayout()
        {
            if (buttonsContainer == null)
            {
                return;
            }

            if (gridLayout == null)
            {
                gridLayout = buttonsContainer.GetComponent<GridLayoutGroup>();
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