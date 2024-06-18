using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class ManipulatorPolymorphicListSort : MouseManipulator
    {
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly TPolymorphicListTool m_Parent;
        
        private bool m_IsDragging;
        
        private int m_StartIndex = -1;
        private int m_CurrentIndex = -1;
        
        private VisualElement m_CurrentTarget;
        private TPolymorphicItemTool m_CurrentParameter;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public TPolymorphicItemTool DraggedParameter => this.m_IsDragging
            ? this.m_CurrentParameter
            : null;

        // INITIALIZERS: --------------------------------------------------------------------------

        public ManipulatorPolymorphicListSort(TPolymorphicListTool parent)
        {
            this.m_Parent = parent;
            this.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            this.target.RegisterCallback<MouseDownEvent>(this.OnMouseDown, TrickleDown.TrickleDown);
            this.target.RegisterCallback<MouseMoveEvent>(this.OnMouseMove, TrickleDown.TrickleDown);
            this.target.RegisterCallback<MouseUpEvent>(this.OnMouseUp, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            this.target.UnregisterCallback<MouseDownEvent>(this.OnMouseDown);
            this.target.UnregisterCallback<MouseMoveEvent>(this.OnMouseMove);
            this.target.UnregisterCallback<MouseUpEvent>(this.OnMouseUp);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static T GetAncestorOfType<T>(VisualElement visualElement) where T : class
        {
            return visualElement as T ?? visualElement.GetFirstAncestorOfType<T>();
        }

        private int ClosestElementIndex(float cursor)
        {
            float minDistance = Mathf.Infinity;
            int minIndex = -1;

            List<TPolymorphicItemTool> parameters = this.m_Parent.PolymorphicItemTools;
            for (int i = 0; i < parameters.Count; ++i)
            {
                TPolymorphicItemTool parameter = parameters[i];
                if (parameter == null) continue;

                float center = parameter.worldBound.y + parameter.worldBound.height * 0.5f;
                float distance = center - cursor;
                float distanceAbsolute = Math.Abs(distance);

                if (minIndex != -1 && distanceAbsolute > minDistance) continue;

                minIndex = distance >= 0 ? i : i + 1;
                minDistance = distanceAbsolute;
            }

            return minIndex;
        }

        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnMouseDown(MouseDownEvent eventMouseDown)
        {
            if (this.m_IsDragging)
            {
                eventMouseDown.StopImmediatePropagation();
                return;
            }

            if (!this.CanStartManipulation(eventMouseDown)) return;
            
            this.m_CurrentTarget = eventMouseDown.currentTarget as VisualElement;
            this.m_CurrentParameter = GetAncestorOfType<TPolymorphicItemTool>(this.m_CurrentTarget);
            
            this.m_StartIndex = this.m_Parent.GetIndexOf(this.m_CurrentParameter);
            this.m_CurrentIndex = this.m_StartIndex;

            this.m_IsDragging = true;
            this.m_CurrentTarget.CaptureMouse();
            
            eventMouseDown.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent eventMouseMove)
        {
            if (!this.m_IsDragging) return;
            
            this.m_CurrentIndex = this.ClosestElementIndex(eventMouseMove.mousePosition.y);
            this.m_Parent.RefreshDragUI(this.m_StartIndex, this.m_CurrentIndex);

            eventMouseMove.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent eventMouseUp)
        {
            if (!this.m_IsDragging) return;
            if (!CanStopManipulation(eventMouseUp)) return;
            
            this.m_Parent.RefreshDragUI(-1, -1);
            
            if (this.m_StartIndex != this.m_CurrentIndex)
            {
                this.m_Parent.MoveItems(this.m_StartIndex, this.m_CurrentIndex);
            }
            else
            {
                this.m_Parent.Refresh();
            }

            this.m_IsDragging = false;
            
            this.m_CurrentTarget.ReleaseMouse();
            this.m_CurrentTarget = null;
            
            eventMouseUp.StopPropagation();
        }
    }
}