using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ModularUIRuntime
{
    [Serializable]
    public class VitalBarReference
    {
        public Slider vitalSlider;
        [Range(0f, 1f)] public float startingValue = 1f;

        [HideInInspector] public float currentPercentage = 1f;
        [HideInInspector] public float targetPercentage = 1f;
        [HideInInspector] public Vector3 originalScale = Vector3.one;
    }

    [Serializable]
    public class HotbarActionBinding
    {
        public string actionName = "New Action";
        public ModularButton slotButton;
        public UnityEvent onActionTriggered;
    }

    public class ModularHudController : MonoBehaviour
    {
        [Header("Global Settings")]
        public float barLerpSpeed = 5.0f;

        [Header("Player Vitals")]
        public VitalBarReference healthBar;
        public VitalBarReference secondaryBar;

        [Header("Hotbar Manual Mapping")]
        public List<HotbarActionBinding> hotbarBindings = new List<HotbarActionBinding>();
        public ModularText selectedObjectNameText;

        [Header("Objectives & Minimap")]
        public ModularText objectiveText;
        public RawImage minimapDisplay;
        public Camera minimapCamera;

        [Header("PopUps")]
        public ModularText simplePopupText;
        public ModularText importantPopupText;

        private Coroutine simplePopupCoroutine;
        private Coroutine importantPopupCoroutine;

        private void Start()
        {
            InitializeVitals();
            InitializeButtons();

            if (simplePopupText != null)
            {
                simplePopupText.gameObject.SetActive(false);
            }

            if (importantPopupText != null)
            {
                importantPopupText.gameObject.SetActive(false);
            }

            SetupMinimap();
        }

        private void Update()
        {
            UpdateVitalBar(healthBar);
            UpdateVitalBar(secondaryBar);
            CheckHotbarInputs();
        }

        private void InitializeVitals()
        {
            if (healthBar == null)
            {
                healthBar = new VitalBarReference();
            }
            if (secondaryBar == null)
            {
                secondaryBar = new VitalBarReference();
            }

            if (healthBar.vitalSlider == null || secondaryBar.vitalSlider == null)
            {
                Slider[] sliders = GetComponentsInChildren<Slider>(true);
                if (healthBar.vitalSlider == null && sliders.Length > 0)
                {
                    healthBar.vitalSlider = sliders[0];
                }
                if (secondaryBar.vitalSlider == null && sliders.Length > 1)
                {
                    secondaryBar.vitalSlider = sliders[1];
                }
            }

            if (healthBar.vitalSlider != null)
            {
                healthBar.vitalSlider.minValue = 0f;
                healthBar.vitalSlider.maxValue = 1f;
                healthBar.currentPercentage = healthBar.startingValue;
                healthBar.targetPercentage = healthBar.startingValue;
                healthBar.vitalSlider.value = healthBar.startingValue;
                healthBar.originalScale = healthBar.vitalSlider.transform.localScale;
            }

            if (secondaryBar.vitalSlider != null)
            {
                secondaryBar.vitalSlider.minValue = 0f;
                secondaryBar.vitalSlider.maxValue = 1f;
                secondaryBar.currentPercentage = secondaryBar.startingValue;
                secondaryBar.targetPercentage = secondaryBar.startingValue;
                secondaryBar.vitalSlider.value = secondaryBar.startingValue;
                secondaryBar.originalScale = secondaryBar.vitalSlider.transform.localScale;
            }
        }

        private void InitializeButtons()
        {
            for (int i = 0; i < hotbarBindings.Count; i++)
            {
                int index = i;

                if (hotbarBindings[i].slotButton != null)
                {
                    Button menuBtn = hotbarBindings[i].slotButton.GetComponent<Button>();

                    if (menuBtn != null)
                    {
                        menuBtn.onClick.AddListener(delegate
                        {
                            ExecuteHotbarAction(index);
                        });
                    }
                }
            }
        }

        private void CheckHotbarInputs()
        {
            for (int i = 0; i < hotbarBindings.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    ExecuteHotbarAction(i);
                }
            }
        }

        public void ExecuteHotbarAction(int index)
        {
            if (index >= 0 && index < hotbarBindings.Count)
            {
                if (hotbarBindings[index].onActionTriggered != null)
                {
                    hotbarBindings[index].onActionTriggered.Invoke();
                    SetHotbarObjectName(hotbarBindings[index].actionName);
                }
            }
        }

        private void UpdateVitalBar(VitalBarReference vital)
        {
            if (vital == null || vital.vitalSlider == null)
            {
                return;
            }

            if (barLerpSpeed <= 0f)
            {
                barLerpSpeed = 5.0f;
            }

            if (vital.currentPercentage != vital.targetPercentage)
            {
                vital.currentPercentage = Mathf.MoveTowards(vital.currentPercentage, vital.targetPercentage, barLerpSpeed * Time.deltaTime);
                vital.vitalSlider.value = vital.currentPercentage;
            }

            if (vital.vitalSlider != null)
            {
                vital.vitalSlider.transform.localScale = vital.originalScale;
            }
            
        }

        public void SetHealth(float percentage)
        {
            SetVitalTarget(healthBar, percentage);
        }

        public void SetSecondaryResource(float percentage)
        {
            SetVitalTarget(secondaryBar, percentage);
        }

        private void SetVitalTarget(VitalBarReference vital, float percentage)
        {
            if (vital != null)
            {
                vital.targetPercentage = Mathf.Clamp01(percentage);
            }
        }

        public void SetObjective(string description)
        {
            if (objectiveText != null)
            {
                objectiveText.UpdateTextFromExternal(description);
            }
        }

        public void SetupMinimap(RenderTexture externalTexture = null)
        {
            if (minimapDisplay == null || minimapCamera == null)
            {
                return;
            }

            if (externalTexture != null)
            {
                minimapCamera.targetTexture = externalTexture;
                minimapDisplay.texture = externalTexture;
                return;
            }

            if (minimapCamera.targetTexture == null)
            {
                RenderTexture rt = new RenderTexture(256, 256, 16);
                rt.name = "RuntimeMinimapTexture";
                minimapCamera.targetTexture = rt;
            }

            minimapDisplay.texture = minimapCamera.targetTexture;
        }

        public void SetHotbarObjectName(string name)
        {
            if (selectedObjectNameText != null)
            {
                selectedObjectNameText.UpdateTextFromExternal(name);
            }
        }

        public void ShowSimplePopUp(string message, float displayTime = 3f)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (simplePopupCoroutine != null)
            {
                StopCoroutine(simplePopupCoroutine);
            }

            simplePopupCoroutine = StartCoroutine(SimplePopUpRoutine(message, displayTime));
        }

        private IEnumerator SimplePopUpRoutine(string message, float time)
        {
            if (simplePopupText == null)
            {
                yield break;
            }

            simplePopupText.UpdateTextFromExternal(message);
            simplePopupText.gameObject.SetActive(true);

            yield return new WaitForSeconds(time);

            simplePopupText.gameObject.SetActive(false);
        }

        public void ShowImportantPopUp(string message, float displayTime = 5f)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (importantPopupCoroutine != null)
            {
                StopCoroutine(importantPopupCoroutine);
            }

            importantPopupCoroutine = StartCoroutine(ImportantPopUpRoutine(message, displayTime));
        }

        private IEnumerator ImportantPopUpRoutine(string message, float time)
        {
            if (importantPopupText == null)
            {
                yield break;
            }

            importantPopupText.UpdateTextFromExternal(message);
            importantPopupText.gameObject.SetActive(true);

            yield return new WaitForSeconds(time);

            importantPopupText.gameObject.SetActive(false);
        }
    }
}