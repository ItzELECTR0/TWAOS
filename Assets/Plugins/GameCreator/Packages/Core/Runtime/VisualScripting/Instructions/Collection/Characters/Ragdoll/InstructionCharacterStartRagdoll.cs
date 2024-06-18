using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Start Ragdoll")]
    [Description("Makes a Character enter a ragdoll state")]

    [Category("Characters/Ragdoll/Start Ragdoll")]

    [Parameter("Character", "The Character game object that changes to a Ragdoll state")]

    [Keywords("Characters", "Ragdoll", "Dead", "Kill", "Die")]
    [Image(typeof(IconSkeleton), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterStartRagdoll : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"Start Ragdoll on {this.m_Character}";

        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            await character.Ragdoll.StartRagdoll();
        }
    }
}