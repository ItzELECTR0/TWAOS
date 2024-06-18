using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Invincible")]
    [Description("Changes the Invincibility state of a Character")]

    [Category("Characters/Combat/Invincibility/Set Invincible")]
    
    [Parameter("Character", "The Character that attempts to change its invincibility")]
    [Parameter("Duration", "The duration of the invincibility")]
    [Parameter("Wait Until Complete", "Whether to wait until the invincibility wears off")]

    [Keywords("Character", "Combat", "Invincibility")]
    [Image(typeof(IconDiamondSolid), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterSetInvincible : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDecimal m_Duration = GetDecimalDecimal.Create(1f);
        [SerializeField] private bool m_WaitUntilComplete;

        public override string Title => $"Set {this.m_Character} Invincible for {this.m_Duration} seconds";

        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            float duration = (float) this.m_Duration.Get(args);
            character.Combat.Invincibility.Set(duration);

            if (this.m_WaitUntilComplete) await this.Time(duration);
        }
    }
}