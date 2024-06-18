using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Character Rotation")]
    [Description("Changes the rotation behavior of the Character")]

    [Category("Characters/Navigation/Set Character Rotation")]

    [Parameter("Character", "The Character that changes its Rotation behavior")]
    [Parameter("Rotation", "The Rotation behavior that decides where the Character faces")]

    [Keywords("Character", "Face", "Look", "Direction", "Pivot", "Lock")]
    [Image(typeof(IconRotationYaw), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterNavigationFacing : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private UnitFacing m_Rotation = new UnitFacing();

        public override string Title => 
            $"Change Rotation on {this.m_Character} to {this.m_Rotation}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            if (this.m_Rotation.Wrapper.GetType() != character.Facing.GetType())
            {
                character.Kernel.ChangeFacing(character, this.m_Rotation.Wrapper);
            }

            return DefaultResult;
        }
    }
}