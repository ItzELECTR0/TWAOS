using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class Condition : TPolymorphicItem<Condition>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField][HideInInspector]
        private bool m_Sign = true;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public sealed override string Title => string.Format(
            "{0} {1}", 
            this.m_Sign ? "If" : "Not", 
            this.Summary
        );

        protected virtual string Summary => TextUtils.Humanize(this.GetType().ToString());

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Check(Args args)
        {
            if (!this.IsEnabled) return this.m_Sign;
            if (this.Breakpoint) Debug.Break();
            
            return this.m_Sign ? this.Run(args) : !this.Run(args);
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected abstract bool Run(Args args);
    }
}