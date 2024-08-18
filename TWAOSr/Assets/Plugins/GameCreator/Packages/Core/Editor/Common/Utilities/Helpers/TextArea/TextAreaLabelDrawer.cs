using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TextAreaLabel))]
    public class TextAreaLabelDrawer : BaseTextAreaDrawer
    {
        protected override string Label(SerializedProperty property) => property.displayName;
    }
}
