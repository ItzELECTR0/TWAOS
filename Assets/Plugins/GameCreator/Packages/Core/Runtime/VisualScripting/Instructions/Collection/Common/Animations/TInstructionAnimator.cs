using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Animator", "The Animator component attached to the game object")]

    [Serializable]
    public abstract class TInstructionAnimator : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        protected PropertyGetGameObject m_Animator = new PropertyGetGameObject();
    }
}