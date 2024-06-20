using System;
using System.IO;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Utility functions to manipulate MasterSequence assets and Sequence objects.
    /// </summary>
    public static class SequenceUtility
    {
        /// <summary>
        /// Mask to define the validity state of a Sequence.
        /// One single Sequence can be invalid for four reasons: its GameObject is missing in the Hierarchy view, its
        /// Timeline asset was deleted, its parent Sequence is invalid (Orphan), its MasterSequence asset itself is
        /// missing.
        /// </summary>
        [Flags]
        internal enum SequenceState
        {
            Valid = 1,
            MissingMasterSequence = 2,
            MissingGameObject = 4,
            MissingTimeline = 8,
            Orphan = 16,
            NotInHierarchy = 32,
            PrefabInstanceRoot = 64,
            PrefabStageRoot = 128
        }

        /// <summary>
        /// Define the loaded status of a MasterSequence structure in the Hierarchy window.
        /// A MasterSequence structure can be fully loaded in the Hierarchy or partially (through a Sequence prefab
        /// instantiated in its own scene for example).
        /// A MasterSequence can be completely absent (None) from the Hierarchy.
        /// A PrefabStage can be open, in this case only the sequences that are loaded in this prefab stage will be
        /// editable (see <see cref="SequenceUtility.GetSequenceState"/>).
        /// Finally, a MasterSequence structure could be loaded in an invalid way in the Hierarchy. In this case, it is
        /// considered not loaded.
        ///
        /// Those loaded status are used in <see cref="SequenceUtility.GetSequenceState"/> and in
        /// <see cref="SequenceUtility.GetSequenceEditionStatus"/>.
        /// </summary>
        internal enum MasterSequenceLoaded
        {
            Fully,
            Partially,
            InPrefabStage,
            None,
            Invalid
        }

        /// <summary>
        /// Mask to define what kind of edit(s) can be performed on a specified Sequence.
        /// </summary>
        [Flags]
        internal enum SequenceEditionStatus
        {
            CanRename = 1,
            CanDelete = 2,
            CanCreate = 4,
            CanManipulate = 8  // Record and Scene Management.
        }

        internal static readonly string k_DefaultMasterSequenceName = "NewMasterSequence";
        internal static readonly string k_DefaultSequenceName = "NewSequence";

        /// <summary>
        /// Each TimelineSequence creation on disk triggers this event.
        /// </summary>
        public static event Action<TimelineSequence, MasterSequence> sequenceCreated;

        /// <summary>
        /// Each TimelineSequence deletion from disk triggers this event.
        /// </summary>
        public static event Action sequenceDeleted;

        internal static SequenceIndexer.DisableEvent disableEvent;

        /// <summary>
        /// Creates a new MasterSequence and saves it on disk.
        /// </summary>
        /// <param name="name">The created MasterSequence name.</param>
        /// <param name="fps">The created MasterSequence frame rate. If you don't specify a framerate, the
        /// MasterSequence uses by default the Timeline framerate value from the Project Settings.</param>
        /// <returns>The newly created MasterSequence asset.</returns>
        public static MasterSequence CreateMasterSequence(string name, float fps = -1.0f)
        {
            // Is closed in HierarchyDataChangeVerifier after the corresponding GameObject is created.
            disableEvent = new SequenceIndexer.DisableEvent();

            Undo.SetCurrentGroupName("Create Master Sequence");
            var groupIndex = Undo.GetCurrentGroup();

            fps = fps < 0.0 ? (float)TimelineUtility.GetProjectFrameRate() : fps;
            var masterSequence = MasterSequence.CreateInstance(name, fps);

            ProjectSettings.CreateDefaultMasterSequenceRegistryIfNeeded();
            ProjectSettings.defaultMasterSequenceRegistry.Register(masterSequence.rootSequence.timeline, string.Empty);
            ProjectSettings.SaveAssetSetting(ProjectSettings.defaultMasterSequenceRegistry);

            masterSequence.Save();

            sequenceCreated?.Invoke(masterSequence.rootSequence, masterSequence);

            Undo.CollapseUndoOperations(groupIndex);

            return masterSequence;
        }

        /// <summary>
        /// Creates a new Sequence in the specified MasterSequence asset. Also saves the Sequence TimelineAsset and the
        /// updated MasterSequence on disk.
        /// </summary>
        /// <param name="name">The name of the created Sequence.</param>
        /// <param name="masterSequence">The MasterSequence asset to add the created Sequence to.</param>
        /// <param name="parent">An optional parent Sequence for the created one.</param>
        /// <returns>The newly created TimelineSequence.</returns>
        public static TimelineSequence CreateSequence(string name, MasterSequence masterSequence, TimelineSequence parent = null)
        {
            // Is closed in HierarchyDataChangeVerifier after the corresponding GameObject is created.
            disableEvent = new SequenceIndexer.DisableEvent();

            Undo.SetCurrentGroupName("Create Sequence");
            var groupIndex = Undo.GetCurrentGroup();

            parent = parent ?? masterSequence.rootSequence;
            var sequence = masterSequence.NewSequence(name, parent);

            parent.Save();
            masterSequence.Save(); // Save the updated SequenceManager structure.
            sequence.Save(); // Save the sequence TimelineAsset on disk.
            masterSequence.Save(); // Save the MasterSequence asset with the new sequence TimelineAsset correctly bind.

            sequenceCreated?.Invoke(sequence, masterSequence);

            Undo.CollapseUndoOperations(groupIndex);

            return sequence;
        }

        /// <summary>
        /// Removes the specified Sequence and all its sub-Sequences from the specified MasterSequence asset. This also
        /// removes from disk each corresponding Sequence TimelineAsset, and saves the updated MasterSequence asset.
        /// </summary>
        /// <param name="sequence">The Sequence to delete.</param>
        /// <param name="masterSequence">The MasterSequence to remove the Sequence from.</param>
        public static void DeleteSequence(TimelineSequence sequence, MasterSequence masterSequence)
        {
            // When I delete a Sequence from the API, I should by-pass the event sent by the indexer.
            // For the UI, if it recieves the event from the API, the sequence was deleted as expected. However, if it recieves the event from the Indexer, it means that the timeline was only deleted on disk.

            Undo.SetCurrentGroupName("Delete Sequence");
            var groupIndex = Undo.GetCurrentGroup();

            if (TimelineEditor.selectedClip == sequence.editorialClip)
                TimelineEditor.selectedClip = null;

            var sequenceFolderPath = SequencesAssetDatabase.GetSequenceFolder(sequence);
            foreach (var removedSequence in masterSequence.RemoveSequence(sequence))
            {
                removedSequence.Delete();
            }

            if (!string.IsNullOrEmpty(sequenceFolderPath) && FilePathUtility.IsFolderEmpty(sequenceFolderPath))
                AssetDatabase.DeleteAsset(sequenceFolderPath);

            masterSequence.Save();
            sequenceDeleted?.Invoke();

            Undo.CollapseUndoOperations(groupIndex);
        }

        /// <summary>
        /// Gets the Sequence associated to the specified TimelineAsset.
        /// </summary>
        /// <param name="timeline">The TimelineAsset the Sequence to search is associated to.</param>
        /// <returns>The TimelineSequence associated with the given TimelineAsset.</returns>
        internal static TimelineSequence GetSequenceFromTimeline(TimelineAsset timeline)
        {
            var masterSequences = SequencesAssetDatabase.FindAsset<MasterSequence>();
            foreach (var masterSequence in masterSequences)
            {
                foreach (var sequence in masterSequence.manager.sequences)
                {
                    var timelineSequence = sequence as TimelineSequence;
                    if (!TimelineSequence.IsNullOrEmpty(timelineSequence) && timelineSequence.timeline == timeline)
                        return timelineSequence;
                }
            }

            return null;
        }

        // TODO: This function is now only used in test and could be removed. Ideally we replaces the test by new ones before.
        /// <summary>
        /// Checks if the specified Sequence is not null and has a valid TimelineAsset associated to it. Also checks the
        /// validity of all the parent Sequences.
        /// </summary>
        /// <param name="sequence">The Sequence to check.</param>
        /// <returns>true if the specified Sequence and all its parent Sequences are valid. Otherwise, false.</returns>
        internal static bool IsValidSequence(TimelineSequence sequence)
        {
            var isValid = !TimelineSequence.IsNullOrEmpty(sequence);
            if (!isValid)
                return false;

            var parentSequence = sequence.parent as TimelineSequence;
            while (isValid && parentSequence != null)
            {
                isValid = !TimelineSequence.IsNullOrEmpty(parentSequence);
                parentSequence = parentSequence.parent as TimelineSequence;
            }

            return isValid;
        }

        /// <summary>
        /// Gets the validity of the specified Sequence. A Sequence is valid if its GameObject is present in the scene,
        /// if its Timeline asset exists and if it's parent Sequence is valid as well.
        /// </summary>
        /// <param name="sequence">The Sequence to get validity from.</param>
        /// <param name="masterSequence">The MasterSequence of the specified sequence. This is used to check if the
        /// whole MasterSequence is loaded in a Scene or not. If it's not, there is no need to check if the Sequence
        /// GameObject exists.</param>
        /// <returns>A <see cref="SequenceState"/> mask.</returns>
        internal static SequenceState GetSequenceState(TimelineSequence sequence, MasterSequence masterSequence)
        {
            if (masterSequence == null)
                return SequenceState.MissingMasterSequence;

            SequenceState result = SequenceState.Valid;

            // Check the state of the parent, if not valid, then the current sequence is an orphan and invalid.
            var parentSequence = sequence.parent as TimelineSequence;
            if (parentSequence != null)
            {
                var parentState = GetSequenceState(parentSequence, masterSequence);
                if (!parentState.HasFlag(SequenceState.Valid))
                {
                    // Sequence's parent is invalid, then Sequence is invalid and an orphan.
                    result &= ~SequenceState.Valid;
                    result |= SequenceState.Orphan;
                }
            }

            if (TimelineSequence.IsNullOrEmpty(sequence))
            {
                // Sequence is not valid and its timeline is missing.
                result &= ~SequenceState.Valid;
                result |= SequenceState.MissingTimeline;
            }

            // If the Master Sequence is not represented at all in the Hierarchy, no need to check for missing
            // GameObjects.
            var masterSequenceLoadedStatus = GetMasterSequenceLoadedStatus(masterSequence);
            if (masterSequenceLoadedStatus != MasterSequenceLoaded.None &&
                masterSequenceLoadedStatus != MasterSequenceLoaded.Invalid)
            {
                var gameObject = GetSequenceGameObject(sequence);
                if (gameObject == null)
                {
                    var rootSequenceFilter = GetRootSequenceFilterInHierarchy(masterSequence);
                    var rootSequenceInHierarchy = rootSequenceFilter.sequence;

                    if (!IsChildSequence(sequence, rootSequenceInHierarchy))
                    {
                        // The sequence is not a child of the the root sequence found in the hierarchy, so we don't
                        // expect to find a corresponding GameObject and it's ok.
                        result |= SequenceState.NotInHierarchy;
                    }
                    else
                    {
                        result &= ~SequenceState.Valid;
                        result |= SequenceState.MissingGameObject;
                    }
                }
                else
                {
                    var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                    {
                        result |= SequenceState.PrefabInstanceRoot;
                    }
                    else if (masterSequenceLoadedStatus == MasterSequenceLoaded.InPrefabStage &&
                             gameObject == prefabStage.prefabContentsRoot)
                    {
                        result |= SequenceState.PrefabStageRoot;
                    }
                }
            }
            else
                result |= SequenceState.NotInHierarchy;

            return result;
        }

        /// <summary>
        /// Gets the GameObject in the current loaded Scenes that corresponds to the specified Sequence.
        /// </summary>
        /// <param name="sequence">The TimelineSequence to look the GameObject for.</param>
        /// <returns>The GameObject that corresponds to the specified Sequence.</returns>
        internal static GameObject GetSequenceGameObject(TimelineSequence sequence)
        {
            var sequenceFilters = ObjectsCache.FindObjectsFromScenes<SequenceFilter>();
            foreach (var sequenceFilter in sequenceFilters)
            {
                // Might happen when instantiating a Sequence prefab under a MasterSequence GameObject.
                if (sequenceFilter.masterSequence == null)
                    continue;

                if (sequenceFilter.masterSequence.manager.GetAt(sequenceFilter.elementIndex) == sequence)
                {
                    return sequenceFilter.gameObject;
                }
            }

            return null;
        }

        static SequenceFilter GetRootSequenceFilterInHierarchy(MasterSequence masterSequence)
        {
            var sequenceFilters = ObjectsCache.FindObjectsFromScenes<SequenceFilter>();
            foreach (var sequenceFilter in sequenceFilters)
            {
                if (sequenceFilter.masterSequence == null || sequenceFilter.masterSequence != masterSequence)
                    continue;

                if (sequenceFilter.gameObject.transform.parent == null ||
                    sequenceFilter.gameObject.transform.parent.GetComponent<SequenceFilter>() == null)
                {
                    return sequenceFilter;
                }
            }

            return null;
        }

        internal static MasterSequenceLoaded GetMasterSequenceLoadedStatus(MasterSequence masterSequence)
        {
            var rootSequenceFilter = GetRootSequenceFilterInHierarchy(masterSequence);

            if (rootSequenceFilter == null)
                return MasterSequenceLoaded.None;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                return MasterSequenceLoaded.InPrefabStage;

            if (rootSequenceFilter.sequence == null)
                return MasterSequenceLoaded.Invalid;

            if (rootSequenceFilter.sequence == rootSequenceFilter.masterSequence.rootSequence)
                return MasterSequenceLoaded.Fully;

            if (PrefabUtility.IsAnyPrefabInstanceRoot(rootSequenceFilter.gameObject))
                return MasterSequenceLoaded.Partially;

            return MasterSequenceLoaded.Invalid;
        }

        internal static SequenceEditionStatus GetSequenceEditionStatus(TimelineSequence sequence, MasterSequence masterSequence)
        {
            var sequenceState = GetSequenceState(sequence, masterSequence);
            var masterSequenceLoadedState = GetMasterSequenceLoadedStatus(masterSequence);

            bool isPlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            bool isValid = sequenceState.HasFlag(SequenceState.Valid);
            bool isPrefabRoot =
                sequenceState.HasFlag(SequenceState.PrefabInstanceRoot) ||
                sequenceState.HasFlag(SequenceState.PrefabStageRoot);
            bool isPartial =
                masterSequenceLoadedState == MasterSequenceLoaded.Partially ||
                masterSequenceLoadedState == MasterSequenceLoaded.InPrefabStage;

            // The basic requirements for a Sequence to be manipulated (create sub-sequence, rename, delete...) are:
            // - Unity is not in PlayMode.
            // - The MasterSequence structure is fully loaded in the Hierarchy or not at all. If it is partially loaded
            //   (Prefab stage or a Sequence prefab is isolated in a scene) then the target sequence must be in the
            //   hierarchy.
            bool basicRequirements =
                !isPlaymode && !(isPartial && sequenceState.HasFlag(SequenceState.NotInHierarchy));

            bool basicValidRequirements = isValid && basicRequirements;

            SequenceEditionStatus result = 0;
            // It is ok for an invalid Sequence to be deleted, except if it is specifically an
            // orphan (i.e. a parent sequence is invalid).
            if (basicRequirements && !(isPartial && isPrefabRoot) && !sequenceState.HasFlag(SequenceState.Orphan))
                result |= SequenceEditionStatus.CanDelete;

            if (basicValidRequirements && !isPrefabRoot)
                result |= SequenceEditionStatus.CanRename;

            if (basicValidRequirements)
                result |= SequenceEditionStatus.CanCreate;

            if (basicValidRequirements &&
                !sequenceState.HasFlag(SequenceState.NotInHierarchy) &&
                masterSequenceLoadedState != MasterSequenceLoaded.InPrefabStage)
            {
                result |= SequenceEditionStatus.CanManipulate;
            }

            return result;
        }

        /// <summary>
        /// Gets an unique hashcode from the provided sequence.
        /// It gets it by building the sequence path. Renaming a Sequence or the MasterSequence will yield a new hashcode.
        /// </summary>
        /// <param name="sequence">The Sequence the returned hashcode belongs to.</param>
        /// <param name="masterSequence">The MasterSequence the <paramref name="sequence"/> belongs to.</param>
        /// <returns>An unique integer</returns>
        internal static int GetHashCode(TimelineSequence sequence, MasterSequence masterSequence)
        {
            string path = string.Empty;

            TimelineSequence current = sequence;
            while (current != null)
            {
                if (current.parent == null)
                    path = Path.Combine(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(masterSequence)), path);
                else
                    path = Path.Combine(
                        masterSequence.manager.GetIndex(sequence).ToString(),
                        "_",
                        path);

                current = current.parent as TimelineSequence;
            }
            return path.GetHashCode();
        }

        static bool IsChildSequence(TimelineSequence sequence, TimelineSequence rootSequence)
        {
            foreach (var child in rootSequence.GetChildren())
            {
                var childSequence = child as TimelineSequence;
                if (sequence == childSequence || IsChildSequence(sequence, childSequence))
                    return true;
            }

            return false;
        }
    }
}
