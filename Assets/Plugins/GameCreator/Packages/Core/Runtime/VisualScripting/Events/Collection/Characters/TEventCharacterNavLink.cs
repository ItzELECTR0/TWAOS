using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AI;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class TEventCharacterNavLink : Event
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] protected NavLinkType m_LinkType = new NavLinkType();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private NavMeshAgent m_PreviousAgent;
        [NonSerialized] private bool m_WasOnMeshLink;

        // METHODS: -------------------------------------------------------------------------------

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            
            NavMeshAgent agent = this.m_Character.Get<NavMeshAgent>(trigger.gameObject);
            if (agent == null) return;

            this.m_LinkType.Update(agent);
            if (agent != this.m_PreviousAgent)
            {
                this.m_WasOnMeshLink = false;
                this.m_PreviousAgent = agent;
            }

            if (this.m_WasOnMeshLink && !agent.isOnOffMeshLink)
            {
                this.m_WasOnMeshLink = false;
                this.OnExitLink(agent);
            }

            if (!this.m_WasOnMeshLink && agent.isOnOffMeshLink)
            {
                this.m_WasOnMeshLink = true;
                this.OnEnterLink(agent);
            }
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void OnEnterLink(NavMeshAgent agent)
        { }
        
        protected virtual void OnExitLink(NavMeshAgent agent)
        { }
    }
}