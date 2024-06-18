using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(MotionJump))]
    public class MotionJumpDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Jump";
    }
}