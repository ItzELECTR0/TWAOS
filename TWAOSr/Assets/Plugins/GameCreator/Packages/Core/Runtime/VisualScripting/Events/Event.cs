using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconCircleOutline), ColorTheme.Type.Pink)]

    [Serializable]
    public abstract class Event
    {
        protected Trigger m_Trigger;

        // PROPERTIES: ----------------------------------------------------------------------------

        public virtual bool RequiresCollider => false;
        public virtual Type RequiresComponent => null;

        protected GameObject Self => this.m_Trigger.gameObject;
        protected bool IsActive => this.m_Trigger.isActiveAndEnabled;

        // INITIALIZER METHODS: -------------------------------------------------------------------

        protected internal virtual void OnAwake(Trigger trigger)
        {
            this.m_Trigger = trigger;
        }

        protected internal virtual void OnStart(Trigger trigger)
        { }

        protected internal virtual void OnEnable(Trigger trigger)
        { }

        protected internal virtual void OnDisable(Trigger trigger)
        { }

        protected internal virtual void OnDestroy(Trigger trigger)
        { }

        protected internal virtual void OnBecameInvisible(Trigger trigger)
        { }

        protected internal virtual void OnBecameVisible(Trigger trigger)
        { }

        // UPDATE METHODS: ------------------------------------------------------------------------

        protected internal virtual void OnUpdate(Trigger trigger)
        { }

        protected internal virtual void OnLateUpdate(Trigger trigger)
        { }
        
        protected internal virtual void OnFixedUpdate(Trigger trigger)
        { }

        // BACKGROUND METHODS: --------------------------------------------------------------------

        protected internal virtual void OnApplicationFocus(Trigger trigger, bool hasFocus)
        { }

        protected internal virtual void OnApplicationPause(Trigger trigger, bool pauseStatus)
        { }

        protected internal virtual void OnApplicationQuit(Trigger trigger)
        { }

        // PHYSICS 3D: ----------------------------------------------------------------------------

        protected internal virtual void OnCollisionEnter3D(Trigger trigger, Collision collision)
        { }

        protected internal virtual void OnCollisionExit3D(Trigger trigger, Collision collision)
        { }

        protected internal virtual void OnCollisionStay3D(Trigger trigger, Collision collision)
        { }

        protected internal virtual void OnTriggerEnter3D(Trigger trigger, Collider collider)
        { }

        protected internal virtual void OnTriggerExit3D(Trigger trigger, Collider collider)
        { }

        protected internal virtual void OnTriggerStay3D(Trigger trigger, Collider collider)
        { }

        protected internal virtual void OnJointBreak3D(Trigger trigger, float breakForce)
        { }

        // PHYSICS 2D: ----------------------------------------------------------------------------

        protected internal virtual void OnCollisionEnter2D(Trigger trigger, Collision2D collision)
        { }

        protected internal virtual void OnCollisionExit2D(Trigger trigger, Collision2D collision)
        { }

        protected internal virtual void OnCollisionStay2D(Trigger trigger, Collision2D collision)
        { }

        protected internal virtual void OnTriggerEnter2D(Trigger trigger, Collider2D collider)
        { }

        protected internal virtual void OnTriggerExit2D(Trigger trigger, Collider2D collider)
        { }

        protected internal virtual void OnTriggerStay2D(Trigger trigger, Collider2D collider)
        { }

        protected internal virtual void OnJointBreak2D(Trigger trigger, Joint2D joint)
        { }

        // INPUT EVENTS: --------------------------------------------------------------------------

        protected internal virtual void OnMouseDown(Trigger trigger)
        { }

        protected internal virtual void OnMouseUp(Trigger trigger)
        { }

        protected internal virtual void OnMouseUpAsButton(Trigger trigger)
        { }

        protected internal virtual void OnMouseEnter(Trigger trigger)
        { }

        protected internal virtual void OnMouseOver(Trigger trigger)
        { }

        protected internal virtual void OnMouseExit(Trigger trigger)
        { }

        protected internal virtual void OnMouseDrag(Trigger trigger)
        { }
        
        // UI: ------------------------------------------------------------------------------------
        
        protected internal virtual void OnPointerEnter(Trigger trigger)
        { }
        
        protected internal virtual void OnPointerExit(Trigger trigger)
        { }
        
        protected internal virtual void OnSelect(Trigger trigger)
        { }
        
        protected internal virtual void OnDeselect(Trigger trigger)
        { }
        
        // GIZMOS: --------------------------------------------------------------------------------

        protected internal virtual void OnDrawGizmos(Trigger trigger)
        { }
        
        protected internal virtual void OnDrawGizmosSelected(Trigger trigger)
        { }
        
        // CUSTOM CALLBACKS: ----------------------------------------------------------------------

        protected internal virtual void OnReceiveSignal(Trigger trigger, SignalArgs args)
        { }
        
        [Obsolete("Soon to deprecate. Use OnReceiveCommand(trigger, CommandArgs)")]
        protected internal virtual void OnReceiveCommand(Trigger trigger, PropertyName command)
        { }
        
        protected internal virtual void OnReceiveCommand(Trigger trigger, CommandArgs args)
        { }
        
        protected internal virtual void OnInteract(Trigger trigger, Character character)
        { }
    }
}