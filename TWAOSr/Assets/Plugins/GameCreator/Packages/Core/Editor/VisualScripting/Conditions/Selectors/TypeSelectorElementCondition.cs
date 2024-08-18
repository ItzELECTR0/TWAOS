using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class TypeSelectorElementCondition : Button
    {
        private static readonly IIcon ICON_ADD = new IconCondition(ColorTheme.Type.TextLight);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementCondition(SerializedProperty propertyList, 
            ConditionListTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add Condition..." });
            
            TypeSelectorCondition typeSelector = new TypeSelectorCondition(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}
