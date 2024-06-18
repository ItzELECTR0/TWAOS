using System;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class MouseDropdownManipulator : MouseManipulator
    {
        private readonly Action<ContextualMenuPopulateEvent> m_menuBuilder;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public MouseDropdownManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
        {
            this.m_menuBuilder = menuBuilder;
            this.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseUpEvent>(OnMouseUpDownEvent);
            target.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseUpEvent>(OnMouseUpDownEvent);
            target.UnregisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
        }

        private void OnMouseUpDownEvent(IMouseEvent mouseEvent)
        {
            if (this.CanStartManipulation(mouseEvent))
            {
                if (target.panel?.contextualMenuManager != null)
                {
                    EventBase eventBase = mouseEvent as EventBase;
                    target.panel.contextualMenuManager.DisplayMenu(eventBase, target);

                    eventBase?.StopPropagation();
                    eventBase?.PreventDefault();
                }
            }
        }

        private void OnContextualMenuEvent(ContextualMenuPopulateEvent contextMenuEvent)
        {
            m_menuBuilder?.Invoke(contextMenuEvent);
        }
    }
}