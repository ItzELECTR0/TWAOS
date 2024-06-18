using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TArrayTool : VisualElement
    {
        // MEMBERS: -------------------------------------------------------------------------------

        protected readonly SerializedProperty m_PropertyArray;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public TArrayTool(SerializedProperty propertyArray, float height)
        {
            this.m_PropertyArray = propertyArray;
            
            ListView listVew = new ListView
            {
                fixedItemHeight = height,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = false,
                showBoundCollectionSize = false,
                showBorder = true,
                showAddRemoveFooter = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                makeItem = this.MakeItem,
                bindItem = this.BindItem
            };
            
            this.Add(listVew);

            listVew.BindProperty(propertyArray);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected virtual VisualElement MakeItem()
        {
            return new PropertyField { label = string.Empty };
        }
        
        protected virtual void BindItem(VisualElement element, int index)
        {
            if (index >= this.m_PropertyArray.arraySize) return;
            SerializedProperty property = this.m_PropertyArray.GetArrayElementAtIndex(index);
            
            if (property == null) return;
            if (element is not PropertyField field) return;
            
            field.BindProperty(property);
        }
    }
}