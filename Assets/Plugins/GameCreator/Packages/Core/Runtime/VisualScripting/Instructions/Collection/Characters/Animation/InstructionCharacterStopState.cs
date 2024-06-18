using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop State")]
    [Description("Stops an animation State from a Character")]

    [Category("Characters/Animation/Stop State")]

    [Parameter("Character", "The character that stops its animation State")]
    [Parameter("Layer", "Slot number from which the state is removed")]

    [Parameter("Delay", "Amount of seconds to wait before the animation stops playing")]
    [Parameter("Transition", "The amount of seconds the animation takes to blend out")]

    [Keywords("Characters", "Animation", "Animate", "State", "Exit", "Stop")]
    [Image(typeof(IconCharacterState), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCharacterStopState : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space]
        [SerializeField] private PropertyGetInteger m_Layer = new PropertyGetInteger(1);

        [Space] 
        [SerializeField] private PropertyGetDecimal m_Delay = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Transition = new PropertyGetDecimal(0.1f);

        public override string Title => $"Stop {this.m_Character} State in Layer {this.m_Layer}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            int layer = (int) this.m_Layer.Get(args);
            character.States.Stop(
                layer,
                (float) this.m_Delay.Get(args),
                (float) this.m_Transition.Get(args)
            );
            
            return DefaultResult;
        }
    }
}