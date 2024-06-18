using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Character", "The game object with the Character target")]

    [Serializable]
    public abstract class TInstructionCharacterProperty : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
    }
}