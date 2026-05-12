using System.Collections.Generic;
using UnityEngine;

namespace ModularUIRuntime
{
    public class ModularWinLoseMenu : ModularScreenBase
    {
        [Header("Menu Content")]
        [SerializeField] private string winMessage = "VICTORY";
        [SerializeField] private string loseMessage = "DEFEAT";
        [TextArea(3, 10)]
        [SerializeField] private string statsPlaceholder = "Score: 000\nTime: 00:00";

        [Header("WinLose References")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private ModularText messageComponent;
        [SerializeField] private ModularText statsComponent;

        [Header("Buttons Configuration")]
        [SerializeField] private List<ModularButtonData> actionButtons = new List<ModularButtonData>();

        protected override Transform GetButtonsContainer()
        {
            return buttonsContainer;
        }

        private void Awake()
        {
            WireButtonActions(actionButtons);
        }


        public void ShowResult(bool isWin, string stats = "")
        {
            if (mainPanel != null)
            {
                mainPanel.SetActive(true);
            }

            SetTextRuntime(messageComponent, isWin ? winMessage : loseMessage);

            if (!string.IsNullOrEmpty(stats))
            {
                SetTextRuntime(statsComponent, stats);
            }
        }

        public void ShowWin(string stats = "")
        {
            ShowResult(true, stats);
        }

        public void ShowLose(string stats = "")
        {
            ShowResult(false, stats);
        }

        public void UpdateStatistics(string stats)
        {
            SetTextRuntime(statsComponent, stats);
        }

        public void Hide()
        {
            if (mainPanel != null)
            {
                mainPanel.SetActive(false);
            }
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

                EditorUpdateModularText(messageComponent, winMessage);
                EditorUpdateModularText(statsComponent, statsPlaceholder);
                ApplyLayout();
                EditorSyncButtonNames(actionButtons);
            };
#endif
        }
    }
}