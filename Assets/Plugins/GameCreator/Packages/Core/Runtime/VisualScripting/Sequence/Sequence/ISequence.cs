using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    public interface ISequence : ICancellable
    {
        TimeMode.UpdateMode UpdateMode { get; }

        float T { get; }
        float Time { get; }
        float Duration { get; }
        
        bool IsRunning { get; }

        // METHODS: -------------------------------------------------------------------------------

        T GetTrack<T>() where T : ITrack;

        float Dilate(float t);
    }
}