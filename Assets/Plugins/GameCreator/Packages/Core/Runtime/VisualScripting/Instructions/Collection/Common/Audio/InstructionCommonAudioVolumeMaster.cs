using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Master volume")]
    [Description(
        @"Change the Master volume. The Master volume controls how loud all other channels are"
    )]

    [Category("Audio/Change Master volume")]
    
    [Parameter("Volume", "A value between 0 and 1 that indicates the volume percentage")]

    [Keywords("Audio", "Sounds", "Volume", "Level")]
    [Image(typeof(IconVolume), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCommonAudioVolumeMaster : Instruction
    {
        public PropertyGetDecimal m_Volume = new PropertyGetDecimal(1f);

        public override string Title => $"Change Master volume to {this.m_Volume}";

        protected override Task Run(Args args)
        {
            AudioManager.Instance.Volume.Master = (float) this.m_Volume.Get(args);
            return DefaultResult;
        }
    }
}