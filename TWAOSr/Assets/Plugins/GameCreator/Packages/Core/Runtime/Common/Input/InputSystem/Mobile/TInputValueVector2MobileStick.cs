using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TInputValueVector2MobileStick : TInputValueVector2
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected ITouchStick Stick { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        public override void OnStartup()
        {
            this.Enable();
        }

        public override void OnDispose()
        {
            this.Disable();
            
            if (this.Stick?.Root == null) return;
            UnityEngine.Object.Destroy(this.Stick.Root);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Vector2 Read()
        {
            return this.Stick?.Value ?? Vector2.zero;
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected virtual void Enable()
        {
            this.Stick ??= CreateTouchStick();
            this.Stick.Root.SetActive(true);
        }

        protected virtual void Disable()
        {
            if (this.Stick?.Root == null) return;
            this.Stick.Root.SetActive(false);
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract ITouchStick CreateTouchStick();
    }
}