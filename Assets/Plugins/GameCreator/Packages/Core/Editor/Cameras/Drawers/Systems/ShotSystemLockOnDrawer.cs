using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemLockOn))]
    public class ShotSystemLockOnDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Lock On";
    }
}