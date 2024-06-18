using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Velocity 3D")]
    [Description("Changes the current velocity of a Rigidbody")]

    [Category("Physics 3D/Change Velocity 3D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody attached that changes its velocity"
    )]
    
    [Parameter(
        "Velocity", 
        "The velocity the game object changes to"
    )]

    [Keywords("Speed", "Movement")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics3DChangeVelocity : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();
        
        [Space]
        [SerializeField] private ChangeDirection m_Velocity = new ChangeDirection(Vector3.forward);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Velocity of {this.m_Rigidbody} {this.m_Velocity}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody rigidbody = gameObject.Get<Rigidbody>();
            if (rigidbody == null) return DefaultResult;

            rigidbody.linearVelocity = this.m_Velocity.Get(rigidbody.linearVelocity, args);
            return DefaultResult;
        }
    }
}