using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Collide")]
    [Category("Physics/On Collide")]
    [Description("Executed when the Trigger collides with a game object")]

    [Image(typeof(IconCollision), ColorTheme.Type.Blue)]
    
    [Keywords("Crash", "Touch", "Bump", "Collision")]

    [Serializable]
    public class EventCollideWith : TEventPhysics
    {
        protected internal override void OnCollisionEnter3D(Trigger trigger, Collision collision)
        {
            base.OnCollisionEnter3D(trigger, collision);
            
            if (!this.IsActive) return;
            if (!this.Match(collision.gameObject)) return;

            GetGameObjectLastCollidedEnter.Instance = collision.gameObject;
            _ = this.m_Trigger.Execute(collision.gameObject);
        }

        protected internal override void OnCollisionEnter2D(Trigger trigger, Collision2D collision)
        {
            base.OnCollisionEnter2D(trigger, collision);

            if (!this.IsActive) return;
            if (!this.Match(collision.gameObject)) return;
            
            GetGameObjectLastCollidedEnter.Instance = collision.gameObject;
            _ = this.m_Trigger.Execute(collision.gameObject);
        }
    }
}