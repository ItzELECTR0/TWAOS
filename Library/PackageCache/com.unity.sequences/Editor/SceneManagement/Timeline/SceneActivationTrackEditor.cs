using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    [CustomTimelineEditor(typeof(SceneActivationTrack))]
    class SceneActivationTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            TrackDrawOptions drawOptions = new TrackDrawOptions();
            drawOptions.icon = (Texture2D)EditorGUIUtility.IconContent("SceneAsset Icon").image;

            SceneActivationTrack sceneTrack = track as SceneActivationTrack;
            if (sceneTrack != null)
            {
                if (!SceneManagement.IsLoaded(sceneTrack.scene.path))
                    drawOptions.errorText = "Scene is not loaded";
            }
            return drawOptions;
        }

        public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
        {
            // Add a default clip to the newly created track
            if (copiedFrom == null)
            {
                var clip = track.CreateClip<SceneActivationPlayableAsset>();
                clip.displayName = "Active";
                clip.duration = System.Math.Max(clip.duration, track.timelineAsset.duration * 0.5f);
            }
        }
    }
}
