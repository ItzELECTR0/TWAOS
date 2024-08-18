using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Trigger Exit")]
    [Category("Physics/On Trigger Exit")]
    [Description("Executed when a game object leaves the Trigger collider")]

    [Image(typeof(IconTriggerExit), ColorTheme.Type.Red)]
    
    [Keywords("Leave", "Through", "Touch", "Collision", "Collide")]

    [Serializable]
    public class EventTriggerExit : TEventPhysics
    {
        protected internal override void OnTriggerExit3D(Trigger trigger, Collider collider)
        {
            base.OnTriggerExit3D(trigger, collider);

            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            GetGameObjectLastTriggerExit.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }

        protected internal override void OnTriggerExit2D(Trigger trigger, Collider2D collider)
        {
            base.OnTriggerExit2D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            GetGameObjectLastTriggerExit.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
    }
}