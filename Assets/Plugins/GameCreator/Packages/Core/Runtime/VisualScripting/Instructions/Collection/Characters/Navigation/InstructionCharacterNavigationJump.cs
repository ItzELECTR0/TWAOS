using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Jump")]
    [Description("Instructs the Character to jump")]

    [Category("Characters/Navigation/Jump")]
    
    [Keywords("Hop", "Leap", "Reach")]
    [Image(typeof(IconCharacterJump), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterNavigationJump : TInstructionCharacterNavigation
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Jump {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            if (character.Busy.AreLegsBusy) return DefaultResult;

            character.Jump.Do();
            return DefaultResult;
        }
    }
}