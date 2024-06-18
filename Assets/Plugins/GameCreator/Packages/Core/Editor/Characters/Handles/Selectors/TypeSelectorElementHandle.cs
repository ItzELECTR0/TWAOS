using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class TypeSelectorElementHandle : Button
    {
        private static readonly IIcon ICON_ADD = new IconHandle(ColorTheme.Type.TextLight);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementHandle(SerializedProperty propertyList, HandleListTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add Handle..." });
            
            TypeSelectorHandle typeSelector = new TypeSelectorHandle(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}