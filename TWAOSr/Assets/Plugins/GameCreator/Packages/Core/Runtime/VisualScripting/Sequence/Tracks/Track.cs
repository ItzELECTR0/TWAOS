using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class Track : ITrack
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public virtual int TrackOrder => 1;
        public virtual TrackType TrackType => TrackType.Single;

        public virtual TrackAddType AllowAdd => TrackAddType.Allow;
        public virtual TrackRemoveType AllowRemove => TrackRemoveType.Allow;
        
        public abstract IClip[] Clips { get; }

        public virtual Color ColorConnectionLeftNormal => default;
        public virtual Color ColorConnectionMiddleNormal => default;
        public virtual Color ColorConnectionRightNormal => default;
        
        public virtual Color ColorConnectionLeftSelect => default;
        public virtual Color ColorConnectionMiddleSelect => default;
        public virtual Color ColorConnectionRightSelect => default;

        public virtual bool IsConnectionLeftThin => false;
        public virtual bool IsConnectionMiddleThin => false;
        public virtual bool IsConnectionRightThin => false;

        public virtual Color ColorClipNormal => ColorTheme.Get(ColorTheme.Type.TextLight);
        public virtual Color ColorClipSelect => ColorTheme.Get(ColorTheme.Type.TextNormal);

        public virtual Texture CustomClipIconNormal => null;
        public virtual Texture CustomClipIconSelect => null;
        
        public virtual bool HasInspector => true;

        // TRACK METHODS: -------------------------------------------------------------------------
        
        void ITrack.OnStart(ISequence sequence, Args args)
        {
            foreach (IClip clip in this.Clips)
            {
                clip?.Reset();
            }
        }

        void ITrack.OnComplete(ISequence sequence, Args args)
        {
            foreach (IClip clip in this.Clips)
            {
                if (!clip.IsComplete)
                {
                    clip.Complete(this, args);
                }
            }
        }

        void ITrack.OnCancel(ISequence sequence, Args args)
        {
            foreach (IClip clip in this.Clips)
            {
                clip?.Cancel(this, args);
            }
        }

        void ITrack.OnUpdate(ISequence sequence, Args args)
        {
            float t = sequence.T;
            
            foreach (IClip clip in this.Clips)
            {
                float clipTimeStart = sequence.Dilate(clip.TimeStart);
                float clipTimeEnd = sequence.Dilate(clip.TimeEnd);
                
                if (t >= clipTimeStart)
                {
                    if (clip.IsStart == false)
                    {
                        clip.Start(this, args);
                        clip.Update(this, args, this.CalculateT(clip, sequence, t));
                    }
                    else if (t <= clipTimeEnd)
                    {
                        clip.Update(this, args, this.CalculateT(clip, sequence, t));
                    }
                    else if (clip.IsComplete == false)
                    {
                        clip.Complete(this, args);
                    }
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private float CalculateT(IClip clip, ISequence sequence, float t)
        {
            float clipTimeStart = sequence.Dilate(clip.TimeStart);
            float clipTimeEnd = sequence.Dilate(clip.TimeEnd);

            if (clipTimeStart >= clipTimeEnd) return 1f;
            return (t - clipTimeStart) / (clipTimeEnd - clipTimeStart);
        }
    }
}