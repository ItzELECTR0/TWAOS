using System.Collections.Generic;
using UnityEngine.Sequences.Timeline;

namespace UnityEngine.Sequences
{
    public partial class TimelineSequence
    {
        /// <summary>
        /// Returns true if the TimelineSequence has at least one related scene (<seealso cref="GetRelatedScenes"/>).
        /// </summary>
        /// <returns></returns>
        internal bool HasScenes() => GetRelatedScenes().Count > 0;

        /// <summary>
        /// Returns a collection of valid scenes found in the associated Timeline.
        /// It looks for Scene Activation Tracks with a valid reference to a scene.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<string> GetScenes()
        {
            if (IsNullOrEmpty(this))
                return new List<string>();

            return timeline.GetScenes();
        }

        /// <summary>
        /// Gets all the scenes needed to have the scene context for this clip complete. This includes all upstream,
        /// downstream scenes and the scenes of the clip itself.
        /// </summary>
        /// <returns>A collection of Scene paths.</returns>
        /// <remarks>The returned paths are ordered from upstream to downstream.</remarks>
        internal IReadOnlyCollection<string> GetRelatedScenes()
        {
            List<string> paths = new List<string>();

            // Get ascendant scenes
            if (parent != null)
                paths.AddRange((parent as TimelineSequence).GetAscendantScenes());

            // Add local scenes
            paths.AddRange(GetScenes());

            // Append descendant scenes
            foreach (var child in children)
                paths.AddRange((child as TimelineSequence).GetDescendantScenes());

            return paths;
        }

        /// <summary>
        /// Iterates through all clip parents and gets the Scene paths found in their respective timeline.
        /// It also includes the Scene paths from the given clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        IReadOnlyCollection<string> GetAscendantScenes()
        {
            List<string> paths = new List<string>();

            if (parent != null)
                paths.AddRange((parent as TimelineSequence).GetAscendantScenes());

            paths.AddRange(GetScenes());

            return paths;
        }

        /// <summary>
        /// Iterates through all clip children and gets the Scene paths found in their respective timeline.
        /// It also includes the Scene paths from the given clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        IReadOnlyCollection<string> GetDescendantScenes()
        {
            List<string> paths = new List<string>();

            paths.AddRange(GetScenes());

            foreach (var child in children)
                paths.AddRange((child as TimelineSequence).GetDescendantScenes());

            return paths;
        }
    }
}
