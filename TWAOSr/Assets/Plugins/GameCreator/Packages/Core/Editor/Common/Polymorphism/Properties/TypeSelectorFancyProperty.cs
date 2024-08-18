using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorFancyProperty : TypeSelectorValueFancy
    {
        public TypeSelectorFancyProperty(SerializedProperty property, Button element)
            : base(property, element)
        { }
    }
}
