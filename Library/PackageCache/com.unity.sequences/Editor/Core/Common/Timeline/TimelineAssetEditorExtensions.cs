using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    static class TimelineAssetEditorExtensions
    {
        /// <summary>
        /// Finds a PlayableDirector in loaded scenes that references the given <see cref="TimelineAsset"/>.
        /// </summary>
        /// <param name="timeline">The instance of <see cref="TimelineAsset"/> to look for.</param>
        /// <returns>Null when no matching <see cref="PlayableDirector"/> is found.</returns>
        public static PlayableDirector FindDirector(this TimelineAsset timeline)
        {
            if (timeline == null)
                throw new System.NullReferenceException("timeline");

            var playables = ObjectsCache.FindObjectsFromScenes<PlayableDirector>();
            foreach (var playable in playables)
            {
                if (playable.playableAsset == timeline)
                    return playable;
            }
            return null;
        }
    }
}
