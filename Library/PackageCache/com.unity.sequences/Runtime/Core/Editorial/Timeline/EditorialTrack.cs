using System;
using System.ComponentModel;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// EditorialTrack support EditorialPlayableAsset that drive the PlayableDirector of a Sequence.
    /// </summary>
    [Serializable]
    [DisplayName("Sequencing/Editorial Track")]
    [TrackColor(0.058f, 0.462f, 0.588f)]
    [TrackClipType(typeof(EditorialPlayableAsset), false)]
    public class EditorialTrack : NestedTimelineTrack
    {
        /// <summary>
        /// Get the effective duration of the track by looking at the clips end time.
        /// </summary>
        /// <returns>The effective duration (double) of the track.</returns>
        internal double GetActualDuration()
        {
            var duration = 0.0;
            foreach (var clip in GetClips())
            {
                duration = Math.Max(duration, clip.end);
            }

            return duration;
        }

        /// <summary>
        /// Updates the duration of either a single clip or all clips of a track to match their sub-editorial content.
        /// </summary>
        /// <param name="director">The PlayableDirector that owns the track's timeline.</param>
        /// <param name="clip">A specific TimelineClip to update. If you specify no clip, the method updates all clips
        /// of the track.</param>
        internal void UpdateClipDurationToMatchEditorialContent(PlayableDirector director, TimelineClip clip = null)
        {
            var delta = 0.0;
            foreach (var trackClip in GetClips())
            {
                var clipAsset = trackClip.asset as EditorialPlayableAsset;
                if (clipAsset == null)
                    continue;

                trackClip.start += delta;

                if (clip != null && clip != trackClip)
                    continue;

                var newDuration = clipAsset.GetEditorialContentDuration(director);
                newDuration -= trackClip.clipIn;
                if (newDuration < 0.00001)
                    continue;

                delta += newDuration - trackClip.duration;
                trackClip.duration = newDuration;
            }
        }
    }
}
