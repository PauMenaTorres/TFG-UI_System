using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public abstract class ModularScreenBase : MonoBehaviour
    {
        [Header("Layout Settings")]
        [SerializeField] protected MenuLayoutType layoutType = MenuLayoutType.Vertical;
        [SerializeField] protected int gridColumns = 2;
        [SerializeField] protected Vector2 spacing = new Vector2(10f, 10f);
        [SerializeField] protected Vector2 cellSize = new Vector2(200f, 50f);
        [SerializeField] protected int paddingLeft = 0;
        [SerializeField] protected int paddingRight = 0;
        [SerializeField] protected int paddingTop = 0;
        [SerializeField] protected int paddingBottom = 0;

        [Header("References")]
        [SerializeField] protected Transform buttonsContainer;

        private GridLayoutGroup cachedGridLayout;

        protected abstract Transform GetButtonsContainer();

        protected void ApplyLayout()
        {
            Transform container = GetButtonsContainer();

            if (container == null)
            {
                return;
            }

            if (cachedGridLayout == null)
            {
                cachedGridLayout = container.GetComponent<GridLayoutGroup>();
            }

            if (cachedGridLayout == null)
            {
                return;
            }

            bool changed = false;

            if (cachedGridLayout.spacing != spacing)
            {
                cachedGridLayout.spacing = spacing;
                changed = true;
            }

            if (cachedGridLayout.cellSize != cellSize)
            {
                cachedGridLayout.cellSize = cellSize;
                changed = true;
            }

            RectOffset pad = cachedGridLayout.padding;

            if (pad == null || pad.left != paddingLeft || pad.right != paddingRight || pad.top != paddingTop || pad.bottom != paddingBottom)
            {
                cachedGridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
                changed = true;
            }

            GridLayoutGroup.Constraint targetConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
            int targetCount = 1;

            switch (layoutType)
            {
                case MenuLayoutType.Vertical:
                    targetConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    targetCount = 1;
                    break;
                case MenuLayoutType.Horizontal:
                    targetConstraint = GridLayoutGroup.Constraint.FixedRowCount;
                    targetCount = 1;
                    break;
                case MenuLayoutType.Grid:
                    targetConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    targetCount = gridColumns;
                    break;
            }

            if (cachedGridLayout.constraint != targetConstraint)
            {
                cachedGridLayout.constraint = targetConstraint;
                changed = true;
            }

            if (cachedGridLayout.constraintCount != targetCount)
            {
                cachedGridLayout.constraintCount = targetCount;
                changed = true;
            }

#if UNITY_EDITOR
            if (changed)
            {
                UnityEditor.EditorUtility.SetDirty(cachedGridLayout);
            }
#endif
        }

        protected void WireButtonActions(IList<ModularButtonData> buttons)
        {
            foreach (ModularButtonData data in buttons)
            {
                if (data.targetButton == null)
                {
                    continue;
                }

                Button btn = data.targetButton.GetComponent<Button>();

                if (btn == null)
                {
                    continue;
                }

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => data.OnClick?.Invoke());
            }
        }

        protected void SetTextRuntime(ModularText component, string text)
        {
            if (component == null)
            {
                return;
            }

            TextMeshProUGUI tmp = component.GetComponent<TextMeshProUGUI>();

            if (tmp != null)
            {
                tmp.text = text;
            }
        }

#if UNITY_EDITOR
        protected void EditorUpdateModularText(ModularText component, string text)
        {
            if (component == null)
            {
                return;
            }

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(component);
            UnityEditor.SerializedProperty prop = so.FindProperty("textContent");

            if (prop != null && prop.stringValue != text)
            {
                prop.stringValue = text;
                so.ApplyModifiedProperties();
            }
        }

        protected void EditorSyncButtonNames(IList<ModularButtonData> buttons)
        {
            foreach (ModularButtonData buttonData in buttons)
            {
                if (buttonData.targetButton == null)
                {
                    continue;
                }

                if (buttonData.targetButton.gameObject.name != buttonData.buttonName)
                {
                    buttonData.targetButton.gameObject.name = buttonData.buttonName;
                }

                ModularText textComp = buttonData.targetButton.GetComponentInChildren<ModularText>();

                if (textComp != null)
                {
                    EditorUpdateModularText(textComp, buttonData.buttonName);
                }
            }
        }

        protected void EditorDetectButtonNames(IList<ModularButtonData> buttons)
        {
            foreach (ModularButtonData data in buttons)
            {
                if (data.targetButton == null)
                {
                    continue;
                }

                TextMeshProUGUI textComp = data.targetButton.GetComponentInChildren<TextMeshProUGUI>();
                string detectedName = data.targetButton.name;

                if (textComp != null)
                {
                    detectedName = textComp.text;
                }

                if (data.buttonName != detectedName)
                {
                    data.buttonName = detectedName;
                }
            }
        }
#endif
    }
}
