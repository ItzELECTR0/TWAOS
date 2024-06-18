using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Force 2D")]
    [Description("Adds a force to a game object with a Rigidbody2D")]

    [Category("Physics 2D/Add Force 2D")]

    [Parameter(
        "Rigidbody", 
        "The game object that will receive the force. A Rigidbody2D attached is required"
    )]
    
    [Parameter(
        "Direction", 
        "The direction in which the force will be applied"
    )]
    
    [Parameter(
        "Force", 
        "The amount of force applied"
    )]
    
    [Parameter(
        "Force Mode", 
        "The type of force applied"
    )]
    
    [Keywords("Apply", "Velocity", "Impulse", "Propel", "Push", "Pull")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionPhysics2DAddForce : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();
        
        [Space]
        [SerializeField] private PropertyGetRotation m_Direction = new PropertyGetRotation();
        [SerializeField] private PropertyGetDecimal m_Force = new PropertyGetDecimal(10f);
        [SerializeField] private ForceMode2D m_ForceMode = ForceMode2D.Impulse;
        [SerializeField] private Space m_SpaceMode = Space.World;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Add {this.m_ForceMode} {this.m_Force} to {this.m_Rigidbody}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;

            Rigidbody2D rigidbody = gameObject.Get<Rigidbody2D>();
            if (rigidbody == null) return DefaultResult;
            
            Quaternion forceRotation = this.m_Direction.Get(args);
            float forceAmount = (float) this.m_Force.Get(args);

            Vector3 direction = forceRotation * Vector3.forward;
            Vector2 force = new Vector2(direction.x, direction.y).normalized * forceAmount;
            
            if (this.m_SpaceMode == Space.Self)
            {
                force = gameObject.transform.InverseTransformDirection(force);
            }
            
            rigidbody.AddForce(force, this.m_ForceMode);
            return DefaultResult;
        }
    }
}