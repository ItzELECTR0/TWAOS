using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    public interface IClip
    {
        float TimeStart { get; }
        float TimeEnd { get; }
        float Duration { get; }

        bool IsStart { get; }
        bool IsComplete { get; }

        // METHODS: -------------------------------------------------------------------------------

        void Reset();
        void Start(ITrack track, Args args);
        void Complete(ITrack track, Args args);
        void Cancel(ITrack track, Args args);
        void Update(ITrack track, Args args, float t);
    }
}