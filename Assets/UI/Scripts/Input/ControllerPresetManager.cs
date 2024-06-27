using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [CreateAssetMenu(fileName = "New Controller Preset Manager", menuName = "Heat UI/Controller/New Controller Preset Manager")]
    public class ControllerPresetManager : ScriptableObject
    {
        public ControllerPreset keyboardPreset;
        public ControllerPreset xboxPreset;
        public ControllerPreset dualsensePreset;
        public List<ControllerPreset> customPresets = new List<ControllerPreset>();
    }
}