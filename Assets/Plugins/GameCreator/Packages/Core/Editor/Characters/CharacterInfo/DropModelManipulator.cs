using System;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class DropModelManipulator : MouseManipulator
    {
        private const string CLASS_DEFAULT = "gc-character-model-drop-default";
        private const string CLASS_ACCEPT = "gc-character-model-drop-accept";
        private const string CLASS_DISREGARD = "gc-character-model-drop-disregard";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly ModelTool m_Tool;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public DropModelManipulator(ModelTool tool)
        {
            this.m_Tool = tool;

            this.m_Tool.dropzone.AddToClassList(CLASS_DEFAULT);
        }
        
        // REGISTERS: -----------------------------------------------------------------------------
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
            target.RegisterCallback<DragExitedEvent>(OnDragExited);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);

            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
            target.UnregisterCallback<DragExitedEvent>(OnDragExited);
        }
        
        // EVENT METHODS: -------------------------------------------------------------------------

        private void OnDragEnter(DragEnterEvent dragEvent)
        {
            this.UpdateAppearance();
        }

        private void OnDragLeave(DragLeaveEvent dragEvent)
        {
            this.m_Tool.dropzone.AddToClassList(CLASS_DEFAULT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_DISREGARD);
        }
        
        private void OnDragUpdated(DragUpdatedEvent dragEvent)
        {
            this.UpdateAppearance();
        }

        private void OnDragPerform(DragPerformEvent dragEvent)
        {
            this.m_Tool.dropzone.AddToClassList(CLASS_DEFAULT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_DISREGARD);

            if (this.AcceptDragType())
            {
                DragAndDrop.AcceptDrag();

                GameObject prefab = DragAndDrop.objectReferences[0] as GameObject;
                switch (EditorApplication.isPlaying)
                {
                    case true:
                        this.m_Tool.character.ChangeModel(prefab, default);
                        break;
                    
                    case false:
                        m_Tool.ChangeModelEditor(prefab, Vector3.zero);
                        break;
                }
            }
        }

        private void OnDragExited(DragExitedEvent dragEvent)
        {
            this.m_Tool.dropzone.AddToClassList(CLASS_DEFAULT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_DISREGARD);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateAppearance()
        {
            bool acceptDragType = this.AcceptDragType();
            
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_DEFAULT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_Tool.dropzone.RemoveFromClassList(CLASS_DISREGARD);

            this.m_Tool.dropzone.AddToClassList(acceptDragType 
                ? CLASS_ACCEPT 
                : CLASS_DISREGARD);

            DragAndDrop.visualMode = acceptDragType
                ? DragAndDropVisualMode.Copy
                : DragAndDropVisualMode.Rejected;
        }
        
        private bool AcceptDragType()
        {
            if (DragAndDrop.objectReferences.Length != 1) return false;
            
            GameObject draggedObject = DragAndDrop.objectReferences[0] as GameObject;
            if (draggedObject == null) return false;
            
            bool prefabAllowed = (
                PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Model   ||
                PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Regular ||
                PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Variant
            );

            return prefabAllowed;
        }
    }
}
