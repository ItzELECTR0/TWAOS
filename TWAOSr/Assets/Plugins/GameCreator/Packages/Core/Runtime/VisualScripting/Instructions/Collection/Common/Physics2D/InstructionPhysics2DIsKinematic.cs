using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Kinematic 2D")]
    [Description("Controls whether physics affects the Rigidbody2D")]

    [Category("Physics 2D/Is Kinematic 2D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody2D attached that changes its kinematic usage"
    )]
    
    [Parameter(
        "Is Kinematic", 
        "If enabled, forces, collisions or joints do not affect the rigidbody anymore"
    )]
    
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics2DIsKinematic : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();

        [Space]
        [SerializeField] private PropertyGetBool m_IsKinematic = GetBoolValue.Create(false);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Is Kinematic = {this.m_IsKinematic} on {this.m_Rigidbody}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody2D rigidbody = gameObject.Get<Rigidbody2D>();
            if (rigidbody == null) return DefaultResult;

            rigidbody.isKinematic = this.m_IsKinematic.Get(args);
            return DefaultResult;
        }
    }
}