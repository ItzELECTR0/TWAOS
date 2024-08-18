using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemAnchor))]
    public class ShotSystemAnchorDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Anchor";
    }
}