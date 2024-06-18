using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Play Footstep")]
    [Description("Plays a Footstep sound from a Material Sound asset")]

    [Category("Characters/Footsteps/Play Footstep")]

    [Parameter("Character", "The character target")]
    [Parameter("Material Sound", "The material sound asset")]

    [Keywords("Step", "Foot", "Impact", "Land", "Sound")]
    [Image(typeof(IconFootprint), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterPlayFootstep : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] 
        private MaterialSoundsAsset m_MaterialSounds;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Play Footstep on {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            if (this.m_MaterialSounds == null) return DefaultResult;
            
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Footsteps.PlayFootstepSound(this.m_MaterialSounds);
            return DefaultResult;
        }
    }
}