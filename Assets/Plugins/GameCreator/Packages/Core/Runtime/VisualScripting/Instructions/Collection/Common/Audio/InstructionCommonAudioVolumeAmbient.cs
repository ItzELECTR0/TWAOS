using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Ambient volume")]
    [Description("Change the Volume of Ambient music")]

    [Category("Audio/Change Ambient volume")]

    [Parameter("Volume", "A value between 0 and 1 that indicates the volume percentage")]
    
    [Keywords("Audio", "Ambience", "Background", "Volume", "Level")]
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCommonAudioVolumeAmbient : Instruction
    {
        public PropertyGetDecimal m_Volume = new PropertyGetDecimal(1f);

        public override string Title => $"Change Ambient volume to {this.m_Volume}";

        protected override Task Run(Args args)
        {
            AudioManager.Instance.Volume.Ambient = (float) this.m_Volume.Get(args);
            return DefaultResult;
        }
    }
}