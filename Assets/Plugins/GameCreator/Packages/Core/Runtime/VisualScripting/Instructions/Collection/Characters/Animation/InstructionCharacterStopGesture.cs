using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Gestures")]
    [Description("Stops any animation Gestures playing on the Character")]

    [Category("Characters/Animation/Stop Gesture")]

    [Parameter("Character", "The character that plays animation Gestures")]
    [Parameter("Delay", "Amount of seconds to wait before the animation starts to blend out")]
    [Parameter("Transition", "The amount of seconds the animation takes to blend out")]

    [Keywords("Characters", "Animation", "Animate", "Gesture", "Play")]
    [Image(typeof(IconCharacterGesture), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCharacterStopGesture : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] 
        [SerializeField] private PropertyGetDecimal m_Delay = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Transition = new PropertyGetDecimal(0.1f);

        public override string Title => $"Stop gestures on {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Gestures.Stop(
                (float) this.m_Delay.Get(args), 
                (float) this.m_Transition.Get(args)
            );
            
            return DefaultResult;
        }
    }
}