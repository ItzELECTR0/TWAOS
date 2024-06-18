using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(UnitPlayer))]
    public class UnitPlayerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty unit = property.FindPropertyRelative("m_Player");
            return new PropertyElement(unit, unit.displayName, false);
        }
    }
}