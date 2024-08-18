using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TDetectorNameVariable<T> where T : INameVariable
    {
        private enum Detection
        {
            Any,
            Name
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private T m_Variable;

        [SerializeField] private Detection m_When = Detection.Any;
        [SerializeField] private IdPathString m_Name;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private int ListenersCount => this.EventOnChange?.GetInvocationList().Length ?? 0;
        
        // EVENTS: --------------------------------------------------------------------------------

        protected event Action<string> EventOnChange; 
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void StartListening(Action<string> callback)
        {
            if (this.m_Variable == null) return;
            if (this.ListenersCount == 0)
            {
                this.m_Variable.Register(this.OnChange);
            }
            
            this.EventOnChange += callback;
        }

        public void StopListening(Action<string> callback)
        {
            if (this.m_Variable == null) return;
            if (this.ListenersCount == 1)
            {
                this.m_Variable.Unregister(this.OnChange);
            }
            
            this.EventOnChange -= callback;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected void OnChange(string name)
        {
            if (this.m_When == Detection.Name)
            {
                string[] split = this.m_Name.String.Split('/');
                if (split[^1] != name) return;
            }
            
            this.EventOnChange?.Invoke(name);
        }
    }
}