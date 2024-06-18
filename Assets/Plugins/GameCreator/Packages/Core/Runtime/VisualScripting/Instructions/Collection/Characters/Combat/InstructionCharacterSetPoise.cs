using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Poise")]
    [Description("Changes the current Poise value of a Character")]

    [Category("Characters/Combat/Poise/Set Poise")]
    
    [Parameter("Character", "The Character that attempts to change its Poise value")]
    [Parameter("Poise", "The new Poise value")]

    [Keywords("Character", "Combat")]
    [Image(typeof(IconShieldOutline), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterSetPoise : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDecimal m_Poise = GetDecimalDecimal.Create(1f);

        public override string Title => $"Set {this.m_Character} Poise = {this.m_Poise}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            float poise = (float) this.m_Poise.Get(args);
            character.Combat.Poise.Set(poise);

            return DefaultResult;
        }
    }
}