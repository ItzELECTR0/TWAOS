using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(SceneEntries))]
    public class SceneEntriesDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Entries";
    }
}
