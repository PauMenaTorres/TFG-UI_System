using System;
using UnityEngine;

namespace ModularUIRuntime
{
    [Serializable]
    public struct WinLoseButtonData
    {
        public string buttonName;
        public ModularButton targetButton;

        [NonSerialized]
        public Action OnClickAction;
    }
}
