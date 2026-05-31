using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModularUIRuntime
{
    public class ModularPauseMenu : ModularScreenBase
    {
        [Header("Menu Content")]
        [SerializeField] private string titleText = "PAUSED";

        [Header("Pause References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private ModularText titleComponent;

        [Header("Buttons Configuration")]
        [SerializeField] private List<ModularButtonData> menuButtons = new List<ModularButtonData>();

        private bool isPaused;

        public bool IsPaused => isPaused;
        public bool IsPanelActive => pausePanel != null && pausePanel.activeSelf;

        protected override Transform GetButtonsContainer()
        {
            return buttonsContainer;
        }

        private void Awake()
        {
            WireButtonActions(menuButtons);
        }


        public void TogglePause()
        {
            isPaused = !isPaused;
            pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
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

        private void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                EditorUpdateModularText(titleComponent, titleText);
                ApplyLayout();
                EditorSyncButtonNames(menuButtons);
            };
#endif
        }
    }
}