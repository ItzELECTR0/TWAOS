using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Speech on Game Object")]
    [Description("Stops any Speech clips being played by a specific Game Object")]

    [Category("Audio/Stop Speech on Game Object")]

    [Parameter("Target", "A game object that is set as the source of the speech")]

    [Keywords("Audio", "Voice", "Voices", "Sounds", "Character", "Silence", "Mute", "Fade")]
    [Image(typeof(IconFace), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCommonAudioSpeechStopGameObject : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        public override string Title => $"Stop {this.m_Target} speech";

        protected override Task Run(Args args)
        {
            GameObject target = this.m_Target.Get(args);
            target = AudioConfigSpeech.GetSpeechSource(target);

            _ = AudioManager.Instance.Speech.Stop(target, 0.1f);
            return DefaultResult;
        }
    }
}