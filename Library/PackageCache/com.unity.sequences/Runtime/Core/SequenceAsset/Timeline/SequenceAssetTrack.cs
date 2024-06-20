using System;
using System.ComponentModel;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// SequenceAssetTrack support SequenceAssetPlayableAsset that drive the PlayableDirector of a Sequence Asset.
    /// </summary>
    [Serializable]
    [DisplayName("Sequencing/Sequence Asset Track")]
    [TrackColor(0.058f, 0.462f, 0.588f)]
    [TrackClipType(typeof(SequenceAssetPlayableAsset), false)]
    public class SequenceAssetTrack : NestedTimelineTrack
    {
    }
}
