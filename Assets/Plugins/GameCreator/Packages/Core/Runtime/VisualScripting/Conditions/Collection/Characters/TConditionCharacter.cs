using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Character", "The Character instance referenced in the condition")]
    
    [Keywords("Character", "Player")]
    
    [Serializable]
    public abstract class TConditionCharacter : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
    }
}
