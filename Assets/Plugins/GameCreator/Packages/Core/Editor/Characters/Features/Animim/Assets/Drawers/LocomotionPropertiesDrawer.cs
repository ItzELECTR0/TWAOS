using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(LocomotionProperties))]
    public class LocomotionPropertiesDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Properties";
    }
}