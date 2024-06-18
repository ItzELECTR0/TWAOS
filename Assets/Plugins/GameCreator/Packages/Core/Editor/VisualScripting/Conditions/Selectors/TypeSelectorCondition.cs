using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class TypeSelectorCondition : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorCondition(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(Condition), element)
        { }
    }
}