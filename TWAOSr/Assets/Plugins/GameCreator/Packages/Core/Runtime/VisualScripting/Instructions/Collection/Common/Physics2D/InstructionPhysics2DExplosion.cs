using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Explosion Force 2D")]
    [Description("Applies a force to a Rigidbody2D that simulates explosion effects")]

    [Category("Physics 2D/Add Explosion Force 2D")]

    [Parameter("Rigidbody", "The game object with a Rigidbody2D component that receives the force")]
    [Parameter("Origin", "The position where the explosion originates")]
    [Parameter("Radius", "How far the blast reaches")]
    [Parameter("Force", "The force of the explosion, which its at its maximum at the origin")]
    [Parameter("Force Mode", "How the force is applied")]
    
    [Keywords("Apply", "Velocity", "Impulse", "Propel", "Push", "Pull", "Boom")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionPhysics2DExplosion : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();
        
        [Space]
        [SerializeField] private PropertyGetPosition m_Origin = new PropertyGetPosition();
        [SerializeField] private PropertyGetDecimal m_Radius = new PropertyGetDecimal(5f);
        
        [Space]
        [SerializeField] private PropertyGetDecimal m_Force = new PropertyGetDecimal(10f);
        [SerializeField] private ForceMode2D m_ForceMode = ForceMode2D.Impulse;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Add Explode on {this.m_Rigidbody} at {this.m_Origin}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody2D rigidbody = gameObject.Get<Rigidbody2D>();
            if (rigidbody == null) return DefaultResult;

            Vector3 origin = this.m_Origin.Get(args);
            double radius = this.m_Radius.Get(args);
            double force = this.m_Force.Get(args);

            Vector2 direction = (gameObject.transform.position - origin).XY();
            double falloff = 1f - Math.Clamp(direction.magnitude / radius, 0, 1);
            
            rigidbody.AddForce(
                direction.normalized * (float) (force * falloff),
                this.m_ForceMode
            );

            return DefaultResult;
        }
    }
}