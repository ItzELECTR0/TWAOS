using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(PercentageWithLabel))]
    public class PercentageWithLabelDrawer : TPercentageDrawer
    {
        protected override string GetLabel(SerializedProperty property)
        {
            return property.displayName;
        }
    }
}