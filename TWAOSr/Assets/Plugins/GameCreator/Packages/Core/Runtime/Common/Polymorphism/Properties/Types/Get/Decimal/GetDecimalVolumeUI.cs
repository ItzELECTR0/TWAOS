using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume UI")]
    [Category("Audio/Volume UI")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The UI volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound", "Effect")]
    
    [Serializable]
    public class GetDecimalVolumeUI : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.UI;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.UI;

        public override string String => "UI Volume";
    }
}