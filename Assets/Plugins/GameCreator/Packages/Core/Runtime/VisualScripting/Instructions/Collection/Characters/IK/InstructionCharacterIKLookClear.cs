using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Clear Looking Around")]
    [Description("Stops looking at any target that isn't in a Hotspot (priority zero)")]

    [Category("Characters/IK/Clear Looking Around")]

    [Parameter("Character", "The character target")]

    [Keywords("Inverse", "Kinematics", "IK")]
    [Image(typeof(IconEye), ColorTheme.Type.Blue, typeof(OverlayCross))]

    [Serializable]
    public class InstructionCharacterIKLookClear : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"{this.m_Character} stop looking around";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            if (character.IK.HasRig<RigLookTo>())
            {
                character.IK.GetRig<RigLookTo>().ClearTargets();
            }
            
            return DefaultResult;
        }
    }
}