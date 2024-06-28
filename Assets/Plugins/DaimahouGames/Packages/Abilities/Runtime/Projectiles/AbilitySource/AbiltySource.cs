using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    public struct AbiltySource
    {
        private GameObject m_GameObject;

        public GameObject GameObject => m_GameObject;
        public Pawn Pawn => m_GameObject ? m_GameObject.Get<Pawn>() : null;
        
        public AbiltySource(Pawn pawn)
        {
            m_GameObject = pawn.gameObject;
        }
        
        public AbiltySource(GameObject gameObject)
        {
            m_GameObject = gameObject;
        }
    }
}