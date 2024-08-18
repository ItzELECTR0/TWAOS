using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemHeadLeaning))]
    public class ShotSystemHeadLeaningDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Head Leaning";
    }
}