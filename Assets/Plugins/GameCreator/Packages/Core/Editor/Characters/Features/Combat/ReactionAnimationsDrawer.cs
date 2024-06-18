using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(ReactionAnimations))]
    public class ReactionAnimationsDrawer : TArrayDrawer
    {
        protected override string PropertyArrayName => "m_Animations";
        protected override float ItemHeight => 22f;
    }
}