using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class DriverNavmeshAgentType
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private int m_AgentTypeIndex;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int AgentType => this.m_AgentTypeIndex < NavMesh.GetSettingsCount() 
            ? NavMesh.GetSettingsByIndex(this.m_AgentTypeIndex).agentTypeID 
            : NavMesh.GetSettingsByIndex(0).agentTypeID;
    }
}