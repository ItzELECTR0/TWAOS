using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On NavLink Enter")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Category("Characters/Navigation/On NavLink Enter")]
    [Description("Executed when a character enters a navigation mesh Off Mesh Link")]

    [Serializable]
    public class EventCharacterOnEnterNavLink : TEventCharacterNavLink
    {
        // METHODS: -------------------------------------------------------------------------------

        protected override void OnEnterLink(NavMeshAgent agent)
        {
            base.OnEnterLink(agent);
            if (!this.m_LinkType.Match(agent)) return;
            
            Args args = new Args(this.m_Trigger.gameObject, agent.gameObject);
            _ = this.m_Trigger.Execute(args);
        }
    }
}