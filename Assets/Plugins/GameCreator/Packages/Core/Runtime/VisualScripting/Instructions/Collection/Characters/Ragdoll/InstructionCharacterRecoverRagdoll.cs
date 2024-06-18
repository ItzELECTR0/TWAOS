using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Recover from Ragdoll")]
    [Description("Recovers a Character from the Ragdoll state and stands up")]

    [Category("Characters/Ragdoll/Recover Ragdoll")]

    [Parameter("Character", "The Character game object that recovers from the Ragdoll state")]

    [Keywords("Characters", "Ragdoll", "Recover", "Stand")]
    [Image(typeof(IconSkeleton), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterRecoverRagdoll : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"Recover Ragdoll on {this.m_Character}";

        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            await character.Ragdoll.StartRecover();
        }
    }
}