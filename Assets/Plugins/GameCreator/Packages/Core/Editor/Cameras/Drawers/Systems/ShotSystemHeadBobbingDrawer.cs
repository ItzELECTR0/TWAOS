using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemHeadBobbing))]
    public class ShotSystemHeadBobbingDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Head Bobbing";
    }
}