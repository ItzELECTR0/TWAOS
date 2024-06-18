using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Music volume")]
    [Description("Change the Volume of Music")]

    [Category("Audio/Change Music volume")]

    [Parameter("Volume", "A value between 0 and 1 that indicates the volume percentage")]
    
    [Keywords("Audio", "Music", "Background", "Volume", "Level")]
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCommonAudioVolumeMusic : Instruction
    {
        public PropertyGetDecimal m_Volume = new PropertyGetDecimal(1f);

        public override string Title => $"Change Music volume to {this.m_Volume}";

        protected override Task Run(Args args)
        {
            AudioManager.Instance.Volume.Music = (float) this.m_Volume.Get(args);
            return DefaultResult;
        }
    }
}