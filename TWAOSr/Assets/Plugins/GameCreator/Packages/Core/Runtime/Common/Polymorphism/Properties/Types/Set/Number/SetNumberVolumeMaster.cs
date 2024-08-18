using System;

namespace GameCreator.Runtime.Common
{
    [Title("Volume Master")]
    [Category("Audio/Volume Master")]

    [Image(typeof(IconVolume), ColorTheme.Type.Blue)]
    [Description("The Master volume value. Ranges between 0 and 1")]

    [Serializable]
    public class SetNumberVolumeMaster : PropertyTypeSetNumber
    {
        public override void Set(double value, Args args)
        {
            AudioManager.Instance.Volume.Master = (float) value;
        }

        public override double Get(Args args)
        {
            return AudioManager.Instance.Volume.Master;
        }

        public override string String => "Master Volume";
    }
}