using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Collide Exit")]
    [Category("Physics/On Collide Exit")]
    [Description("Executed when the Trigger that collided with a game object, stops colliding")]

    [Image(typeof(IconCollision), ColorTheme.Type.Red)]
    
    [Keywords("Crash", "Touch", "Bump", "Collision", "Stop")]

    [Serializable]
    public class EventCollideExitWith : TEventPhysics
    {
        protected internal override void OnCollisionExit3D(Trigger trigger, Collision collision)
        {
            base.OnCollisionExit3D(trigger, collision);
            
            if (!this.IsActive) return;
            if (!this.Match(collision.gameObject)) return;

            GetGameObjectLastCollidedExit.Instance = collision.gameObject;
            _ = this.m_Trigger.Execute(collision.gameObject);   
        }

        protected internal override void OnCollisionExit2D(Trigger trigger, Collision2D collision)
        {
            base.OnCollisionExit2D(trigger, collision);

            if (!this.IsActive) return;
            if (!this.Match(collision.gameObject)) return;
            
            GetGameObjectLastCollidedExit.Instance = collision.gameObject;
            _ = this.m_Trigger.Execute(collision.gameObject);
        }
    }
}