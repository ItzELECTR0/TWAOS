#if UNITY_EDITOR
using System.ComponentModel;
#endif
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// A Playable Asset used by the <seealso cref="SceneActivationTrack"/>.
    /// </summary>
    [System.Serializable]
#if UNITY_EDITOR
    [DisplayName("Activation clip")]
#endif
    public class SceneActivationPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        /// [doc-replica] com.unity.timeline
        /// <summary>
        /// Returns the capabilities of TimelineClips that contain an SceneActivationPlayableAsset.
        /// </summary>
        public ClipCaps clipCaps => ClipCaps.None;

        /// [doc-replica] com.unity.timeline
        /// <summary>
        /// Creates the root of a Playable subgraph to play the scene activation clip.
        /// </summary>
        /// <param name="graph">PlayableGraph that will own the playable.</param>
        /// <param name="owner">The gameobject that triggered the graph build.</param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}
