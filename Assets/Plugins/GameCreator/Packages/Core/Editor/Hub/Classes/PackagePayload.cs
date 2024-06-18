using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class PackagePayload
    {
        public PackageBlob[] values = Array.Empty<PackageBlob>();
    }
}