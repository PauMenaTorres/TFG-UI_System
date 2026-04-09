using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace ModularUIRuntime
{
    public class ModularDialogueManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private DialogueData currentDialogue;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.05f;

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameTextComponent;
        [SerializeField] private TextMeshProUGUI bodyTextComponent;
        [SerializeField] private Image portraitImageComponent;

        private int currentLineIndex = 0;
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private string fullText;
        private Coroutine typeCoroutine;

        private void Start()
        {
            if (currentDialogue != null)
            {
                StartDialogue(currentDialogue);
            }
            else
            {
                if (dialoguePanel != null) dialoguePanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                HandleInput();
            }
        }

        public void StartDialogue(DialogueData newDialogue)
        {
            currentDialogue = newDialogue;
            currentLineIndex = 0;
            isDialogueActive = true;

            if (dialoguePanel != null) dialoguePanel.SetActive(true);

            DisplayCurrentLine();
        }

        private void HandleInput()
        {
            if (isTyping)
            {
                StopCoroutine(typeCoroutine);
                isTyping = false;
                UpdateTextComponent(bodyTextComponent, fullText);
            }
            else
            {
                AdvanceDialogue();
            }
        }

        public void AdvanceDialogue()
        {
            currentLineIndex++;

            if (currentLineIndex < currentDialogue.dialogueLines.Count)
            {
                DisplayCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }

        private void DisplayCurrentLine()
        {
            DialogueLine line = currentDialogue.dialogueLines[currentLineIndex];
            fullText = line.dialogueText;

            UpdateTextComponent(nameTextComponent, line.characterName);

            if (typeCoroutine != null) StopCoroutine(typeCoroutine);
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

        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            bodyTextComponent.text = "";

            foreach (char letter in text.ToCharArray())
            {
                bodyTextComponent.text += letter;

                ModularText modText = bodyTextComponent.GetComponent<ModularText>();
                if (modText != null) modText.UpdateTextFromExternal(bodyTextComponent.text);

                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }

        private void UpdateTextComponent(TextMeshProUGUI component, string text)
        {
            if (component != null)
            {
                component.text = text;
                ModularText modText = component.GetComponent<ModularText>();
                if (modText != null) modText.UpdateTextFromExternal(text);
            }
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
    }
}