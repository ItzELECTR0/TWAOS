using System.ComponentModel;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// SequenceAssetPlayableAsset controls the PlayableDirector of a Sequence Asset instance.
    /// </summary>
    [DisplayName("Sequence Asset Clip")]
    public class SequenceAssetPlayableAsset : NestedTimelinePlayableAsset, ITimelineClipAsset
    {
        // TODO: Hide the `director` ExposedReference by a SequenceAsset component.
        //       The Editor of this clip could display something similar to what is displayed in the Sequence Assembly.
        // TODO: Add validation to ensure the controlled PlayableDirector belong to an actual Sequence Asset.

        /// <summary>
        /// Gets the clip caps. For a SequenceAssetPlayableAsset, SpeedMultiplier, ClipIn and Looping are the three
        /// extra clip options available.
        /// </summary>
        public ClipCaps clipCaps => ClipCaps.SpeedMultiplier | ClipCaps.ClipIn | ClipCaps.Looping;

        internal override GameObject GetGameObjectToActivate(PlayableDirector playableDirector)
        {
            var components = playableDirector.GetComponentsInParent<SequenceAsset>(true);
            SequenceAsset sequenceAsset = null;
            if (components.Length > 0)
                sequenceAsset = components[0];

            return sequenceAsset != null ? sequenceAsset.gameObject : base.GetGameObjectToActivate(playableDirector);
        }
    }
}
