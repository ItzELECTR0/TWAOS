using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class TypeSelectorRigLayer : TypeSelectorListFancy
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TypeSelectorRigLayer(SerializedProperty propertyList, Button element)
            : base(propertyList, typeof(TRig), element)
        { }
    }
}