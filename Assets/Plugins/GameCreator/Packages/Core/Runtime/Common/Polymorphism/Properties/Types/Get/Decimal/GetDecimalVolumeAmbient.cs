using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Ambient")]
    [Category("Audio/Volume Ambient")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Ambient volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound")]
    
    [Serializable]
    public class GetDecimalVolumeAmbient : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.Ambient;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.Ambient;

        public override string String => "Ambient Volume";
    }
}