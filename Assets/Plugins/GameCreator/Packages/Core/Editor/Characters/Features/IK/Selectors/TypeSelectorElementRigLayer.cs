using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class TypeSelectorElementRigLayer : Button
    {
        private static readonly IIcon ICON_ADD = new IconIK(ColorTheme.Type.TextLight);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorElementRigLayer(SerializedProperty propertyList, RigLayersTool tool)
        {
            this.Add(new Image { image = ICON_ADD.Texture });
            this.Add(new Label { text = "Add IK Rig Layer..." });
            
            TypeSelectorRigLayer typeSelector = new TypeSelectorRigLayer(propertyList, this);
            typeSelector.EventChange += (prevType, newType) =>
            {
                object instance = Activator.CreateInstance(newType);
                tool.InsertItem(propertyList.arraySize, instance);
            };
        }
    }
}