using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace ModularUIRuntime
{
    [Serializable]
    public struct DialogueSceneEvent
    {
        public string eventID;
        public UnityEvent sceneEvent;
    }

    public class ModularDialogueManager : MonoBehaviour
    {
        public static event Action<string> OnDialogueAction;

        [Header("Data")]
        [SerializeField] private DialogueNode startingNode;

        [Header("Scene Events (Visual Bridge)")]
        [SerializeField] private List<DialogueSceneEvent> sceneEvents = new();

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.05f;

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameTextComponent;
        [SerializeField] private TextMeshProUGUI bodyTextComponent;
        [SerializeField] private Image portraitImageComponent;

        [Header("Choices UI")]
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;

        private DialogueNode currentNode;
        private int currentLineIndex = 0;
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private string fullText;
        private Coroutine typeCoroutine;
        private List<GameObject> activeChoiceButtons = new();

        private void Start()
        {
            if (startingNode != null)
            {
                StartDialogue(startingNode);
            }
            else if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                HandleInput();
            }
        }

        public void StartDialogue(DialogueNode startNode)
        {
            currentNode = startNode;
            currentLineIndex = 0;
            isDialogueActive = true;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            DisplayCurrentLine();
        }

        private void HandleInput()
        {
            if (isTyping)
            {
                StopCoroutine(typeCoroutine);
                isTyping = false;
                UpdateTextComponent(bodyTextComponent, fullText);

                if (currentNode.dialogueLines[currentLineIndex].hasChoices)
                {
                    ShowChoices();
                }
            }
            else
            {
                DialogueLine currentLine = currentNode.dialogueLines[currentLineIndex];

                if (!currentLine.hasChoices)
                {
                    if (currentLine.hasAction)
                    {
                        TriggerEvent(currentLine.actionID);
                    }

                    if (currentLine.endDialogue)
                    {
                        EndDialogue();
                    }
                    else if (currentLineIndex < currentNode.dialogueLines.Count - 1)
                    {
                        currentLineIndex++;
                        DisplayCurrentLine();
                    }
                    else
                    {
                        EndDialogue();
                    }
                }
            }
        }

        private void DisplayCurrentLine()
        {
            ClearChoices();
            DialogueLine line = currentNode.dialogueLines[currentLineIndex];
            fullText = line.dialogueText;

            UpdateTextComponent(nameTextComponent, line.characterName);

            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
            }

            typeCoroutine = StartCoroutine(TypeText(fullText));

            if (portraitImageComponent != null)
            {
                if (line.characterPortrait != null)
                {
                    portraitImageComponent.sprite = line.characterPortrait;
                    portraitImageComponent.gameObject.SetActive(true);
                }
                else
                {
                    portraitImageComponent.gameObject.SetActive(false);
                }
            }
        }

        private System.Text.StringBuilder textBuilder = new System.Text.StringBuilder();
        private ModularText bodyModTextCache;
        private Stack<GameObject> choiceButtonPool = new Stack<GameObject>();

        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            bodyTextComponent.text = "";
            textBuilder.Clear();

            if (bodyModTextCache == null)
            {
                bodyTextComponent.TryGetComponent(out bodyModTextCache);
            }

            foreach (char letter in text)
            {
                textBuilder.Append(letter);
                string currentText = textBuilder.ToString();
                bodyTextComponent.text = currentText;

                if (bodyModTextCache != null)
                {
                    bodyModTextCache.UpdateTextFromExternal(currentText);
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;

            if (currentNode.dialogueLines[currentLineIndex].hasChoices)
            {
                ShowChoices();
            }
        }

        private void ShowChoices()
        {
            DialogueLine currentLine = currentNode.dialogueLines[currentLineIndex];

            if (currentLine.choices != null && currentLine.choices.Count > 0)
            {
                foreach (DialogueChoice choice in currentLine.choices)
                {
                    GameObject buttonObj = GetChoiceButton();
                    buttonObj.transform.SetParent(choicesContainer, false);
                    buttonObj.transform.SetAsLastSibling();
                    buttonObj.SetActive(true);
                    
                    activeChoiceButtons.Add(buttonObj);

                    if (buttonObj.TryGetComponent(out ModularButton modBtn))
                    {
                        modBtn.UpdateButtonText(choice.choiceText);
                    }

                    if (buttonObj.TryGetComponent(out Button btn))
                    {
                        btn.onClick.RemoveAllListeners();
                        DialogueChoice capturedChoice = choice;
                        btn.onClick.AddListener(() => OnChoiceSelected(capturedChoice));
                    }
                }
            }
        }

        private GameObject GetChoiceButton()
        {
            if (choiceButtonPool.Count > 0)
            {
                return choiceButtonPool.Pop();
            }
            return Instantiate(choiceButtonPrefab);
        }

        private void OnChoiceSelected(DialogueChoice choice)
        {
            if (choice.hasAction)
            {
                TriggerEvent(choice.actionID);
            }

            if (choice.actionType == ChoiceActionType.Continue)
            {
                if (currentLineIndex < currentNode.dialogueLines.Count - 1)
                {
                    currentLineIndex++;
                    DisplayCurrentLine();
                }
                else
                {
                    EndDialogue();
                }
            }
            else if (choice.actionType == ChoiceActionType.JumpToLine)
            {
                if (choice.targetLineIndex >= 0 && choice.targetLineIndex < currentNode.dialogueLines.Count)
                {
                    currentLineIndex = choice.targetLineIndex;
                    DisplayCurrentLine();
                }
                else
                {
                    EndDialogue();
                }
            }
            else if (choice.actionType == ChoiceActionType.JumpToNode)
            {
                if (choice.nextNode != null)
                {
                    StartDialogue(choice.nextNode);
                }
                else
                {
                    EndDialogue();
                }
            }
            else if (choice.actionType == ChoiceActionType.EndDialogue)
            {
                EndDialogue();
            }
        }

        private void TriggerEvent(string idToTrigger)
        {
            if (string.IsNullOrEmpty(idToTrigger))
            {
                return;
            }

            OnDialogueAction?.Invoke(idToTrigger);

            if (sceneEvents != null)
            {
                foreach (DialogueSceneEvent evt in sceneEvents)
                {
                    if (evt.eventID == idToTrigger)
                    {
                        evt.sceneEvent?.Invoke();
                    }
                }
            }
        }

        private void ClearChoices()
        {
            foreach (GameObject btn in activeChoiceButtons)
            {
                btn.SetActive(false);
                choiceButtonPool.Push(btn);
            }

            activeChoiceButtons.Clear();
        }

        private void UpdateTextComponent(TextMeshProUGUI component, string text)
        {
            if (component != null)
            {
                component.text = text;

                if (component.TryGetComponent(out ModularText modText))
                {
                    modText.UpdateTextFromExternal(text);
                }
            }
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            ClearChoices();

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
}