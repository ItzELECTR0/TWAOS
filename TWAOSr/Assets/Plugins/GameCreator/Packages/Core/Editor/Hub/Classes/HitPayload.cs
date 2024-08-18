using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class HitPayload
    {
        public HitData[] values = Array.Empty<HitData>();
    }
}