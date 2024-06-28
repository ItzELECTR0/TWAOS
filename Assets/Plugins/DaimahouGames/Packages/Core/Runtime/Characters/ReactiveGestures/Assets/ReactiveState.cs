using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    public class ReactiveState : StateAnimation
    {
        [SerializeReference] private INotify[] m_Notifies;
        [SerializeField] private PropertyGetDecimal m_Speed = new(1);
        
        public float LoopDuration => this.m_Controller["Human@Action"].length;

        public IEnumerable<INotify> GetNotifies() => m_Notifies;

        public float GetSpeed(Args args) => (float) m_Speed.Get(args);
    }
}