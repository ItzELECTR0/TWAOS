using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Gravity Scale 2D")]
    [Description("Controls whether how gravity affects the Rigidbody2D")]

    [Category("Physics 2D/Gravity Scale 2D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody2D attached that changes its gravity scale"
    )]
    
    [Parameter(
        "Gravity Scale", 
        "The degree to which this object is affected by gravity"
    )]
    
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics2DGravityScale : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();

        [Space] 
        [SerializeField] private PropertyGetDecimal m_GravityScale = GetDecimalDecimal.Create(1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Gravity Scale on {this.m_Rigidbody}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody2D rigidbody = gameObject.Get<Rigidbody2D>();
            if (rigidbody == null) return DefaultResult;

            rigidbody.gravityScale = (float) this.m_GravityScale.Get(args);
            return DefaultResult;
        }
    }
}