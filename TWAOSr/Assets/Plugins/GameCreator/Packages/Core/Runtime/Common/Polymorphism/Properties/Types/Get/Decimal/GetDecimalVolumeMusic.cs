using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Music")]
    [Category("Audio/Volume Music")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Music volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound")]
    
    [Serializable]
    public class GetDecimalVolumeMusic : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.Music;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.Music;

        public override string String => "Music Volume";
    }
}