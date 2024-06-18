using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class ManipulatorDropToListVariables : MouseManipulator
    {
        private const string CLASS_DEFAULT = "gc-list-variables-drop-default";
        private const string CLASS_ACCEPT = "gc-list-variables-drop-accept";
        private const string CLASS_DISREGARD = "gc-list-variables-drop-disregard";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly IndexListTool m_IndexListTool;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ManipulatorDropToListVariables(IndexListTool indexListTool)
        {
            this.m_IndexListTool = indexListTool;
            this.m_IndexListTool.DropZone.AddToClassList(CLASS_DEFAULT);
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
            this.m_IndexListTool.DropZone.AddToClassList(CLASS_DEFAULT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_DISREGARD);
        }
        
        private void OnDragUpdated(DragUpdatedEvent dragEvent)
        {
            this.UpdateAppearance();
        }

        private void OnDragPerform(DragPerformEvent dragEvent)
        {
            this.m_IndexListTool.DropZone.AddToClassList(CLASS_DEFAULT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_DISREGARD);

            if (this.AcceptDragType())
            {
                DragAndDrop.AcceptDrag();
                
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                this.m_IndexListTool.FillWith(DragAndDrop.objectReferences);
            }
        }

        private void OnDragExited(DragExitedEvent dragEvent)
        {
            this.m_IndexListTool.DropZone.AddToClassList(CLASS_DEFAULT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_DISREGARD);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateAppearance()
        {
            bool acceptDragType = this.AcceptDragType();
            
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_DEFAULT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_ACCEPT);
            this.m_IndexListTool.DropZone.RemoveFromClassList(CLASS_DISREGARD);

            this.m_IndexListTool.DropZone.AddToClassList(acceptDragType 
                ? CLASS_ACCEPT
                : CLASS_DISREGARD
            );
            
            DragAndDrop.visualMode = acceptDragType
                ? DragAndDropVisualMode.Copy
                : DragAndDropVisualMode.Rejected;
        }
        
        private bool AcceptDragType()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return false;
            
            int dragSize = DragAndDrop.objectReferences.Length; 
            if (dragSize <= 0) return false;
            
            Type type = DragAndDrop.objectReferences[0]?.GetType();
            if (type == null) return false;
            
            for (int i = 1; i < dragSize; ++i)
            {
                Type nextType = DragAndDrop.objectReferences[i]?.GetType();
                if (type != nextType) return false;
            }
            
            IdString typeId = TValue.GetTypeIDFromObjectType(type);
            return typeId != ValueNull.TYPE_ID;
        }
    }
}
