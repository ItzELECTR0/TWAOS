using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ChangeBool))]
    public class ChangeBoolDrawer : TChangeValueDrawer
    { }
}