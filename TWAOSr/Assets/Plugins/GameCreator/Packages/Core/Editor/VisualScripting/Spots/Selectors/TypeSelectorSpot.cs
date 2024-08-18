using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class TypeSelectorSpot : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorSpot(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(Spot), element)
        { }
    }
}