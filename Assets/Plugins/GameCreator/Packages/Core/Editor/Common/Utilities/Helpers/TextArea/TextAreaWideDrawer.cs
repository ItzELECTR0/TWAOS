using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TextAreaWide))]
    public class TextAreaWideDrawer : BaseTextAreaDrawer
    {
        protected override string Label(SerializedProperty property) => string.Empty;
    }
}
