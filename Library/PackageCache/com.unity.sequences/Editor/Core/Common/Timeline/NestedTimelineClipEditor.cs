using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Sequences.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    /// <summary>
    /// Base class for all Timeline ClipEditor that are derivation of a NestedTimeline.
    /// </summary>
    class NestedTimelineClipEditor : ClipEditor
    {
        /// <inheritdoc/>
        /// <remarks>Update the clip asset sub timeline lenght so that timeline correctly display when the clip as a
        /// duration that is different from it's content duration.</remarks>
        public override void OnClipChanged(TimelineClip clip)
        {
            AdjustSubTimelineLength(clip, out _, out _);
        }

        /// <inheritdoc/>
        public override void GetSubTimelines(
            TimelineClip clip,
            PlayableDirector director,
            List<PlayableDirector> subTimelines)
        {
            var subDirector = GetControlledPlayableDirector(clip, director);
            if (subDirector != null && subDirector.playableAsset != null)
                subTimelines.Add(subDirector);
        }

        /// <summary>
        /// Set the clip subTimelineLength. This is to ensure that clip.duration is accurate and will allow correct UI
        /// feedback (showing when the clip is shorter or longer than it's actual data content).
        /// </summary>
        /// <param name="clip">The TimelineClip for which to adjust the sub timeline length if possible.</param>
        /// <param name="subTimeline">The found controlled TimelineAsset or null.</param>
        /// <param name="clipAsset">The found NestedTimelinePlayableAsset or null.</param>
        /// <seealso cref="OnClipChanged"/>
        protected void AdjustSubTimelineLength(TimelineClip clip, out TimelineAsset subTimeline, out NestedTimelinePlayableAsset clipAsset)
        {
            var subDirector = GetControlledPlayableDirector(clip, TimelineEditor.inspectedDirector);
            if (subDirector == null || (subDirector.playableAsset as TimelineAsset) == null)
            {
                subTimeline = null;
                clipAsset = null;
                return;
            }

            subTimeline = subDirector.playableAsset as TimelineAsset;
            clipAsset = clip.asset as NestedTimelinePlayableAsset;

            clipAsset.subTimelineLength = subTimeline.duration;
        }

        /// <summary>
        /// Get the PlayableDirector controlled by the given clip.
        /// </summary>
        /// <param name="clip">Get the PlayableDirector controlled by this clip.</param>
        /// <param name="director">The PlayableDirector that control the timeline in which the clip is.</param>
        /// <returns>The PlayableDirector controlled by the given clip.</returns>
        PlayableDirector GetControlledPlayableDirector(TimelineClip clip, PlayableDirector director)
        {
            if (director == null)
                return null;

            var clipAsset = clip.asset as NestedTimelinePlayableAsset;
            if (clipAsset == null)
                return null;

            return clipAsset.director.Resolve(director);
        }
    }
}
