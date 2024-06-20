using System;
using System.ComponentModel;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// EditorialPlayableAsset controls the PlayableDirector of a Sequence.
    /// </summary>
    [DisplayName("Editorial Clip")]
    public class EditorialPlayableAsset : NestedTimelinePlayableAsset, ITimelineClipAsset, ISerializationCallbackReceiver
    {
        // TODO: Hide the `director` ExposedReference by a Sequence.
        // TODO: Add validation to ensure the controlled PlayableDirector belong to an actual Sequence.

        [SerializeField] TimelineAsset m_Timeline;

        /// <summary>
        /// References the TimelineAsset driven by <see cref="NestedTimelinePlayableAsset.director"/>.
        /// It's automatically set in <see cref="OnValidate"/> and <see cref="OnBeforeSerialize"/>.
        /// </summary>
        internal TimelineAsset timeline
        {
            get => m_Timeline;
            set => m_Timeline = value; // TODO: When TimelineSequence will disappear, this should become private.
        }

        /// <summary>
        /// Holds a reference to the PlayableDirector that is driving the instance of a clip.
        /// </summary>
        PlayableDirector m_GameObjectDirector;

        /// <summary>
        /// Get the clip caps. For a EditorialPlayableAsset, SpeedMultiplier and ClipIn are the two
        /// extra clip options available.
        /// </summary>
        public ClipCaps clipCaps => ClipCaps.SpeedMultiplier | ClipCaps.ClipIn;

        public void OnBeforeSerialize()
        {
            SerializeTimelineReference();
        }

        public void OnAfterDeserialize() {}

        void OnValidate()
        {
            SerializeTimelineReference();
        }

        internal double GetEditorialContentDuration(PlayableDirector inspectedDirector)
        {
            var resolvedDirector = director.Resolve(inspectedDirector);
            if (resolvedDirector == null)
                return 0.0;

            var timeline = resolvedDirector.playableAsset as TimelineAsset;
            if (timeline == null)
                return 0.0;

            var editorialDuration = 0.0;
            foreach (var track in timeline.GetOutputTracks())
            {
                var editorialTrack = track as EditorialTrack;
                if (editorialTrack == null)
                    continue;

                editorialDuration = Math.Max(editorialDuration, editorialTrack.GetActualDuration());
            }

            return editorialDuration;
        }

        /// <summary>
        /// Creates two playables, one to control the PlayableDirector of this asset and another to control the
        /// activation of this PlayableDirector.
        /// </summary>
        /// <param name="graph">The graph to inject playables into.</param>
        /// <param name="owner">The game object which initiated the build.</param>
        /// <returns>The root playable of all the playables injected.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = base.CreatePlayable(graph, owner);

            m_GameObjectDirector = owner.GetComponent<PlayableDirector>();

            var directorInstance = director.Resolve(graph.GetResolver());
            if (directorInstance != null)
                timeline = directorInstance.playableAsset as TimelineAsset;

            return playable;
        }

        /// <summary>
        /// Serialize the timeline reference found in <see cref="NestedTimelinePlayableAsset.director"/>.
        /// </summary>
        void SerializeTimelineReference()
        {
            if (m_GameObjectDirector == null)
                return;

            var resolvedDirector = director.Resolve(m_GameObjectDirector);
            if (resolvedDirector == null || resolvedDirector.playableAsset == null)
                return;

            if (resolvedDirector.playableAsset != m_Timeline)
                m_Timeline = resolvedDirector.playableAsset as TimelineAsset;
        }
    }
}
