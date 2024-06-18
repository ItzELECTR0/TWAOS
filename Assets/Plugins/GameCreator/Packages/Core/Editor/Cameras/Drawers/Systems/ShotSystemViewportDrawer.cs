using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemViewport))]
    public class ShotSystemViewportDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Viewport";
    }
}