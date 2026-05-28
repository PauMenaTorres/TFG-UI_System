using UnityEditor;
using UnityEngine;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularPanelFitter))]
    [CanEditMultipleObjects]
    public class ModularPanelFitterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Fit to Parent", GUILayout.Height(30)))
            {
                foreach (var t in targets)
                {
                    if (t is ModularPanelFitter fitter)
                    {
                        var rectTransform = fitter.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            Undo.RecordObject(rectTransform, "Fit to Parent");
                        }
                        Undo.RecordObject(fitter, "Fit to Parent");

                        fitter.FitToParent();

                        if (rectTransform != null)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
                        }
                        PrefabUtility.RecordPrefabInstancePropertyModifications(fitter);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
