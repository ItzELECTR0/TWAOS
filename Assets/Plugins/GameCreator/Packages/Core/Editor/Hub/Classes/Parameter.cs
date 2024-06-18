using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class Parameter
    {
        public string name;
        public string description;

        public Parameter()
        {
            this.name = string.Empty;
            this.description = string.Empty;
        }

        public Parameter(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}