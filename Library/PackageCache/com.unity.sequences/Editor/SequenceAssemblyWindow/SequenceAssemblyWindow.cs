using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    [PackageHelpURL("sequence-assembly-window")]
    partial class SequenceAssemblyWindow : BaseEditorWindow
    {
        SequenceAssemblyInspector m_CachedEditor;

        Label m_SequenceNameLabel;
        TextElement m_SequenceEmptyStateText;
        VisualElement m_EmptyStateMessageContainer;

        readonly string k_SequenceNotCreatedMessage = "Create and select a Sequence";
        readonly string k_SequenceNotSelectedMessage = "Select a Sequence";

        internal class Styles
        {
            public static readonly string k_AssemblyStyleSheetName = "SequenceAssemblyInspector";
            public static readonly string k_AssemblyEmptyStateMessageContainer = "seq-empty-state-msg";
            public static readonly string k_AssemblyEmptyStateText = "seq-empty-text-element";
        }

        /// <summary>
        /// Called by OnEnable sets the view
        /// </summary>
        protected override void SetupView()
        {
            base.SetupView();

            StyleSheetUtility.SetStyleSheets(rootVisualElement, Styles.k_AssemblyStyleSheetName);
            titleContent = new GUIContent("Sequence Assembly",
                IconUtility.LoadIcon("MasterSequence/Shot", IconUtility.IconType.UniqueToSkin));

            m_SequenceNameLabel = new Label { bindingPath = "m_Name" };

            m_SequenceEmptyStateText = new TextElement();

            m_EmptyStateMessageContainer = new VisualElement();
            m_EmptyStateMessageContainer.Add(m_SequenceEmptyStateText);

            m_EmptyStateMessageContainer.AddToClassList(Styles.k_AssemblyEmptyStateMessageContainer);
            m_SequenceEmptyStateText.AddToClassList(Styles.k_AssemblyEmptyStateText);

            InitializeView();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            SelectionUtility.playableDirectorChanged += ShowSelection;
            ObjectChangeEvents.changesPublished += OnObjectChanged;
            SequenceIndexer.validityChanged += OnSequenceInvalidated;
            SequenceIndexer.sequencesRemoved += OnSequenceInvalidated; // Timeline asset removed on disk (not via API).
        }

        void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            SelectionUtility.playableDirectorChanged -= ShowSelection;
            ObjectChangeEvents.changesPublished -= OnObjectChanged;
            SequenceIndexer.validityChanged -= OnSequenceInvalidated;
            SequenceIndexer.sequencesRemoved -= OnSequenceInvalidated;

            ClearView();
        }

        void OnFocus()
        {
            if (m_CachedEditor != null)
                m_CachedEditor.SelectPlayableDirector();
        }

        void InitializeView()
        {
            SetHeaderContent(m_SequenceNameLabel);

            if (m_CachedEditor && m_CachedEditor.target != null) // Domain reload or going in PlayMode might trigger this user case.
            {
                if (m_CachedEditor.rootVisualElement == null)
                    m_CachedEditor.Initialize();

                rootVisualElement.Bind(new SerializedObject((m_CachedEditor.target as PlayableDirector).playableAsset as TimelineAsset));
                rootVisualElement.Add(m_CachedEditor.rootVisualElement);
            }
            else // The Editor doesn't exist or the target is null.
                ShowSelection();
        }

        void ShowSelection()
        {
            // An existing view's target becomes null when the user deletes the sequence game object,
            // in which case the view should show nothing.
            if (m_CachedEditor != null && m_CachedEditor.target == null)
            {
                ClearView();
                return;
            }

            var director = SelectionUtility.activePlayableDirector;
            var isSelectionSequence = director != null &&
                director.gameObject == Selection.activeGameObject &&
                director.playableAsset != null &&
                director.gameObject.GetComponent<SequenceFilter>() != null;

            // If the window is not showing any sequence and the selected playable director is not a sequence: refresh
            // the empty state message.
            if (m_CachedEditor == null && !isSelectionSequence)
            {
                SetEmptyState();
                return;
            }

            // If the window is already showing the expected sequence, there's no need to change the view.
            if (!isSelectionSequence || IsAlreadyShown(director))
                return;

            // The sequence to show was changed.
            ClearView();
            CreateView(director);
        }

        bool IsAlreadyShown(PlayableDirector target)
        {
            return (m_CachedEditor && m_CachedEditor.target == target);
        }

        void CreateView(PlayableDirector data)
        {
            m_EmptyStateMessageContainer.RemoveFromHierarchy();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                m_CachedEditor = SequenceAssemblyInspector.CreateEditor(
                    data,
                    typeof(SequenceAssemblyInspector)) as SequenceAssemblyInspector;
            else
                m_CachedEditor = SequenceAssemblyInspector.CreateEditor(
                    data,
                    typeof(SequenceAssemblyPlayModeInspector)) as SequenceAssemblyPlayModeInspector;

            m_CachedEditor.Initialize();

            rootVisualElement.Add(m_CachedEditor.CreateInspectorGUI());
            rootVisualElement.Bind(new SerializedObject(data.playableAsset as TimelineAsset));
        }

        void ClearView()
        {
            if (m_CachedEditor && m_CachedEditor.rootVisualElement != null)
            {
                if (!rootVisualElement.Contains(m_CachedEditor.rootVisualElement))
                    return;

                rootVisualElement.Remove(m_CachedEditor.rootVisualElement);
                rootVisualElement.Unbind();
                m_SequenceNameLabel.text = "";

                m_CachedEditor.Unload();
                DestroyImmediate(m_CachedEditor);
            }

            SetEmptyState();
        }

        void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            // Check if the Sequence that has been shown before PlayMode is still there.
            // It could be null if scenes are dynamically loaded for example.
            var lastDirector = m_CachedEditor != null ? m_CachedEditor.target as PlayableDirector : null;

            if (m_CachedEditor != null)
                ClearView();

            // TODO: remove check to `lastDirector.playableAsset` when we make the code detect
            // the deletion of a Sequence.
            // Temporary check to ensure the view we get is still on a valid TimelineAsset.
            // Could be null if a user deletes it manually from the Project for example.
            if (lastDirector != null && lastDirector.playableAsset != null)
                CreateView(lastDirector);
        }

        void OnObjectChanged(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; ++i)
            {
                var eventType = stream.GetEventType(i);
                switch (eventType)
                {
                    case ObjectChangeKind.ChangeScene:
                        stream.GetChangeSceneEvent(i, out var sceneChangeData);
                        OnSceneChanged(sceneChangeData);
                        break;
                    case ObjectChangeKind.CreateGameObjectHierarchy:
                        stream.GetCreateGameObjectHierarchyEvent(i, out var createdGameObjectData);
                        OnCreationOfGameObjectHierarchy(createdGameObjectData);
                        break;
                    case ObjectChangeKind.DestroyGameObjectHierarchy:
                        stream.GetDestroyGameObjectHierarchyEvent(i, out var destroyGameObjectData);
                        OnDestroyGameObjectHierarchy(destroyGameObjectData);
                        break;
                }
            }
        }

        void OnSceneChanged(ChangeSceneEventArgs data)
        {
            if (m_CachedEditor != null)
            {
                // PlayableDirector got deleted. Possibly from deleting a MasterSequence/Sequence
                // as it doesn't register the operation to undo but still mark the scene Dirty.
                if (m_CachedEditor.target == null)
                    ClearView();

                else if ((m_CachedEditor.target as PlayableDirector).gameObject.scene == data.scene)
                    m_CachedEditor.Refresh();
            }
        }

        void OnCreationOfGameObjectHierarchy(CreateGameObjectHierarchyEventArgs data)
        {
            var createdGameObject = EditorUtility.InstanceIDToObject(data.instanceId) as GameObject;
            if (createdGameObject == null)
                return;

            if (m_CachedEditor == null || m_CachedEditor.target == null)
                return;

            var directorGameObject = (m_CachedEditor.target as PlayableDirector).gameObject;
            if (IsChildOf(createdGameObject, directorGameObject))
                m_CachedEditor.Refresh();
        }

        void OnDestroyGameObjectHierarchy(DestroyGameObjectHierarchyEventArgs data)
        {
            var parentGameObject = EditorUtility.InstanceIDToObject(data.parentInstanceId) as GameObject;

            // A GameObject has been destroyed outside of a Sequence.
            if (parentGameObject == null)
                return;

            // No sequence is inspected, nothing to do.
            if (m_CachedEditor == null)
                return;

            var director = m_CachedEditor.target as PlayableDirector;
            if (director == null)
            {
                // The inspected Sequence has been deleted.
                ClearView();
                return;
            }

            // A GameObject within a Sequence tree has been deleted.
            var directorGameObject = director.gameObject;
            if (parentGameObject == directorGameObject || IsChildOf(parentGameObject, directorGameObject))
                m_CachedEditor.Refresh();
        }

        void OnSequenceInvalidated()
        {
            if (m_CachedEditor == null || m_CachedEditor.target == null)
            {
                // There is no sequence currently inspected. Refresh the empty state message.
                SetEmptyState();
                return;
            }

            var target = m_CachedEditor.target as PlayableDirector;
            var targetTimeline = target.playableAsset as TimelineAsset;
            if (targetTimeline == null)
            {
                // Use cases:
                // - Timeline asset is deleted on disk (not via API).
                // - PlayableDirector binding is removed.
                ClearView();
            }
            else
            {
                // Use case: sequence editorial clip binding is removed.
                var sequence = SequenceIndexer.instance.GetSequence(targetTimeline);
                if (!sequence.isValid)
                    ClearView();
            }
        }

        bool IsChildOf(GameObject child, GameObject parent)
        {
            for (var parentPointer = child.transform.parent; parentPointer != null; parentPointer = parentPointer.parent)
            {
                if (parentPointer.gameObject == parent)
                    return true;
            }
            return false;
        }

        void SetEmptyState()
        {
            if (!rootVisualElement.Contains(m_EmptyStateMessageContainer))
            {
                rootVisualElement.Add(m_EmptyStateMessageContainer);
                rootVisualElement.Unbind();
                m_SequenceNameLabel.text = "";
            }

            m_SequenceEmptyStateText.text = SequenceIndexer.instance.isEmpty ? k_SequenceNotCreatedMessage : k_SequenceNotSelectedMessage;
        }
    }
}
