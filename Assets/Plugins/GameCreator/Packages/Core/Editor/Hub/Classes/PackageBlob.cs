using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class PackageBlob
    {
        public string id;
        public Package data = null;
    }
}