using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// The SequenceAssetException is thrown each time a Prefab is not a valid Sequence Asset. For example if it
    /// doesn't have the SequenceAsset component on it, or if it is not a Regular or a Variant Prefab as expected.
    /// </summary>
    public class SequenceAssetException : Exception
    {
        /// <summary>
        /// Creates a new SequenceAssetException with a custom message.
        /// </summary>
        /// <param name="message">The message to display in the Console.</param>
        public SequenceAssetException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// SequenceAssetUtility provides multiple functions to help creates and manipulates Sequence Asset Prefabs and
    /// Variants.
    /// A Sequence Asset is a Regular or a Variant Prefab "tagged" using the <paramref name="SequenceAsset"/> component.
    /// Sequence Assets are visible and manageable in the Sequences window and are Prefabs ready to be used in
    /// a MasterSequence.
    /// </summary>
    public static class SequenceAssetUtility
    {
        internal static readonly string k_SequenceAssetsBaseFolder = "SequenceAssets";

        /// <summary>
        /// Event triggered when a SequenceAsset gets assigned to a Sequence.
        /// </summary>
        internal static event Action<GameObject, GameObject, PlayableDirector> sequenceAssetAssignedTo;

        /// <summary>
        /// Event triggered when a SequenceAsset has been removed from a Sequence.
        /// </summary>
        internal static event Action<PlayableDirector> sequenceAssetRemovedFrom;

        /// <summary>
        /// Creates a new Regular prefab with the SequenceAsset component on it.
        /// </summary>
        /// <param name="name">The name of the Prefab asset to create.</param>
        /// <param name="type">The Collection type of the Sequence Asset that should be set on the SequenceAsset
        /// component. This is to categorize Sequence Assets.</param>
        /// <param name="animate">If true, a new TimelineAsset will be created and setup in the Prefab. The
        /// TimelineAsset itself is saved in the AssetDatabase next to the Prefab asset. Otherwise, the TimelineAsset
        /// can be manually created and setup later on if needed.</param>
        /// <param name="instantiate">If true, the created Prefab asset will be instantiated in the current active
        /// scene. Otherwise it is only created on disk.</param>
        /// <returns>The created Regular Prefab asset.</returns>
        public static GameObject CreateSource(string name, string type = null, bool animate = true, bool instantiate = false)
        {
            var sourceGo = new GameObject(name);
            var sequenceAsset = sourceGo.AddComponent<SequenceAsset>();
            sequenceAsset.type = type ?? CollectionType.undefined;

            var outputPath = GenerateUniqueAssetPath(sourceGo);
            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                sourceGo,
                outputPath,
                InteractionMode.AutomatedAction);

            if (animate)
                SetupTimeline(prefab);

            if (!instantiate)
                Object.DestroyImmediate(sourceGo);

            EditorGUIUtility.PingObject(prefab);
            AssetDatabase.SaveAssets();

            return prefab;
        }

        /// <summary>
        /// Creates a new Variant Prefab for the specified Source Prefab.
        /// </summary>
        /// <param name="source">A Regular Prefab to use to create the new Variant. This Prefab must have the
        /// SequenceAsset component and it has to be a Regular Prefab. Model Prefab or Variant Prefab are not supported
        /// as potential Source Sequence Asset for a new Sequence Asset Variant.</param>
        /// <param name="name">The name of the Prefab variant to create.</param>
        /// <param name="instantiate">If true, the created Prefab asset will be instantiated in the current active
        /// scene. Otherwise it is only created on disk.</param>
        /// <returns>The created Variant Prefab asset.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Source is not a valid Sequence Asset Source.
        /// It must be a Regular Prefab with the SequenceAsset component on it.</exception>
        /// <remarks>If the specified source Sequence Asset has a Timeline setup, a new TimelineAsset is created and
        /// saved for the Variant prefab as well. The created TimelineAsset is a copy of the source Sequence Asset
        /// TimelineAsset but it won't inherit the changes made on the source TimelineAsset later on.</remarks>
        public static GameObject CreateVariant(GameObject source, string name = null, bool instantiate = false)
        {
            if (!IsSource(source))
                throw new SequenceAssetException("Invalid source Prefab. It must be a Regular Prefab with the " +
                    "SequenceAsset component on it.");

            var sourceInstance = (GameObject)PrefabUtility.InstantiatePrefab(source);

            var variantPath = GenerateUniqueVariantAssetPath(source, name);
            var newVariant = PrefabUtility.SaveAsPrefabAssetAndConnect(sourceInstance, variantPath, InteractionMode.AutomatedAction);

            // We don't need the source prefab instance anymore.
            Object.DestroyImmediate(sourceInstance);

            if (HasTimelineSetup(source))
                DuplicateAndSetupTimeline(source, newVariant); // must be a copy of the original timeline, not a new timeline.

            if (instantiate)
                PrefabUtility.InstantiatePrefab(newVariant);

            EditorGUIUtility.PingObject(newVariant);
            AssetDatabase.SaveAssets();

            return newVariant;
        }

        /// <summary>
        /// Duplicates a Sequence Asset Variant.
        /// </summary>
        /// <param name="variant">The Sequence Asset Variant to duplicate.</param>
        /// <returns>The duplicated Sequence Asset Variant.</returns>
        /// <remarks>If the Sequence Asset Variant to duplicate has a TimelineAsset setup, it is duplicated as well and
        /// setup on the duplicated Variant. Any Timeline bindings are correctly preserved.</remarks>
        public static GameObject DuplicateVariant(GameObject variant)
        {
            var variantPath = AssetDatabase.GetAssetPath(variant);
            var outputPath = AssetDatabase.GenerateUniqueAssetPath(variantPath);
            var success = AssetDatabase.CopyAsset(variantPath, outputPath);

            if (!success)
                return null;

            var duplicatedVariant = AssetDatabase.LoadAssetAtPath<GameObject>(outputPath);

            if (HasTimelineSetup(variant))
                DuplicateAndSetupTimeline(variant, duplicatedVariant);

            EditorGUIUtility.PingObject(duplicatedVariant);

            return duplicatedVariant;
        }

        /// <summary>
        /// Deletes a Sequence Asset Source. If there is a TimelineAsset attached, it is deleted as well. The folder that
        /// contains the Sequence Asset Source is deleted as well if it is empty after deleting the Sequence Asset itself.
        /// </summary>
        /// <param name="source">The Sequence Asset Source to delete.</param>
        /// <param name="deleteVariants">If true, also delete any Sequence Asset Variant of the provided Source
        /// (see <seealso cref="DeleteVariantAsset"/>).
        /// Otherwise, only the Source is deleted. In this last case, the first Variant become a Regular Prefab
        /// and every other Variant have the first Variant as their Source.</param>
        /// <returns>True is the Sequence Asset Source and any Variants are correctly deleted.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Source is not a valid Sequence Asset Source.
        /// It must be a Regular Prefab with the SequenceAsset component on it.</exception>
        public static bool DeleteSourceAsset(GameObject source, bool deleteVariants = true)
        {
            if (!IsSource(source))
                throw new SequenceAssetException("Invalid Sequence Asset Prefab source. It must have a " +
                    "'SequenceAsset' component and be a Regular prefab.");

            var isSuccess = true;
            if (deleteVariants)
            {
                foreach (var variant in GetVariants(source))
                    isSuccess &= DeleteVariantAsset(variant);
            }

            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceFolder = Path.GetDirectoryName(sourcePath);

            isSuccess &= DeleteSequenceAsset(source);

            // Also delete the parent folder if it is empty.
            if (FilePathUtility.IsFolderEmpty(sourceFolder))
                isSuccess &= AssetDatabase.DeleteAsset(sourceFolder);

            AssetDatabase.SaveAssets();

            return isSuccess;
        }

        /// <summary>
        /// Deletes a provided Sequence Asset Variant. If there is a TimelineAsset attached, it is deleted as well.
        /// </summary>
        /// <param name="variant">The Sequence Asset Variant to delete.</param>
        /// <returns>True if the Sequence Asset Variant is correctly deleted.</returns>
        public static bool DeleteVariantAsset(GameObject variant)
        {
            var success = DeleteSequenceAsset(variant);
            AssetDatabase.SaveAssets();
            return success;
        }

        /// <summary>
        /// Instantiates the provided Sequence Asset (Source or Variant) in a Sequence.
        /// </summary>
        /// <param name="prefab">The Sequence Asset Prefab to instantiate (Source or Variant).</param>
        /// <param name="sequenceDirector">The Sequence PlayableDirector in which to instantiate the Sequence Asset.
        /// The Sequence Asset Prefab is instantiated under PlayableDirector.gameObject. If the provided Sequence Asset
        /// has a TimelineAsset setup to it, then a new <see cref="SequenceAssetTrack"/> and a clip are created in the
        /// PlayableDirector.playableAsset (the Sequence TimelineAsset) and bind to the Sequence Asset Prefab instance.</param>
        /// <param name="clip">User can optionally provide an existing TimelineClip to bind the Sequence Asset Prefab
        /// instance to. The asset of this TimelineClip must be a <see cref="SequenceAssetPlayableAsset"/>.</param>
        /// <returns>The Sequence Asset Prefab instance.</returns>
        public static GameObject InstantiateInSequence(GameObject prefab, PlayableDirector sequenceDirector, TimelineClip clip = null)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(sequenceDirector.transform);
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate SequenceAsset in Hierarchy");

            var timeline = sequenceDirector.playableAsset as TimelineAsset;
            if (timeline != null && HasTimelineSetup(prefab))
            {
                Undo.RegisterCompleteObjectUndo(timeline, "Add SequenceAsset to Timeline");

                // Make all the fps match.
                var instanceTimeline = GetTimelineAsset(instance);
                instanceTimeline.SetFrameRate(timeline.GetFrameRate());

                if (clip == null)
                {
                    var groupTrack = timeline.GetOrCreateTrack<GroupTrack>(GetType(prefab));
                    var track = timeline.CreateTrack<SequenceAssetTrack>(groupTrack, GetSource(prefab).name);
                    clip = track.CreateClip<SequenceAssetPlayableAsset>();
                    clip.displayName = instance.name;
                }

                SetClipDuration(clip, timeline, instanceTimeline);

                var clipAsset = clip.asset as SequenceAssetPlayableAsset;
                if (clipAsset == null)
                    return null;

                if (clipAsset.director.exposedName == null)
                {
                    Undo.RecordObject(clipAsset, "Add SequenceAsset to Timeline");
                    var guid = GUID.Generate().ToString();
                    clipAsset.director.exposedName = new PropertyName(guid);
                }

                Undo.RecordObject(sequenceDirector, "Add SequenceAsset to Timeline");
                sequenceDirector.SetReferenceValue(clipAsset.director.exposedName, instance.GetComponentInChildren<PlayableDirector>(true));

                EditorUtility.SetDirty(timeline);
                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            }

            Undo.SetCurrentGroupName("Instantiate SequenceAsset in Sequence");
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            sequenceAssetAssignedTo?.Invoke(prefab, instance, sequenceDirector);

            return instance;
        }

        /// <summary>
        /// Removes a Sequence Asset Prefab instance (Source or Variant) from a Sequence. This is the opposite of the
        /// <see cref="InstantiateInSequence"/> action.
        /// </summary>
        /// <param name="instance">The Sequence Asset Prefab instance to remove from the Scene.</param>
        /// <param name="sequenceDirector">The Sequence PlayableDirector in which the Sequence Asset is currently
        /// instantiated.</param>
        public static void RemoveFromSequence(GameObject instance, PlayableDirector sequenceDirector)
            => RemoveFromSequence(instance, sequenceDirector, InteractionMode.UserAction);

        /// <summary>
        /// Removes a Sequence Asset Prefab instance (Source or Variant) from a Sequence. This is the opposite of the
        /// <see cref="InstantiateInSequence"/> action.
        /// </summary>
        /// <param name="instance">The Sequence Asset Prefab instance to remove from the Scene.</param>
        /// <param name="sequenceDirector">The Sequence PlayableDirector in which the Sequence Asset is currently
        /// instantiated.</param>
        /// <param name="interactionMode">Choose the mode of interaction, user or automated.</param>
        public static void RemoveFromSequence(
            GameObject instance,
            PlayableDirector sequenceDirector,
            InteractionMode interactionMode)
        {
            if (!Application.isBatchMode && interactionMode is InteractionMode.UserAction &&
                PrefabUtility.IsPartOfPrefabInstance(sequenceDirector) &&
                !PrefabUtility.IsAddedGameObjectOverride(instance))
            {
                UserVerifications.OpenPrefabStage(sequenceDirector.gameObject);
                return;
            }

            var clip = RemoveFromSequenceInternal(instance, sequenceDirector);
            if (clip == null)
                return;

            var sequenceAssetTrack = clip.GetParentTrack();
            var timeline = sequenceAssetTrack.timelineAsset;

            Undo.RegisterCompleteObjectUndo(timeline, "Remove SequenceAsset from Sequence");

            // Delete clip and tracks related to the removed instance.
            sequenceAssetTrack.DeleteClip(clip);
            var groupTrack = sequenceAssetTrack.parent as GroupTrack;

            if (!sequenceAssetTrack.hasClips)
                timeline.DeleteTrack(sequenceAssetTrack);

            if (groupTrack != null && !groupTrack.GetChildTracks().Any())
                timeline.DeleteTrack(groupTrack);

            EditorUtility.SetDirty(timeline);
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            Undo.SetCurrentGroupName("Remove SequenceAsset in Sequence");
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            sequenceAssetRemovedFrom?.Invoke(sequenceDirector);
        }

        /// <summary>
        /// Updates a Sequence Asset instance to another in a specified Sequence. This is used to switch a Sequence
        /// Asset Source for one of its Variant or switch a Sequence Asset Variant for another Variant of the
        /// same Sequence Asset source.
        /// </summary>
        /// <param name="oldInstance">The old Sequence Asset Prefab to remove (see <see cref="RemoveFromSequence"/>)</param>
        /// <param name="newPrefab">The new Sequence Asset Prefab to instantiate (see <see cref="InstantiateInSequence"/>)</param>
        /// <param name="sequenceDirector">The Sequence PlayableDirector to update.</param>
        /// <param name="interactionMode">If the interactionMode is InteractionMode.UserAction, then the user is asked
        /// to validate the update if the removed Sequence Asset needs to be saved. If this is called in a script that
        /// can't ask for user validation, use InteractionMode.AutomatedAction and verify that no changes can be lost
        /// on the <paramref name="oldInstance"/> prior calling this function.</param>
        /// <returns>The new Sequence Asset instance (i.e. the instance of <paramref name="newPrefab"/>).</returns>
        public static GameObject UpdateInstanceInSequence(
            GameObject oldInstance,
            GameObject newPrefab,
            PlayableDirector sequenceDirector,
            InteractionMode interactionMode = InteractionMode.UserAction)
        {
            bool needUserAction = !Application.isBatchMode && interactionMode is InteractionMode.UserAction;
            if (needUserAction && PrefabUtility.IsPartOfPrefabInstance(sequenceDirector) &&
                !PrefabUtility.IsAddedGameObjectOverride(oldInstance))
            {
                UserVerifications.OpenPrefabStage(sequenceDirector.gameObject);
                return null;
            }

            if (needUserAction && !UserVerifications.ValidateInstanceChange(oldInstance))
            {
                // Don't proceed with the swap, user choose to keep the current instance.
                return null;
            }

            var clip = RemoveFromSequenceInternal(oldInstance, sequenceDirector);
            var newInstance = InstantiateInSequence(newPrefab, sequenceDirector, clip);

            return newInstance;
        }

        /// <summary>
        /// Gets all Sequence Asset Prefab instances present in a Sequence. The list of instances is constructed by
        /// looking at the Sequence TimelineAsset and the children GameObjects of the Sequence GameObject.
        /// </summary>
        /// <param name="sequenceDirector">The Sequence PlayableDirector to look for Sequence Asset instances.</param>
        /// <param name="type">Optionally specify one Collection type to look for. Only Sequence Asset instances of
        /// this type are returned. Leave to null to return all instances.</param>
        /// <returns>A list of Sequence Asset Prefab instances.</returns>
        public static IEnumerable<GameObject> GetInstancesInSequence(PlayableDirector sequenceDirector, string type = null)
        {
            var timelineInstances = GetInstancesInSequenceTimeline(sequenceDirector, type);
            var gameObjectInstances = GetInstancesUnderSequenceGameObject(sequenceDirector.gameObject, type);
            var allFoundInstances = timelineInstances.Concat(gameObjectInstances);

            // Ensure that one instance is not represented twice in the result.
            var allInstances = new HashSet<GameObject>(allFoundInstances);

            return allInstances;
        }

        /// <summary>
        /// Renames a Sequence Asset Prefab (Source or Variant). Any related assets (TimelineAsset if any) and folders
        /// are also renamed.
        /// </summary>
        /// <param name="prefab">The Sequence Asset Prefab (Source or Variant) to rename.</param>
        /// <param name="oldName">The old name of the Sequence Asset Prefab.</param>
        /// <param name="newName">The new name to use for the rename.</param>
        /// <param name="renameVariants">Set to true (default) to also rename the associated Variants. This is
        /// only used if the Sequence Asset Prefab renamed is a Source.</param>
        /// <param name="renameInstances">Set to true (default) to rename any instances of the Sequence Asset Prefab.</param>
        /// <returns>The actual new name of the Sequence Asset Prefab. This function will ensure that the Sequence Asset
        /// Prefab is renamed with a unique name, so this returned value can be different from the
        /// <paramref name="newName"/>. If it is, it is simply post-fixed with a unique number.</returns>
        public static string Rename(
            GameObject prefab,
            string oldName,
            string newName,
            bool renameVariants = true,
            bool renameInstances = true)
        {
            if (!FilePathUtility.SanitizeAndValidateName(oldName, newName, out var sanitizedName))
                return oldName;

            var actualNewName = sanitizedName;
            bool isSource = IsSource(prefab);
            if (isSource)
                actualNewName = RenameFolder(prefab, oldName, sanitizedName);

            RenameInternal(prefab, oldName, actualNewName, renameInstances);

            if (isSource && renameVariants)
            {
                foreach (var variant in GetVariants(prefab))
                    RenameInternal(variant, oldName, actualNewName, renameInstances);
            }
            AssetDatabase.SaveAssets();

            return actualNewName;
        }

        /// <summary>
        /// Finds all the Sequence Asset Prefab Sources that exists in the Project.
        /// </summary>
        /// <param name="type">An optional Sequence Asset Collection type to limit the find on Sequence Assets.</param>
        /// <returns>A list of all the Sequence Asset Sources found.</returns>
        public static IEnumerable<GameObject> FindAllSources(string type = null)
        {
            var indexes = SequenceAssetIndexer.instance.indexes;
            var isTypeNullOrEmpty = string.IsNullOrEmpty(type);

            foreach (var index in indexes)
            {
                if (index == null || index.mainPrefab == null)
                    continue;

                if (isTypeNullOrEmpty || GetType(index.mainPrefab) == type)
                    yield return index.mainPrefab;
            }
        }

        /// <summary>
        /// Gets the Collection type of a Sequence Asset Prefab.
        /// </summary>
        /// <param name="prefab">A Sequence Asset Prefab (Source or Variant) to get the Collection type from.</param>
        /// <returns>The Collection type name of the Sequence Asset.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Prefab is not a valid Sequence Asset.
        /// It must have the SequenceAsset component on it.</exception>
        public static string GetType(GameObject prefab)
        {
            var sequenceAsset = prefab.GetComponent<SequenceAsset>();
            if (sequenceAsset == null)
                throw new SequenceAssetException("The specified Prefab is not a Sequence Asset. Sequence " +
                    "Asset Prefabs must have the 'SequenceAsset' component.)");

            return sequenceAsset.type;
        }

        /// <summary>
        /// Gets the Sequence Asset Source of a specified Sequence Asset (Source or Variant).
        /// </summary>
        /// <param name="prefab">A Sequence Asset Prefab or Sequence Asset Prefab Variant to get the Source from.</param>
        /// <returns>The Sequence Asset Prefab Source of the specified Prefab. If the specified Prefab is already a Source,
        /// it is returned as is.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Prefab is not a valid Sequence Asset.
        /// It must have the SequenceAsset component on it.</exception>
        public static GameObject GetSource(GameObject prefab)
        {
            if (!IsSequenceAsset(prefab))
                throw new SequenceAssetException("The specified Prefab is not a Sequence Asset or a Variant of one. " +
                    "It must have a 'SequenceAsset' component.");

            return PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefab);
        }

        /// <summary>
        /// Gets the Sequence Asset Variants of a specified Sequence Asset Source.
        /// </summary>
        /// <param name="source">The Sequence Asset Source to get the Variants from.</param>
        /// <returns>The list of Sequence Asset Variants of the specified Source.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Source is not a valid Sequence Asset Source.
        /// It must be a Regular Prefab with the SequenceAsset component on it.</exception>
        public static IEnumerable<GameObject> GetVariants(GameObject source)
        {
            if (!IsSource(source))
                throw new SequenceAssetException("Invalid Sequence Asset source Prefab. It must be a regular " +
                    "Prefab and have a 'SequenceAsset' component.");

            var index = SequenceAssetIndexer.instance.GetIndexOf(source);
            return index < 0 ? new GameObject[] {} : SequenceAssetIndexer.instance.indexes[index].variants;
        }

        /// <summary>
        /// Indicates if the specified Sequence Asset is a Source Prefab or not.
        /// </summary>
        /// <param name="prefab">The Sequence Asset Prefab to check if it is a Source.</param>
        /// <returns>True if the specified Sequence Asset is a Source, false otherwise.</returns>
        public static bool IsSource(GameObject prefab)
        {
            return prefab.GetComponent<SequenceAsset>() != null &&
                PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Regular;
        }

        /// <summary>
        /// Indicates if the specified Sequence Asset is a Variant Prefab or not.
        /// </summary>
        /// <param name="prefab">The Sequence Asset Prefab to check if it is a Variant.</param>
        /// <returns>True if the specified Sequence Asset is Variant, false otherwise.</returns>
        public static bool IsVariant(GameObject prefab)
        {
            return prefab.GetComponent<SequenceAsset>() != null &&
                PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Variant;
        }

        /// <summary>
        /// Indicates if the specified Prefab a Sequence Asset or not.
        /// </summary>
        /// <param name="prefab">A Prefab to check if it is a Sequence Asset. I.e. if it has the SequenceAsset component
        /// on it.</param>
        /// <returns>True if the specified Prefab is a Sequence Asset, false otherwise.</returns>
        public static bool IsSequenceAsset(GameObject prefab)
        {
            return prefab.GetComponent<SequenceAsset>() != null;
        }

        /// <summary>
        /// Checks if a specified Sequence Asset Prefab has Variants. Only Source Sequence Asset can possibly have Variants.
        /// </summary>
        /// <param name="source">The Sequence Asset Prefab Source to search existing Variants from.</param>
        /// <returns>True if the specified Sequence Asset Prefab has Variants. Otherwise, false.</returns>
        public static bool HasVariants(GameObject source)
        {
            if (!IsSource(source))
                return false;

            return GetVariants(source).Any();
        }

        /// <summary>
        /// Gets the Prefab Asset that correspond to the provided Sequence Asset Prefab instance.
        /// </summary>
        /// <param name="instance">A Sequence Asset instance to look for its corresponding asset.</param>
        /// <returns>The Sequence Asset Prefab asset that correspond to the provided instance. If the specified GameObject
        /// is already an asset, it is returned as is.</returns>
        /// <exception cref="SequenceAssetException">Thrown when the specified Prefab instance is not a valid Sequence Asset.
        /// It must have the SequenceAsset component on it.</exception>
        internal static GameObject GetAssetFromInstance(GameObject instance)
        {
            if (!IsSequenceAsset(instance))
                throw new SequenceAssetException("The specified Prefab instance is not a Sequence Asset or a Variant " +
                    "of one. It must have a 'SequenceAsset' component.");

            if (PrefabUtility.IsPartOfPrefabAsset(instance))
                return instance;

            var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance);
            return PrefabUtility.GetCorrespondingObjectFromSourceAtPath(instance, prefabPath);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="sequenceDirector"></param>
        /// <returns></returns>
        internal static TimelineClip GetClipFromInstance(GameObject instance, PlayableDirector sequenceDirector)
        {
            if (!HasTimelineSetup(instance))
                return null;

            var timeline = sequenceDirector.playableAsset as TimelineAsset;
            foreach (var clip in timeline.GetSequenceAssetClips())
            {
                var bindInstance = GetInstanceFromClip(clip, sequenceDirector);
                if (instance == bindInstance)
                    return clip;
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        static IEnumerable<GameObject> GetInstances(GameObject prefab)
        {
            var sequenceAssetComponents = ObjectsCache.FindObjectsFromScenes<SequenceAsset>();
            foreach (var sequenceAssetComp in sequenceAssetComponents)
            {
                var instance = sequenceAssetComp.gameObject;
                var sourceAsset = PrefabUtility.GetCorrespondingObjectFromSource(instance);
                if (sourceAsset == prefab)
                    yield return instance;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sequenceDirector"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="SequenceAssetException"></exception>
        static IEnumerable<GameObject> GetInstancesInSequenceTimeline(PlayableDirector sequenceDirector, string type = null)
        {
            var timeline = sequenceDirector.playableAsset as TimelineAsset;
            if (timeline == null)
                throw new SequenceAssetException("Invalid Sequence's PlayableDirector. This director doesn't control any timeline.");

            foreach (var clip in timeline.GetSequenceAssetClips())
            {
                var instance = GetInstanceFromClip(clip, sequenceDirector);
                if (instance == null || !IsSequenceAsset(instance) || PrefabUtility.IsPrefabAssetMissing(instance))
                    continue;

                if (string.IsNullOrEmpty(type) || type == GetType(instance))
                    yield return instance;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="type"></param>
        /// <returns>The list of Sequence Asset instances present under a sequence GameObject. Sequence Asset prefab
        /// instance with no Prefab asset on disk are ignored except in PlayMode when the concept of "Prefab" doesn't
        /// exists anymore.</returns>
        static IEnumerable<GameObject> GetInstancesUnderSequenceGameObject(GameObject sequence, string type = null)
        {
            for (var i = 0; i < sequence.transform.childCount; ++i)
            {
                var instance = sequence.transform.GetChild(i).gameObject;
                if (!IsSequenceAsset(instance) || PrefabUtility.IsPrefabAssetMissing(instance) ||
                    (!EditorApplication.isPlayingOrWillChangePlaymode && !PrefabUtility.IsAnyPrefabInstanceRoot(instance)))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(type) || type == GetType(instance))
                    yield return instance;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        static void SetupTimeline(GameObject prefab)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            timeline.name = prefab.name + "_Timeline";

            var prefabFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab));
            var timelinePath = Path.Combine(prefabFolderPath, $"{timeline.name}.playable");
            AssetDatabase.CreateAsset(timeline, timelinePath);

            var director = prefab.GetComponentInChildren<PlayableDirector>(true);
            if (director == null)
                director = prefab.AddComponent<PlayableDirector>();

            director.playableAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(timelinePath);
            director.playOnAwake = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fromPrefab"></param>
        /// <param name="toPrefab"></param>
        /// <returns></returns>
        static bool DuplicateAndSetupTimeline(GameObject fromPrefab, GameObject toPrefab)
        {
            var fromDirector = fromPrefab.GetComponentInChildren<PlayableDirector>(true);
            if (fromDirector == null)
                return false;

            var fromTimeline = fromDirector.playableAsset as TimelineAsset;
            if (fromTimeline == null)
                return false;

            var fromTimelinePath = AssetDatabase.GetAssetPath(fromTimeline);
            var fromTimelineFolderPath = Path.GetDirectoryName(fromTimelinePath);
            var toTimelinePath = Path.Combine(fromTimelineFolderPath, $"{toPrefab.name}_Timeline.playable");

            var copySucceed = AssetDatabase.CopyAsset(fromTimelinePath, toTimelinePath);
            if (!copySucceed)
                return false;

            var toTimeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(toTimelinePath);

            var toDirector = toPrefab.GetComponentInChildren<PlayableDirector>(true);
            var oldBindings = toDirector.playableAsset.outputs.ToArray();

            toDirector.playableAsset = toTimeline;
            var newBindings = toDirector.playableAsset.outputs.ToArray();

            for (int i = 0; i < oldBindings.Length; i++)
            {
                toDirector.SetGenericBinding(
                    newBindings[i].sourceObject,
                    toDirector.GetGenericBinding(oldBindings[i].sourceObject));
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        static TimelineAsset GetTimelineAsset(GameObject prefab)
        {
            var director = prefab.GetComponentInChildren<PlayableDirector>(true);
            if (director == null)
                return null;

            var timeline = director.playableAsset as TimelineAsset;
            return timeline;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        static bool HasTimelineSetup(GameObject prefab)
        {
            var playableDirector = prefab.GetComponentInChildren<PlayableDirector>(true);
            return playableDirector != null && (playableDirector.playableAsset as TimelineAsset) != null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="parentTimeline"></param>
        /// <param name="nestedTimeline"></param>
        static void SetClipDuration(TimelineClip clip, TimelineAsset parentTimeline, TimelineAsset nestedTimeline)
        {
            if (nestedTimeline.duration > 0)
            {
                clip.duration = nestedTimeline.duration;
                return;
            }

            var sequence = SequenceUtility.GetSequenceFromTimeline(parentTimeline);
            if (sequence != null)
                clip.duration = sequence.duration;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        static bool DeleteSequenceAsset(GameObject asset)
        {
            var isSuccess = true;

            var variantTimeline = GetTimelineAsset(asset);
            if (variantTimeline != null)
                isSuccess &= AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variantTimeline));

            isSuccess &= AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));

            return isSuccess;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="renameInstances"></param>
        static void RenameInternal(
            GameObject prefab,
            string oldName,
            string newName,
            bool renameInstances = true)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var assetName = Path.GetFileNameWithoutExtension(path);
            if (assetName != null && !assetName.Contains(oldName))
            {
                // Assets are renamed by replacing "oldName" by "newName" in them. If the specified assets doesn't contains
                // "oldName" then it doesn't need (and won't be able) to be renamed. This case may happen when
                // renaming the variants of a source prefab.
                return;
            }

            var newAssetName = GenerateUniqueAssetName(prefab, oldName, newName);
            AssetDatabase.RenameAsset(path, newAssetName);

            var timeline = GetTimelineAsset(prefab);
            if (timeline != null)
            {
                var newTimelineAssetName = timeline.name.ReplaceFirst(assetName, newAssetName);
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(timeline), newTimelineAssetName);
            }

            if (renameInstances)
            {
                foreach (var instance in GetInstances(prefab))
                    instance.name = instance.name.ReplaceFirst(oldName, newName);
            }
        }

        /// <summary>
        /// Gets the Sequence Asset prefab instance that holds the PlayableDirector controlled by the specified TimelineClip.
        /// </summary>
        /// <param name="clip">A TimelineClip that controls a Sequence Asset prefab PlayableDirector. This TimelineClip asset
        /// must be a SequenceAssetPlayableAsset.</param>
        /// <param name="director">The director that controls the timeline that contains the specified clip.</param>
        /// <returns></returns>
        static GameObject GetInstanceFromClip(TimelineClip clip, PlayableDirector director)
        {
            var clipAsset = clip.asset as SequenceAssetPlayableAsset;
            if (clipAsset == null)
                return null;

            var resolvedDirector = (PlayableDirector)director.GetReferenceValue(clipAsset.director.exposedName, out _);
            return resolvedDirector == null ? null : PrefabUtility.GetNearestPrefabInstanceRoot(resolvedDirector.gameObject);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="sequenceDirector"></param>
        /// <returns></returns>
        static TimelineClip RemoveFromSequenceInternal(GameObject instance, PlayableDirector sequenceDirector)
        {
            var clip = GetClipFromInstance(instance, sequenceDirector);
            Undo.DestroyObjectImmediate(instance);

            if (clip == null)
                return null;

            Undo.RecordObject(sequenceDirector, "Remove SequenceAsset from Sequence");

            var clipAsset = clip.asset as SequenceAssetPlayableAsset;
            sequenceDirector.ClearReferenceValue(clipAsset.director.exposedName);

            return clip;
        }

        internal static string GetVariantName(GameObject source, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var sourcePath = AssetDatabase.GetAssetPath(source);
                var sourceName = Path.GetFileNameWithoutExtension(sourcePath);
                name = $"{sourceName}_Variant";
            }

            return name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        static string GenerateUniqueAssetPath(GameObject prefab)
        {
            var typeFolderPath = Path.Combine(
                "Assets",
                k_SequenceAssetsBaseFolder);

            if (!AssetDatabase.IsValidFolder(typeFolderPath))
                Directory.CreateDirectory(typeFolderPath);

            var folderPath = Path.Combine(typeFolderPath, prefab.name);
            var uniqueFolderPath = AssetDatabase.GenerateUniqueAssetPath(folderPath);

            Directory.CreateDirectory(uniqueFolderPath);

            var fileName = Path.GetFileName(uniqueFolderPath);
            var uniqueOutputPath = Path.Combine(uniqueFolderPath, $"{fileName}.prefab");

            return uniqueOutputPath;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static string GenerateUniqueVariantAssetPath(GameObject source, string name = null)
        {
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceFolder = Path.GetDirectoryName(sourcePath);
            var variantName = GetVariantName(source, name);

            return AssetDatabase.GenerateUniqueAssetPath(Path.Combine(sourceFolder, $"{variantName}.prefab"));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        static string GenerateUniqueAssetName(GameObject prefab, string oldName, string newName)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var folderPath = Path.GetDirectoryName(path);
            var assetName = Path.GetFileNameWithoutExtension(path);

            var newAssetName = assetName.ReplaceFirst(oldName, newName);
            var newAssetPath = Path.Combine(folderPath, $"{newAssetName}.prefab");
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
            newAssetName = Path.GetFileNameWithoutExtension(newAssetPath);

            return newAssetName;
        }

        /// <summary>
        /// Renames a Sequence Asset folder with a generated unique name.
        /// If the user has renamed the folder manually, leaves the name as is.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns>The new folder name.</returns>
        static string RenameFolder(GameObject prefab, string oldName, string newName)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var folderPath = Path.GetDirectoryName(path);

            // Checks if the prefab name matches the folder name. If no match, don't rename it.
            if (oldName != Path.GetFileName(folderPath))
                return newName;

            var actualNewName = GenerateUniqueAssetName(prefab, oldName, newName);

            // Build new folder path.
            var parentFolderPath = Path.GetDirectoryName(folderPath);
            var newFolderPath = Path.Combine(parentFolderPath, actualNewName);

            // Validate uniqueness of folder path.
            newFolderPath = AssetDatabase.GenerateUniqueAssetPath(newFolderPath);
            actualNewName = Path.GetFileName(newFolderPath);

            AssetDatabase.RenameAsset(folderPath, actualNewName);
            return actualNewName;
        }

        /// <summary>
        /// Replace only the first occurence of "oldValue" in the provided string.
        /// </summary>
        /// <param name="stringToEdit"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        static string ReplaceFirst(this string stringToEdit, string oldValue, string newValue)
        {
            var regex = new Regex(Regex.Escape(oldValue));
            return regex.Replace(stringToEdit, newValue, 1);
        }

        /// <summary>
        /// Returns the default name to assign to a new Sequence Asset.
        /// </summary>
        /// <param name="collectionType"></param>
        /// <returns></returns>
        internal static string GetDefaultSequenceAssetName(string collectionType)
        {
            return $"{collectionType}Asset";
        }
    }
}
