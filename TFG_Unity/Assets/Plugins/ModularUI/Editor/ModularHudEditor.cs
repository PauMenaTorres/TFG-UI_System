using UnityEngine;
using UnityEditor;

namespace ModularUIEditor
{
    [CustomEditor(typeof(ModularUIRuntime.ModularHudController))]
    public class ModularHudEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ModularUIRuntime.ModularHudController hud = (ModularUIRuntime.ModularHudController)target;

            DrawDefaultInspector();
        }
    }
}   