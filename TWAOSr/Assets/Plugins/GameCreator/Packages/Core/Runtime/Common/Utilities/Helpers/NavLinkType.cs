using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class NavLinkType
    {
        public enum LinkType
        {
            Manual = OffMeshLinkType.LinkTypeManual,
            Drop = OffMeshLinkType.LinkTypeDropDown,
            Jump = OffMeshLinkType.LinkTypeJumpAcross
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private LinkType m_LinkType = LinkType.Jump;
        [SerializeField] private NavAreaMask m_ForAreas = new NavAreaMask();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private OffMeshLinkData m_LastLinkData;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(NavMeshAgent agent)
        {
            if (agent == null) return;
            if (!agent.isOnOffMeshLink) return;
            if (!agent.currentOffMeshLinkData.valid) return;
            
            this.m_LastLinkData = agent.currentOffMeshLinkData;
        }
        
        public bool Match(NavMeshAgent agent)
        {
            if (agent == null) return false;
            if (!this.m_LastLinkData.valid) return false;

            if (this.m_LastLinkData.linkType != (OffMeshLinkType) this.m_LinkType) return false;
            if (this.m_LastLinkData.linkType != OffMeshLinkType.LinkTypeManual) return true;
            
            if (this.m_LastLinkData.offMeshLink == null) return false;
            return (1 << this.m_LastLinkData.offMeshLink.area & this.m_ForAreas.Mask) > 0;
        }
    }
}