using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Trigger Enter")]
    [Category("Physics/On Trigger Enter")]
    [Description("Executed when a game object enters the Trigger collider")]

    [Image(typeof(IconTriggerEnter), ColorTheme.Type.Green)]
    
    [Keywords("Pass", "Through", "Touch", "Collision", "Collide")]

    [Serializable]
    public class EventTriggerEnter : TEventPhysics
    {
        protected internal override void OnTriggerEnter3D(Trigger trigger, Collider collider)
        {
            base.OnTriggerEnter3D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
        
        protected internal override void OnTriggerEnter2D(Trigger trigger, Collider2D collider)
        {
            base.OnTriggerEnter2D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!this.Match(collider.gameObject)) return;
            
            GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
    }
}