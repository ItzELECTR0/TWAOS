using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ChangeInteger))]
    public class ChangeIntegerDrawer : TChangeValueDrawer
    { }
}