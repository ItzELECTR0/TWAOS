using GameCreator.Editor.Common;
using UnityEditor;

namespace GameCreator.Editor.Installs
{
    [CustomPropertyDrawer(typeof(Install))]
    public class InstallDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Configuration";
    }
}