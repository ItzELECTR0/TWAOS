using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true
    )]
    public class DependencyAttribute : Attribute
    {
        public string ID { get; }
        public Version Version { get; }

        public DependencyAttribute(string id, int x, int y, int z)
        {
            this.ID = id;
            this.Version = new Version(x, y, z);
        }
    }
}