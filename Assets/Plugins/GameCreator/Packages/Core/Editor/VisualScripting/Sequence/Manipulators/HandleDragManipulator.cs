using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class HandleDragManipulator : Clickable
    {
        private const int RIGHT_MOUSE_BUTTON = 1;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        public event Action EventDragStart;
        public event Action EventDragFinish;
        public event Action EventDragMove;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public IEventHandler Target { get; private set; }
        
        public bool IsDragging { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 FinishPosition { get; private set; }

        public Vector2 Difference => this.FinishPosition - this.StartPosition;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HandleDragManipulator(Action onStart, Action onFinish, Action onMove) 
            : base(null, 250, 30)
        {
            this.EventDragStart += onStart;
            this.EventDragFinish += onFinish;
            this.EventDragMove += onMove;
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        protected override void ProcessDownEvent(EventBase eventDown, Vector2 position, int id)
        {
            this.Target = eventDown.target;
            this.IsDragging = true;
            this.StartPosition = position;
            this.FinishPosition = position;

            base.ProcessDownEvent(eventDown, position, id);
            this.EventDragStart?.Invoke();
        }

        protected override void ProcessCancelEvent(EventBase eventCancel, int id)
        {
            this.IsDragging = false;
            base.ProcessCancelEvent(eventCancel, id);
            this.EventDragFinish?.Invoke();
        }

        protected override void ProcessUpEvent(EventBase eventUp, Vector2 position, int id)
        {
            this.IsDragging = false;
            this.FinishPosition = position;
            base.ProcessUpEvent(eventUp, position, id);
            this.EventDragFinish?.Invoke();
        }

        protected override void ProcessMoveEvent(EventBase eventMove, Vector2 position)
        {
            this.FinishPosition = position;
            base.ProcessMoveEvent(eventMove, position);
            
            this.EventDragMove?.Invoke();
        }
    }
}