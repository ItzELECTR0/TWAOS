using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemZoom))]
    public class ShotSystemZoomDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Zoom";
    }
}