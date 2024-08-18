using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemFollow))]
    public class ShotSystemFollowDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Follow";
    }
}