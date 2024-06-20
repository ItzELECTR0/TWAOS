using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;

namespace UnityEditor.Sequences
{
    internal static class UserVerifications
    {
        /// <summary>
        /// Skips the Editor popups asking users for confirmation.
        /// Set it to True when methods are called from automation.
        /// </summary>
        internal static bool skipUserVerification = false;

        internal static bool ValidateSequencesDeletion(TimelineAsset[] timelines)
        {
            if (skipUserVerification || timelines.Length == 0)
                return true;

            bool cantDelete = false;
            GameObject prefabToOpen = null;

            var timelineNames = new List<string>();
            foreach (var timeline in timelines)
            {
                var sequence = SequenceIndexer.instance.GetSequence(timeline);
                if (IsPartOfPrefab(sequence.gameObject))
                {
                    if (timelines.Length == 1)
                        prefabToOpen = sequence.gameObject;
                    else
                        cantDelete = true;
                }

                timelineNames.Add(timeline.name);
            }

            // There is no assets to delete, the selected items are only invalid items and there deletion is undoable.
            // No validation needed.
            if (timelineNames.Count == 0)
                return true;

            if (cantDelete)
            {
                EditorUtility.DisplayDialog("Sequence deletion",
                    "You can't delete the current selection because some sequences are part of Prefab instances. " +
                    "To delete them, you must enter the Prefab Mode.",
                    "Ok");
                return false;
            }

            if (prefabToOpen != null)
            {
                OpenPrefabStage(prefabToOpen);
                return false;
            }

            var deleteAssets = EditorUtility.DisplayDialog(
                "Sequence deletion",
                $"Do you want to delete the following sequence(s)?: \n\t- {string.Join("\n\t- ", timelineNames)}\n\n" +
                "You cannot undo this action.",
                "Delete",
                "Cancel"
            );

            return deleteAssets;
        }

        static bool IsPartOfPrefab(GameObject gameObject)
        {
            return gameObject != null &&
                PrefabUtility.IsPartOfPrefabInstance(gameObject) &&
                !PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject) &&
                !PrefabUtility.IsAddedGameObjectOverride(gameObject);
        }

        internal static bool ValidateSequenceAssetDeletion(List<GameObject> sequenceAssets, bool areCollections = false)
        {
            if (skipUserVerification)
                return true;

            var customizedMessage = "";
            if (areCollections) // Only collections were selected.
            {
                customizedMessage = $"Do you want to delete the {sequenceAssets.Count} Sequence Asset(s) " +
                    "from the selected Asset Collection(s)?";
            }
            else
            {
                customizedMessage = "Do you want to delete the selected Sequence Asset(s) and/or variant(s)?";
            }

            return EditorUtility.DisplayDialog(
                "Sequence Asset deletion",
                customizedMessage +
                "\n\nYou cannot undo this action.",
                "Delete",
                "Cancel"
            );
        }

        internal static bool ValidateInstanceChange(GameObject instance)
        {
            if (!PrefabUtility.HasPrefabInstanceAnyOverrides(instance, false))
                return true;

            // Deactivating an instance counts as an override, so ignore that case
            bool hasNonDefaultOverrides = false;
            if (PrefabUtility.GetAddedComponents(instance).Count > 0 ||
                PrefabUtility.GetAddedGameObjects(instance).Count > 0)
            {
                hasNonDefaultOverrides = true;
            }
            else
            {
                foreach (var modification in PrefabUtility.GetPropertyModifications(instance))
                {
                    if (!PrefabUtility.IsDefaultOverride(modification) &&
                        // m_InitialState is controlled by "Play on Awake" that is changed when the playable director is
                        // targeted by a SequenceAsset clip (i.e. it's a nested timeline).
                        !modification.propertyPath.Equals("m_InitialState"))
                    {
                        hasNonDefaultOverrides = true;
                    }
                }
            }

            if (!hasNonDefaultOverrides)
                return true;

            if (skipUserVerification)
                return true;

            var result = EditorUtility.DisplayDialogComplex(
                "Sequence Asset instance has been modified",
                $"Do you want to save the changes you made on \"{instance.name}\"?\n\n" +
                "Your changes will be lost if you don't save them.",
                "Save Changes",
                "Cancel",
                "Discard Changes"
            );

            switch (result)
            {
                case 0:  // Apply Overrides
                    PrefabUtility.ApplyPrefabInstance(
                        instance,
                        InteractionMode.AutomatedAction);

                    return true;

                case 1:  // Cancel
                    return false;

                case 2:  // Discard Changes
                    return true;

                default:
                    return false;
            }
        }

        internal static void OpenPrefabStage(GameObject gameObject)
        {
            if (skipUserVerification)
                return;

            var openPrefab = EditorUtility.DisplayDialog(
                "Cannot restructure Prefab instance",
                "Children of a Prefab instance cannot be deleted or moved, and components cannot be reordered.\n\n" +
                "You can open the Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab" +
                " instance to remove its Prefab connection.",
                "Open Prefab", "Cancel");

            if (openPrefab)
            {
                var rootPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(rootPrefab);
                var assetPath = AssetDatabase.GetAssetPath(prefabAsset);
                PrefabStageUtility.OpenPrefab(assetPath, rootPrefab);
            }
        }
    }
}
