using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class TypeSelectorMemory : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorMemory(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(Memory), element)
        { }
    }
}