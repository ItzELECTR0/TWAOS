using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change UI volume")]
    [Description("Change the Volume of UI elements")]

    [Category("Audio/Change UI volume")]

    [Parameter("Volume", "A value between 0 and 1 that indicates the volume percentage")]
    
    [Keywords("Audio", "User", "Interface", "Button", "Volume", "Level")]
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCommonAudioVolumeUI : Instruction
    {
        public PropertyGetDecimal m_Volume = new PropertyGetDecimal(1f);

        public override string Title => $"Change UI volume to {this.m_Volume}";

        protected override Task Run(Args args)
        {
            AudioManager.Instance.Volume.UI = (float) this.m_Volume.Get(args);
            return DefaultResult;
        }
    }
}