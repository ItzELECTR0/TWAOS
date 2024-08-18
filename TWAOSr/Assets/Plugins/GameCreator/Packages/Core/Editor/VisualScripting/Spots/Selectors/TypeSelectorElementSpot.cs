using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class TypeSelectorElementSpot : Button
    {
        private static readonly IIcon ICON_ADD = new IconSpot(ColorTheme.Type.TextLight);
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementSpot(SerializedProperty propertyList, SpotListTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add Spot..." });
            
            TypeSelectorSpot typeSelector = new TypeSelectorSpot(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}
