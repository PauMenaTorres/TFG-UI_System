using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModularUIRuntime
{
    public enum ChoiceActionType
    {
        Continue,
        JumpToLine,
        JumpToNode,
        EndDialogue
    }

    [Serializable]
    public struct DialogueChoice
    {
        public string choiceText;

        public ChoiceActionType actionType;
        public int targetLineIndex;
        public DialogueNode nextNode;

        public bool hasAction;
        public string actionID;
    }

    [Serializable]
    public struct DialogueLine
    {
        public string characterName;
        public Sprite characterPortrait;
        public AudioClip voiceAudioClip;

        [TextArea(3, 10)]
        public string dialogueText;

        public bool hasAction;
        public string actionID;

        public bool endDialogue;
        public bool hasChoices;
        public List<DialogueChoice> choices;
    }

    [CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Modular UI/Dialogue Node")]

    public class DialogueNode : ScriptableObject
    {
        [Header("Settings")]
        public int characterLimit = 140;
        public int choiceCharacterLimit = 25;

        [Header("Dialogue Sequence")]
        public List<DialogueLine> dialogueLines = new List<DialogueLine>();

        private void OnValidate()
        {
            if (dialogueLines == null) return;

            for (int i = 0; i < dialogueLines.Count; i++)
            {
                DialogueLine line = dialogueLines[i];
                bool lineModified = false;

                if (line.dialogueText != null && line.dialogueText.Length > characterLimit)
                {
                    line.dialogueText = line.dialogueText.Substring(0, characterLimit);
                    lineModified = true;
                }

                if (line.choices != null)
                {
                    for (int j = 0; j < line.choices.Count; j++)
                    {
                        DialogueChoice choice = line.choices[j];
                        if (choice.choiceText != null && choice.choiceText.Length > choiceCharacterLimit)
                        {
                            choice.choiceText = choice.choiceText.Substring(0, choiceCharacterLimit);
                            line.choices[j] = choice;
                            lineModified = true;
                        }
                    }
                }

                if (lineModified)
                {
                    dialogueLines[i] = line;
                }
            }
        }
    }
    
}