using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class Package
    {
        public string id;
        public string name;
        public string description;
        public string category;

        public string type;
        public string user;

        public string filename;
        public string content;

        public Version version = Version.Zero;
        public string[] keywords = Array.Empty<string>();

        public Date dateCreate;
        public Date dateUpdate;

        public Dependency[] dependencies = Array.Empty<Dependency>();
        public Parameter[] parameters = Array.Empty<Parameter>();
        public string[] examples = Array.Empty<string>();
        public RenderPipelines renderPipelines;
    }
}