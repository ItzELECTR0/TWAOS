using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Force 3D")]
    [Description("Adds a force to a game object with a Rigidbody")]

    [Category("Physics 3D/Add Force 3D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody component that receives the force"
    )]
    
    [Parameter(
        "Direction", 
        "The direction in which the force is applied"
    )]
    
    [Parameter(
        "Force", 
        "The amount of force applied"
    )]
    
    [Parameter(
        "Force Mode", 
        "The type of force applied"
    )]
    
    [Parameter(
        "Space Mode", 
        "Whether the force is applied in local or world space"
    )]
    
    [Keywords("Apply", "Velocity", "Impulse", "Propel", "Push", "Pull")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionPhysics3DAddForce : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();

        [Space]
        [SerializeField] private PropertyGetRotation m_Direction = new PropertyGetRotation();
        [SerializeField] private PropertyGetDecimal m_Force = new PropertyGetDecimal(10f);
        [SerializeField] private ForceMode m_ForceMode = ForceMode.Impulse;
        [SerializeField] private Space m_SpaceMode = Space.World;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Add {this.m_ForceMode} {this.m_Force} to {this.m_Rigidbody}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody rigidbody = gameObject.Get<Rigidbody>();
            if (rigidbody == null) return DefaultResult;

            Quaternion forceRotation = this.m_Direction.Get(args);
            float forceAmount = (float) this.m_Force.Get(args);

            Vector3 direction = forceRotation * Vector3.forward;
            Vector3 force = direction.normalized * forceAmount;

            if (this.m_SpaceMode == Space.Self)
            {
                force = gameObject.transform.InverseTransformDirection(force);
            }
            
            rigidbody.AddForce(force, this.m_ForceMode);
            return DefaultResult;
        }
    }
}