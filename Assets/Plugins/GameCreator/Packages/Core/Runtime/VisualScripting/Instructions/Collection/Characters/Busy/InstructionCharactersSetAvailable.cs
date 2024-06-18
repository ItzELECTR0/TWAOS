using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Available")]
    [Description("Sets the Available state of a Character's Limbs")]

    [Category("Characters/Busy/Set Available")]

    [Parameter("Character", "The Character game object")]
    [Parameter("Limbs", "The Limbs that are changed to Available")]

    [Keywords("Characters", "Busy", "Occupied", "Using")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharactersSetAvailable : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private Busy.Limb m_Limbs = Busy.Limb.Every;

        public override string Title => $"Set {this.m_Character} Available = {this.m_Limbs}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character != null) character.Busy.RemoveState(this.m_Limbs);
            return DefaultResult;
        }
    }
}