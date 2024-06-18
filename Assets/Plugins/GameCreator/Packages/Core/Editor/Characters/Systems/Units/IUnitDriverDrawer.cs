using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(IUnitDriver), true)]
    public class IUnitDriverDrawer : TUnitDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return this.MakePropertyGUI(property, "Driver");
        }
        
        protected override IIcon UnitIcon => new IconWheel(ColorTheme.Type.TextLight);
    }
}