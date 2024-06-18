using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TDetectorListVariable<T> where T : IListVariable
    {
        private enum Detection
        {
            AnyChange = -2,
            SetIndex = -1,
            SetAny = ListVariableRuntime.Change.Set,
            Insert = ListVariableRuntime.Change.Insert,
            Remove = ListVariableRuntime.Change.Remove,
            Move = ListVariableRuntime.Change.Move,
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private T m_Variable;

        [SerializeField] private Detection m_When = Detection.AnyChange;
        [SerializeReference] private TListGetPick m_Index = new GetPickFirst();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Args m_Args;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private int ListenersCount => this.EventOnChange?.GetInvocationList().Length ?? 0;
        
        // EVENTS: --------------------------------------------------------------------------------

        protected event Action EventOnChange; 
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void StartListening(Action callback, Args args)
        {
            this.m_Args = args;
            if (this.m_Variable == null) return;
            
            if (this.ListenersCount == 0)
            {
                this.m_Variable.Register(this.OnChange);
            }
            
            this.EventOnChange += callback;
        }

        public void StopListening(Action callback, Args args)
        {
            this.m_Args = args;
            if (this.m_Variable == null) return;
            
            if (this.ListenersCount == 1)
            {
                this.m_Variable.Unregister(this.OnChange);
            }
            
            this.EventOnChange -= callback;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected void OnChange(ListVariableRuntime.Change change, int index)
        {
            if (this.m_Variable == null) return;
            int count = this.m_Variable.Count;
            
            switch (this.m_When)
            {
                case Detection.AnyChange:
                    this.EventOnChange?.Invoke();
                    break;
                
                case Detection.SetIndex:
                    if (change != ListVariableRuntime.Change.Set) return;
                    if (index == this.m_Index.GetIndex(count, this.m_Args))
                    {
                        this.EventOnChange?.Invoke();
                    }
                    break;
                
                case Detection.SetAny:
                    if (change != ListVariableRuntime.Change.Set) return;
                    this.EventOnChange?.Invoke();
                    break;
                
                case Detection.Insert:
                    if (change != ListVariableRuntime.Change.Insert) return;
                    this.EventOnChange?.Invoke();
                    break;
                
                case Detection.Remove:
                    if (change != ListVariableRuntime.Change.Remove) return;
                    this.EventOnChange?.Invoke();
                    break;
                
                case Detection.Move:
                    if (change != ListVariableRuntime.Change.Move) return;
                    this.EventOnChange?.Invoke();
                    break;
            }
        }
    }
}