using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Music")]
    [Category("Audio/Volume Music")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Music volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeMusic : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.Music = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.Music;
        }

        public override string String => "Music Volume";
    }
}