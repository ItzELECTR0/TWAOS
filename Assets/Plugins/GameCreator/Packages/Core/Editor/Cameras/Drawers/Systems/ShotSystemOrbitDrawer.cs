using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemOrbit))]
    public class ShotSystemOrbitDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Orbit";
    }
}