using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume SFX")]
    [Category("Audio/Volume SFX")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The SFX volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound", "Effect")]
    
    [Serializable]
    public class GetDecimalVolumeSFX : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.SoundEffects;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.SoundEffects;

        public override string String => "SFX Volume";
    }
}