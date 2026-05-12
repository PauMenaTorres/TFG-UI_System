using System;
using UnityEngine;
using UnityEngine.Events;

namespace ModularUIRuntime
{
    [Serializable]
    public class ModularButtonData
    {
        public string buttonName = "New Button";
        public ModularButton targetButton;
        public UnityEvent OnClick;
    }
}
