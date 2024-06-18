using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Move")]
    [Description("Attempts to stop the character from moving")]

    [Category("Characters/Navigation/Stop Move")]
    
    [Parameter("Priority", "Indicates the priority of this command against others")]

    [Keywords("Constant", "Walk", "Run", "To", "Vector")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Red, typeof(OverlayCross))]

    [Serializable]
    public class InstructionCharacterNavigationMoveStop : TInstructionCharacterNavigation
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetInteger m_Priority = GetDecimalInteger.Create(1);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Stop {this.m_Character} movement";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            int priority = (int) this.m_Priority.Get(args);
            character.Motion.StopToDirection(priority);
            
            return DefaultResult;
        }
    }
}