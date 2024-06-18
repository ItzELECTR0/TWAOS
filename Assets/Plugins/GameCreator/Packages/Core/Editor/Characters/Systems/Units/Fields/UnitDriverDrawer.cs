using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(UnitDriver))]
    public class UnitDriverDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty unit = property.FindPropertyRelative("m_Driver");
            return new PropertyElement(unit, unit.displayName, false);
        }
    }
}