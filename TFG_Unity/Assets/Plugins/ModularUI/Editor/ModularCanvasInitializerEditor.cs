using UnityEditor;
using UnityEngine;
using ModularUIRuntime;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularCanvasInitializer))]
    [CanEditMultipleObjects]
    public class ModularCanvasInitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw default inspector
            DrawDefaultInspector();

            if (serializedObject.ApplyModifiedProperties())
            {
                foreach (var t in targets)
                {
                    if (t is ModularCanvasInitializer ci)
                    {
                        ci.ForceInitialize();
                        // Mark target as dirty since this is an inspector change
                        EditorUtility.SetDirty(ci.gameObject);
                        var scaler = ci.GetComponent<UnityEngine.UI.CanvasScaler>();
                        if (scaler != null)
                        {
                            EditorUtility.SetDirty(scaler);
                        }
                    }
                }
            }
        }
    }
}
