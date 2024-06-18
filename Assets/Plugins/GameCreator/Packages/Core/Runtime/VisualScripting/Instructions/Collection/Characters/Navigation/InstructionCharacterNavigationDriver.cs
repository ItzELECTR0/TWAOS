using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Character Driver")]
    [Description("Changes the driver behavior of the Character")]

    [Category("Characters/Navigation/Set Character Driver")]

    [Parameter("Character", "The Character that changes its Driver behavior")]
    [Parameter("Driver", "The Driver behavior that decides how the Character moves")]

    [Keywords("Character", "Drive", "Controller", "Navmesh", "Agent", "Rigidbody")]
    [Image(typeof(IconWheel), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterNavigationDriver : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private UnitDriver m_Driver = new UnitDriver();

        public override string Title => 
            $"Change Driver on {this.m_Character} to {this.m_Driver}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            if (this.m_Driver.Wrapper.GetType() != character.Driver.GetType())
            {
                character.Kernel.ChangeDriver(character, this.m_Driver.Wrapper);
            }
            
            return DefaultResult;
        }
    }
}