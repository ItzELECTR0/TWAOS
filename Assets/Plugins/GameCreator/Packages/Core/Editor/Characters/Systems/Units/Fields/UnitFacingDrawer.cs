using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(UnitFacing))]
    public class UnitFacingDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty unit = property.FindPropertyRelative("m_Facing");
            return new PropertyElement(unit, unit.displayName, false);
        }
    }
}