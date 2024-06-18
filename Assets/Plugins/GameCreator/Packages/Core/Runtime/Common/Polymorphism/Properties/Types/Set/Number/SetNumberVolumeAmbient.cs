using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Ambient")]
    [Category("Audio/Volume Ambient")]

    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    [Description("The Ambient volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeAmbient : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.Ambient = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.Ambient;
        }

        public override string String => "Ambient Volume";
    }
}