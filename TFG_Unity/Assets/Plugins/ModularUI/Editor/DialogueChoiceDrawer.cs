using UnityEngine;
using UnityEditor;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [CustomPropertyDrawer(typeof(DialogueChoice))]
    public class DialogueChoiceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty choiceText = property.FindPropertyRelative("choiceText");
            SerializedProperty actionType = property.FindPropertyRelative("actionType");
            SerializedProperty targetLineIndex = property.FindPropertyRelative("targetLineIndex");
            SerializedProperty nextNode = property.FindPropertyRelative("nextNode");
            SerializedProperty hasAction = property.FindPropertyRelative("hasAction");
            SerializedProperty actionID = property.FindPropertyRelative("actionID");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

            int limit = property.serializedObject.FindProperty("choiceCharacterLimit").intValue;

            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, lineHeight);
            Rect fieldRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, lineHeight);

            EditorGUI.LabelField(labelRect, choiceText.displayName);

            EditorGUI.BeginChangeCheck();
            string newText = GUI.TextField(fieldRect, choiceText.stringValue, limit, EditorStyles.textField);
            if (EditorGUI.EndChangeCheck())
            {
                choiceText.stringValue = newText;
                property.serializedObject.ApplyModifiedProperties();
            }

            rect.y += lineHeight + spacing;
            EditorGUI.PropertyField(rect, actionType);
            rect.y += lineHeight + spacing;

            ChoiceActionType type = (ChoiceActionType)actionType.enumValueIndex;

            if (type == ChoiceActionType.JumpToLine)
            {
                EditorGUI.PropertyField(rect, targetLineIndex);
                rect.y += lineHeight + spacing;
            }
            else if (type == ChoiceActionType.JumpToNode)
            {
                EditorGUI.PropertyField(rect, nextNode);
                rect.y += lineHeight + spacing;
            }

            EditorGUI.PropertyField(rect, hasAction);
            rect.y += lineHeight + spacing;

            if (hasAction.boolValue)
            {
                EditorGUI.PropertyField(rect, actionID);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty actionType = property.FindPropertyRelative("actionType");
            SerializedProperty hasAction = property.FindPropertyRelative("hasAction");

            float height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;

            ChoiceActionType type = (ChoiceActionType)actionType.enumValueIndex;

            if (type == ChoiceActionType.Continue || type == ChoiceActionType.EndDialogue)
            {
                height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (hasAction.boolValue)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}