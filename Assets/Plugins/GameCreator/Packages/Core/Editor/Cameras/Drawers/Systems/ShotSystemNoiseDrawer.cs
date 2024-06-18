using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemNoise))]
    public class ShotSystemNoiseDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Noise";
    }
}