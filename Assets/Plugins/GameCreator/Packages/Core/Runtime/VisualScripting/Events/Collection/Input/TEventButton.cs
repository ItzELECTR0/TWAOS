using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Button", "The button that triggers the event")]
    [Parameter(
        "Min Distance", 
        "If set to None, the input acts globally. If set to Game Object, the event " +
        "only fires if the target object is within the specified radius"
    )]

    // TODO: [10/3/2023] Remove in a year
    [MovedFrom(false, null, null, "TEventInput")]
    
    [Serializable]
    public abstract class TEventButton : Event
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private InputPropertyButton m_Button = InputButtonJump.Create();

        [SerializeField]
        private CompareMinDistanceOrNone m_MinDistance = new CompareMinDistanceOrNone();

        // METHODS: -------------------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            this.m_Button.OnStartup();
        }

        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            this.m_Button.RegisterPerform(this.OnInput);
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            this.m_Button.ForgetPerform(this.OnInput);
        }

        protected internal override void OnDestroy(Trigger trigger)
        {
            base.OnDestroy(trigger);
            this.m_Button.OnDispose();
        }

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            this.m_Button.OnUpdate();
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void Execute()
        {
            bool matchDistance = this.m_MinDistance.Match(
                this.m_Trigger.transform,
                new Args(this.Self)
            );
            
            if (!matchDistance) return;
            _ = this.m_Trigger.Execute(this.Self);
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void OnInput()
        { }
        
        // GIZMOS: --------------------------------------------------------------------------------

        protected internal override void OnDrawGizmosSelected(Trigger trigger)
        {
            base.OnDrawGizmosSelected(trigger);
            this.m_MinDistance.OnDrawGizmos(
                trigger.transform,
                new Args(trigger.gameObject)
            );
        }
    }
}