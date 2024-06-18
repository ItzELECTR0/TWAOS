using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Use Gravity 3D")]
    [Description("Controls whether gravity affects the Rigidbody")]

    [Category("Physics 3D/Use Gravity 3D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody attached that changes its gravity usage"
    )]
    
    [Parameter(
        "Use Gravity", 
        "If set to false the rigidbody behaves as in outer space"
    )]
    
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics3DSetUseGravity : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();

        [Space] 
        [SerializeField] private PropertyGetBool m_UseGravity = GetBoolValue.Create(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Use Gravity on {this.m_Rigidbody}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody rigidbody = gameObject.Get<Rigidbody>();
            if (rigidbody == null) return DefaultResult;

            rigidbody.useGravity = this.m_UseGravity.Get(args);
            return DefaultResult;
        }
    }
}