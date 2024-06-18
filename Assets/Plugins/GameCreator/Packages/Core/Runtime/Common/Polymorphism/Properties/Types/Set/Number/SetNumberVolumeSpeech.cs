using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Speech")]
    [Category("Audio/Volume Speech")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Speech volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeSpeech : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.Speech = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.Speech;
        }

        public override string String => "Speech Volume";
    }
}