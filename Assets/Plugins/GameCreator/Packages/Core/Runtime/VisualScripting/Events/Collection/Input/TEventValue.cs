using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Keywords("Left", "Right", "Down", "Up", "Press", "Move", "Direction")]
    [Keywords("Keyboard", "Mouse", "Button", "Gamepad", "Controller", "Joystick")]
    
    [Parameter("Value", "The Input value read")]
    [Parameter("Compare", "The comparison applied to the input value")]
    [Parameter(
        "Min Distance", 
        "If set to None, the input acts globally. If set to Game Object, the event " +
        "only fires if the target object is within the specified radius"
    )]

    [Serializable]
    public abstract class TEventValue : Event
    {
        private const float EPSILON = 0.01f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Args m_Args;
        [NonSerialized] private bool m_Used;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected abstract float Value { get; }
        
        protected abstract CompareMinDistanceOrNone MinDistance { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            this.m_Args = new Args(trigger);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void CheckExecute()
        {
            if (Math.Abs(this.Value) >= EPSILON) this.Execute();
            else this.m_Used = false;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Execute()
        {
            if (this.m_Used) return;
            
            bool matchDistance = this.MinDistance.Match(
                this.m_Trigger.transform,
                this.m_Args
            );
            
            if (!matchDistance) return;

            this.m_Used = true;
            _ = this.m_Trigger.Execute(this.Self);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        protected internal override void OnDrawGizmosSelected(Trigger trigger)
        {
            base.OnDrawGizmosSelected(trigger);
            this.MinDistance.OnDrawGizmos(
                trigger.transform,
                this.m_Args
            );
        }
    }
}