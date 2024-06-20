using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    /// <summary>
    /// The SequenceAssetPlayableAsset clip editor.
    /// </summary>
    [CustomTimelineEditor(typeof(SequenceAssetPlayableAsset))]
    class SequenceAssetClipEditor : NestedTimelineClipEditor
    {
        /// <inheritdoc/>
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            var director = TimelineEditor.inspectedDirector;

            if (director == null)
                return options;

            var clipAsset = clip.asset as SequenceAssetPlayableAsset;
            var clipDirector = clipAsset.director.Resolve(director);

            if (clipDirector == null || clipDirector.playableAsset == null)
                options.errorText = "No PlayableDirector set, or PlayableDirector has no valid PlayableAsset.";

            // This can't be done in Playmode because the link between the instance and the Prefab asset doesn't
            // exist at this point.
            else if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(clipDirector.gameObject);
                if (prefabRoot == null || PrefabUtility.IsPrefabAssetMissing(prefabRoot))
                    options.errorText = "The corresponding prefab asset is missing.";
            }

            // Highlight clip if play head is over it.
            if (director.time >= clip.start && director.time <= clip.end)
                options.highlightColor = new Color(0.501f, 1f, 0.901f);

            return options;
        }
    }
}
