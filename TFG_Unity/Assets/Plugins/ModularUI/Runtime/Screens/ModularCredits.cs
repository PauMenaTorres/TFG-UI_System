using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ModularUIRuntime
{
    public class ModularCredits : MonoBehaviour
    {
        [Header("Credits Settings")]
        [SerializeField] private float scrollSpeed = 50f;
        [SerializeField] private bool autoScrollOnEnable = true;
        [TextArea(10, 20)]
        [SerializeField] private string creditsText = "CREDITS";

        [Header("References")]
        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private TextMeshProUGUI creditsTextComponent;

        [Header("Events")]
        public UnityEvent OnCreditsFinished;

        private bool isScrolling = false;
        private float endPositionY;

        private void OnEnable()
        {
            UpdateCreditsText();

            if (autoScrollOnEnable)
            {
                Invoke("StartScrolling", 0.1f);
            }
        }

        public void StartScrolling()
        {
            if (contentTransform == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            RectTransform viewport = null;

            if (contentTransform.parent != null)
            {
                viewport = contentTransform.parent.GetComponent<RectTransform>();
            }

            float viewportHeight = Screen.height;

            if (viewport != null)
            {
                viewportHeight = viewport.rect.height;
            }

            float contentHeight = contentTransform.rect.height;

            contentTransform.anchoredPosition = new Vector2(contentTransform.anchoredPosition.x, -viewportHeight);

            endPositionY = contentHeight;

            isScrolling = true;
        }

        private void Update()
        {
            if (!isScrolling || contentTransform == null)
            {
                return;
            }

            contentTransform.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);

            if (contentTransform.anchoredPosition.y >= endPositionY + 10f)
            {
                isScrolling = false;

                if (OnCreditsFinished != null)
                {
                    OnCreditsFinished.Invoke();
                }
            }
        }

        private void UpdateCreditsText()
        {
            if (creditsTextComponent != null)
            {
                if (creditsTextComponent.text != creditsText)
                {
                    creditsTextComponent.text = creditsText;
                    creditsTextComponent.SetAllDirty();

                    ModularText modText = creditsTextComponent.GetComponent<ModularText>();

                    if (modText != null)
                    {
                        modText.UpdateTextFromExternal(creditsText);
                    }
                }
            }
        }

        #if UNITY_EDITOR
            protected void OnValidate()
            {
                if (Application.isPlaying)
                {
                    return;
                }

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        UpdateCreditsText();
                    }
                };
            }
        #endif
    }
}