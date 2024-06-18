using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(InputPropertyButton))]
    public class InputPropertyButtonDrawer : TInputPropertyDrawer
    {
        protected override string InputReference => "m_Input";
    }
}