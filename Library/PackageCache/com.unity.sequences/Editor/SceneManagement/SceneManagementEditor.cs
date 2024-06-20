using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// An interface for manipulating Scenes in the context of <seealso cref="TimelineSequence"/>.
    /// </summary>
    internal class SceneManagement
    {
        /// <summary>
        /// Creates a new empty Scene and links it with a <seealso cref="SceneActivationTrack"/> in the given Sequence.
        /// </summary>
        /// <param name="timeline">The Sequence you want the new Scene to belong to.</param>
        /// <returns>True if the operation has succeeded. False if the Editor could not save the new Scene in the project.</returns>
        internal static bool AddNewScene(TimelineAsset timeline)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            if (EditorSceneManager.SaveScene(scene))
            {
                var track = timeline.CreateTrack<SceneActivationTrack>();
                track.scene = new SceneReference() {path = scene.path};
                track.name = scene.name;
                var activationClip = track.CreateClip<SceneActivationPlayableAsset>();
                activationClip.displayName = "Active";
                activationClip.duration = timeline.duration;

                EditorUtility.SetDirty(timeline);
                AssetDatabase.SaveAssetIfDirty(timeline);

                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

                return true;
            }

            EditorSceneManager.CloseScene(scene, true);
            return false;
        }

        /// <summary>
        /// Opens the Scene located at the specified path in <seealso cref="OpenSceneMode.Additive"/>.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        /// <param name="deactivate">Set to true to disable the root GameObjects of the loaded Scenes. Default value: false.</param>
        internal static void OpenScene(string path, bool deactivate = false)
        {
            var scene = EditorSceneManager.GetSceneByPath(path);
            if (!scene.isLoaded)
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            if (deactivate)
            {
                List<GameObject> rootObjects = new List<GameObject>();
                scene.GetRootGameObjects(rootObjects);

                foreach (GameObject root in rootObjects)
                    root.SetActive(false);
            }
        }

        /// <summary>
        /// Closes the Scene located at the specified path.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        internal static void CloseScene(string path)
        {
            if (IsLoaded(path))
                EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByPath(path), true);
        }

        /// <summary>
        /// Opens all Scenes referenced through a <seealso cref="SceneActivationTrack"/> in the specified <seealso cref="TimelineSequence"/>.
        /// </summary>
        /// <param name="sequence">The Sequence you want to open all related activatable Scenes from.</param>
        /// <param name="deactivate">Set to true to disable the root GameObjects of the loaded Scenes. Default value: false.</param>
        internal static void OpenAllScenes(SequenceNode sequence, bool deactivate = false)
        {
            foreach (var path in sequence.GetRelatedScenes())
                OpenScene(path, deactivate);
        }

        /// <summary>
        /// Indicates if the Scene located at the specified path is already loaded or not in the Hierarchy.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        /// <returns>True if the Scene located at <paramref name="path"/> is already loaded in the Hierarchy. False otherwise.</returns>
        internal static bool IsLoaded(string path)
        {
            var scene = EditorSceneManager.GetSceneByPath(path);
            return scene.isLoaded;
        }
    }
}
