using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class ElementDrag : ElementManipulator
    {
        public ElementDrag(GenericInspector item) : base(item) {}

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<DragExitedEvent>(OnDragExit);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<DragExitedEvent>(OnDragExit);
        }

        private void OnMouseDown(MouseDownEvent mouseEvent)
        {
            if (!CanStartManipulation(mouseEvent)) return;
            
            DragAndDrop.PrepareStartDrag();

            SetDragData(m_Item);

            DragAndDrop.StartDrag(m_Item.Title);
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;

            SetSourceState(true);
            m_IsActive = true;
        }

        private void OnDragExit(DragExitedEvent evt)
        {
            SetSourceState(false);
        }
    }
}
