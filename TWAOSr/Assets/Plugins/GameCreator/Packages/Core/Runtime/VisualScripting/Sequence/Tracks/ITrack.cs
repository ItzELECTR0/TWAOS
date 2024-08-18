using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    public interface ITrack
    {
        int TrackOrder { get; }
        TrackType TrackType { get; }
        
        TrackAddType AllowAdd { get; }
        TrackRemoveType AllowRemove { get; }
        
        IClip[] Clips { get; }

        Color ColorConnectionLeftNormal   { get; }
        Color ColorConnectionMiddleNormal { get; }
        Color ColorConnectionRightNormal  { get; }
        
        Color ColorConnectionLeftSelect   { get; }
        Color ColorConnectionMiddleSelect { get; }
        Color ColorConnectionRightSelect  { get; }
        
        bool IsConnectionLeftThin   { get; }
        bool IsConnectionMiddleThin { get; }
        bool IsConnectionRightThin  { get; }
        
        Color ColorClipNormal { get; }
        Color ColorClipSelect { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        void OnStart(ISequence sequence, Args args);
        void OnComplete(ISequence sequence, Args args);
        void OnCancel(ISequence sequence, Args args);
        void OnUpdate(ISequence sequence, Args args);
    }
}