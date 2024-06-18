using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(InputPropertyValueFloat))]
    public class InputPropertyValueFloatDrawer : TInputPropertyDrawer
    {
        protected override string InputReference => "m_Input";
    }
}