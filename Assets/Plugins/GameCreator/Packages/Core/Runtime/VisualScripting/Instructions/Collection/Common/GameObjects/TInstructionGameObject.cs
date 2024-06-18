using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Game Object", "Target game object")]

    [Serializable]
    public abstract class TInstructionGameObject : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        protected PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
    }
}