using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public abstract class ElementManipulator : MouseManipulator
    {
        //============================================================================================================||
        
        [Serializable]
        protected class DragData
        {
            public GenericInspector source;
        }
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private static readonly string DRAG_DATA_TYPE = typeof(DragData).ToString();
        
        protected static bool m_IsActive = false;
        
        protected readonly GenericInspector m_Item;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        protected ElementManipulator(GenericInspector item)
        {
            m_Item = item;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        // ※  Protected Methods: ------------------------------------------------------------------------------------|

        protected void SetSourceState(bool transparent)
        {
            var data = GetDragData();
            if (data == null) return;

            data.source.style.opacity = transparent ? 0.25f : 1f;
        }

        protected bool SamePolymorphicList()
        {
            var sourceParentTool = GetDragData()?.source?.ListInspector;

            if (sourceParentTool == null) return false;

            return m_Item.ListInspector == sourceParentTool;
        }

        protected static DragData GetDragData()
        {
            return DragAndDrop.GetGenericData(DRAG_DATA_TYPE) as DragData;
        }

        protected static void SetDragData(GenericInspector item)
        {
            DragAndDrop.SetGenericData(DRAG_DATA_TYPE, new DragData
            {
                source = item
            });
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}