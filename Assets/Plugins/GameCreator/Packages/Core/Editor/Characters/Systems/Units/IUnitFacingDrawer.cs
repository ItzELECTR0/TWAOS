using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(IUnitFacing), true)]
    public class IUnitFacingDrawer : TUnitDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return this.MakePropertyGUI(property, "Rotation");
        }
        
        protected override IIcon UnitIcon => new IconRotationYaw(ColorTheme.Type.TextLight);
    }
}