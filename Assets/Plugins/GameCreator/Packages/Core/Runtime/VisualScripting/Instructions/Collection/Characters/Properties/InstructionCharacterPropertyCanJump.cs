using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Can Jump")]
    [Description("Changes whether the Character is allowed to jump or not")]

    [Category("Characters/Properties/Can Jump")]

    [Parameter("Character", "The character target")]
    [Parameter("Can Jump", "Whether the character is allowed to jump or not")]
    
    [Keywords("Hop", "Elevate")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyCanJump : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        [Space]
        [SerializeField] private PropertyGetBool m_CanJump = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Can Jump {this.m_Character} = {this.m_CanJump}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            bool valueTarget = this.m_CanJump.Get(args);
            character.Motion.CanJump = valueTarget;
            return DefaultResult;
        }
    }
}