using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ModularPanelFitter : MonoBehaviour
    {
        private RectTransform rectTransform;

        private bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.005f;
        }

        private bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) < 0.005f &&
                   Mathf.Abs(a.y - b.y) < 0.005f;
        }

        private bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) < 0.005f &&
                   Mathf.Abs(a.y - b.y) < 0.005f &&
                   Mathf.Abs(a.z - b.z) < 0.005f;
        }

        private void OnEnable()
        {
            FitToParent();
        }

        private void OnTransformParentChanged()
        {
            FitToParent();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    FitToParent();
                }
            };
        }
#endif

        public void FitToParent()
        {
#if UNITY_EDITOR
            if (!gameObject.scene.IsValid())
            {
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                if (stage == null)
                {
                    return;
                }
            }
#endif

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform == null || rectTransform.parent == null)
            {
                return;
            }

            bool changed = false;

            if (!Approximately(rectTransform.anchorMin, Vector2.zero))
            {
                rectTransform.anchorMin = Vector2.zero;
                changed = true;
            }

            if (!Approximately(rectTransform.anchorMax, Vector2.one))
            {
                rectTransform.anchorMax = Vector2.one;
                changed = true;
            }

            if (!Approximately(rectTransform.offsetMin, Vector2.zero))
            {
                rectTransform.offsetMin = Vector2.zero;
                changed = true;
            }

            if (!Approximately(rectTransform.offsetMax, Vector2.zero))
            {
                rectTransform.offsetMax = Vector2.zero;
                changed = true;
            }

            if (!Approximately(rectTransform.localScale, Vector3.one))
            {
                rectTransform.localScale = Vector3.one;
                changed = true;
            }

            Vector3 lp = rectTransform.localPosition;
            if (!Approximately(lp.z, 0f))
            {
                rectTransform.localPosition = new Vector3(lp.x, lp.y, 0f);
                changed = true;
            }

#if UNITY_EDITOR
            if (changed && !Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(rectTransform);
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
#endif
        }
    }
}