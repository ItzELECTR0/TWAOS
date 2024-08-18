using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TextAreaField))]
    public class TextAreaFieldDrawer : BaseTextAreaDrawer
    {
        protected override string Label(SerializedProperty property) => " ";
    }
}
