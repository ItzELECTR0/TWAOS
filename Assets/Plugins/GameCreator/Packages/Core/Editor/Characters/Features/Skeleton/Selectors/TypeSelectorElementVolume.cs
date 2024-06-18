using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class TypeSelectorElementVolume : Button
    {
        private static readonly IIcon ICON_ADD = new IconSkeleton(ColorTheme.Type.TextLight);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementVolume(SerializedProperty propertyList, VolumesTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add Volume..." });
            
            TypeSelectorVolume typeSelector = new TypeSelectorVolume(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}