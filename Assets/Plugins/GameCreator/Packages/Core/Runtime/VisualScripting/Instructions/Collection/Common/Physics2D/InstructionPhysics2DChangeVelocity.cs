using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Velocity 2D")]
    [Description("Changes the current velocity of a Rigidbody2D")]

    [Category("Physics 2D/Change Velocity 2D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody2D attached that will change its velocity"
    )]
    
    [Parameter(
        "Velocity", 
        "The velocity the game object will change to"
    )]

    [Keywords("Speed", "Movement")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics2DChangeVelocity : Instruction
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
            
            Rigidbody2D rigidbody = gameObject.Get<Rigidbody2D>();
            if (rigidbody == null) return DefaultResult;

            rigidbody.velocity = this.m_Velocity.Get(rigidbody.velocity, args);
            return DefaultResult;
        }
    }
}