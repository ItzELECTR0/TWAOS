using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true
    )]
    public class ParameterAttribute : Attribute, ISearchable
    {
        public string Name { get; }
        public string Description { get; }

        public ParameterAttribute(string name, string description)
        {
            this.Name = name.Trim();
            this.Description = description.Trim();
        }

        public string SearchText => this.Name;
        public int SearchPriority => 2;
    }
}
