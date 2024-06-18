using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Active Feet IK")]
    [Description("Changes the active state of the Character Feet IK")]

    [Category("Characters/IK/Active Feet IK")]

    [Parameter("Character", "The character target")]
    [Parameter("Active", "Whether the IK system is active or not")]

    [Keywords("Inverse", "Kinematics", "IK")]
    [Image(typeof(IconIK), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterIKFeetActive : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] 
        private PropertyGetBool m_Active = GetBoolValue.Create(false);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Feet IK of {this.m_Character} = {this.m_Active}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            if (character.IK.HasRig<RigFeetPlant>())
            {
                bool state = this.m_Active.Get(args);
                character.IK.GetRig<RigFeetPlant>().IsActive = state;
            }
            
            return DefaultResult;
        }
    }
}