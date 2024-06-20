using System.Collections.Generic;
using UnityEngine.Sequences.Timeline;

namespace UnityEditor.Sequences
{
    internal partial class SequenceNode
    {
        /// <summary>
        /// Returns true if the TimelineSequence has at least one related scene (<seealso cref="GetRelatedScenes"/>).
        /// </summary>
        /// <returns></returns>
        internal bool HasScenes() => GetRelatedScenes().Count > 0;

        /// <summary>
        /// Gets all the scenes needed to have the scene context for this clip complete. This includes all upstream,
        /// downstream scenes and the scenes of the clip itself.
        /// </summary>
        /// <returns>A collection of Scene paths.</returns>
        /// <remarks>The returned paths are ordered from upstream to downstream.</remarks>
        public IReadOnlyCollection<string> GetRelatedScenes()
        {
            List<string> paths = new List<string>();

            // Get ascendant scenes
            if (m_Parent != null)
                paths.AddRange(m_Parent.GetAscendantScenes());

            // Add local scenes
            paths.AddRange(m_Timeline.GetScenes());

            // Append descendant scenes
            foreach (var child in m_Children)
                paths.AddRange(child.GetDescendantScenes());

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

            if (m_Parent != null)
                paths.AddRange(m_Parent.GetAscendantScenes());

            paths.AddRange(m_Timeline.GetScenes());

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

            paths.AddRange(m_Timeline.GetScenes());

            foreach (var child in m_Children)
                paths.AddRange(child.GetDescendantScenes());

            return paths;
        }
    }
}
