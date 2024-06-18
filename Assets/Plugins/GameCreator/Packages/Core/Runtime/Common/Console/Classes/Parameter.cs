using System;

namespace GameCreator.Runtime.Console
{
    public readonly struct Parameter
    {
        public const char QUOTES = '"';
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public string Name { get; }
        [field: NonSerialized] public string Value { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Parameter(string name, string value)
        {
            this.Name = name.ToLowerInvariant();
            this.Value = value;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            string name = this.Name.Contains(" ") ? $"{QUOTES}{this.Name}{QUOTES}" : this.Name;
            string value = this.Value.Contains(" ") ? $"{QUOTES}{this.Value}{QUOTES}" : this.Value;
            
            return $"{name} {value}";
        }
    }
}