using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorElementMemory : Button
    {
        private static readonly IIcon ICON_ADD = new IconDiskSolid(ColorTheme.Type.TextLight);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementMemory(SerializedProperty propertyList, MemoriesTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add Memory..." });
            
            TypeSelectorMemory typeSelector = new TypeSelectorMemory(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}