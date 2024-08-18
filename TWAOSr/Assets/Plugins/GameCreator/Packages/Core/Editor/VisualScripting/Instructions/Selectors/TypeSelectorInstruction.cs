using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class TypeSelectorInstruction : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorInstruction(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(Instruction), element)
        { }
    }
}