using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(InputPropertyValueVector2))]
    public class InputPropertyValueVector2Drawer : TInputPropertyDrawer
    {
        protected override string InputReference => "m_Input";
    }
}