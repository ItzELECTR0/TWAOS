using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(MotionDash))]
    public class MotionDashDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Dash";
    }
}