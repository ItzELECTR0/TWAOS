using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume UI")]
    [Category("Audio/Volume UI")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The UI volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeUI : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.UI = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.UI;
        }

        public override string String => "UI Volume";
    }
}