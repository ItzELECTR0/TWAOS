using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Sound Effects volume")]
    [Description("Change the Volume of Sound Effects")]

    [Category("Audio/Change Sound Effects volume")]

    [Parameter("Volume", "A value between 0 and 1 that indicates the volume percentage")]
    
    [Keywords("Audio", "Sounds", "Volume", "Level")]
    [Image(typeof(IconVolume), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCommonAudioVolumeSoundEffects : Instruction
    {
        public PropertyGetDecimal m_Volume = new PropertyGetDecimal(1f);

        public override string Title => $"Change SFX volume to {this.m_Volume}";

        protected override Task Run(Args args)
        {
            AudioManager.Instance.Volume.SoundEffects = (float) this.m_Volume.Get(args);
            return DefaultResult;
        }
    }
}