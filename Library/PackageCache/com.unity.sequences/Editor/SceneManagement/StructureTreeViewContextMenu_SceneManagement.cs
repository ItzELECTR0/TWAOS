using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class StructureTreeView
    {
        void AppendSceneManagementActions(DropdownMenu menu, int index, bool prependSeparator = true)
        {
            if (prependSeparator)
                menu.AppendSeparator();

            var itemData = GetItemDataForIndex(index);
            var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);

            menu.AppendAction("Load Scenes", LoadScenesAction, LoadSceneActionStatus, sequence);

            if (sequence.HasScenes())
                foreach (var path in sequence.GetRelatedScenes())
                {
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    menu.AppendAction(
                        $"Load specific Scene/{fileName}",
                        LoadSpecificSceneAction,
                        LoadSpecificSceneActionStatus,
                        path);
                }
            else
            {
                menu.AppendAction("Load specific Scene", _ => {}, DropdownMenuAction.Status.Disabled);
            }

            menu.AppendAction("Create scene...", CreateSceneAction, CreateSceneActionStatus, sequence);
        }

        void LoadScenesAction(DropdownMenuAction action)
        {
            var sequence = action.userData as SequenceNode;
            SceneManagement.OpenAllScenes(sequence, true);
        }

        DropdownMenuAction.Status LoadSceneActionStatus(DropdownMenuAction action)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || PrefabStageUtility.GetCurrentPrefabStage() != null)
                return DropdownMenuAction.Status.Disabled;

            var sequence = action.userData as SequenceNode;
            if (sequence.HasScenes() && sequence.hasGameObject)
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }

        void LoadSpecificSceneAction(DropdownMenuAction action)
        {
            string scenePath = action.userData as string;
            SceneManagement.OpenScene(scenePath, true);
        }

        DropdownMenuAction.Status LoadSpecificSceneActionStatus(DropdownMenuAction action)
        {
            var scenePath = action.userData as string;
            var isLoaded = SceneManagement.IsLoaded(scenePath);

            if (isLoaded)
                return DropdownMenuAction.Status.Checked | DropdownMenuAction.Status.Disabled;

            return DropdownMenuAction.Status.Normal;
        }

        void CreateSceneAction(DropdownMenuAction action)
        {
            var sequence = action.userData as SequenceNode;
            SceneManagement.AddNewScene(sequence.timeline);
        }

        DropdownMenuAction.Status CreateSceneActionStatus(DropdownMenuAction action)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || PrefabStageUtility.GetCurrentPrefabStage() != null)
                return DropdownMenuAction.Status.Disabled;

            var sequence = action.userData as SequenceNode;
            if (sequence.hasGameObject)
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }
    }
}
