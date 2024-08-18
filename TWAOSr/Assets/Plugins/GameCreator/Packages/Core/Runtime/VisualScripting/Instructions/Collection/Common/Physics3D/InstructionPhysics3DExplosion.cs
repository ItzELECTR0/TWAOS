using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Explosion Force 3D")]
    [Description("Applies a force to a Rigidbody that simulates explosion effects")]

    [Category("Physics 3D/Add Explosion Force 3D")]

    [Parameter("Rigidbody", "The game object with a Rigidbody component that receives the force")]
    [Parameter("Origin", "The position where the explosion originates")]
    [Parameter("Radius", "How far the blast reaches")]
    [Parameter("Force", "The force of the explosion, which its at its maximum at the origin")]
    [Parameter("Force Mode", "How the force is applied")]
    
    [Keywords("Apply", "Velocity", "Impulse", "Propel", "Push", "Pull", "Boom")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionPhysics3DExplosion : Instruction
    {
        private const float RELAY_UPWARDS_MODIFIER = 0.2f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();
        
        [Space]
        [SerializeField] private PropertyGetPosition m_Origin = new PropertyGetPosition();
        [SerializeField] private PropertyGetDecimal m_Radius = new PropertyGetDecimal(5f);
        
        [Space]
        [SerializeField] private PropertyGetDecimal m_Force = new PropertyGetDecimal(10f);
        [SerializeField] private ForceMode m_ForceMode = ForceMode.Impulse;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Add Explode on {this.m_Rigidbody} at {this.m_Origin}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody rigidbody = gameObject.Get<Rigidbody>();
            if (rigidbody == null) return DefaultResult;

            Vector3 origin = this.m_Origin.Get(args);
            float radius = (float) this.m_Radius.Get(args);
            float force = (float) this.m_Force.Get(args);

            rigidbody.AddExplosionForce(
                force, origin, radius, 
                force * RELAY_UPWARDS_MODIFIER, 
                this.m_ForceMode
            );
            
            return DefaultResult;
        }
    }
}