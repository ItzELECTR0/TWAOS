using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Busy")]
    [Description("Sets the Busy state of a Character's Limbs")]

    [Category("Characters/Busy/Set Busy")]

    [Parameter("Character", "The Character game object")]
    [Parameter("Limbs", "The Limbs that are changed to Busy")]

    [Keywords("Characters", "Busy", "Occupied", "Using")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]

    [Serializable]
    public class InstructionCharactersSetBusy : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private Busy.Limb m_Limbs = Busy.Limb.Every;

        public override string Title => $"Set {this.m_Character} Busy = {this.m_Limbs}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character != null) character.Busy.AddState(this.m_Limbs);
            return DefaultResult;
        }
    }
}