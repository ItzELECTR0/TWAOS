using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume SFX")]
    [Category("Audio/Volume SFX")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The SFX volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeSFX : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.SoundEffects = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.SoundEffects;
        }

        public override string String => "SFX Volume";
    }
}