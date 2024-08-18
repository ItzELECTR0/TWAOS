using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ChangeDecimal))]
    public class ChangeFloatDrawer : TChangeValueDrawer
    { }
}