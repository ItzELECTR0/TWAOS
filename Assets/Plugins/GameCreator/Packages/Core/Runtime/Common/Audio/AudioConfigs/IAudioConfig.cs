using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    public interface IAudioConfig
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        float Volume { get; }
        float Pitch { get; }

        float TransitionIn { get; }
        float SpatialBlend { get; }
        
        TimeMode.UpdateMode UpdateMode { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        GameObject GetTrackTarget(Args args);
    }
}