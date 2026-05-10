using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ModularUIRuntime
{
    public class ModularWinLoseMenu : MonoBehaviour
    {
        public enum MenuLayoutType
        {
            Vertical,
            Horizontal,
            Grid
        }

        [Header("Menu Content")]
        [SerializeField] private string winMessage = "VICTORY";
        [SerializeField] private string loseMessage = "DEFEAT";

        [Header("References")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private ModularText messageComponent;

        [Header("Layout Settings")]
        [SerializeField] private MenuLayoutType layoutType = MenuLayoutType.Vertical;
        [SerializeField] private int gridColumns = 2;
        [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
        [SerializeField] private Vector2 cellSize = new Vector2(200f, 50f);
        [SerializeField] private int paddingLeft = 0;
        [SerializeField] private int paddingRight = 0;
        [SerializeField] private int paddingTop = 0;
        [SerializeField] private int paddingBottom = 0;

        [Header("Buttons Configuration")]
        [SerializeField] private List<WinLoseButtonData> actionButtons = new List<WinLoseButtonData>();

        private void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                if (messageComponent != null)
                {
                    UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(messageComponent);
                    UnityEditor.SerializedProperty prop = so.FindProperty("textContent");

                    if (prop != null && prop.stringValue != winMessage)
                    {
                        prop.stringValue = winMessage;
                        so.ApplyModifiedProperties();
                    }
                }

                UpdateLayout();
                UpdateButtons();
            };
#endif
        }

        public void ShowWin()
        {
            winPanel.SetActive(true);
            losePanel.SetActive(false);

            if (messageComponent != null)
            {
                TextMeshProUGUI txt = messageComponent.GetComponent<TextMeshProUGUI>();

                if (txt != null)
                {
                    txt.text = winMessage;
                }
            }
        }

        public void ShowLose()
        {
            winPanel.SetActive(false);
            losePanel.SetActive(true);

            if (messageComponent != null)
            {
                TextMeshProUGUI txt = messageComponent.GetComponent<TextMeshProUGUI>();

                if (txt != null)
                {
                    txt.text = loseMessage;
                }
            }
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Continue(string nextSceneName)
        {
            SceneManager.LoadScene(nextSceneName);
        }

        private void UpdateButtons()
        {
            foreach (WinLoseButtonData btn in actionButtons)
            {
                if (btn.targetButton != null)
                {
                    if (btn.targetButton.gameObject.name != btn.buttonName)
                    {
                        btn.targetButton.gameObject.name = btn.buttonName;
                    }

                    ModularText textComp = btn.targetButton.GetComponentInChildren<ModularText>();

                    if (textComp != null)
                    {
#if UNITY_EDITOR
                        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(textComp);
                        UnityEditor.SerializedProperty prop = so.FindProperty("textContent");

                        if (prop != null && prop.stringValue != btn.buttonName)
                        {
                            prop.stringValue = btn.buttonName;
                            so.ApplyModifiedProperties();
                        }
#endif
                    }
                }
            }
        }

        private void UpdateLayout()
        {
            if (buttonsContainer == null)
            {
                return;
            }

            GridLayoutGroup gridLayout = buttonsContainer.GetComponent<GridLayoutGroup>();

            if (gridLayout != null)
            {
                gridLayout.spacing = spacing;
                gridLayout.cellSize = cellSize;
                gridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);

                if (layoutType == MenuLayoutType.Vertical)
                {
                    gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    gridLayout.constraintCount = 1;
                }

                if (layoutType == MenuLayoutType.Horizontal)
                {
                    gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    gridLayout.constraintCount = 1;
                }

                if (layoutType == MenuLayoutType.Grid)
                {
                    gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    gridLayout.constraintCount = gridColumns;
                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gridLayout);
#endif
            }
        }
    }
}