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

            DrawDefaultInspector();

            if (serializedObject.ApplyModifiedProperties())
            {
                foreach (var t in targets)
                {
                    if (t is ModularCanvasInitializer ci)
                    {
                        ci.ForceInitialize();
                        
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
