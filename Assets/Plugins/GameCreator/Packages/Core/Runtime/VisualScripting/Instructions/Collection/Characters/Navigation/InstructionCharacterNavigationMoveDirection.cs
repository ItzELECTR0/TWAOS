using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Move Direction")]
    [Description("Attempts to move the Character towards the specified direction")]

    [Category("Characters/Navigation/Move Direction")]
    
    [Parameter("Direction", "The the direction to move towards")]
    [Parameter("Priority", "Indicates the priority of this command against others")]

    [Keywords("Constant", "Walk", "Run", "To", "Vector")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]

    [Serializable]
    public class InstructionCharacterNavigationMoveDirection : TInstructionCharacterNavigation
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionVector.Create();
        [SerializeField] private PropertyGetInteger m_Priority = GetDecimalInteger.Create(0);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Move {this.m_Character} to {this.m_Direction}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            Vector3 direction = this.m_Direction.Get(args);
            int priority = (int) this.m_Priority.Get(args);
            
            character.Motion.MoveToDirection(direction, Space.World, priority);
            return DefaultResult;
        }
    }
}