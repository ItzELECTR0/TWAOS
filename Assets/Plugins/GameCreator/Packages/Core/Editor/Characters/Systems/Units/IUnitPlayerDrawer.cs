using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(IUnitPlayer), true)]
    public class IUnitPlayerDrawer : TUnitDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return this.MakePropertyGUI(property, "Player");
        }
        
        protected override IIcon UnitIcon => new IconPlayer(ColorTheme.Type.TextLight);
    }
}