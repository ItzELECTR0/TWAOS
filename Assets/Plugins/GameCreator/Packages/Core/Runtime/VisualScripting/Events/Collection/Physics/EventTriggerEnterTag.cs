using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Trigger Enter Tag")]
    [Category("Physics/On Trigger Enter Tag")]
    [Description("Executed when a game object with a Tag enters the Trigger collider")]

    [Parameter("Tag", "A string that represents a group of game objects")]
    [Image(typeof(IconTag), ColorTheme.Type.Green)]
    
    [Keywords("Pass", "Through", "Touch", "Collision", "Collide")]

    [Serializable]
    public class EventTriggerEnterTag : Event
    {
        [SerializeField] private TagValue m_Tag = new TagValue();
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            trigger.RequireRigidbody();
        }
        
        protected internal override void OnTriggerEnter3D(Trigger trigger, Collider collider)
        {
            base.OnTriggerEnter3D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!collider.gameObject.CompareTag(this.m_Tag.Value)) return;
            
            GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
        
        protected internal override void OnTriggerEnter2D(Trigger trigger, Collider2D collider)
        {
            base.OnTriggerEnter2D(trigger, collider);
            
            if (!this.IsActive) return;
            if (!collider.gameObject.CompareTag(this.m_Tag.Value)) return;
            
            GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
            _ = this.m_Trigger.Execute(collider.gameObject);
        }
    }
}