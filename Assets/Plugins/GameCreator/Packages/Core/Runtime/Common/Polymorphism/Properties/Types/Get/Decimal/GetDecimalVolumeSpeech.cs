using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Speech")]
    [Category("Audio/Volume Speech")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Speech volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound")]
    
    [Serializable]
    public class GetDecimalVolumeSpeech : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.Speech;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.Speech;

        public override string String => "Speech Volume";
    }
}