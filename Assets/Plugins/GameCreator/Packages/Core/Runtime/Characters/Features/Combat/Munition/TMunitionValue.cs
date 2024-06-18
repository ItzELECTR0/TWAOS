using System;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TMunitionValue : ICloneable
    {
        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventChange;
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void ExecuteEventChange()
        {
            this.EventChange?.Invoke();
        }
        
        // EDITOR METHODS: ------------------------------------------------------------------------

        public abstract override string ToString();

        // CLONE: ---------------------------------------------------------------------------------
        
        public abstract object Clone();
    }
}