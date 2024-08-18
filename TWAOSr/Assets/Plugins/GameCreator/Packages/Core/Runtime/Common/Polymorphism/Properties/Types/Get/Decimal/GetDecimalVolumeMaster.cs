using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Master")]
    [Category("Audio/Volume Master")]
    
    [Image(typeof(IconVolume), ColorTheme.Type.Blue)]
    [Description("The Master volume value. Ranges between 0 and 1")]

    [Keywords("Audio", "Sound")]
    
    [Serializable]
    public class GetDecimalVolumeMaster : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => AudioManager.Instance.Volume.Master;
        public override double Get(GameObject gameObject) => AudioManager.Instance.Volume.Master;

        public override string String => "Master Volume";
    }
}