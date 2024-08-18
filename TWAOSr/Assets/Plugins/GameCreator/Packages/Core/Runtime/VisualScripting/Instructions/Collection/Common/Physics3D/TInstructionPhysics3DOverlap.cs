using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter(
        "Store In", 
        "The list where the colliders (if any) are stored"
    )]
    
    [Parameter(
        "Layer Mask", 
        "A mask that determines which colliders are ignored and which aren't"
    )]
    
    [Keywords("Cast", "Collect")]
    [Keywords("Physics", "Rigidbody")]

    [Serializable]
    public abstract class TInstructionPhysics3DOverlap : Instruction
    {
        protected const int LENGTH = 30;
        private static readonly Collider[] COLLIDERS = new Collider[LENGTH];
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        protected CollectorListVariable m_StoreIn = new CollectorListVariable();

        [SerializeField]
        protected LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            int count = this.GetColliders(COLLIDERS, args);
            GameObject[] result = new GameObject[count];
            
            for (int i = 0; i < count; ++i)
            {
                result[i] = COLLIDERS[i].gameObject;
            }

            this.m_StoreIn.Fill(result, args);
            return DefaultResult;
        }

        protected abstract int GetColliders(Collider[] colliders, Args args);
    }
}