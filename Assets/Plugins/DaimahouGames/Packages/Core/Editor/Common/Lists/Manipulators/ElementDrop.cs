using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class ElementDrop : ElementManipulator
    {
        private const string TXT_UNDO_MOVE = "Move List Element";

        // INITIALIZERS: --------------------------------------------------------------------------

        public ElementDrop(GenericInspector item) : base(item) { }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);

            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);

            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        // EVENT METHODS: -------------------------------------------------------------------------

        private void OnDragEnter(DragEnterEvent dragEvent)
        {
            if (!m_IsActive) return;
            if (!SamePolymorphicList()) return;

            SetSourceState(true);
            UpdateDropZone(dragEvent.mousePosition);
        }

        private void OnDragLeave(DragLeaveEvent dragEvent)
        {
            if (!m_IsActive) return;
            SetSourceState(false);

            m_Item.m_DropAbove.style.display = DisplayStyle.None;
            m_Item.m_DropBelow.style.display = DisplayStyle.None;
        }

        private void OnDragUpdate(DragUpdatedEvent dragEvent)
        {
            if (!m_IsActive) return;
            if (!SamePolymorphicList()) return;

            UpdateDropZone(dragEvent.mousePosition);            
        }

        private void OnDragPerform(DragPerformEvent dragEvent)
        {
            if (!m_IsActive) return;
            DragAndDrop.AcceptDrag();

            MakeMovement(dragEvent);
            FinalizeDrag();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void MakeMovement(IMouseEvent dragEvent)
        {
            var position = GetMousePercentagePosition(dragEvent.mousePosition);

            var source = GetDragData();

            if (source?.source == null) return;
            if (!SamePolymorphicList()) return;

            int sourceIndex = source.source.Index;
            int destinationIndex = m_Item.Index;

            if (position > 0.5f) destinationIndex += 1;
            if (sourceIndex < destinationIndex) destinationIndex -= 1;
            
            m_Item.ListInspector.MoveItems(sourceIndex, destinationIndex);
        }

        private void UpdateDropZone(Vector2 mousePosition)
        {
            if (GetDragData().source == m_Item) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            var position = GetMousePercentagePosition(mousePosition);

            if (position <= 0.5f)
            {
                m_Item.m_DropAbove.style.display = DisplayStyle.Flex;
                m_Item.m_DropBelow.style.display = DisplayStyle.None;
            }
            else
            {
                m_Item.m_DropAbove.style.display = DisplayStyle.None;
                m_Item.m_DropBelow.style.display = DisplayStyle.Flex;
            }
        }

        private float GetMousePercentagePosition(Vector2 mousePosition)
        {
            var bounds = m_Item.worldBound;
            var position = (mousePosition.y - bounds.y) / bounds.height;

            position = Math.Max(0f, position);
            position = Math.Min(1f, position);

            return position;
        }

        private void FinalizeDrag()
        {
            SetSourceState(false);

            m_Item.m_DropAbove.style.display = DisplayStyle.None;
            m_Item.m_DropBelow.style.display = DisplayStyle.None;

            m_IsActive = false;
        }
    }
}