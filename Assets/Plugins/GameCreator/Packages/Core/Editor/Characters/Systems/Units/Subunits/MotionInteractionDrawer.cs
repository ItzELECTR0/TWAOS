using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(MotionInteraction))]
    public class MotionInteractionDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Interaction";
    }
}