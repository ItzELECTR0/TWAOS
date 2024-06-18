using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change State Weight")]
    [Description("Changes the weight of the State over time at the specified layer")]

    [Category("Characters/Animation/Change State Weight")]

    [Parameter("Character", "The character that plays the animation state")]
    [Parameter("Layer", "Slot number in which the animation state is allocated")]
    [Parameter("Weight", "The targeted opacity of the animation")]
    [Parameter("Transition", "The duration of the transition, in seconds")]

    [Keywords("Characters", "Animation", "Blend", "State", "Opacity")]
    [Image(typeof(IconCharacterState), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCharacterStateWeight : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space]
        [SerializeField] private PropertyGetInteger m_Layer = new PropertyGetInteger(1);
        [SerializeField] private PropertyGetDecimal m_Weight = GetDecimalDecimal.Create(1f);
        [SerializeField] private PropertyGetDecimal m_Transition = GetDecimalConstantOne.Create;

        public override string Title => $"Change {this.m_Character} State weight to {this.m_Weight}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            int layer = (int) this.m_Layer.Get(args);
            float weight = (float) this.m_Weight.Get(args);
            float transition = (float) this.m_Transition.Get(args);
            
            character.States.ChangeWeight(layer, weight, transition);
            return DefaultResult;
        }
    }
}