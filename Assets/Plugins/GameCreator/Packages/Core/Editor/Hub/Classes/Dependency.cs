using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class Dependency
    {
        public string id;
        public Version version;

        public Dependency()
        {
            this.id = string.Empty;
            this.version = Version.Zero;
        }

        public Dependency(string id, Version version) : this()
        {
            this.id = id;
            this.version = version;
        }
        
        public Dependency(string id, System.Version version) 
            : this(id, new Version(version.Major, version.Minor, version.Build))
        { }
    }
}