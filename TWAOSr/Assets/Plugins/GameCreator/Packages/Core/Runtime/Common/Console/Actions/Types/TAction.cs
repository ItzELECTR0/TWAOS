using System;

namespace GameCreator.Runtime.Console
{
    public abstract class TAction<T> : IAction
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public string Name { get; }
        [field: NonSerialized] public string Description { get; }

        [field: NonSerialized] private Func<string, T> Execute { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TAction(string name, string description, Func<string, T> method)
        {
            this.Name = name.ToLowerInvariant();
            this.Description = description;
            this.Execute = method;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Run(string value)
        {
            return this.Execute != null 
                ? this.Execute.Invoke(value)
                : default;
        }
    }
}