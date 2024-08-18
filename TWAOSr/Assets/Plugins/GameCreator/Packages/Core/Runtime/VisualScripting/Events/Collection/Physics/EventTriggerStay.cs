using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Trigger Stay")]
    [Category("Physics/On Trigger Stay")]
    [Description("Executed while a game object stays inside the Trigger collider")]

    [Image(typeof(IconTriggerStay), ColorTheme.Type.Blue)]
    
    [Keywords("Pass", "Through", "Touch", "Collision", "Collide")]

    [Serializable]
    public class EventTriggerStay : TEventPhysics
    {
        protected internal override void OnTriggerStay3D(Trigger trigger, Collider collider)
        {
            base.OnTriggerStay3D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            _ = this.m_Trigger.Execute(collider.gameObject);   
        }

        protected internal override void OnTriggerStay2D(Trigger trigger, Collider2D collider)
        {
            base.OnTriggerStay2D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
    }
}