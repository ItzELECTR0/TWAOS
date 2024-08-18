using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(PercentageWithoutLabel))]
    public class PercentageWithoutLabelDrawer : TPercentageDrawer
    {
        protected override string GetLabel(SerializedProperty property)
        {
            return " ";
        }
    }
}