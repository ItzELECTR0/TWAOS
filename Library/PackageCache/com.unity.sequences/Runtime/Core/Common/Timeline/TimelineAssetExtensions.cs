using System.Collections.Generic;
using System.Linq;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// A collection of helpers to manipulate <see cref="TimelineAsset"/> instances.
    /// </summary>
    internal static partial class TimelineAssetExtensions
    {
        /// <summary>
        /// Gets a track of type T with the specified <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of the track to look for.</typeparam>
        /// <param name="asset">The instance of <see cref="TimelineAsset"/> to look into.</param>
        /// <param name="name">The name the TrackAsset has in Timeline.</param>
        /// <returns>Null when no matching track is found.</returns>
        static T GetTrack<T>(this TimelineAsset asset, string name) where T : TrackAsset
        {
            var allTracks = asset.GetRootTracks().Concat(asset.GetOutputTracks());
            foreach (var item in allTracks)
            {
                if (item is T && item.name == name)
                    return item as T;
            }

            return null;
        }

        /// <summary>
        /// Gets a track of type T with the specified <paramref name="name"/>.
        /// Creates a new track with the specified <paramref name="name"/> if none is found.
        /// </summary>
        /// <typeparam name="T">The type of the track to look for.</typeparam>
        /// <param name="asset">The instance of <see cref="TimelineAsset"/> to look into.</param>
        /// <param name="name">The name the TrackAsset has in Timeline.</param>
        /// <returns>A valid instance of a <see cref="TrackAsset"/> of type T.</returns>
        internal static T GetOrCreateTrack<T>(this TimelineAsset asset, string name)
            where T : TrackAsset, new()
        {
            return asset.GetTrack<T>(name) ?? asset.CreateTrack<T>(name);
        }

        /// <summary>
        /// Gets a collection of <see cref="SequenceAssetPlayableAsset"/> found in this instance of TimelineAsset.
        /// </summary>
        /// <param name="timeline">The instance of <see cref="TimelineAsset"/> to look into.</param>
        /// <returns>An enumerator on <see cref="TimelineClip"/> found in this instance.</returns>
        internal static IEnumerable<TimelineClip> GetSequenceAssetClips(this TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (!(track is SequenceAssetTrack))
                    continue;

                foreach (var clip in track.GetClips())
                {
                    var clipAsset = clip.asset as SequenceAssetPlayableAsset;
                    if (clipAsset != null)
                        yield return clip;
                }
            }
        }

        internal static IEnumerable<EditorialTrack> GetEditorialTracks(this TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (!(track is EditorialTrack))
                    continue;

                yield return track as EditorialTrack;
            }
        }

        internal static IEnumerable<TimelineClip> GetEditorialClips(this TimelineAsset timeline)
        {
            foreach (var track in timeline.GetEditorialTracks())
            {
                foreach (var clip in track.GetClips())
                {
                    if (clip.asset as EditorialPlayableAsset == null)
                        continue;

                    yield return clip;
                }
            }
        }

        /// <summary>
        /// Gets the frame rate value assigned to this instance of <see cref="TimelineAsset"/>.
        /// </summary>
        /// <param name="timeline">The instance of <see cref="TimelineAsset"/>.</param>
        /// <returns>The frame rate value of this instance.</returns>
        internal static double GetFrameRate(this TimelineAsset timeline)
        {
            return timeline.editorSettings.frameRate;
        }

        /// <summary>
        /// Sets the frame rate value assigned to this instance of <see cref="TimelineAsset"/>.
        /// </summary>
        /// <param name="timeline">The instance of <see cref="TimelineAsset"/> this method applies to.</param>
        /// <param name="fps">The frame rate value to assign.</param>
        internal static void SetFrameRate(this TimelineAsset timeline, double fps)
        {
            timeline.editorSettings.frameRate = fps;
        }
    }
}
