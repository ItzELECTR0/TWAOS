using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    // Note: This class is excluded from the API documentation, see Documentation > filter.yml.
    //       It is public only to be able to propose the EditorialTrack and the SequenceAssetTrack as public
    //       class in the Sequences API. But it has no need to be exposed in the documentation.

    /// <summary>
    /// Base class for all Track that host clips that controls other PlayableDirector.
    /// </summary>
    [HideInMenu]
    public abstract class NestedTimelineTrack : TrackAsset
    {
        static readonly HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();

        /// <summary>
        /// Gather properties for the Timeline Preview mode. In the case of NestedTimelineTrack, only the
        /// Active state property of GameObject needs to be tracked.
        /// </summary>
        /// <param name="director"></param>
        /// <param name="driver"></param>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            if (director == null)
                return;

            // Avoid recursion
            if (s_ProcessedDirectors.Contains(director))
                return;

            s_ProcessedDirectors.Add(director);
            foreach (var clip in GetClips())
            {
                var clipAsset = clip.asset as NestedTimelinePlayableAsset;
                if (clipAsset == null)
                    continue;

                var resolvedDirector = clipAsset.director.Resolve(director);
                if (resolvedDirector == null)
                    continue;

                Preview(clipAsset, resolvedDirector, driver);
            }
            s_ProcessedDirectors.Remove(director);
        }

        void Preview(NestedTimelinePlayableAsset clipAsset, PlayableDirector subDirector, IPropertyCollector driver)
        {
            if (clipAsset == null || subDirector == null)
                return;

            // Activation
            var gameObject = clipAsset.GetGameObjectToActivate(subDirector);
            driver.AddFromName(gameObject, "m_IsActive");

            // Propagate GatherProperties to sub timelines.
            var timeline = subDirector.playableAsset as TimelineAsset;
            if (timeline == null)
                return;

            timeline.GatherProperties(subDirector, driver);
        }
    }
}
