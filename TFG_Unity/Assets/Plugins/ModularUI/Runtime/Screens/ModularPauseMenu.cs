using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ModularUIRuntime
{
    [Serializable]
    public class PauseButtonData
    {
        public string buttonName = "New Button";
        public ModularButton targetButton;

        [field: NonSerialized]
        public event Action OnClickAction;
    }

    public class ModularPauseMenu : MonoBehaviour
    {
        public enum MenuLayoutType
        {
            Vertical,
            Horizontal,
            Grid
        }

        [Header("Menu Content")]
        [SerializeField] private string titleText = "PAUSED";

        [Header("References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private ModularText titleComponent;

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
        [SerializeField] private List<PauseButtonData> menuButtons = new List<PauseButtonData>();

        private bool isPaused = false;

        private void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                if (titleComponent != null)
                {
                    UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(titleComponent);
                    UnityEditor.SerializedProperty prop = so.FindProperty("textContent");

                    if (prop != null && prop.stringValue != titleText)
                    {
                        prop.stringValue = titleText;
                        so.ApplyModifiedProperties();
                    }
                }

                UpdateLayout();
                UpdateButtons();
            };
#endif
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            pausePanel.SetActive(isPaused);

            if (isPaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        public void Resume()
        {
            isPaused = false;
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitToMenu(string menuSceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        private void UpdateButtons()
        {
            foreach (PauseButtonData buttonData in menuButtons)
            {
                if (buttonData.targetButton != null)
                {
                    if (buttonData.targetButton.gameObject.name != buttonData.buttonName)
                    {
                        buttonData.targetButton.gameObject.name = buttonData.buttonName;
                    }

                    ModularText textComp = buttonData.targetButton.GetComponentInChildren<ModularText>();

                    if (textComp != null)
                    {
#if UNITY_EDITOR
                        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(textComp);
                        UnityEditor.SerializedProperty prop = so.FindProperty("textContent");

                        if (prop != null && prop.stringValue != buttonData.buttonName)
                        {
                            prop.stringValue = buttonData.buttonName;
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

            if (gridLayout == null)
            {
                return;
            }

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