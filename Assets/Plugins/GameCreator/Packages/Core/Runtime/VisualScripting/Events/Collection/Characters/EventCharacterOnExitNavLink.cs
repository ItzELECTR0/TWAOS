using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On NavLink Exit")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Red, typeof(OverlayArrowLeft))]
    
    [Category("Characters/Navigation/On NavLink Exit")]
    [Description("Executed when a character exists a navigation mesh Off Mesh Link")]

    [Serializable]
    public class EventCharacterOnExitNavLink : TEventCharacterNavLink
    {
        // METHODS: -------------------------------------------------------------------------------

        protected override void OnExitLink(NavMeshAgent agent)
        {
            base.OnExitLink(agent);
            if (!this.m_LinkType.Match(agent)) return;
            
            Args args = new Args(this.m_Trigger.gameObject, agent.gameObject);
            _ = this.m_Trigger.Execute(args);
        }
    }
}