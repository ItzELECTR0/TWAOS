using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Transform", "The Transform of the game object")]

    [Serializable]
    public abstract class TInstructionTransform : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        protected PropertyGetGameObject m_Transform = GetGameObjectTransform.Create();
    }
}