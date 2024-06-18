using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class HitData
    {
        public string objectID;
        public string id;
        public string name;
        public string description;
        public string category;
        public string filename;
        
        public Dependency[] dependencies = Array.Empty<Dependency>();
        public Parameter[] parameters = Array.Empty<Parameter>();
        
        public int likes;
        public string image;

        public string type;
        public Version version = Version.Zero;
        public string[] keywords = Array.Empty<string>();
        public RenderPipelines renderPipelines;

        public Date dateCreate;
        public Date dateUpdate;
    }
}