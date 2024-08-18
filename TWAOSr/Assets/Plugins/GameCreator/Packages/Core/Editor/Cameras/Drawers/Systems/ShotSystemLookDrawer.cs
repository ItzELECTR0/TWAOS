using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemLook))]
    public class ShotSystemLookDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Look";
    }
}