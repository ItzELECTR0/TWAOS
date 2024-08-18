using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Mass 3D")]
    [Description("Changes the mass of a Rigidbody")]

    [Category("Physics 3D/Change Mass 3D")]

    [Parameter(
        "Rigidbody", 
        "The game object with a Rigidbody attached that changes its mass"
    )]
    
    [Parameter(
        "Mass", 
        "The new mass the game object"
    )]

    [Keywords("Weight")]
    [Keywords("Physics", "Rigidbody")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionPhysics3DChangeMass : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Rigidbody = GetGameObjectSelf.Create();
        
        [Space]
        [SerializeField] private ChangeDecimal m_Mass = new ChangeDecimal(10f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Change Mass of {this.m_Rigidbody} {this.m_Mass}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Rigidbody.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Rigidbody rigidbody = gameObject.Get<Rigidbody>();
            if (rigidbody == null) return DefaultResult;
            
            rigidbody.mass = (float) this.m_Mass.Get(rigidbody.mass, args);
            return DefaultResult;
        }
    }
}