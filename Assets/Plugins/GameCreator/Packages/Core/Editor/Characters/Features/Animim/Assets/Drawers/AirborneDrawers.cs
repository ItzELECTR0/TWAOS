using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(AirborneSingle))]
    public class AirborneSingleDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Airborne: Single";
    }
    
    [CustomPropertyDrawer(typeof(AirborneVertical))]
    public class AirborneVerticalDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Airborne: Vertical";
    }
    
    [CustomPropertyDrawer(typeof(AirborneDirectional))]
    public class AirborneDirectionalDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Airborne: Directional";
    }
}