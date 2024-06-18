using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Character", "The game object with the Character target")]

    [Keywords("Character", "Player")]
    [Serializable]
    public abstract class TInstructionCharacterNavigation : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
    }
}