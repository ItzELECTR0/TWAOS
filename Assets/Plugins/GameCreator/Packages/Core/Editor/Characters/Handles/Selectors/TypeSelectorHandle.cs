using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class TypeSelectorHandle : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorHandle(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(HandleItem), element)
        { }
    }
}