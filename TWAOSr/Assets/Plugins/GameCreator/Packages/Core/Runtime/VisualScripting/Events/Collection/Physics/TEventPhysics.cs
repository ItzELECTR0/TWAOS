using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class TEventPhysics : Event
    {
        // MEMBERS: -------------------------------------------------------------------------------

        private Args m_ArgsCollider;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private CompareGameObjectOrAny m_Collider = new CompareGameObjectOrAny();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override bool RequiresCollider => true;

        protected GameObject Collider => m_Collider.Get(this.m_ArgsCollider);

        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            
            this.m_ArgsCollider = new Args(trigger.gameObject);
            trigger.RequireRigidbody();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool Match(GameObject gameObject)
        {
            return this.m_Collider.Match(gameObject, this.m_ArgsCollider);
        }
    }
}