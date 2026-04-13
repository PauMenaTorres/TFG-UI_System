using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModularUIRuntime
{
    [Serializable]
    public class DialogueLine
    {
        public string characterName;

        [TextArea(3, 10)]
        public string dialogueText;

        public Sprite characterPortrait;
        public AudioClip voiceAudioClip;
    }

    [CreateAssetMenu(fileName = "NewDialogueData", menuName = "Modular UI/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("Dialogue Configuration")]
        [Tooltip("Recommended limit: 140 characters to prevent UI overflow.")]
        public int characterLimit = 140;

        public List<DialogueLine> dialogueLines = new List<DialogueLine>();

        private void OnValidate()
        {
            if (dialogueLines == null) return;

            foreach (var line in dialogueLines)
            {
                if (line.dialogueText != null && line.dialogueText.Length > characterLimit)
                {
                    line.dialogueText = line.dialogueText.Substring(0, characterLimit);
                    Debug.LogWarning("ModularUI: Dialogue text was truncated because it exceeded the character limit.");
                }
            }
        }
    }
}